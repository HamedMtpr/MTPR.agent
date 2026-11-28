# M1 — LLM Integration: Building

- [Back to index](index.md)

## Prerequisites

- .NET SDK 10.0 (`dotnet --version` should report `10.0.x`)
- Git

## Restore, build and test

From the repository root:

```bash
dotnet restore MTPR.Agent.slnx
dotnet build MTPR.Agent.slnx --no-restore --configuration Release
dotnet test MTPR.Agent.slnx --no-build --configuration Release
```

## Projects added in this milestone

- `src/MTPR.Agent.Providers.OpenAiProtocol` — shared OpenAI chat completions implementation. References `MTPR.Agent.Core`.
- `src/MTPR.Agent.Providers.GapGpt` — GapGPT provider. References `OpenAiProtocol`.
- `src/MTPR.Agent.Providers.OpenAi` — OpenAI provider. References `OpenAiProtocol`.
- `tests/MTPR.Agent.Providers.OpenAi.Tests` — unit tests for OpenAI.
- `src/MTPR.Agent.Providers.OpenAiCompatible` — provider for any OpenAI-compatible server. References `OpenAiProtocol`.
- `src/MTPR.Agent.Providers.Anthropic` — Anthropic Messages API provider. References `MTPR.Agent.Core`.
- `tests/MTPR.Agent.Providers.Anthropic.Tests` — unit tests for Anthropic.
- `src/MTPR.Agent.Providers.Google` — Google Gemini API provider. References `MTPR.Agent.Core`.
- `tests/MTPR.Agent.Providers.Google.Tests` — unit tests for Google.
- `tests/MTPR.Agent.Providers.GapGpt.Tests` — unit tests for GapGPT.
- `tests/MTPR.Agent.Providers.OpenAiCompatible.Tests` — unit tests for OpenAI-compatible.

No new NuGet packages are introduced. Versions remain pinned in `Directory.Packages.props`.

## Continuous integration

The workflow `.github/workflows/ci.yml` restores, builds and tests the solution on Ubuntu, Windows and macOS for every push to `main` or `M*` branches and for pull requests targeting `main`.
