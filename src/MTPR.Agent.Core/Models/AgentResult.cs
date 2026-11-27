namespace MTPR.Agent.Core.Models;

/// <summary>
/// Outcome of an agent run.
/// </summary>
/// <param name="Status">Final status of the run.</param>
/// <param name="Summary">Human-readable summary of the run.</param>
public sealed record AgentResult(AgentStatus Status, string Summary);
