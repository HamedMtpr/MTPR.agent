namespace MTPR.Agent.Core.Abstractions;

/// <summary>
/// Asks the user to approve an action that changes the development environment.
/// </summary>
public interface IApprovalService
{
    /// <summary>
    /// Requests approval for an action.
    /// </summary>
    /// <param name="action">Short name of the action, for example the tool name.</param>
    /// <param name="details">Description of what the action will do.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    /// <returns><see langword="true"/> when the user approved the action.</returns>
    Task<bool> RequestApprovalAsync(string action, string details, CancellationToken cancellationToken);
}
