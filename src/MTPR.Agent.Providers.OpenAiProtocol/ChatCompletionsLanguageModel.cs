using System.Net.Http.Json;
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

        using HttpRequestMessage httpRequest = new (HttpMethod.Post,
            chatCompletionsPath);
        httpRequest.Content = JsonContent.Create(toChatCompletionRequest(request));
        ApplyAuthorization(httpRequest);

        ChatCompletionResponse? body;

        try
        {
            using var httpResponse =
                await httpClient.SendAsync(httpRequest, cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new LanguageModelException(
                    $"{ProviderName} request failed with status {(int)httpResponse.StatusCode}.",
                    (int)httpResponse.StatusCode);
            }

            body =
                await httpResponse.Content
                    .ReadFromJsonAsync<ChatCompletionResponse>(cancellationToken);
        }
        catch (HttpRequestException exception)
        {
            throw new LanguageModelException(
                $"{ProviderName} request could not be sent.",
                exception);
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

    /// <summary>
    /// Adds the authorization information required by the provider to the request.
    /// </summary>
    /// <param name="request">The outgoing HTTP request.</param>
    protected abstract void ApplyAuthorization(HttpRequestMessage request);

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
