namespace MTPR.Agent.Providers.Anthropic.Options;

/// <summary>
/// Settings used to reach the Anthropic API.
/// </summary>
public sealed class AnthropicOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "Anthropic";

    /// <summary>
    /// Gets or sets the base address of the Anthropic API.
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.anthropic.com/v1/";

    /// <summary>
    /// Gets or sets the API key sent in the <c>x-api-key</c> header.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the API version sent in the <c>anthropic-version</c> header.
    /// </summary>
    public string ApiVersion { get; set; } = "2023-06-01";

    /// <summary>
    /// Gets or sets the token limit used when a request does not define one.
    /// The Messages API requires a limit on every call.
    /// </summary>
    public int DefaultMaxTokens { get; set; } = 4096;
}
