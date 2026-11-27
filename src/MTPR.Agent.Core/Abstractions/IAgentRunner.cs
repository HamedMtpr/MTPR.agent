using MTPR.Agent.Core.Models;

namespace MTPR.Agent.Core.Abstractions;

/// <summary>
/// Executes an agent task and reports the outcome.
/// </summary>
public interface IAgentRunner
{
    /// <summary>
    /// Runs the given task until it completes, fails, or is cancelled.
    /// </summary>
    /// <param name="task">The task to execute.</param>
    /// <param name="cancellationToken">Token used to cancel the run.</param>
    /// <returns>The result of the run.</returns>
    Task<AgentResult> RunAsync(IAgentTask task, CancellationToken cancellationToken);
}
