using System.Text.Json.Serialization;

namespace MTPR.Agent.Providers.Anthropic.Responses;

/// <summary>
/// Incremental change carried by a streamed event.
/// </summary>
internal sealed class MessagesStreamDelta
{
    /// <summary>
    /// Gets or sets the text added by a <c>content_block_delta</c> event.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; init; }

    /// <summary>
    /// Gets or sets the stop reason carried by a <c>message_delta</c> event.
    /// </summary>
    [JsonPropertyName("stop_reason")]
    public string? StopReason { get; init; }
}
