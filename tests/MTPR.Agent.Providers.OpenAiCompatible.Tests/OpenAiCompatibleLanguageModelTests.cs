using System.Net;
using System.Text;
using MTPR.Agent.Core.Exceptions;
using MTPR.Agent.Core.Models;
using MTPR.Agent.Providers.OpenAiCompatible.Options;
using MTPR.Agent.Providers.OpenAiCompatible.Tests.TestDoubles;
using Mtpr.TestUtilities;

namespace MTPR.Agent.Providers.OpenAiCompatible.Tests;

public sealed class OpenAiCompatibleLanguageModelTests : UnitTest
{
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

    [Theory]
    [InlineData("stop", FinishReason.Stop)]
    [InlineData("length", FinishReason.Length)]
    [InlineData("content_filter", FinishReason.ContentFilter)]
    [InlineData("other", FinishReason.Unknown)]
    public async Task CompleteAsync_Should_MapFinishReason(
        string reason,
        FinishReason expected)
    {
        var body =
            $$"""
            {
              "choices": [
                { "message": { "content": "Hi" }, "finish_reason": "{{reason}}" }
              ]
            }
            """;
        var model = createModel(createHandler(HttpStatusCode.OK, body));

        var actual =
            await model.CompleteAsync(createRequest(), CancellationToken.None);

        actual.FinishReason.Should().Be(expected);
    }

    [Fact]
    public async Task CompleteAsync_Should_UseDefaults_When_MessageAndUsageAreMissing()
    {
        var body = """{ "choices": [ { } ] }""";
        var model = createModel(createHandler(HttpStatusCode.OK, body));

        var actual =
            await model.CompleteAsync(createRequest(), CancellationToken.None);

        actual.Content.Should().BeEmpty();
        actual.FinishReason.Should().Be(FinishReason.Unknown);
        actual.Usage.Should().BeNull();
    }

    [Fact]
    public async Task CompleteAsync_Should_SendBearerHeader_When_ApiKeyIsSet()
    {
        var handler = createHandler(
            HttpStatusCode.OK,
            """{ "choices": [ { } ] }""");
        var model = createModel(handler, "test-key");

        await model.CompleteAsync(createRequest(), CancellationToken.None);

        handler.AuthorizationHeader.Should().Be("Bearer test-key");
    }

    [Fact]
    public async Task CompleteAsync_Should_OmitAuthorizationHeader_When_ApiKeyIsEmpty()
    {
        var handler = createHandler(
            HttpStatusCode.OK,
            """{ "choices": [ { } ] }""");
        var model = createModel(handler, string.Empty);

        await model.CompleteAsync(createRequest(), CancellationToken.None);

        handler.AuthorizationHeader.Should().BeNull();
    }

    [Theory]
    [InlineData(ChatRole.System, "system")]
    [InlineData(ChatRole.User, "user")]
    [InlineData(ChatRole.Assistant, "assistant")]
    public async Task CompleteAsync_Should_SendRoleName(
        ChatRole role,
        string expected)
    {
        var handler = createHandler(
            HttpStatusCode.OK,
            """{ "choices": [ { } ] }""");
        var model = createModel(handler);
        var request = new LanguageModelRequest(
            "gpt-4o-mini",
            [new ChatMessage(role, "Hi")]);

        await model.CompleteAsync(request, CancellationToken.None);

        handler.RequestBody.Should().Contain($"\"role\":\"{expected}\"");
    }

