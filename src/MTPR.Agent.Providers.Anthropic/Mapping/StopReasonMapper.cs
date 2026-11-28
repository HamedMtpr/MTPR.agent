using MTPR.Agent.Core.Models;

namespace MTPR.Agent.Providers.Anthropic.Mapping;

/// <summary>
/// Converts Anthropic stop reason names to core finish reasons.
/// </summary>
internal static class StopReasonMapper
{
    /// <summary>
    /// Maps a stop reason name to a core finish reason.
    /// </summary>
    /// <param name="reason">Stop reason returned by the API.</param>
    /// <returns>The core finish reason, or <see cref="FinishReason.Unknown"/>.</returns>
    public static FinishReason Map(string? reason)
    {
        return reason switch
        {
            "end_turn" or "stop_sequence" => FinishReason.Stop,
            "max_tokens" => FinishReason.Length,
            "refusal" => FinishReason.ContentFilter,
            _ => FinishReason.Unknown,
        };
    }
}
