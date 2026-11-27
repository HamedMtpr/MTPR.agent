# M0 — Project Foundation: Building

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

## Build configuration

- `Directory.Build.props` sets the shared target framework (`net10.0`), nullable reference types, implicit usings, analyzers (`latest-recommended`) and warnings-as-errors.
- `Directory.Packages.props` pins every NuGet package version centrally (Central Package Management). Package references in project files do not specify versions.
- `.editorconfig` defines the code style enforced by the build.

## Continuous integration

The workflow `.github/workflows/ci.yml` restores, builds and tests the solution on Ubuntu, Windows and macOS for every push to `main` or `M*` branches and for pull requests targeting `main`.
