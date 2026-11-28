using System.Text.Json.Serialization;

namespace MTPR.Agent.Providers.Google.Requests;

/// <summary>
/// Body of a Gemini generate content call.
/// </summary>
internal sealed class GenerateContentRequest
{
    /// <summary>
    /// Gets or sets the system instruction.
    /// </summary>
    [JsonPropertyName("systemInstruction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GeminiContent? SystemInstruction { get; init; }

    /// <summary>
    /// Gets or sets the ordered conversation turns.
    /// </summary>
    [JsonPropertyName("contents")]
    public required IReadOnlyList<GeminiContent> Contents { get; init; }

    /// <summary>
    /// Gets or sets the generation settings.
    /// </summary>
    [JsonPropertyName("generationConfig")]
    public required GenerationConfig GenerationConfig { get; init; }
}
