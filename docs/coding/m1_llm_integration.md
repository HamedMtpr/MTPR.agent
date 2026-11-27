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
  MTPR.Agent.Providers.GapGpt/
    GapGptLanguageModel.cs
    Mapping/
      FinishReasonMapper.cs
      RoleMapper.cs
    Options/
      GapGptOptions.cs
    Requests/
      ChatCompletionMessage.cs
      ChatCompletionRequest.cs
    Responses/
      ChatCompletionChoice.cs
      ChatCompletionMessageResult.cs
      ChatCompletionResponse.cs
      ChatCompletionUsage.cs
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
```

## Design

- `ILanguageModel` is the provider-independent contract. Core only knows `LanguageModelRequest` and `LanguageModelResponse`.
- `Conversation` stores ordered messages and optional system instructions. `CreateRequest` places the system instruction first and omits it when blank.
- `LanguageModelException` is the single error type raised by providers. It carries the HTTP status code when the provider returned one.
- The GapGPT provider lives in its own project so that Core has no HTTP or JSON dependency. It uses the OpenAI-compatible `chat/completions` endpoint.
- Provider wire models are `internal`. Mapping from core roles and finish reasons is isolated in `Mapping/`.

## GapGPT configuration

`GapGptOptions` is bound from the `GapGpt` configuration section:

| Property | Default |
|----------|---------|
| `BaseUrl` | `https://api.gapgpt.app/v1/` |
| `ApiKey` | empty (must be set) |
| `DefaultModel` | `gpt-4o-mini` |

## Pending work

- OpenAI, Anthropic, OpenAI-compatible, Google providers
- Local model execution
