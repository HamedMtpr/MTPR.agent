# Unreleased

- [Back to index](index.md)

## M1 — LLM Integration

### Added

- Provider-independent LLM abstraction in `MTPR.Agent.Core`: `ILanguageModel`, `LanguageModelRequest`, `LanguageModelResponse`, `LanguageModelChunk`, `Conversation`, `ChatMessage`, `TokenUsage`, `FinishReason`, and `LanguageModelException`.
- Conversation context with optional system instructions.
- Completion and streaming (`CompleteAsync`, `StreamAsync`) for all providers.
- Shared OpenAI chat completions implementation (`OpenAiProtocol`).
- Providers: GapGPT, OpenAI, OpenAI-compatible (including local models such as Ollama, LM Studio, and llama.cpp), Anthropic, and Google Gemini.
- Unit tests for Core and every provider, with branch and statement coverage at 100% for the merged Core, GapGpt, and OpenAiCompatible projects.

### Changed

- `coverlet.collector` is referenced once in `Mtpr.TestUtilities`.

### Documentation

- [Implementation details](../coding/m1_llm_integration.md)
- [Build guide](../building/m1_llm_integration.md)
- [Testing](../testing/m1_llm_integration.md)

## M0 — Project Foundation

### Added

- .NET 10 solution (`MTPR.Agent.slnx`) with shared `Directory.Build.props`, `Directory.Packages.props`, and `.editorconfig`.
- `MTPR.Agent.Core` with the initial abstractions: `IAgentTask`, `IAgentRunner`, `AgentTask`, `AgentResult`, and `AgentStatus`.
- Test infrastructure: `MTPR.Agent.Core.Tests` (xUnit v3, AwesomeAssertions, NSubstitute) and the shared `Mtpr.TestUtilities` base classes `UnitTest` and `IntegrationTest`.
- Continuous integration workflow that builds and tests on Ubuntu, Windows, and macOS.
- Apache License 2.0 (`LICENSE`, `NOTICE`).
- Initial documentation structure, README, roadmap, and commit guide.

### Documentation

- [Implementation details](../coding/m0_project_foundation.md)
- [Build guide](../building/m0_project_foundation.md)
- [Testing](../testing/m0_project_foundation.md)
