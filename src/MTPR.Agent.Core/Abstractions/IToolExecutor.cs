using MTPR.Agent.Core.Models;

namespace MTPR.Agent.Core.Abstractions;

/// <summary>
/// Runs the tool requested by the language model after validating its arguments.
/// </summary>
public interface IToolExecutor
{
    /// <summary>
    /// Finds the tool, validates the arguments against its schema, and invokes it.
    /// Failures are returned as <see cref="ToolStatus.Failed"/> results instead of exceptions.
    /// </summary>
    /// <param name="call">The call requested by the language model.</param>
    /// <param name="cancellationToken">Token used to cancel the execution.</param>
    /// <returns>The result to return to the language model.</returns>
    Task<ToolResult> ExecuteAsync(ToolCall call, CancellationToken cancellationToken);
}
