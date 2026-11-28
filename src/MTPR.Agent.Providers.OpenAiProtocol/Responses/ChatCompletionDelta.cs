using System.Text.Json.Serialization;

namespace MTPR.Agent.Providers.OpenAiProtocol.Responses;

/// <summary>
/// Incremental message content of a streamed choice.
/// </summary>
internal sealed class ChatCompletionDelta
{
    /// <summary>
    /// Gets the text generated since the previous event.
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; init; }
}
