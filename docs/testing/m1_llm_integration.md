# M1 — LLM Integration: Testing

- [Back to index](index.md)

## Running tests

```bash
dotnet test MTPR.Agent.slnx --configuration Release
```

## Coverage

```bash
dotnet test MTPR.Agent.slnx --configuration Release --collect:"XPlat Code Coverage"
```

The report includes both line and branch coverage (Cobertura XML). The `coverlet.collector` package is referenced once in `Mtpr.TestUtilities` with `PrivateAssets="none"`, so every test project that references it gets the collector without a direct package reference.

## Test suite

| Project | Scope |
|---------|-------|
| `MTPR.Agent.Core.Tests` | `Conversation` ordering, system instruction placement, argument validation, `LanguageModelException` constructors, `TokenUsage.TotalTokens` |
| `MTPR.Agent.Providers.GapGpt.Tests` | Successful completion mapping, finish reason and role mapping, missing message or usage defaults, HTTP failure, network failure, empty or null body, null request; streaming deltas, finish reason, Bearer header, HTTP and network failures, comments and null entries in the stream |
| `MTPR.Agent.Providers.Google.Tests` | Default options, header authentication, model in request path, role and system instruction mapping, generation config, finish reason mapping, missing candidates, usage, HTTP and network failures, empty body; streaming deltas, finish reason, path with `alt=sse`, comments and null entries, HTTP failure |
| `MTPR.Agent.Providers.OpenAi.Tests` | Default `BaseUrl` points to `api.openai.com`, Bearer header always sent, successful completion and usage, HTTP and network failures, streaming deltas, finish reason, Bearer header and HTTP failure while streaming |
| `MTPR.Agent.Providers.Anthropic.Tests` | Default options, header authentication, system prompt separation, default and explicit `max_tokens`, text block concatenation, stop reason mapping, usage, HTTP and network failures, empty body; streaming deltas, stop reason, `message_stop` termination, error event and HTTP failure |
| `MTPR.Agent.Providers.OpenAiCompatible.Tests` | Same behaviors as GapGPT, plus optional API key: Bearer header sent when a key is set and omitted when empty; streaming deltas, finish reason on the last chunk, `stream` flag only on streaming calls, HTTP and network failures while streaming; default `BaseUrl` points to local Ollama |
| `Mtpr.TestUtilities` | Shared base classes (`UnitTest`, `IntegrationTest`) for test traits |

The provider tests use `StubHttpMessageHandler` (returns a canned response and records the request body) and `ThrowingHttpMessageHandler` (simulates network failure) in place of the network, so they run offline.

Each provider test project owns its stub handlers. The shared base in `OpenAiProtocol` is covered through both providers.

Internal mappers (`RoleMapper`, `FinishReasonMapper`) are verified through the public `CompleteAsync` behavior, so no `InternalsVisibleTo` is needed.

## Coverage status

Branch coverage, merged across the Core, GapGpt, and OpenAiCompatible test projects, is 100% (42 of 42 branches). Streaming edge cases covered: null request, `null` entries, empty `choices`, missing `delta`, comment lines, and the `[DONE]` marker.

Statement coverage, merged across the same test projects, is 100% .

Per project, GapGpt and OpenAiCompatible report about 95% because the `Conversation.CreateRequest` branch is exercised only by the Core tests.
