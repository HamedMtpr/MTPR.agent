using MTPR.Agent.Core.Abstractions;
using MTPR.Agent.Core.Exceptions;

namespace MTPR.Agent.Core.Policies;

/// <summary>
/// Allows access only to paths inside a workspace root directory.
/// </summary>
public sealed class WorkspacePathPolicy : IPathPolicy
{
    private readonly string _root;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="root">The workspace root directory.</param>
    public WorkspacePathPolicy(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);

        _root = Path.GetFullPath(root);
    }

    /// <inheritdoc />
    public string Resolve(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        var full = Path.GetFullPath(Path.Combine(_root, path));

        if (!isInsideRoot(full))
        {
            throw new PolicyViolationException($"Path '{path}' is outside the workspace.");
        }

        return full;
    }

    private bool isInsideRoot(string fullPath)
    {
        var relative = Path.GetRelativePath(_root, fullPath);

        return relative != ".."
            && !relative.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal)
            && !Path.IsPathRooted(relative);
    }
}
