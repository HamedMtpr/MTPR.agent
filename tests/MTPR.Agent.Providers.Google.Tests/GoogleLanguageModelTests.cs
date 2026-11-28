using System.Net;
using System.Text;
using System.Text.Json;
using MTPR.Agent.Core.Exceptions;
using MTPR.Agent.Core.Models;
using MTPR.Agent.Providers.Google.Options;
using MTPR.Agent.Providers.Google.Tests.TestDoubles;
using Mtpr.TestUtilities;

namespace MTPR.Agent.Providers.Google.Tests;

public sealed class GoogleLanguageModelTests : UnitTest
{
    [Fact]
    public void Options_Should_TargetGemini_ByDefault()
    {
        var actual = new GoogleOptions();

        actual.BaseUrl.Should().Be("https://generativelanguage.googleapis.com/v1beta/");
        actual.ApiKey.Should().BeEmpty();
    }

    [Fact]
    public async Task CompleteAsync_Should_ReturnTextAndUsage()
    {
        var successBody =
            """
            {
              "candidates": [
                {
                  "content": { "parts": [ { "text": "Hel" }, { "text": "lo" } ] },
                  "finishReason": "STOP"
                }
              ],
              "usageMetadata": { "promptTokenCount": 5, "candidatesTokenCount": 2 }
            }
            """;
        var handler = createHandler(HttpStatusCode.OK, successBody);
        var model = createModel(handler);

        var actual =
            await model.CompleteAsync(createRequest(), CancellationToken.None);

        actual.Content.Should().Be("Hello");
        actual.FinishReason.Should().Be(FinishReason.Stop);
        actual.Usage.Should().Be(new TokenUsage(5, 2));
        handler.RequestPath.Should().EndWith("models/gemini-pro:generateContent");
        handler.ApiKeyHeader.Should().Be("test-key");
    }

    [Theory]
    [InlineData("MAX_TOKENS", FinishReason.Length)]
    [InlineData("SAFETY", FinishReason.ContentFilter)]
    [InlineData("RECITATION", FinishReason.ContentFilter)]
    [InlineData("OTHER", FinishReason.Unknown)]
    [InlineData(null, FinishReason.Unknown)]
    public async Task CompleteAsync_Should_MapFinishReason(
        string? reason,
        FinishReason expected)
    {
        var body = JsonSerializer.Serialize(
            new { candidates = new[] { new { finishReason = reason } } });
        var model = createModel(createHandler(HttpStatusCode.OK, body));

        var actual =
            await model.CompleteAsync(createRequest(), CancellationToken.None);

        actual.FinishReason.Should().Be(expected);
        actual.Content.Should().BeEmpty();
        actual.Usage.Should().BeNull();
    }

    [Fact]
    public async Task CompleteAsync_Should_ReturnUnknown_When_NoCandidates()
    {
        var model = createModel(createHandler(HttpStatusCode.OK, "{}"));

        var actual =
            await model.CompleteAsync(createRequest(), CancellationToken.None);

        actual.Content.Should().BeEmpty();
        actual.FinishReason.Should().Be(FinishReason.Unknown);
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
    public async Task CompleteAsync_Should_MapRolesAndSystemInstruction()
    {
        var handler = createHandler(HttpStatusCode.OK, "{}");
        var model = createModel(handler);
        var request = new LanguageModelRequest(
            "gemini-pro",
            [
                new ChatMessage(ChatRole.System, "Be brief."),
                new ChatMessage(ChatRole.User, "Hi"),
                new ChatMessage(ChatRole.Assistant, "Hello"),
                new ChatMessage(ChatRole.System, "Be kind."),
            ],
            Temperature: 0.5,
            MaxOutputTokens: 100);

        await model.CompleteAsync(request, CancellationToken.None);

        using var document = JsonDocument.Parse(handler.RequestBody!);
        var root = document.RootElement;
        var system = root.GetProperty("systemInstruction");
        system.GetProperty("parts")[0].GetProperty("text").GetString()
            .Should().Be("Be brief.\n\nBe kind.");
        system.TryGetProperty("role", out _).Should().BeFalse();
        root.GetProperty("contents").GetArrayLength().Should().Be(2);
        root.GetProperty("contents")[1].GetProperty("role").GetString()
            .Should().Be("model");
        root.GetProperty("generationConfig").GetProperty("maxOutputTokens")
            .GetInt32().Should().Be(100);
        root.GetProperty("generationConfig").GetProperty("temperature")
            .GetDouble().Should().Be(0.5);
    }

    [Fact]
    public async Task CompleteAsync_Should_OmitSystemInstruction_When_NoSystemMessage()
    {
        var handler = createHandler(HttpStatusCode.OK, "{}");
        var model = createModel(handler);

        await model.CompleteAsync(createRequest(), CancellationToken.None);

        using var document = JsonDocument.Parse(handler.RequestBody!);
        document.RootElement.TryGetProperty("systemInstruction", out _)
            .Should().BeFalse();
    }

    [Fact]
    public async Task CompleteAsync_Should_Throw_WithStatusCode_When_ProviderFails()
    {
        var model = createModel(createHandler(HttpStatusCode.Forbidden, "{}"));

        var actual = () =>
            model.CompleteAsync(createRequest(), CancellationToken.None);

        var exception =
            await actual.Should().ThrowAsync<LanguageModelException>();
        exception.Which.StatusCode.Should().Be(403);
        exception.Which.Message.Should().StartWith("Google");
    }

    [Fact]
    public async Task CompleteAsync_Should_Throw_When_RequestCannotBeSent()
    {
        var inner = new HttpRequestException("network down");
        var httpClient = new HttpClient(new ThrowingHttpMessageHandler(inner))
        {
            BaseAddress = new Uri("https://example.test/v1beta/"),
        };
        var model = new GoogleLanguageModel(httpClient, createOptions());

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

            data: {"candidates":[{"content":{"parts":[{"text":"Hel"}]}}]}

            data: null

            data: {"candidates":[{"content":{"parts":[{"text":"lo"}]},"finishReason":"STOP"}]}
            """;
        var handler = createHandler(HttpStatusCode.OK, body);
        var model = createModel(handler);

        var actual = await collectAsync(
            model.StreamAsync(createRequest(), CancellationToken.None));

        actual.Should().BeEquivalentTo(
        [
            new LanguageModelChunk("Hel"),
            new LanguageModelChunk("lo", FinishReason.Stop),
        ]);
        handler.RequestPath.Should()
            .EndWith("models/gemini-pro:streamGenerateContent?alt=sse");
        handler.ApiKeyHeader.Should().Be("test-key");
    }

    [Fact]
    public async Task StreamAsync_Should_Throw_WithStatusCode_When_ProviderFails()
    {
        var model = createModel(createHandler(HttpStatusCode.TooManyRequests, "{}"));

        var actual = () => collectAsync(
            model.StreamAsync(createRequest(), CancellationToken.None));

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
            "gemini-pro",
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

    private static GoogleOptions createOptions()
    {
        return new GoogleOptions
        {
            ApiKey = "test-key",
        };
    }

    private static GoogleLanguageModel createModel(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.test/v1beta/"),
        };

        return new GoogleLanguageModel(httpClient, createOptions());
    }
}
