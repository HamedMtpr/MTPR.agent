namespace MTPR.Agent.Providers.Anthropic.Tests.TestDoubles;

public sealed class StubHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
{
    public string? RequestBody { get; private set; }

    public string? ApiKeyHeader { get; private set; }

    public string? VersionHeader { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ApiKeyHeader = request.Headers.GetValues("x-api-key").SingleOrDefault();
        VersionHeader = request.Headers.GetValues("anthropic-version").SingleOrDefault();

        RequestBody = request.Content is null
            ? null
            : await request.Content.ReadAsStringAsync(cancellationToken);

        return response;
    }
}
