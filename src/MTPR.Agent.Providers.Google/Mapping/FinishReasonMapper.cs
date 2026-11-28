using MTPR.Agent.Core.Models;

namespace MTPR.Agent.Providers.Google.Mapping;

/// <summary>
/// Converts Gemini finish reason names to core finish reasons.
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
            "STOP" => FinishReason.Stop,
            "MAX_TOKENS" => FinishReason.Length,
            "SAFETY" or "RECITATION" or "BLOCKLIST" or "PROHIBITED_CONTENT"
                => FinishReason.ContentFilter,
            _ => FinishReason.Unknown,
        };
    }
}
