namespace MTPR.Agent.Core.Abstractions;

/// <summary>
/// Restricts which file system locations tools may access.
/// </summary>
public interface IPathPolicy
{
    /// <summary>
    /// Resolves a path requested by the language model to an absolute path.
    /// </summary>
    /// <param name="path">Absolute path, or path relative to the workspace root.</param>
    /// <returns>The absolute path inside the allowed area.</returns>
    /// <exception cref="Exceptions.PolicyViolationException">The path is outside the allowed area.</exception>
    string Resolve(string path);
}
