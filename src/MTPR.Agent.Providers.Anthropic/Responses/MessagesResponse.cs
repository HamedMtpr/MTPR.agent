using System.Text.Json.Serialization;

namespace MTPR.Agent.Providers.Anthropic.Responses;

/// <summary>
/// Body of a Messages API response.
/// </summary>
internal sealed class MessagesResponse
{
    /// <summary>
    /// Gets the content blocks.
    /// </summary>
    [JsonPropertyName("content")]
    public List<MessagesContentBlock> Content { get; init; } = [];

    /// <summary>
    /// Gets or sets the reason generation stopped.
    /// </summary>
    [JsonPropertyName("stop_reason")]
    public string? StopReason { get; init; }

    /// <summary>
    /// Gets or sets the token usage.
    /// </summary>
    [JsonPropertyName("usage")]
    public MessagesUsage? Usage { get; init; }
}
