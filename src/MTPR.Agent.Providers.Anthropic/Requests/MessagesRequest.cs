using System.Text.Json.Serialization;

namespace MTPR.Agent.Providers.Anthropic.Requests;

/// <summary>
/// Body of an Anthropic Messages API call.
/// </summary>
internal sealed class MessagesRequest
{
    /// <summary>
    /// Gets or sets the model identifier.
    /// </summary>
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    /// <summary>
    /// Gets or sets the system prompt, which the API keeps outside the messages.
    /// </summary>
    [JsonPropertyName("system")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? System { get; init; }

    /// <summary>
    /// Gets or sets the ordered user and assistant messages.
    /// </summary>
    [JsonPropertyName("messages")]
    public required IReadOnlyList<MessagesMessage> Messages { get; init; }

    /// <summary>
    /// Gets or sets the required token limit.
    /// </summary>
    [JsonPropertyName("max_tokens")]
    public required int MaxTokens { get; init; }

    /// <summary>
    /// Gets or sets the sampling temperature.
    /// </summary>
    [JsonPropertyName("temperature")]
    public double? Temperature { get; init; }

    /// <summary>
    /// Gets or sets whether the response is streamed as server-sent events.
    /// Omitted for regular completions.
    /// </summary>
    [JsonPropertyName("stream")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Stream { get; init; }
}
