namespace MTPR.Agent.Core.Models;

/// <summary>
/// Reason why a model stopped generating output.
/// </summary>
public enum FinishReason
{
    /// <summary>The provider did not report a known reason.</summary>
    Unknown,

    /// <summary>The model finished its answer naturally.</summary>
    Stop,

    /// <summary>The output was cut off by the token limit.</summary>
    Length,

    /// <summary>The output was withheld by a content filter.</summary>
    ContentFilter,
}
