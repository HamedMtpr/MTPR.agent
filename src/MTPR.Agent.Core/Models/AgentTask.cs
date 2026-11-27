using MTPR.Agent.Core.Abstractions;

namespace MTPR.Agent.Core.Models;

/// <summary>
/// Default immutable implementation of <see cref="IAgentTask"/>.
/// </summary>
/// <param name="Description">Natural-language description of the task.</param>
public sealed record AgentTask(string Description) : IAgentTask;
