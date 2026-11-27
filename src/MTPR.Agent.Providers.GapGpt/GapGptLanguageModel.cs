using System.Net.Http.Headers;
using System.Net.Http.Json;
using MTPR.Agent.Core.Abstractions;
using MTPR.Agent.Core.Exceptions;
using MTPR.Agent.Core.Models;
using MTPR.Agent.Providers.GapGpt.Mapping;
using MTPR.Agent.Providers.GapGpt.Options;
using MTPR.Agent.Providers.GapGpt.Requests;
using MTPR.Agent.Providers.GapGpt.Responses;

namespace MTPR.Agent.Providers.GapGpt;

/// <summary>
/// Language model backed by the GapGPT chat completion API.
/// </summary>

/// <summary>
/// Initializes a new GapGPT language model.
/// </summary>
/// <param name="httpClient">HTTP client used to reach the API.</param>
/// <param name="options">Connection settings.</param>
public sealed class GapGptLanguageModel(HttpClient httpClient,
    GapGptOptions options) : ILanguageModel
{
    private const string chatCompletionsPath = "chat/completions";

    /// <inheritdoc />
    public async Task<LanguageModelResponse> CompleteAsync(
        LanguageModelRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        using HttpRequestMessage httpRequest = new (HttpMethod.Post,
            chatCompletionsPath);
        httpRequest.Content = JsonContent.Create(toChatCompletionRequest(request));
        httpRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer",
                options.ApiKey);

        ChatCompletionResponse? body;

        try
        {
            using var httpResponse =
                await httpClient.SendAsync(httpRequest, cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new LanguageModelException(
                    $"GapGPT request failed with status {(int)httpResponse.StatusCode}.",
                    (int)httpResponse.StatusCode);
            }

            body =
                await httpResponse.Content
                    .ReadFromJsonAsync<ChatCompletionResponse>(cancellationToken);
        }
        catch (HttpRequestException exception)
        {
            throw new LanguageModelException("GapGPT request could not be sent.",
                exception);
        }

        var choice = body?.Choices.FirstOrDefault();

        if (body is null || choice is null)
        {
            throw new LanguageModelException("GapGPT response contains no choices.");
        }

        return new LanguageModelResponse(
            choice.Message?.Content ?? string.Empty,
            FinishReasonMapper.Map(choice.FinishReason),
            toTokenUsage(body.Usage));
    }

    private static ChatCompletionRequest toChatCompletionRequest(LanguageModelRequest request)
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
        };
    }

    private static TokenUsage? toTokenUsage(ChatCompletionUsage? usage)
    {
        return usage is null
            ? null
            : new TokenUsage(usage.PromptTokens, usage.CompletionTokens);
    }
}
