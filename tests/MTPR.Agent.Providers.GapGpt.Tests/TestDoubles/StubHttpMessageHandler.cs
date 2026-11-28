namespace MTPR.Agent.Providers.GapGpt.Tests.TestDoubles;

public sealed class StubHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
{
    public string? RequestBody { get; private set; }

    public string? AuthorizationHeader { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        AuthorizationHeader = request.Headers.Authorization?.ToString();

        RequestBody = request.Content is null
            ? null
            : await request.Content.ReadAsStringAsync(cancellationToken);

        return response;
    }
}
