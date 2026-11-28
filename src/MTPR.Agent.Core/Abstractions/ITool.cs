using MTPR.Agent.Core.Models;

namespace MTPR.Agent.Core.Abstractions;

/// <summary>
/// A capability the language model can invoke to interact with the development environment.
/// </summary>
public interface ITool
{
    /// <summary>
    /// Describes the tool to the language model.
    /// </summary>
    ToolDefinition Definition { get; }

    /// <summary>
    /// Executes the tool with the arguments supplied by the language model.
    /// </summary>
    /// <param name="call">The call requested by the language model.</param>
    /// <param name="cancellationToken">Token used to cancel the execution.</param>
    /// <returns>The result to return to the language model.</returns>
    Task<ToolResult> ExecuteAsync(ToolCall call, CancellationToken cancellationToken);
}
