using System.Text.Json.Serialization;

namespace MTPR.Agent.Providers.Google.Requests;

/// <summary>
/// Text part of a Gemini content entry.
/// </summary>
internal sealed class GeminiPart
{
    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; init; }
}
