using System.Text.Json.Serialization;

namespace MTPR.Agent.Providers.OpenAiProtocol.Requests;

/// <summary>
/// A single message in an OpenAI-compatible chat completion call.
/// </summary>
internal sealed class ChatCompletionMessage
{
    /// <summary>
    /// Gets or sets the role name.
    /// </summary>
    [JsonPropertyName("role")]
    public required string Role { get; init; }

    /// <summary>
    /// Gets or sets the message text.
    /// </summary>
    [JsonPropertyName("content")]
    public required string Content { get; init; }
}
