# M0 — Project Foundation: Testing

- [Back to index](index.md)

## Running tests

```bash
dotnet test MTPR.Agent.slnx --configuration Release
```

## Coverage

Coverage is collected with the .NET SDK, using the `coverlet.collector` package:

```bash
dotnet test MTPR.Agent.slnx --configuration Release --collect:"XPlat Code Coverage"
```

The report is written to `TestResults/<guid>/coverage.cobertura.xml` under each test project. CI runs the same command.

## Current test suite

| Project | Scope |
|---------|-------|
| `MTPR.Agent.Core.Tests` | Model behaviour of `AgentTask` and `AgentResult` |
| `Mtpr.TestUtilities` | Shared base classes (`UnitTest`, `IntegrationTest`) for test traits |
