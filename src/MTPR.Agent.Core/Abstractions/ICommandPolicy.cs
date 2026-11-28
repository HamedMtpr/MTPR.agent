namespace MTPR.Agent.Core.Abstractions;

/// <summary>
/// Restricts which shell commands tools may run.
/// </summary>
public interface ICommandPolicy
{
    /// <summary>
    /// Checks a command line requested by the language model.
    /// </summary>
    /// <param name="commandLine">The full command line.</param>
    /// <returns><see langword="true"/> when the command may run.</returns>
    bool IsAllowed(string commandLine);
}
