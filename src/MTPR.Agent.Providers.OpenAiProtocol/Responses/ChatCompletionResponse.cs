using System.Text.Json.Serialization;

namespace MTPR.Agent.Providers.OpenAiProtocol.Responses;

/// <summary>
/// Body returned by an OpenAI-compatible chat completion call.
/// </summary>
internal sealed class ChatCompletionResponse
{
    /// <summary>
    /// Gets or sets the generated choices.
    /// </summary>
    [JsonPropertyName("choices")]
    public List<ChatCompletionChoice> Choices { get; init; } = [];

    /// <summary>
    /// Gets or sets the token usage.
    /// </summary>
    [JsonPropertyName("usage")]
    public ChatCompletionUsage? Usage { get; init; }
}
