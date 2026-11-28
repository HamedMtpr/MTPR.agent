using System.Text.Json.Serialization;

namespace MTPR.Agent.Providers.Google.Requests;

/// <summary>
/// Generation settings of a Gemini call.
/// </summary>
internal sealed class GenerationConfig
{
    /// <summary>
    /// Gets or sets the sampling temperature.
    /// </summary>
    [JsonPropertyName("temperature")]
    public double? Temperature { get; init; }

    /// <summary>
    /// Gets or sets the upper bound on generated tokens.
    /// </summary>
    [JsonPropertyName("maxOutputTokens")]
    public int? MaxOutputTokens { get; init; }
}