    [Fact]
    public async Task CompleteAsync_Should_Throw_When_RoleIsUnsupported()
    {
        var model = createModel(
            createHandler(HttpStatusCode.OK, """{ "choices": [ { } ] }"""));
        var request = new LanguageModelRequest(
            "gpt-4o-mini",
            [new ChatMessage((ChatRole)99, "Hi")]);

        var actual = () => model.CompleteAsync(request, CancellationToken.None);

        await actual.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task CompleteAsync_Should_Throw_When_RequestIsNull()
    {
        var model = createModel(createHandler(HttpStatusCode.OK, "{}"));

        var actual = () => model.CompleteAsync(null!, CancellationToken.None);

        await actual.Should().ThrowAsync<ArgumentNullException>();
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
    }

    [Fact]
    public async Task CompleteAsync_Should_Throw_When_ResponseHasNoChoices()
    {
        var model = createModel(
            createHandler(HttpStatusCode.OK, """{ "choices": [] }"""));

        var actual = () =>
            model.CompleteAsync(createRequest(), CancellationToken.None);

        await actual.Should().ThrowAsync<LanguageModelException>();
    }

    [Fact]
    public async Task CompleteAsync_Should_Throw_When_ResponseBodyIsNull()
    {
        var model = createModel(createHandler(HttpStatusCode.OK, "null"));

        var actual = () =>
            model.CompleteAsync(createRequest(), CancellationToken.None);

        await actual.Should().ThrowAsync<LanguageModelException>();
    }

    [Fact]
    public async Task CompleteAsync_Should_WrapException_When_RequestCannotBeSent()
    {
        var inner = new HttpRequestException("offline");
        var model = createModel(new ThrowingHttpMessageHandler(inner));

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
            : keep-alive

            data: {"choices":[{"delta":{"content":"Hel"}}]}

            data: {"choices":[{"delta":{"content":"lo"}}]}

            data: {"choices":[{"delta":{},"finish_reason":"stop"}]}

            data: [DONE]

            """;
        var model = createModel(createHandler(HttpStatusCode.OK, body));

        var actual =
            await collectAsync(model.StreamAsync(createRequest(), CancellationToken.None));

        actual.Should().BeEquivalentTo(
        [
            new LanguageModelChunk("Hel"),
            new LanguageModelChunk("lo"),
            new LanguageModelChunk(string.Empty, FinishReason.Stop),
        ],
        options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task StreamAsync_Should_SendStreamFlag()
    {
        var handler = createHandler(HttpStatusCode.OK, "data: [DONE]\n\n");
        var model = createModel(handler);

        await collectAsync(model.StreamAsync(createRequest(), CancellationToken.None));

        handler.RequestBody.Should().Contain("\"stream\":true");
    }

    [Fact]
    public async Task CompleteAsync_Should_NotSendStreamFlag()
    {
        var handler = createHandler(
            HttpStatusCode.OK,
            """{ "choices": [ { } ] }""");
        var model = createModel(handler);

        await model.CompleteAsync(createRequest(), CancellationToken.None);

        handler.RequestBody.Should().NotContain("stream");
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

    [Fact]
    public async Task StreamAsync_Should_Throw_WhenRequestCannotBeSent()
    {
        var model = createModel(new ThrowingHttpMessageHandler(new HttpRequestException("offline")));

        var actual = () =>
            collectAsync(model.StreamAsync(createRequest(), CancellationToken.None));

        await actual.Should().ThrowAsync<LanguageModelException>();
    }

    [Fact]
    public async Task StreamAsync_Should_Throw_When_RequestIsNull()
    {
        var model = createModel(createHandler(HttpStatusCode.OK, "data: [DONE]"));

        var actual = () =>
            collectAsync(model.StreamAsync(null!, CancellationToken.None));

        await actual.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task StreamAsync_Should_SkipEntries_When_ChoiceIsMissing()
    {
        var body =
            """
            data: null

            data: {"choices":[]}

            data: {"choices":[{"delta":{"content":"Hi"}}]}
            """;
        var model = createModel(createHandler(HttpStatusCode.OK, body));

        var actual =
            await collectAsync(model.StreamAsync(createRequest(), CancellationToken.None));

        actual.Should().BeEquivalentTo([new LanguageModelChunk("Hi")]);
    }

    [Fact]
    public async Task StreamAsync_Should_UseEmptyContent_When_DeltaIsMissing()
    {
        var model = createModel(
            createHandler(HttpStatusCode.OK, """data: {"choices":[{}]}"""));

        var actual =
            await collectAsync(model.StreamAsync(createRequest(), CancellationToken.None));

        actual.Should().BeEquivalentTo([new LanguageModelChunk(string.Empty)]);
    }

    [Fact]
    public async Task StreamAsync_Should_SendBearerHeader_When_ApiKeyIsSet()
    {
        var handler = createHandler(HttpStatusCode.OK, "data: [DONE]");
        var model = createModel(handler);

        await collectAsync(model.StreamAsync(createRequest(), CancellationToken.None));

        handler.AuthorizationHeader.Should().Be("Bearer test-key");
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

    [Fact]
    public void Options_Should_TargetLocalOllama_ByDefault()
    {
        var actual = new OpenAiCompatibleOptions();

        actual.BaseUrl.Should().Be("http://localhost:11434/v1/");
        actual.ApiKey.Should().BeEmpty();
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

    private static OpenAiCompatibleLanguageModel createModel(
        HttpMessageHandler handler,
        string apiKey = "test-key")
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:11434/v1/"),
        };

        return new OpenAiCompatibleLanguageModel(
            httpClient,
            new OpenAiCompatibleOptions
            {
                ApiKey = apiKey,
            });
    }
}
