using System.Net.Http.Headers;
using MTPR.Agent.Providers.OpenAiCompatible.Options;
using MTPR.Agent.Providers.OpenAiProtocol;

namespace MTPR.Agent.Providers.OpenAiCompatible;

/// <summary>
/// Language model backed by any server that implements the OpenAI chat completion API.
/// The API key is optional because some servers need none.
/// </summary>
/// <param name="httpClient">HTTP client whose base address points to the API.</param>
/// <param name="options">Connection settings.</param>
public sealed class OpenAiCompatibleLanguageModel(HttpClient httpClient,
    OpenAiCompatibleOptions options) : ChatCompletionsLanguageModel(httpClient)
{
    /// <inheritdoc />
    protected override string ProviderName => "OpenAI-compatible";

    /// <inheritdoc />
    protected override void ApplyAuthorization(HttpRequestMessage request)
    {
        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            return;
        }

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer",
                options.ApiKey);
    }
}
