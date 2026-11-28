using System.Text.Json;

namespace MTPR.Agent.Core.Models;

/// <summary>
/// A request from the language model to invoke a tool.
/// </summary>
/// <param name="Id">Identifier assigned by the language model, used to match the result.</param>
/// <param name="Name">Name of the requested tool.</param>
/// <param name="Arguments">JSON arguments supplied by the language model.</param>
public sealed record ToolCall(
    string Id,
    string Name,
    JsonElement Arguments);
