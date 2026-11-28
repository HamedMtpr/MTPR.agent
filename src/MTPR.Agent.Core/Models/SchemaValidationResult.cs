namespace MTPR.Agent.Core.Models;

/// <summary>
/// Outcome of validating a JSON instance against a JSON Schema.
/// </summary>
/// <param name="Errors">Human-readable validation errors, each prefixed with the instance path.</param>
public sealed record SchemaValidationResult(IReadOnlyList<string> Errors)
{
    /// <summary>
    /// Whether the instance satisfies the schema.
    /// </summary>
    public bool IsValid => Errors.Count == 0;
}
