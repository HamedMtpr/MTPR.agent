using System.Net;
using System.Text;
using System.Text.Json;
using MTPR.Agent.Core.Exceptions;
using MTPR.Agent.Core.Models;
using MTPR.Agent.Providers.Anthropic.Options;
using MTPR.Agent.Providers.Anthropic.Tests.TestDoubles;
using Mtpr.TestUtilities;

namespace MTPR.Agent.Providers.Anthropic.Tests;

public sealed class AnthropicLanguageModelTests : UnitTest
{
    [Fact]
    public void Options_Should_TargetAnthropic_ByDefault()
    {
        var actual = new AnthropicOptions();

        actual.BaseUrl.Should().Be("https://api.anthropic.com/v1/");
        actual.ApiKey.Should().BeEmpty();
        actual.ApiVersion.Should().Be("2023-06-01");
        actual.DefaultMaxTokens.Should().Be(4096);
    }

    [Fact]
    public async Task CompleteAsync_Should_ReturnTextAndUsage()
    {
        var successBody =
            """
            {
              "content": [
                { "type": "text", "text": "Hel" },
                { "type": "tool_use" },
                { "type": "text", "text": "lo" }
              ],
              "stop_reason": "end_turn",
              "usage": { "input_tokens": 5, "output_tokens": 2 }
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
    [InlineData("stop_sequence", FinishReason.Stop)]
    [InlineData("max_tokens", FinishReason.Length)]
    [InlineData("refusal", FinishReason.ContentFilter)]
    [InlineData("tool_use", FinishReason.Unknown)]
    [InlineData(null, FinishReason.Unknown)]
    public async Task CompleteAsync_Should_MapStopReason(
        string? stopReason,
        FinishReason expected)
    {
        var body = JsonSerializer.Serialize(new { content = Array.Empty<object>(), stop_reason = stopReason });
        var model = createModel(createHandler(HttpStatusCode.OK, body));

        var actual =
            await model.CompleteAsync(createRequest(), CancellationToken.None);

        actual.FinishReason.Should().Be(expected);
        actual.Usage.Should().BeNull();
    }

    [Fact]
    public async Task CompleteAsync_Should_Throw_When_BodyIsEmpty()
    {
        var model = createModel(createHandler(HttpStatusCode.OK, "null"));

        var actual = () =>
            model.CompleteAsync(createRequest(), CancellationToken.None);

        await actual.Should().ThrowAsync<LanguageModelException>();
    }

    [Fact]
    public async Task CompleteAsync_Should_SendHeaders()
    {
        var handler = createHandler(HttpStatusCode.OK, "{}");
        var model = createModel(handler);

        await model.CompleteAsync(createRequest(), CancellationToken.None);

        handler.ApiKeyHeader.Should().Be("test-key");
        handler.VersionHeader.Should().Be("2023-06-01");
    }

    [Fact]
    public async Task CompleteAsync_Should_SeparateSystemPrompt_And_UseDefaultMaxTokens()
    {
        var handler = createHandler(HttpStatusCode.OK, "{}");
        var model = createModel(handler);
        var request = new LanguageModelRequest(
            "claude-sonnet",
            [
                new ChatMessage(ChatRole.System, "Be brief."),
                new ChatMessage(ChatRole.User, "Hi"),
                new ChatMessage(ChatRole.Assistant, "Hello"),
                new ChatMessage(ChatRole.System, "Be kind."),
            ]);

        await model.CompleteAsync(request, CancellationToken.None);

        using var document = JsonDocument.Parse(handler.RequestBody!);
        var root = document.RootElement;
        root.GetProperty("system").GetString().Should().Be("Be brief.\n\nBe kind.");
        root.GetProperty("max_tokens").GetInt32().Should().Be(4096);
        root.GetProperty("messages").GetArrayLength().Should().Be(2);
        root.GetProperty("messages")[1].GetProperty("role").GetString().Should().Be("assistant");
        root.TryGetProperty("stream", out _).Should().BeFalse();
    }

    [Fact]
    public async Task CompleteAsync_Should_OmitSystem_And_HonorRequestLimits()
    {
        var handler = createHandler(HttpStatusCode.OK, "{}");
        var model = createModel(handler);
        var request = new LanguageModelRequest(
            "claude-sonnet",
            [new ChatMessage(ChatRole.User, "Hi")],
            Temperature: 0.5,
            MaxOutputTokens: 100);

        await model.CompleteAsync(request, CancellationToken.None);

        using var document = JsonDocument.Parse(handler.RequestBody!);
        var root = document.RootElement;
        root.TryGetProperty("system", out _).Should().BeFalse();
        root.GetProperty("max_tokens").GetInt32().Should().Be(100);
        root.GetProperty("temperature").GetDouble().Should().Be(0.5);
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
        exception.Which.Message.Should().StartWith("Anthropic");
    }

    [Fact]
    public async Task CompleteAsync_Should_Throw_When_RequestCannotBeSent()
    {
        var inner = new HttpRequestException("network down");
        var httpClient = new HttpClient(new ThrowingHttpMessageHandler(inner))
        {
            BaseAddress = new Uri("https://api.anthropic.com/v1/"),
        };
        var model = new AnthropicLanguageModel(httpClient, createOptions());

        var actual = () =>
            model.CompleteAsync(createRequest(), CancellationToken.None);

        var exception =
            await actual.Should().ThrowAsync<LanguageModelException>();
        exception.Which.InnerException.Should().BeSameAs(inner);
    }

    [Fact]
    public async Task StreamAsync_Should_YieldDeltasAndStopReason()
    {
        var body =
            """
            event: message_start
            data: {"type":"message_start"}

            event: content_block_delta
            data: {"type":"content_block_delta","delta":{"type":"text_delta","text":"Hel"}}

            data: {"type":"content_block_delta","delta":{"type":"input_json_delta"}}

            data: {"type":"content_block_delta","delta":{"type":"text_delta","text":"lo"}}

            data: {"type":"message_delta","delta":{"stop_reason":"max_tokens"}}

            data: {"type":"message_stop"}

            data: {"type":"content_block_delta","delta":{"text":"ignored"}}
            """;
        var handler = createHandler(HttpStatusCode.OK, body);
        var model = createModel(handler);

        var actual =
            await collectAsync(model.StreamAsync(createRequest(), CancellationToken.None));

        actual.Should().BeEquivalentTo(
        [
            new LanguageModelChunk("Hel"),
            new LanguageModelChunk("lo"),
            new LanguageModelChunk(string.Empty, FinishReason.Length),
        ]);
        handler.ApiKeyHeader.Should().Be("test-key");
        handler.RequestBody.Should().Contain("\"stream\":true");
    }

    [Fact]
    public async Task StreamAsync_Should_Throw_When_StreamReportsError()
    {
        var body = """data: {"type":"error"}""";
        var model = createModel(createHandler(HttpStatusCode.OK, body));

        var actual = () =>
            collectAsync(model.StreamAsync(createRequest(), CancellationToken.None));

        await actual.Should().ThrowAsync<LanguageModelException>();
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
            "claude-sonnet",
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

    private static AnthropicOptions createOptions()
    {
        return new AnthropicOptions
        {
            ApiKey = "test-key",
        };
    }

    private static AnthropicLanguageModel createModel(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.anthropic.com/v1/"),
        };

        return new AnthropicLanguageModel(httpClient, createOptions());
    }
}
