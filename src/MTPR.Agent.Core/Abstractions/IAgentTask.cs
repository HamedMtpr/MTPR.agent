namespace MTPR.Agent.Core.Abstractions;

/// <summary>
/// Describes a unit of work the agent is asked to perform.
/// </summary>
public interface IAgentTask
{
    /// <summary>
    /// Gets the natural-language description of the task.
    /// </summary>
    string Description { get; }
}
