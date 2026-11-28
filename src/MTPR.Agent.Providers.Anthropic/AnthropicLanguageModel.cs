using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using MTPR.Agent.Core.Abstractions;
using MTPR.Agent.Core.Exceptions;
using MTPR.Agent.Core.Models;
using MTPR.Agent.Providers.Anthropic.Mapping;
using MTPR.Agent.Providers.Anthropic.Options;
using MTPR.Agent.Providers.Anthropic.Requests;
using MTPR.Agent.Providers.Anthropic.Responses;

namespace MTPR.Agent.Providers.Anthropic;

/// <summary>
/// Language model backed by the Anthropic Messages API.
/// </summary>
/// <param name="httpClient">HTTP client whose base address points to the API.</param>
/// <param name="options">Connection settings.</param>
public sealed class AnthropicLanguageModel(HttpClient httpClient,
    AnthropicOptions options) : ILanguageModel
{
    private const string messagesPath = "messages";
    private const string providerName = "Anthropic";
    private const string streamDataPrefix = "data:";
    private const string textDeltaEvent = "content_block_delta";
    private const string messageDeltaEvent = "message_delta";
    private const string messageStopEvent = "message_stop";
    private const string errorEvent = "error";

    /// <inheritdoc />
    public async Task<LanguageModelResponse> CompleteAsync(
        LanguageModelRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        MessagesResponse? body;

        using (var httpResponse = await sendAsync(
            toMessagesRequest(request, stream: null),
            HttpCompletionOption.ResponseContentRead,
            cancellationToken))
        {
            body =
                await httpResponse.Content
                    .ReadFromJsonAsync<MessagesResponse>(cancellationToken);
        }

        if (body is null)
        {
            throw new LanguageModelException(
                $"{providerName} response is empty.");
        }

        var text = string.Concat(
            body.Content
                .Where(block => block.Type == "text")
                .Select(block => block.Text));

        return new LanguageModelResponse(
            text,
            StopReasonMapper.Map(body.StopReason),
            body.Usage is null
                ? null
                : new TokenUsage(body.Usage.InputTokens, body.Usage.OutputTokens));
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<LanguageModelChunk> StreamAsync(
        LanguageModelRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpResponse = await sendAsync(
            toMessagesRequest(request, stream: true),
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        using StreamReader reader = new (
            await httpResponse.Content.ReadAsStreamAsync(cancellationToken));

        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            if (!line.StartsWith(streamDataPrefix, StringComparison.Ordinal))
            {
                continue;
            }

            var streamEvent = JsonSerializer.Deserialize<MessagesStreamEvent>(
                line[streamDataPrefix.Length..]);

            switch (streamEvent?.Type)
            {
                case textDeltaEvent when streamEvent.Delta?.Text is { } text:
                    yield return new LanguageModelChunk(text);
                    break;

                case messageDeltaEvent:
                    yield return new LanguageModelChunk(
                        string.Empty,
                        StopReasonMapper.Map(streamEvent.Delta?.StopReason));
                    break;

                case errorEvent:
                    throw new LanguageModelException(
                        $"{providerName} stream reported an error.");

                case messageStopEvent:
                    yield break;
            }
        }
    }

    private async Task<HttpResponseMessage> sendAsync(
        MessagesRequest body,
        HttpCompletionOption completionOption,
        CancellationToken cancellationToken)
    {
        using HttpRequestMessage httpRequest = new (HttpMethod.Post, messagesPath);
        httpRequest.Content = JsonContent.Create(body);
        httpRequest.Headers.Add("x-api-key", options.ApiKey);
        httpRequest.Headers.Add("anthropic-version", options.ApiVersion);

        HttpResponseMessage httpResponse;

        try
        {
            httpResponse =
                await httpClient.SendAsync(httpRequest, completionOption, cancellationToken);
        }
        catch (HttpRequestException exception)
        {
            throw new LanguageModelException(
                $"{providerName} request could not be sent.",
                exception);
        }

        if (httpResponse.IsSuccessStatusCode)
        {
            return httpResponse;
        }

        var statusCode = (int)httpResponse.StatusCode;
        httpResponse.Dispose();

        throw new LanguageModelException(
            $"{providerName} request failed with status {statusCode}.",
            statusCode);
    }

    private MessagesRequest toMessagesRequest(
        LanguageModelRequest request,
        bool? stream)
    {
        var systemPrompts = request.Messages
            .Where(message => message.Role == ChatRole.System)
            .Select(message => message.Content)
            .ToList();

        return new MessagesRequest
        {
            Model = request.Model,
            System = systemPrompts.Count == 0
                ? null
                : string.Join("\n\n", systemPrompts),
            Messages = request.Messages
                .Where(message => message.Role != ChatRole.System)
                .Select(message => new MessagesMessage
                {
                    Role = message.Role == ChatRole.Assistant ? "assistant" : "user",
                    Content = message.Content,
                })
                .ToList(),
            MaxTokens = request.MaxOutputTokens ?? options.DefaultMaxTokens,
            Temperature = request.Temperature,
            Stream = stream,
        };
    }
}
