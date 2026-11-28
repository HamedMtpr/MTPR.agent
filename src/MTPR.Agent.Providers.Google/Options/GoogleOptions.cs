namespace MTPR.Agent.Providers.Google.Options;

/// <summary>
/// Settings used to reach the Google Gemini API.
/// </summary>
public sealed class GoogleOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "Google";

    /// <summary>
    /// Gets or sets the base address of the Gemini API.
    /// </summary>
    public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com/v1beta/";

    /// <summary>
    /// Gets or sets the API key sent in the <c>x-goog-api-key</c> header.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;
}
