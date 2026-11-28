namespace MTPR.Agent.Providers.Google.Tests.TestDoubles;

public sealed class StubHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
{
    public string? RequestBody { get; private set; }

    public string? ApiKeyHeader { get; private set; }

    public string? RequestPath { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ApiKeyHeader = request.Headers.GetValues("x-goog-api-key").SingleOrDefault();
        RequestPath = request.RequestUri?.PathAndQuery;

        RequestBody = request.Content is null
            ? null
            : await request.Content.ReadAsStringAsync(cancellationToken);

        return response;
    }
}
