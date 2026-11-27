namespace MTPR.Agent.Core.Exceptions;

/// <summary>
/// Raised when a language model call fails.
/// </summary>
public sealed class LanguageModelException : Exception
{
    /// <summary>
    /// Initializes a new instance without a status code.
    /// </summary>
    /// <param name="message">Description of the failure.</param>
    public LanguageModelException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance that wraps another exception.
    /// </summary>
    /// <param name="message">Description of the failure.</param>
    /// <param name="innerException">The exception that caused the failure.</param>
    public LanguageModelException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance for a failed provider response.
    /// </summary>
    /// <param name="message">Description of the failure.</param>
    /// <param name="statusCode">HTTP status code returned by the provider.</param>
    public LanguageModelException(string message, int statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    /// <summary>
    /// Gets the HTTP status code returned by the provider, when available.
    /// </summary>
    public int? StatusCode { get; }
}
