using System.Net.Http.Headers;
using MTPR.Agent.Providers.GapGpt.Options;
using MTPR.Agent.Providers.OpenAiProtocol;

namespace MTPR.Agent.Providers.GapGpt;

/// <summary>
/// Language model backed by the GapGPT chat completion API.
/// </summary>
/// <param name="httpClient">HTTP client used to reach the API.</param>
/// <param name="options">Connection settings.</param>
public sealed class GapGptLanguageModel(HttpClient httpClient,
    GapGptOptions options) : ChatCompletionsLanguageModel(httpClient)
{
    /// <inheritdoc />
    protected override string ProviderName => "GapGPT";

    /// <inheritdoc />
    protected override void ApplyAuthorization(HttpRequestMessage request)
    {
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer",
                options.ApiKey);
    }
}
