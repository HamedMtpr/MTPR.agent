using System.Text.Json.Serialization;

namespace MTPR.Agent.Providers.Anthropic.Responses;

/// <summary>
/// One server-sent event of a streamed message.
/// </summary>
internal sealed class MessagesStreamEvent
{
    /// <summary>
    /// Gets or sets the event type, such as <c>content_block_delta</c>.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    /// <summary>
    /// Gets or sets the delta carried by this event.
    /// </summary>
    [JsonPropertyName("delta")]
    public MessagesStreamDelta? Delta { get; init; }
}
