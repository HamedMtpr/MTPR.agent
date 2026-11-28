using System.Text.Json;

namespace MTPR.Agent.Core.Models;

/// <summary>
/// Provider-independent description of a tool exposed to a language model.
/// </summary>
/// <param name="Name">Unique name the language model uses to call the tool.</param>
/// <param name="Description">Explains to the language model what the tool does.</param>
/// <param name="ParametersSchema">JSON Schema describing the accepted arguments.</param>
public sealed record ToolDefinition(
    string Name,
    string Description,
    JsonElement ParametersSchema);
