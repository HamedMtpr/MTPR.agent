namespace MTPR.Agent.Core.Models;

/// <summary>
/// Incremental part of a streamed completion.
/// </summary>
/// <param name="ContentDelta">Text generated since the previous chunk.</param>
/// <param name="FinishReason">Why the model stopped, set only on the last chunk.</param>
public sealed record LanguageModelChunk(
    string ContentDelta,
    FinishReason? FinishReason = null);
