using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using MTPR.Agent.Core.Abstractions;
using MTPR.Agent.Core.Exceptions;
using MTPR.Agent.Core.Models;
using MTPR.Agent.Providers.Google.Mapping;
using MTPR.Agent.Providers.Google.Options;
using MTPR.Agent.Providers.Google.Requests;
using MTPR.Agent.Providers.Google.Responses;

namespace MTPR.Agent.Providers.Google;

/// <summary>
/// Language model backed by the Google Gemini API.
/// </summary>
/// <param name="httpClient">HTTP client whose base address points to the API.</param>
/// <param name="options">Connection settings.</param>
public sealed class GoogleLanguageModel(HttpClient httpClient,
    GoogleOptions options) : ILanguageModel
{
    private const string providerName = "Google";
    private const string streamDataPrefix = "data:";

    /// <inheritdoc />
    public async Task<LanguageModelResponse> CompleteAsync(
        LanguageModelRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        GenerateContentResponse? body;

        using (var httpResponse = await sendAsync(
            $"models/{request.Model}:generateContent",
            request,
            HttpCompletionOption.ResponseContentRead,
            cancellationToken))
        {
            body =
                await httpResponse.Content
                    .ReadFromJsonAsync<GenerateContentResponse>(cancellationToken);
        }

        if (body is null)
        {
            throw new LanguageModelException(
                $"{providerName} response is empty.");
        }

        return new LanguageModelResponse(
            body.Text,
            FinishReasonMapper.Map(body.Candidates.FirstOrDefault()?.FinishReason),
            body.UsageMetadata is null
                ? null
                : new TokenUsage(
                    body.UsageMetadata.PromptTokenCount,
                    body.UsageMetadata.CandidatesTokenCount));
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<LanguageModelChunk> StreamAsync(
        LanguageModelRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpResponse = await sendAsync(
            $"models/{request.Model}:streamGenerateContent?alt=sse",
            request,
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

            var body = JsonSerializer.Deserialize<GenerateContentResponse>(
                line[streamDataPrefix.Length..]);

            if (body is null)
            {
                continue;
            }

            var finishReason = body.Candidates.FirstOrDefault()?.FinishReason;

            yield return new LanguageModelChunk(
                body.Text,
                finishReason is null
                    ? null
                    : FinishReasonMapper.Map(finishReason));
        }
    }

    private async Task<HttpResponseMessage> sendAsync(
        string path,
        LanguageModelRequest request,
        HttpCompletionOption completionOption,
        CancellationToken cancellationToken)
    {
        using HttpRequestMessage httpRequest = new (HttpMethod.Post, path);
        httpRequest.Content = JsonContent.Create(toGenerateContentRequest(request));
        httpRequest.Headers.Add("x-goog-api-key", options.ApiKey);

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

    private static GenerateContentRequest toGenerateContentRequest(
        LanguageModelRequest request)
    {
        var systemPrompts = request.Messages
            .Where(message => message.Role == ChatRole.System)
            .Select(message => message.Content)
            .ToList();

        return new GenerateContentRequest
        {
            SystemInstruction = systemPrompts.Count == 0
                ? null
                : toContent(null, string.Join("\n\n", systemPrompts)),
            Contents = request.Messages
                .Where(message => message.Role != ChatRole.System)
                .Select(message => toContent(
                    message.Role == ChatRole.Assistant ? "model" : "user",
                    message.Content))
                .ToList(),
            GenerationConfig = new GenerationConfig
            {
                Temperature = request.Temperature,
                MaxOutputTokens = request.MaxOutputTokens,
            },
        };
    }

    private static GeminiContent toContent(string? role, string text)
    {
        return new GeminiContent
        {
            Role = role,
            Parts = [new GeminiPart { Text = text }],
        };
    }
}
