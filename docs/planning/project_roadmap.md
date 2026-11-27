# MTPR Agent Roadmap

- [Back to index](index.md)

## Development Strategy

The project will be developed incrementally through milestones.

Each milestone should produce a usable capability rather than only internal infrastructure.

The roadmap is intentionally outcome-oriented and may evolve as the architecture and experiments reveal new requirements.

## Confirmed Decisions

- **Type:** a Claude Code / Codex-style coding agent.
- **Platforms:** Windows, macOS, and Linux.
- **Language and runtime:** C# on .NET 10.
- **UI:** Avalonia desktop application, with a WebAssembly (browser) target as a later step (see Desktop Application Track).
- **LLM providers:** every major provider is planned; GapGPT is the first implementation.
- **Name:** MTPR Agent (repository: `MTPR.agent`; namespaces: `MTPR.Agent.*`).
- **License:** Apache License 2.0 (`LICENSE` and `NOTICE` files added in M0).

---

# M0 — Project Foundation

### Goal

Create the initial .NET solution and establish the architectural foundation.

### Deliverables

- [x] Create repository `MTPR.agent`
- [x] Create .NET solution
- [x] Define initial project structure
- [x] Define coding standards
- [x] Configure Git
- [x] Add basic documentation
- [x] Add test project
- [x] Establish CI build
- [x] Define initial abstractions
- [x] Verify build and tests on Windows, macOS, and Linux
- [x] Apply Apache-2.0 license (`LICENSE`, `NOTICE`)

### Expected Result

A clean, buildable and testable .NET solution ready for agent development.

---

# M1 — LLM Integration

### Goal

Create a provider-independent LLM abstraction.

### Deliverables

- [ ] Define LLM request/response model
- [ ] Define `ILanguageModel`
- [ ] Implement first LLM provider (GapGPT)
- [ ] Implement remaining providers incrementally (OpenAI, Anthropic, OpenAI-compatible, Google, local models)
- [ ] Support local model execution
- [ ] Implement conversation context
- [ ] Implement system instructions
- [ ] Add basic error handling
- [ ] Add unit tests

### Expected Result

The application can communicate with an LLM without coupling the agent core to a specific provider.

---

# M2 — Tool System

### Goal

Allow the LLM to interact with the development environment through controlled tools.

### Initial Tools

- [ ] File read
- [ ] File write
- [ ] File search
- [ ] Directory listing
- [ ] Command execution
- [ ] Git status
- [ ] Git diff

### Deliverables

- [ ] Define tool abstraction
- [ ] Implement tool registry
- [ ] Implement tool invocation
- [ ] Implement tool result model
- [ ] Add validation
- [ ] Add execution restrictions
- [ ] Add tool tests

### Expected Result

The LLM can inspect and modify a real repository through controlled capabilities.

---

# M3 — Agent Execution Loop

### Goal

Transform the LLM + tools into an actual coding agent.

### Execution Flow

```text
Task → Reason → Select Tool → Execute Tool → Observe Result → Reason Again → Continue
```

### Deliverables

- [ ] Implement agent loop
- [ ] Implement tool-call handling
- [ ] Maintain execution state
- [ ] Handle tool failures
- [ ] Detect completion
- [ ] Add iteration limits
- [ ] Add cancellation
- [ ] Add execution logging

### Expected Result

The agent can autonomously perform a multi-step coding task.

---

# M4 — Repository Intelligence

### Goal

Enable the agent to understand .NET repositories more effectively.

### Deliverables

- [ ] Repository scanner
- [ ] Solution detection
- [ ] Project detection
- [ ] Dependency discovery
- [ ] Source tree analysis
- [ ] Test project detection
- [ ] Git repository analysis
- [ ] Initial Roslyn integration

### Expected Result

The agent can build a structured understanding of a .NET repository before modifying it.

---

# M5 — Autonomous Coding

### Goal

Enable the agent to implement and validate real development tasks.

### Workflow

```text
Task → Inspect Repository → Plan → Implement → Build → Test → Diagnose → Repair → Validate
```

### Deliverables

- [ ] Task planning
- [ ] Code implementation
- [ ] Automatic build
- [ ] Automatic test execution
- [ ] Compilation-error diagnosis
- [ ] Test-failure diagnosis
- [ ] Automatic repair loop
- [ ] Completion criteria

### Expected Result

The agent can complete small-to-medium repository-level coding tasks with minimal human intervention.

---

# M6 — Git Integration

### Goal

Make Git a first-class capability of the agent.

