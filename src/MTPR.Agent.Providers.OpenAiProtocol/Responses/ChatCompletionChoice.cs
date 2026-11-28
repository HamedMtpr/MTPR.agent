using System.Text.Json.Serialization;

namespace MTPR.Agent.Providers.OpenAiProtocol.Responses;

/// <summary>
/// A generated choice.
/// </summary>
internal sealed class ChatCompletionChoice
{
    /// <summary>
    /// Gets or sets the generated message.
    /// </summary>
    [JsonPropertyName("message")]
    public ChatCompletionMessageResult? Message { get; init; }

    /// <summary>
    /// Gets or sets the finish reason name.
    /// </summary>
    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}
