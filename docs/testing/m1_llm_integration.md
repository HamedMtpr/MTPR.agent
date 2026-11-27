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
| `Mtpr.TestUtilities` | Shared base classes (`UnitTest`, `IntegrationTest`) for test traits |

The GapGPT tests use `StubHttpMessageHandler` (returns a canned response and records the request body) and `ThrowingHttpMessageHandler` (simulates network failure) in place of the network, so they run offline.

Internal mappers (`RoleMapper`, `FinishReasonMapper`) are verified through the public `CompleteAsync` behavior, so no `InternalsVisibleTo` is needed.

## Coverage status

All M1 production code (`MTPR.Agent.Core`, `MTPR.Agent.Providers.GapGpt`) is covered at 100% (dotCover). New providers must keep this level.
