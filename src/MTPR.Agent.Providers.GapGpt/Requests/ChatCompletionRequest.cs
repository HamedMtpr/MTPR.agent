using System.Text.Json.Serialization;

namespace MTPR.Agent.Providers.GapGpt.Requests;

/// <summary>
/// Body of a GapGPT chat completion call.
/// </summary>
internal sealed class ChatCompletionRequest
{
    /// <summary>
    /// Gets or sets the model identifier.
    /// </summary>
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    /// <summary>
    /// Gets or sets the ordered messages.
    /// </summary>
    [JsonPropertyName("messages")]
    public required IReadOnlyList<ChatCompletionMessage> Messages { get; init; }

    /// <summary>
    /// Gets or sets the sampling temperature.
    /// </summary>
    [JsonPropertyName("temperature")]
    public double? Temperature { get; init; }

    /// <summary>
    /// Gets or sets the token limit.
    /// </summary>
    [JsonPropertyName("max_tokens")]
    public int? MaxTokens { get; init; }
}
