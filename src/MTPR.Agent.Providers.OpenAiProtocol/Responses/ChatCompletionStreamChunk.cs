using System.Text.Json.Serialization;

namespace MTPR.Agent.Providers.OpenAiProtocol.Responses;

/// <summary>
/// One server-sent event of a streamed chat completion.
/// </summary>
internal sealed class ChatCompletionStreamChunk
{
    /// <summary>
    /// Gets the choices carried by this event.
    /// </summary>
    [JsonPropertyName("choices")]
    public List<ChatCompletionStreamChoice> Choices { get; init; } = [];
}
