namespace MTPR.Agent.Core.Models;

/// <summary>
/// Outcome of a tool invocation, returned to the language model.
/// </summary>
/// <param name="CallId">Identifier of the <see cref="ToolCall"/> this result answers.</param>
/// <param name="Status">Outcome of the invocation.</param>
/// <param name="Content">Output on success, or an error message otherwise.</param>
public sealed record ToolResult(
    string CallId,
    ToolStatus Status,
    string Content);
