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
  MTPR.Agent.Providers.GapGpt/
    GapGptLanguageModel.cs
    Options/
      GapGptOptions.cs
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
  MTPR.Agent.Providers.GapGpt.Tests/
    GapGptLanguageModelTests.cs
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
- Providers using other protocols (Anthropic, Google) will not use this base.

## GapGPT configuration

`GapGptOptions` is bound from the `GapGpt` configuration section:

| Property | Default |
|----------|---------|
| `BaseUrl` | `https://api.gapgpt.app/v1/` |
| `ApiKey` | empty (must be set) |
| `DefaultModel` | `gpt-4o-mini` |

## OpenAI-compatible configuration

`OpenAiCompatibleOptions` is bound from the `OpenAiCompatible` configuration section. The base address comes from the `HttpClient` (for example `http://localhost:11434/v1/` for Ollama).

| Property | Default |
|----------|---------|
| `ApiKey` | empty (optional; the `Authorization` header is omitted when empty) |

## Shared OpenAI protocol

`GapGptLanguageModel` always sends a Bearer token. `OpenAiCompatibleLanguageModel` sends it only when an API key is configured. Error messages are prefixed with the provider name. Provider projects reference `OpenAiProtocol`, which references `Core`.

## Streaming design

`ILanguageModel` exposes two methods:

- `CompleteAsync` returns a single `LanguageModelResponse` and is unchanged.
- `StreamAsync` returns `IAsyncEnumerable<LanguageModelChunk>`. Each chunk carries a `ContentDelta` and, on the last chunk, a `FinishReason`.

`ChatCompletionsLanguageModel` implements streaming with server-sent events. It sends `stream: true` (the flag is omitted for `CompleteAsync`), reads the response with `HttpCompletionOption.ResponseHeadersRead`, and parses `data:` lines until `[DONE]`. Other lines (comments, keep-alives, blank lines) are ignored. Both methods share one request helper that maps HTTP failures to `LanguageModelException`.

Usage is not reported for streamed calls, so `LanguageModelChunk` has no usage field.

## Pending work

- OpenAI, Anthropic, Google providers
- Local model execution
