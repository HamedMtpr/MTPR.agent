using System.Net.Http.Headers;
using MTPR.Agent.Providers.OpenAi.Options;
using MTPR.Agent.Providers.OpenAiProtocol;

namespace MTPR.Agent.Providers.OpenAi;

/// <summary>
/// Language model backed by the OpenAI chat completion API.
/// </summary>
/// <param name="httpClient">HTTP client used to reach the API.</param>
/// <param name="options">Connection settings.</param>
public sealed class OpenAiLanguageModel(HttpClient httpClient,
    OpenAiOptions options) : ChatCompletionsLanguageModel(httpClient)
{
    /// <inheritdoc />
    protected override string ProviderName => "OpenAI";

    /// <inheritdoc />
    protected override void ApplyAuthorization(HttpRequestMessage request)
    {
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer",
                options.ApiKey);
    }
}
