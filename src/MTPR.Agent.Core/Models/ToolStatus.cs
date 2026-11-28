namespace MTPR.Agent.Core.Models;

/// <summary>
/// Outcome of a tool invocation.
/// </summary>
public enum ToolStatus
{
    /// <summary>The tool completed successfully.</summary>
    Succeeded,

    /// <summary>The tool failed, for example because of invalid arguments or a runtime error.</summary>
    Failed,

    /// <summary>The tool was blocked by an execution restriction.</summary>
    Denied,
}
