using System.Text.Json.Serialization;

namespace MTPR.Agent.Providers.GapGpt.Responses;

/// <summary>
/// Generated message content.
/// </summary>
internal sealed class ChatCompletionMessageResult
{
    /// <summary>
    /// Gets or sets the generated text.
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; init; }
}
