# M0 — Project Foundation: Implementation Details

- [Back to index](index.md)

## Solution layout

```text
MTPR.Agent.slnx
Directory.Build.props
Directory.Packages.props
.editorconfig
src/
  MTPR.Agent.Core/
    Abstractions/
      IAgentRunner.cs
      IAgentTask.cs
    Models/
      AgentResult.cs
      AgentStatus.cs
      AgentTask.cs
tests/
  Directory.Build.props
  Mtpr.TestUtilities/
    IntegrationTest.cs
    UnitTest.cs
  MTPR.Agent.Core.Tests/
    Models/
      AgentResultTests.cs
      AgentTaskTests.cs
```

`tests/Directory.Build.props` imports the root `Directory.Build.props` and adds the shared test usings (`Xunit`, `AwesomeAssertions`, `NSubstitute`), so test projects do not repeat them. Test projects are executable (`OutputType` Exe for `MTPR.Agent.Core.Tests`), while `Mtpr.TestUtilities` is a library of base test classes.

Folders are named in plural form when they group multiple classes of the same kind (`Models`, `Abstractions`).

## Initial abstractions

- `IAgentTask`: describes a unit of work (`Description`).
- `IAgentRunner`: executes a task asynchronously and honours a `CancellationToken`.
- `AgentResult` and `AgentStatus` (`Succeeded`, `Failed`, `Cancelled`): outcome model of a run.
- `AgentTask`: default immutable implementation of `IAgentTask`.

No provider, tool or LLM types are referenced here; they arrive in M1 and M2 to keep the core independent.

## Test conventions

- xUnit v3 with AwesomeAssertions.
- Test method names follow `Method_ShouldExpectedBehavior`.
- Assertions use the short chain form (`actual.Should().Be(...)`).
- Test doubles, when needed, are placed in separate `partial` class files.

## Code style

- File-scoped namespaces, Allman braces, braces always required.
- Long chains are broken at each dot; long argument lists are wrapped.
- JSON files are formatted, not minified.
