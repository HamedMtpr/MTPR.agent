using System.Text.Json.Serialization;

namespace MTPR.Agent.Providers.OpenAiProtocol.Responses;

/// <summary>
/// A streamed choice holding an incremental delta.
/// </summary>
internal sealed class ChatCompletionStreamChoice
{
    /// <summary>
    /// Gets the incremental content.
    /// </summary>
    [JsonPropertyName("delta")]
    public ChatCompletionDelta? Delta { get; init; }

    /// <summary>
    /// Gets the finish reason name, set on the last event.
    /// </summary>
    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}
