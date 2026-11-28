# M1 — LLM Integration: Implementation Details

- [Back to index](index.md)

## Solution layout

```text
MTPR.Agent.slnx
src/
  MTPR.Agent.Core/
    Abstractions/
      IAgentRunner.cs
      IAgentTask.cs
      ILanguageModel.cs
    Exceptions/
      LanguageModelException.cs
    Models/
      AgentResult.cs
      AgentStatus.cs
      AgentTask.cs
      ChatMessage.cs
      ChatRole.cs
      Conversation.cs
      FinishReason.cs
      LanguageModelRequest.cs
      LanguageModelResponse.cs
      TokenUsage.cs
  MTPR.Agent.Providers.OpenAiProtocol/
    ChatCompletionsLanguageModel.cs
    Mapping/
      FinishReasonMapper.cs
      RoleMapper.cs
    Requests/
      ChatCompletionMessage.cs
      ChatCompletionRequest.cs
    Responses/
      ChatCompletionChoice.cs
      ChatCompletionMessageResult.cs
      ChatCompletionResponse.cs
      ChatCompletionUsage.cs
  MTPR.Agent.Providers.Anthropic/
    AnthropicLanguageModel.cs
    Mapping/
      StopReasonMapper.cs
    Options/
      AnthropicOptions.cs
    Requests/
      MessagesMessage.cs
      MessagesRequest.cs
    Responses/
      MessagesContentBlock.cs
      MessagesResponse.cs
      MessagesStreamDelta.cs
      MessagesStreamEvent.cs
      MessagesUsage.cs
  MTPR.Agent.Providers.GapGpt/
    GapGptLanguageModel.cs
    Options/
      GapGptOptions.cs
  MTPR.Agent.Providers.Google/
    GoogleLanguageModel.cs
    Mapping/
      FinishReasonMapper.cs
    Options/
      GoogleOptions.cs
    Requests/
      GeminiContent.cs
      GeminiPart.cs
      GenerateContentRequest.cs
      GenerationConfig.cs
    Responses/
      GeminiCandidate.cs
      GeminiUsage.cs
      GenerateContentResponse.cs
  MTPR.Agent.Providers.OpenAi/
    OpenAiLanguageModel.cs
    Options/
      OpenAiOptions.cs
  MTPR.Agent.Providers.OpenAiCompatible/
    OpenAiCompatibleLanguageModel.cs
    Options/
      OpenAiCompatibleOptions.cs
tests/
  MTPR.Agent.Core.Tests/
    Exceptions/
      LanguageModelExceptionTests.cs
    Models/
      ConversationTests.cs
      TokenUsageTests.cs
  MTPR.Agent.Providers.Anthropic.Tests/
    AnthropicLanguageModelTests.cs
    TestDoubles/
      StubHttpMessageHandler.cs
      ThrowingHttpMessageHandler.cs
  MTPR.Agent.Providers.GapGpt.Tests/
    GapGptLanguageModelTests.cs
    TestDoubles/
      StubHttpMessageHandler.cs
      ThrowingHttpMessageHandler.cs
  MTPR.Agent.Providers.Google.Tests/
    GoogleLanguageModelTests.cs
    TestDoubles/
      StubHttpMessageHandler.cs
      ThrowingHttpMessageHandler.cs
  MTPR.Agent.Providers.OpenAi.Tests/
    OpenAiLanguageModelTests.cs
    TestDoubles/
      StubHttpMessageHandler.cs
      ThrowingHttpMessageHandler.cs
  MTPR.Agent.Providers.OpenAiCompatible.Tests/
    OpenAiCompatibleLanguageModelTests.cs
    TestDoubles/
      StubHttpMessageHandler.cs
      ThrowingHttpMessageHandler.cs
```

## Design

