namespace MTPR.Agent.Core.Exceptions;

/// <summary>
/// Raised when a tool request breaks an execution restriction.
/// </summary>
public sealed class PolicyViolationException(string message) : Exception(message);
