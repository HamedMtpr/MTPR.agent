using System.Text.Json.Serialization;

namespace MTPR.Agent.Providers.Anthropic.Requests;

/// <summary>
/// One conversation message of a Messages API call.
/// </summary>
internal sealed class MessagesMessage
{
    /// <summary>
    /// Gets or sets the role name (<c>user</c> or <c>assistant</c>).
    /// </summary>
    [JsonPropertyName("role")]
    public required string Role { get; init; }

    /// <summary>
    /// Gets or sets the text content.
    /// </summary>
    [JsonPropertyName("content")]
    public required string Content { get; init; }
}
