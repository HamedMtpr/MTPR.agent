using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using MTPR.Agent.Core.Abstractions;
using MTPR.Agent.Core.Exceptions;
using MTPR.Agent.Core.Models;
using MTPR.Agent.Providers.OpenAiProtocol.Mapping;
using MTPR.Agent.Providers.OpenAiProtocol.Requests;
using MTPR.Agent.Providers.OpenAiProtocol.Responses;

namespace MTPR.Agent.Providers.OpenAiProtocol;

/// <summary>
/// Base class for language models that speak the OpenAI chat completions protocol.
/// Derived classes supply the provider name and the authorization scheme.
/// </summary>
/// <param name="httpClient">HTTP client whose base address points to the API.</param>
public abstract class ChatCompletionsLanguageModel(HttpClient httpClient)
    : ILanguageModel
{
    private const string chatCompletionsPath = "chat/completions";
    private const string streamDataPrefix = "data:";
    private const string streamDoneMarker = "[DONE]";

    /// <summary>
    /// Gets the provider name used in error messages.
    /// </summary>
    protected abstract string ProviderName { get; }

    /// <inheritdoc />
    public async Task<LanguageModelResponse> CompleteAsync(
        LanguageModelRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ChatCompletionResponse? body;

        using (var httpResponse = await sendAsync(
            toChatCompletionRequest(request, stream: null),
            HttpCompletionOption.ResponseContentRead,
            cancellationToken))
        {
            body =
                await httpResponse.Content
                    .ReadFromJsonAsync<ChatCompletionResponse>(cancellationToken);
        }

        var choice = body?.Choices.FirstOrDefault();

        if (body is null || choice is null)
        {
            throw new LanguageModelException(
                $"{ProviderName} response contains no choices.");
        }

        return new LanguageModelResponse(
            choice.Message?.Content ?? string.Empty,
            FinishReasonMapper.Map(choice.FinishReason),
            toTokenUsage(body.Usage));
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<LanguageModelChunk> StreamAsync(
        LanguageModelRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpResponse = await sendAsync(
            toChatCompletionRequest(request, stream: true),
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

            var data = line[streamDataPrefix.Length..].Trim();

            if (data == streamDoneMarker)
            {
                yield break;
            }

            var chunk = JsonSerializer.Deserialize<ChatCompletionStreamChunk>(data);
            var choice = chunk?.Choices.FirstOrDefault();

            if (choice is null)
            {
                continue;
            }

            yield return new LanguageModelChunk(
                choice.Delta?.Content ?? string.Empty,
                choice.FinishReason is null
                    ? null
                    : FinishReasonMapper.Map(choice.FinishReason));
        }
    }

    /// <summary>
    /// Adds the authorization information required by the provider to the request.
    /// </summary>
    /// <param name="request">The outgoing HTTP request.</param>
    protected abstract void ApplyAuthorization(HttpRequestMessage request);

    private async Task<HttpResponseMessage> sendAsync(
        ChatCompletionRequest body,
        HttpCompletionOption completionOption,
        CancellationToken cancellationToken)
    {
        using HttpRequestMessage httpRequest = new (HttpMethod.Post,
            chatCompletionsPath);
        httpRequest.Content = JsonContent.Create(body);
        ApplyAuthorization(httpRequest);

        HttpResponseMessage httpResponse;

        try
        {
            httpResponse =
                await httpClient.SendAsync(httpRequest, completionOption, cancellationToken);
        }
        catch (HttpRequestException exception)
        {
            throw new LanguageModelException(
                $"{ProviderName} request could not be sent.",
                exception);
        }

        if (httpResponse.IsSuccessStatusCode)
        {
            return httpResponse;
        }

        var statusCode = (int)httpResponse.StatusCode;
        httpResponse.Dispose();

        throw new LanguageModelException(
            $"{ProviderName} request failed with status {statusCode}.",
            statusCode);
    }

    private static ChatCompletionRequest toChatCompletionRequest(
        LanguageModelRequest request,
        bool? stream)
    {
        return new ChatCompletionRequest
        {
            Model = request.Model,
            Messages = request.Messages
                .Select(message => new ChatCompletionMessage
                {
                    Role = RoleMapper.Map(message.Role),
                    Content = message.Content,
                })
                .ToList(),
            Temperature = request.Temperature,
            MaxTokens = request.MaxOutputTokens,
            Stream = stream,
        };
    }

    private static TokenUsage? toTokenUsage(ChatCompletionUsage? usage)
    {
        return usage is null
            ? null
            : new TokenUsage(usage.PromptTokens, usage.CompletionTokens);
    }
}