### Deliverables

- [ ] Git status
- [ ] Git diff
- [ ] Git log
- [ ] Branch awareness
- [ ] Change summary
- [ ] Commit generation
- [ ] Commit message generation
- [ ] Optional automatic commit

### Expected Result

The agent can manage its changes safely within a Git workflow.

---

# M7 — Code Review

### Goal

Add an independent review phase before completing a task.

### Deliverables

- [ ] Change analysis
- [ ] Code quality review
- [ ] Test coverage review
- [ ] Potential regression detection
- [ ] Architecture review
- [ ] Review report

### Expected Result

The agent does not simply generate code; it evaluates its own work before declaring completion.

---

# M8 — Extension System

### Goal

Make the agent extensible without modifying its core.

### Deliverables

- [ ] Extension abstraction
- [ ] Plugin discovery
- [ ] Plugin lifecycle
- [ ] Extension-provided tools
- [ ] Extension-provided context
- [ ] Extension configuration
- [ ] Extension isolation

### Expected Result

Third-party or domain-specific capabilities can be added independently.

---

# M9 — MTPR Extension

### Goal

Integrate MTPR as the first major agent extension.

### Potential Capabilities

- [ ] MTPR project metadata reader
- [ ] MTPR project model integration
- [ ] Metadata-aware code generation
- [ ] Architecture-aware planning
- [ ] MTPR-specific tools
- [ ] Metadata validation
- [ ] MTPR workflow automation

### Expected Result

The generic coding agent gains advanced metadata-driven software engineering capabilities through an extension rather than core coupling.

---

# M10 — Advanced Context

### Goal

Improve repository and knowledge retrieval.

### Potential Capabilities

- [ ] Semantic code search
- [ ] Symbol indexing
- [ ] Dependency graph
- [ ] Documentation indexing
- [ ] Architecture knowledge
- [ ] Conversation memory
- [ ] Optional RAG

### Important Principle

RAG should be introduced only where retrieval from large external or project knowledge sources provides measurable value.

Code understanding should remain primarily tool- and structure-driven.

---

# M11 — Autonomous Software Engineering

### Goal

Move beyond individual coding tasks.

### Potential Capabilities

- [ ] Multi-file feature implementation
- [ ] Refactoring
- [ ] Bug investigation
- [ ] Regression fixing
- [ ] Test generation
- [ ] Migration assistance
- [ ] Dependency upgrades
- [ ] Pull request preparation
- [ ] Long-running tasks
- [ ] Task checkpoints

### Expected Result

The agent behaves increasingly like an autonomous software engineer rather than a code-generation assistant.

---

# M12 — Production Readiness

### Goal

Prepare the agent for real-world usage.

### Deliverables

- [ ] Security model
- [ ] Sandbox execution
- [ ] Permission system
- [ ] Resource limits
- [ ] Audit logging
- [ ] Cost/token tracking
- [ ] Performance monitoring
- [ ] Robust error recovery
- [ ] Configuration management
- [ ] Documentation
- [ ] End-to-end test suite

---

# Desktop Application Track

Runs in parallel with the agent milestones. Each step should give the user a usable desktop capability.

### D1 — Desktop Shell

- [ ] Avalonia application running on Windows, macOS, and Linux
- [ ] Open a repository folder
- [ ] Settings screen (provider, API key, model)

### D2 — Agent Chat UI

- [ ] Chat view connected to the agent loop
- [ ] Live display of tool calls and results
- [ ] Approval prompts for file writes and commands
- [ ] Session history

### D3 — WebAssembly Target *(scope to confirm)*

- [ ] Run the Avalonia UI in the browser via WebAssembly

---

# Guiding Principles

1. **Agent first, chatbot second.**
2. **Tools are capabilities; the LLM is the reasoning engine.**
3. **Keep the core independent of MTPR.**
4. **Prefer structured code intelligence over blindly embedding source code.**
5. **Validate generated code through build and tests.**
6. **Every autonomous action should be observable.**
7. **Fail safely and recover explicitly.**
8. **Keep LLM providers replaceable.**
9. **Introduce complexity only when experiments justify it.**
10. **Each milestone should produce a measurable capability.**

---

# Current Target

The immediate target is:

> Build a minimal .NET coding agent capable of receiving a coding task, inspecting a repository, modifying files, running `dotnet build` and `dotnet test`, diagnosing failures, and iterating until the task succeeds or the agent reaches its execution limits.

Everything beyond this target should be treated as incremental evolution rather than a prerequisite.
