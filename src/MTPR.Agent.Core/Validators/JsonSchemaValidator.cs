using System.Text.Json;
using MTPR.Agent.Core.Abstractions;
using MTPR.Agent.Core.Models;

namespace MTPR.Agent.Core.Validators;

/// <summary>
/// Validates JSON instances against the JSON Schema subset used by tool parameters:
/// type, enum, properties, required, additionalProperties, and items.
/// </summary>
public sealed class JsonSchemaValidator : IJsonSchemaValidator
{
    private const string RootPath = "$";

    /// <inheritdoc />
    public SchemaValidationResult Validate(JsonElement schema, JsonElement instance)
    {
        List<string> errors = new();

        validateNode(schema, instance, RootPath, errors);

        return new(errors);
    }

    private static void validateNode(JsonElement schema, JsonElement instance, string path, List<string> errors)
    {
        if (schema.ValueKind is JsonValueKind.True or JsonValueKind.False)
        {
            if (schema.ValueKind == JsonValueKind.False)
            {
                errors.Add($"{path}: no value is allowed here.");
            }

            return;
        }

        if (schema.ValueKind != JsonValueKind.Object)
        {
            throw new ArgumentException("A schema must be an object or a boolean.", nameof(schema));
        }

        if (schema.TryGetProperty("type", out var type) && !matchesType(type, instance))
        {
            errors.Add($"{path}: expected type {string.Join(" or ", typeNames(type))}.");
            return;
        }

        if (schema.TryGetProperty("enum", out var options) && !isOneOf(options, instance))
        {
            errors.Add($"{path}: value is not one of the allowed values.");
        }

        if (instance.ValueKind == JsonValueKind.Object)
        {
            validateObject(schema, instance, path, errors);
        }
        else if (instance.ValueKind == JsonValueKind.Array && schema.TryGetProperty("items", out var items))
        {
            validateItems(items, instance, path, errors);
        }
    }

    private static void validateObject(JsonElement schema, JsonElement instance, string path, List<string> errors)
    {
        if (schema.TryGetProperty("required", out var required))
        {
            foreach (var name in typeNames(required))
            {
                if (!instance.TryGetProperty(name, out _))
                {
                    errors.Add($"{path}: required property '{name}' is missing.");
                }
            }
        }

        foreach (var property in instance.EnumerateObject())
        {
            var propertyPath = $"{path}.{property.Name}";

            if (tryGetPropertySchema(schema, property.Name, out var propertySchema))
            {
                validateNode(propertySchema, property.Value, propertyPath, errors);
            }
            else if (schema.TryGetProperty("additionalProperties", out var additionalSchema))
            {
                validateNode(additionalSchema, property.Value, propertyPath, errors);
            }
        }
    }

    private static void validateItems(JsonElement itemSchema, JsonElement instance, string path, List<string> errors)
    {
        var index = 0;

        foreach (var item in instance.EnumerateArray())
        {
            validateNode(itemSchema, item, $"{path}[{index}]", errors);
            index++;
        }
    }

    private static bool tryGetPropertySchema(JsonElement schema, string name, out JsonElement propertySchema)
    {
        propertySchema = default;

        return schema.TryGetProperty("properties", out var properties)
            && properties.TryGetProperty(name, out propertySchema);
    }

    private static IEnumerable<string> typeNames(JsonElement type) =>
        type.ValueKind == JsonValueKind.String
            ? [type.GetString()!]
            : type.EnumerateArray().Select(name => name.GetString()!);

    private static bool matchesType(JsonElement type, JsonElement instance) =>
        typeNames(type).Any(name => matchesTypeName(name, instance));

    private static bool matchesTypeName(string name, JsonElement instance) =>
        name switch
        {
            "object" => instance.ValueKind == JsonValueKind.Object,
            "array" => instance.ValueKind == JsonValueKind.Array,
            "string" => instance.ValueKind == JsonValueKind.String,
            "boolean" => instance.ValueKind is JsonValueKind.True or JsonValueKind.False,
            "null" => instance.ValueKind == JsonValueKind.Null,
            "number" => instance.ValueKind == JsonValueKind.Number,
            "integer" => instance.ValueKind == JsonValueKind.Number && isIntegral(instance),
            _ => throw new ArgumentException($"Unsupported schema type '{name}'.")
        };

    private static bool isIntegral(JsonElement number) =>
        number.TryGetInt64(out _)
        || Math.Floor(number.GetDouble()) == number.GetDouble();

    private static bool isOneOf(JsonElement options, JsonElement instance)
    {
        var expected = instance.GetRawText();

        return options.EnumerateArray().Any(option => option.GetRawText() == expected);
    }
}
