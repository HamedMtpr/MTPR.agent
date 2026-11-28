using MTPR.Agent.Core.Models;

namespace MTPR.Agent.Core.Abstractions;

/// <summary>
/// Holds the tools available to the agent and resolves them by name.
/// </summary>
public interface IToolRegistry
{
    /// <summary>
    /// Definitions of all registered tools, to be sent to the language model.
    /// </summary>
    IReadOnlyList<ToolDefinition> Definitions { get; }

    /// <summary>
    /// Finds a registered tool by its name.
    /// </summary>
    /// <param name="name">Name of the tool, compared case-sensitively.</param>
    /// <returns>The tool, or <c>null</c> when no tool has that name.</returns>
    ITool? Find(string name);
}
