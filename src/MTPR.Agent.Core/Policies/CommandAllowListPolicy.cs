using System.Buffers;
using MTPR.Agent.Core.Abstractions;

namespace MTPR.Agent.Core.Policies;

/// <summary>
/// Allows only commands whose executable is on an allow list and that do not chain other commands.
/// </summary>
public sealed class CommandAllowListPolicy : ICommandPolicy
{
    private static readonly SearchValues<char> _shellOperators =
        SearchValues.Create(";&|<>`$\n\r");

    private readonly HashSet<string> _allowedExecutables;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="allowedExecutables">Executable names that may run.</param>
    public CommandAllowListPolicy(IEnumerable<string> allowedExecutables)
    {
        ArgumentNullException.ThrowIfNull(allowedExecutables);

        _allowedExecutables = new(allowedExecutables, StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public bool IsAllowed(string commandLine)
    {
        ArgumentNullException.ThrowIfNull(commandLine);

        if (commandLine.AsSpan().ContainsAny(_shellOperators))
        {
            return false;
        }

        var executable = commandLine.
            Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).
            FirstOrDefault();

        return executable is not null && _allowedExecutables.Contains(executable);
    }
}
