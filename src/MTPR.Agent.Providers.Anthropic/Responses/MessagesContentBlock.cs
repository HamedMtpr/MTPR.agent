using System.Text.Json.Serialization;

namespace MTPR.Agent.Providers.Anthropic.Responses;

/// <summary>
/// One content block of a Messages API response.
/// </summary>
internal sealed class MessagesContentBlock
{
    /// <summary>
    /// Gets or sets the block type, such as <c>text</c>.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    /// <summary>
    /// Gets or sets the text of a <c>text</c> block.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; init; }
}
