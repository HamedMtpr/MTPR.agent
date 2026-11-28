namespace MTPR.Agent.Providers.GapGpt.Options;

/// <summary>
/// Settings used to reach the GapGPT API.
/// </summary>
public sealed class GapGptOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "GapGpt";

    /// <summary>
    /// Gets or sets the base address of the GapGPT API.
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.gapgpt.app/v1/";

    /// <summary>
    /// Gets or sets the API key sent as a bearer token.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default model identifier.
    /// </summary>
    public string DefaultModel { get; set; } = "gpt-4o-mini";
}
