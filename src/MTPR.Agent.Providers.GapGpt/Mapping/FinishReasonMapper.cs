using MTPR.Agent.Core.Models;

namespace MTPR.Agent.Providers.GapGpt.Mapping;

/// <summary>
/// Converts GapGPT finish reason names to core finish reasons.
/// </summary>
internal static class FinishReasonMapper
{
    /// <summary>
    /// Maps a finish reason name to a core finish reason.
    /// </summary>
    /// <param name="reason">Finish reason returned by the API.</param>
    /// <returns>The core finish reason, or <see cref="FinishReason.Unknown"/>.</returns>
    public static FinishReason Map(string? reason)
    {
        return reason switch
        {
            "stop" => FinishReason.Stop,
            "length" => FinishReason.Length,
            "content_filter" => FinishReason.ContentFilter,
            _ => FinishReason.Unknown,
        };
    }
}
