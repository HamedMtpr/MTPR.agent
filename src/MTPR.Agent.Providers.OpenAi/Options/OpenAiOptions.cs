namespace MTPR.Agent.Providers.OpenAi.Options;

/// <summary>
/// Settings used to reach the OpenAI API.
/// </summary>
public sealed class OpenAiOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "OpenAi";

    /// <summary>
    /// Gets or sets the base address of the OpenAI API.
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.openai.com/v1/";

    /// <summary>
    /// Gets or sets the API key sent as a bearer token.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;
}
