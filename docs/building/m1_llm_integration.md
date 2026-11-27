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

- `src/MTPR.Agent.Providers.GapGpt` — GapGPT language model provider. References `MTPR.Agent.Core`.
- `tests/MTPR.Agent.Providers.GapGpt.Tests` — unit tests for the provider.

No new NuGet packages are introduced. Versions remain pinned in `Directory.Packages.props`.

## Continuous integration

The workflow `.github/workflows/ci.yml` restores, builds and tests the solution on Ubuntu, Windows and macOS for every push to `main` or `M*` branches and for pull requests targeting `main`.
