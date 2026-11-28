using MTPR.Agent.Core.Abstractions;
using MTPR.Agent.Core.Models;

namespace MTPR.Agent.Core.Registries;

/// <summary>
/// Immutable registry of tools, indexed by their unique names.
/// </summary>
public sealed class ToolRegistry : IToolRegistry
{
    private readonly Dictionary<string, ITool> _toolsByName = new(StringComparer.Ordinal);

    /// <summary>
    /// Creates a registry containing the given tools.
    /// </summary>
    /// <param name="tools">Tools to register.</param>
    /// <exception cref="ArgumentException">Two tools share the same name.</exception>
    public ToolRegistry(IEnumerable<ITool> tools)
    {
        ArgumentNullException.ThrowIfNull(tools);

        foreach (var tool in tools)
        {
            var name = tool.Definition.Name;

            if (!_toolsByName.TryAdd(name, tool))
            {
                throw new ArgumentException($"A tool named '{name}' is already registered.", nameof(tools));
            }
        }

        Definitions = _toolsByName.Values.Select(tool => tool.Definition).ToList();
    }

    /// <inheritdoc />
    public IReadOnlyList<ToolDefinition> Definitions { get; }

    /// <inheritdoc />
    public ITool? Find(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        return _toolsByName.GetValueOrDefault(name);
    }
}
