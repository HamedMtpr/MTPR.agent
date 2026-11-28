using System.Text.Json.Serialization;

namespace MTPR.Agent.Providers.Google.Requests;

/// <summary>
/// One content entry (a turn, or the system instruction) of a Gemini call.
/// </summary>
internal sealed class GeminiContent
{
    /// <summary>
    /// Gets or sets the role name (<c>user</c> or <c>model</c>).
    /// Omitted for the system instruction.
    /// </summary>
    [JsonPropertyName("role")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Role { get; init; }

    /// <summary>
    /// Gets or sets the parts of this entry.
    /// </summary>
    [JsonPropertyName("parts")]
    public List<GeminiPart> Parts { get; init; } = [];
}
