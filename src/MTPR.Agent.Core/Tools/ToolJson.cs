using System.Text.Json;

namespace MTPR.Agent.Core.Tools;

/// <summary>
/// Helpers shared by tools to define schemas and read validated arguments.
/// </summary>
internal static class ToolJson
{
    public static JsonElement Schema(string json) => JsonDocument.Parse(json).RootElement;

    public static string GetString(JsonElement arguments, string name) =>
        arguments.GetProperty(name).GetString()!;

    public static string GetOptionalString(JsonElement arguments, string name, string fallback) =>
        arguments.TryGetProperty(name, out var value) ? value.GetString()! : fallback;
}
