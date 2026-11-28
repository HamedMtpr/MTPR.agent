using System.Text.Json.Serialization;

namespace MTPR.Agent.Providers.Google.Responses;

/// <summary>
/// Body of a Gemini response, also used for each streamed event.
/// </summary>
internal sealed class GenerateContentResponse
{
    /// <summary>
    /// Gets the generated candidates.
    /// </summary>
    [JsonPropertyName("candidates")]
    public List<GeminiCandidate> Candidates { get; init; } = [];

    /// <summary>
    /// Gets or sets the token usage.
    /// </summary>
    [JsonPropertyName("usageMetadata")]
    public GeminiUsage? UsageMetadata { get; init; }

    /// <summary>
    /// Gets the text of the first candidate.
    /// </summary>
    [JsonIgnore]
    public string Text =>
        string.Concat(
            Candidates.FirstOrDefault()?.Content?.Parts.Select(part => part.Text)
            ?? []);
}
