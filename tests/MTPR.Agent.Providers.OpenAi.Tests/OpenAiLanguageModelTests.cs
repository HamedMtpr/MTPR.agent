using System.Net;
using System.Text;
using MTPR.Agent.Core.Exceptions;
using MTPR.Agent.Core.Models;
using MTPR.Agent.Providers.OpenAi.Options;
using MTPR.Agent.Providers.OpenAi.Tests.TestDoubles;
using Mtpr.TestUtilities;

namespace MTPR.Agent.Providers.OpenAi.Tests;

public sealed class OpenAiLanguageModelTests : UnitTest
{
    [Fact]
    public void Options_Should_TargetOpenAi_ByDefault()
    {
        var actual = new OpenAiOptions();

        actual.BaseUrl.Should().Be("https://api.openai.com/v1/");
        actual.ApiKey.Should().BeEmpty();
    }

    [Fact]
    public async Task CompleteAsync_Should_ReturnContentAndUsage()
    {
        var successBody =
            """
            {
              "choices": [
                { "message": { "content": "Hello" }, "finish_reason": "stop" }
              ],
              "usage": { "prompt_tokens": 5, "completion_tokens": 2 }
            }
            """;

        var model = createModel(createHandler(HttpStatusCode.OK, successBody));

        var actual =
            await model.CompleteAsync(createRequest(), CancellationToken.None);

        actual.Content.Should().Be("Hello");
        actual.FinishReason.Should().Be(FinishReason.Stop);
        actual.Usage.Should().Be(new TokenUsage(5, 2));
    }

    [Fact]
    public async Task CompleteAsync_Should_SendBearerHeader()
    {
        var handler = createHandler(
            HttpStatusCode.OK,
            """{ "choices": [ { } ] }""");
        var model = createModel(handler);

        await model.CompleteAsync(createRequest(), CancellationToken.None);

        handler.AuthorizationHeader.Should().Be("Bearer test-key");
    }

    [Fact]
    public async Task CompleteAsync_Should_Throw_WithStatusCode_When_ProviderFails()
    {
        var model = createModel(createHandler(HttpStatusCode.Unauthorized, "{}"));

        var actual = () =>
            model.CompleteAsync(createRequest(), CancellationToken.None);

        var exception =
            await actual.Should().ThrowAsync<LanguageModelException>();
        exception.Which.StatusCode.Should().Be(401);
        exception.Which.Message.Should().StartWith("OpenAI");
    }

    [Fact]
    public async Task CompleteAsync_Should_Throw_When_RequestCannotBeSent()
    {
        var inner = new HttpRequestException("network down");
        var httpClient = new HttpClient(new ThrowingHttpMessageHandler(inner))
        {
            BaseAddress = new Uri("https://api.openai.com/v1/"),
        };
        var model = new OpenAiLanguageModel(httpClient, createOptions());

        var actual = () =>
            model.CompleteAsync(createRequest(), CancellationToken.None);

        var exception =
            await actual.Should().ThrowAsync<LanguageModelException>();
        exception.Which.InnerException.Should().BeSameAs(inner);
    }

    [Fact]
    public async Task StreamAsync_Should_YieldDeltasAndFinishReason()
    {
        var body =
            """
            data: {"choices":[{"delta":{"content":"Hel"}}]}

            data: {"choices":[{"delta":{"content":"lo"},"finish_reason":"stop"}]}

            data: [DONE]
            """;
        var model = createModel(createHandler(HttpStatusCode.OK, body));

        var actual =
            await collectAsync(model.StreamAsync(createRequest(), CancellationToken.None));

        actual.Should().BeEquivalentTo(
        [
            new LanguageModelChunk("Hel"),
            new LanguageModelChunk("lo", FinishReason.Stop),
        ]);
    }

    [Fact]
    public async Task StreamAsync_Should_SendBearerHeader()
    {
        var handler = createHandler(HttpStatusCode.OK, "data: [DONE]");
        var model = createModel(handler);

        await collectAsync(model.StreamAsync(createRequest(), CancellationToken.None));

        handler.AuthorizationHeader.Should().Be("Bearer test-key");
    }

    [Fact]
    public async Task StreamAsync_Should_Throw_WithStatusCode_When_ProviderFails()
    {
        var model = createModel(createHandler(HttpStatusCode.TooManyRequests, "{}"));

        var actual = () =>
            collectAsync(model.StreamAsync(createRequest(), CancellationToken.None));

        var exception =
            await actual.Should().ThrowAsync<LanguageModelException>();
        exception.Which.StatusCode.Should().Be(429);
    }

    private static async Task<List<LanguageModelChunk>> collectAsync(
        IAsyncEnumerable<LanguageModelChunk> chunks)
    {
        var actual = new List<LanguageModelChunk>();

        await foreach (var chunk in chunks)
        {
            actual.Add(chunk);
        }

        return actual;
    }

    private static LanguageModelRequest createRequest()
    {
        return new LanguageModelRequest(
            "gpt-4o-mini",
            [new ChatMessage(ChatRole.User, "Hi")]);
    }

    private static StubHttpMessageHandler createHandler(
        HttpStatusCode statusCode,
        string body)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(
                body,
                Encoding.UTF8,
                "application/json"),
        };

        return new StubHttpMessageHandler(response);
    }

    private static OpenAiOptions createOptions()
    {
        return new OpenAiOptions
        {
            ApiKey = "test-key",
        };
    }

    private static OpenAiLanguageModel createModel(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.openai.com/v1/"),
        };

        return new OpenAiLanguageModel(httpClient, createOptions());
    }
}
