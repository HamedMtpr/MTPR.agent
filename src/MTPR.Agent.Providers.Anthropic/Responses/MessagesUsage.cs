using System.Text.Json.Serialization;

namespace MTPR.Agent.Providers.Anthropic.Responses;

/// <summary>
/// Token usage reported by Anthropic.
/// </summary>
internal sealed class MessagesUsage
{
    /// <summary>
    /// Gets or sets the input token count.
    /// </summary>
    [JsonPropertyName("input_tokens")]
    public int InputTokens { get; init; }

    /// <summary>
    /// Gets or sets the output token count.
    /// </summary>
    [JsonPropertyName("output_tokens")]
    public int OutputTokens { get; init; }
}
