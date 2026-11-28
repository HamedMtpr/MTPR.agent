using System.Text.Json.Serialization;
using MTPR.Agent.Providers.Google.Requests;

namespace MTPR.Agent.Providers.Google.Responses;

/// <summary>
/// One generated candidate.
/// </summary>
internal sealed class GeminiCandidate
{
    /// <summary>
    /// Gets or sets the generated content.
    /// </summary>
    [JsonPropertyName("content")]
    public GeminiContent? Content { get; init; }

    /// <summary>
    /// Gets or sets the reason generation stopped.
    /// </summary>
    [JsonPropertyName("finishReason")]
    public string? FinishReason { get; init; }
}