- `ILanguageModel` is the provider-independent contract. Core only knows `LanguageModelRequest` and `LanguageModelResponse`.
- `Conversation` stores ordered messages and optional system instructions. `CreateRequest` places the system instruction first and omits it when blank.
- `LanguageModelException` is the single error type raised by providers. It carries the HTTP status code when the provider returned one.
- Each provider lives in its own project so that Core has no HTTP or JSON dependency.
- `OpenAiProtocol` implements the OpenAI `chat/completions` protocol once (DRY). Its wire models and mappers are `internal`.
- `ChatCompletionsLanguageModel` is a template method. Derived providers supply only `ProviderName` and `ApplyAuthorization`.
- Providers using other protocols (Anthropic, Google) do not use this base.

## GapGPT configuration

`GapGptOptions` is bound from the `GapGpt` configuration section:

| Property | Default |
|----------|---------|
| `BaseUrl` | `https://api.gapgpt.app/v1/` |
| `ApiKey` | empty (must be set) |
| `DefaultModel` | `gpt-4o-mini` |

## OpenAI-compatible configuration

`OpenAiCompatibleOptions` is bound from the `OpenAiCompatible` configuration section. `BaseUrl` is the address the host application assigns to the `HttpClient`. It defaults to the Ollama endpoint, so local models work without a key.

| Property | Default |
|----------|---------|
| `BaseUrl` | `http://localhost:11434/v1/` (Ollama) |
| `ApiKey` | empty (optional; the `Authorization` header is omitted when empty) |

## Local model execution

Local models run through `OpenAiCompatibleLanguageModel`, because Ollama, LM Studio, llama.cpp server, and vLLM expose the OpenAI chat completions API. No extra provider is needed. Both `CompleteAsync` and `StreamAsync` work, and the API key stays empty.

| Server | `BaseUrl` |
|--------|-----------|
| Ollama | `http://localhost:11434/v1/` |
| LM Studio | `http://localhost:1234/v1/` |
| llama.cpp server | `http://localhost:8080/v1/` |

## Shared OpenAI protocol

`GapGptLanguageModel` and `OpenAiLanguageModel` always send a Bearer token. `OpenAiCompatibleLanguageModel` sends it only when an API key is configured. Error messages are prefixed with the provider name. Provider projects reference `OpenAiProtocol`, which references `Core`.

## Streaming design

`ILanguageModel` exposes two methods:

- `CompleteAsync` returns a single `LanguageModelResponse` and is unchanged.
- `StreamAsync` returns `IAsyncEnumerable<LanguageModelChunk>`. Each chunk carries a `ContentDelta` and, on the last chunk, a `FinishReason`.

`ChatCompletionsLanguageModel` implements streaming with server-sent events. It sends `stream: true` (the flag is omitted for `CompleteAsync`), reads the response with `HttpCompletionOption.ResponseHeadersRead`, and parses `data:` lines until `[DONE]`. Other lines (comments, keep-alives, blank lines) are ignored. Both methods share one request helper that maps HTTP failures to `LanguageModelException`.

Usage is not reported for streamed calls, so `LanguageModelChunk` has no usage field.

## Anthropic provider

`AnthropicLanguageModel` talks to the Messages API directly because its protocol differs from OpenAI chat completions:

- Authentication uses the `x-api-key` and `anthropic-version` headers.
- System messages are joined into the top-level `system` field and removed from `messages`.
- `max_tokens` is mandatory, so `AnthropicOptions.DefaultMaxTokens` is used when the request has no limit.
- Responses contain content blocks; only `text` blocks are concatenated.
- Streaming reads `data:` lines. `content_block_delta` yields text, `message_delta` yields the finish reason, `message_stop` ends the stream and `error` raises `LanguageModelException`.

## Google provider

`GoogleLanguageModel` talks to the Gemini API directly:

- The model name is part of the path: `models/{model}:generateContent`, or `models/{model}:streamGenerateContent?alt=sse` for streaming.
- Authentication uses the `x-goog-api-key` header.
- System messages are joined into `systemInstruction`; the assistant role is sent as `model`.
- `temperature` and `maxOutputTokens` go into `generationConfig`; both are optional.
- Text is read from the first candidate and its parts are concatenated.
- Streaming reads `data:` lines; each event is a full response whose text is yielded and whose `finishReason`, when present, is mapped on the chunk.
