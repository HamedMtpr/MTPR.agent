namespace MTPR.Agent.Core.Models;

/// <summary>
/// Provider-independent request sent to a language model.
/// </summary>
/// <param name="Model">Identifier of the model to use.</param>
/// <param name="Messages">Ordered messages, including system instructions.</param>
/// <param name="Temperature">Optional sampling temperature.</param>
/// <param name="MaxOutputTokens">Optional upper bound on generated tokens.</param>
public sealed record LanguageModelRequest(
    string Model,
    IReadOnlyList<ChatMessage> Messages,
    double? Temperature = null,
    int? MaxOutputTokens = null);
