using System.Text.Json.Serialization;

namespace MTPR.Agent.Providers.OpenAiProtocol.Responses;

/// <summary>
/// Token usage reported by the provider.
/// </summary>
internal sealed class ChatCompletionUsage
{
    /// <summary>
    /// Gets or sets the prompt token count.
    /// </summary>
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; init; }

    /// <summary>
    /// Gets or sets the completion token count.
    /// </summary>
    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; init; }
}
