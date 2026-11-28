using System.Text.Json.Serialization;

namespace MTPR.Agent.Providers.Google.Responses;

/// <summary>
/// Token usage reported by Gemini.
/// </summary>
internal sealed class GeminiUsage
{
    /// <summary>
    /// Gets or sets the prompt token count.
    /// </summary>
    [JsonPropertyName("promptTokenCount")]
    public int PromptTokenCount { get; init; }

    /// <summary>
    /// Gets or sets the generated token count.
    /// </summary>
    [JsonPropertyName("candidatesTokenCount")]
    public int CandidatesTokenCount { get; init; }
}
