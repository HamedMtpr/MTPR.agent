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

## Test suite

| Project | Scope |
|---------|-------|
| `MTPR.Agent.Core.Tests` | `Conversation` ordering, system instruction placement, argument validation, `LanguageModelException` constructors, `TokenUsage.TotalTokens` |
| `MTPR.Agent.Providers.GapGpt.Tests` | Successful completion mapping, finish reason and role mapping, missing message or usage defaults, HTTP failure, network failure, empty or null body, null request |
| `MTPR.Agent.Providers.OpenAiCompatible.Tests` | Same behaviors as GapGPT, plus optional API key: Bearer header sent when a key is set and omitted when empty |
| `Mtpr.TestUtilities` | Shared base classes (`UnitTest`, `IntegrationTest`) for test traits |

The provider tests use `StubHttpMessageHandler` (returns a canned response and records the request body) and `ThrowingHttpMessageHandler` (simulates network failure) in place of the network, so they run offline.

Each provider test project owns its stub handlers. The shared base in `OpenAiProtocol` is covered through both providers.

Internal mappers (`RoleMapper`, `FinishReasonMapper`) are verified through the public `CompleteAsync` behavior, so no `InternalsVisibleTo` is needed.

## Coverage status

All M1 production code (`Core`, `GapGpt`) was covered at 100% before the OpenAI-compatible provider and shared base were added. Coverage of `OpenAiProtocol` and `OpenAiCompatible` must be re-measured and kept at 100%.
