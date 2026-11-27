namespace MTPR.Agent.Core.Models;

/// <summary>
/// Final status of an agent run.
/// </summary>
public enum AgentStatus
{
    /// <summary>The task completed successfully.</summary>
    Succeeded,

    /// <summary>The task failed.</summary>
    Failed,

    /// <summary>The run was cancelled.</summary>
    Cancelled,
}
