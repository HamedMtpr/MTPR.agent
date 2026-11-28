using System.Text.Json;
using MTPR.Agent.Core.Models;

namespace MTPR.Agent.Core.Abstractions;

/// <summary>
/// Validates JSON instances against a JSON Schema.
/// </summary>
public interface IJsonSchemaValidator
{
    /// <summary>
    /// Validates an instance against the schema.
    /// </summary>
    /// <param name="schema">The JSON Schema to validate against.</param>
    /// <param name="instance">The JSON value to validate.</param>
    /// <returns>The validation result, containing all errors found.</returns>
    SchemaValidationResult Validate(JsonElement schema, JsonElement instance);
}
