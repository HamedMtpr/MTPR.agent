namespace MTPR.Agent.Providers.OpenAiCompatible.Options;

/// <summary>
/// Settings used to reach an OpenAI-compatible API.
/// </summary>
public sealed class OpenAiCompatibleOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "OpenAiCompatible";

    /// <summary>
    /// Gets or sets the base address of the API. The default targets a local Ollama server.
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:11434/v1/";

    /// <summary>
    /// Gets or sets the API key sent as a bearer token. Leave empty for servers that need no key.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;
}
