namespace MTPR.Agent.Providers.OpenAi.Tests.TestDoubles;

public sealed class ThrowingHttpMessageHandler(Exception exception) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        throw exception;
    }
}
