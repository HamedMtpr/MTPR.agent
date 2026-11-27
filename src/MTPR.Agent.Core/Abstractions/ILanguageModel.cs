using MTPR.Agent.Core.Models;

namespace MTPR.Agent.Core.Abstractions;

/// <summary>
/// Provider-independent contract of a language model.
/// </summary>
public interface ILanguageModel
{
    /// <summary>
    /// Sends the request to the model and returns its completion.
    /// </summary>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">Token used to cancel the call.</param>
    /// <returns>The completion produced by the model.</returns>
    /// <exception cref="Exceptions.LanguageModelException">The provider call failed.</exception>
    Task<LanguageModelResponse> CompleteAsync(LanguageModelRequest request, CancellationToken cancellationToken);
}
