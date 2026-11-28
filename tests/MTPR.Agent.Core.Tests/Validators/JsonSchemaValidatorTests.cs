using MTPR.Agent.Core.Validators;
using Mtpr.TestUtilities;

namespace MTPR.Agent.Core.Tests.Validators;

public sealed class JsonSchemaValidatorTests : UnitTest
{
    private const string FileReadSchema = """
        {
          "type": "object",
          "properties": {
            "path": { "type": "string" },
            "maxLines": { "type": "integer" }
          },
          "required": ["path"],
          "additionalProperties": false
        }
        """;

    private readonly JsonSchemaValidator _validator = new();

    [Fact]
    public void Validate_Should_Pass_When_InstanceMatchesSchema()
    {
        var actual = _validator.Validate(
            JsonTestData.Parse(FileReadSchema),
            JsonTestData.Parse("""{ "path": "a.txt", "maxLines": 10 }"""));

        actual.IsValid.Should().BeTrue();
        actual.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_Should_ReportMissingRequiredProperty()
    {
        var actual = _validator.Validate(
            JsonTestData.Parse(FileReadSchema),
            JsonTestData.Parse("""{ "maxLines": 10 }"""));

        actual.IsValid.Should().BeFalse();
        actual.Errors.Should().ContainSingle()
            .Which.Should().Be("$: required property 'path' is missing.");
    }

    [Fact]
    public void Validate_Should_ReportTypeMismatch_OfProperty()
    {
        var actual = _validator.Validate(
            JsonTestData.Parse(FileReadSchema),
            JsonTestData.Parse("""{ "path": 42 }"""));

        actual.Errors.Should().ContainSingle()
            .Which.Should().Be("$.path: expected type string.");
    }

    [Fact]
    public void Validate_Should_ReportUnexpectedProperty_When_AdditionalPropertiesIsFalse()
    {
        var actual = _validator.Validate(
            JsonTestData.Parse(FileReadSchema),
            JsonTestData.Parse("""{ "path": "a.txt", "force": true }"""));

        actual.Errors.Should().ContainSingle()
            .Which.Should().Be("$.force: no value is allowed here.");
    }

    [Fact]
    public void Validate_Should_AllowUnlistedProperties_When_AdditionalPropertiesIsNotRestricted()
    {
        var actual = _validator.Validate(
            JsonTestData.Parse("""{ "type": "object", "properties": {} }"""),
            JsonTestData.Parse("""{ "anything": 1 }"""));

        actual.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Should_ValidateAdditionalProperties_AgainstSchema()
    {
        var actual = _validator.Validate(
            JsonTestData.Parse("""{ "type": "object", "additionalProperties": { "type": "number" } }"""),
            JsonTestData.Parse("""{ "extra": "text" }"""));

        actual.Errors.Should().ContainSingle()
            .Which.Should().Be("$.extra: expected type number.");
    }

    [Fact]
    public void Validate_Should_ValidateItems_WithIndexedPath()
    {
        var actual = _validator.Validate(
            JsonTestData.Parse("""{ "type": "array", "items": { "type": "string" } }"""),
            JsonTestData.Parse("""["a", 2]"""));

        actual.Errors.Should().ContainSingle()
            .Which.Should().Be("$[1]: expected type string.");
    }

    [Fact]
    public void Validate_Should_RejectValue_NotInEnum()
    {
        var actual = _validator.Validate(
            JsonTestData.Parse("""{ "enum": ["read", "write"] }"""),
            JsonTestData.Parse("\"delete\""));

        actual.Errors.Should().ContainSingle()
            .Which.Should().Be("$: value is not one of the allowed values.");
    }

    [Fact]
    public void Validate_Should_AcceptValue_When_TypeListContainsMatchingType()
    {
        var actual = _validator.Validate(
            JsonTestData.Parse("""{ "type": ["string", "null"] }"""),
            JsonTestData.Parse("null"));

        actual.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("2", true)]
    [InlineData("2.0", true)]
    [InlineData("2.5", false)]
    [InlineData("\"2\"", false)]
    public void Validate_Should_CheckIntegerType_ByNumericValue(string json, bool expected)
    {
        var actual = _validator.Validate(
            JsonTestData.Parse("""{ "type": "integer" }"""),
            JsonTestData.Parse(json));

        actual.IsValid.Should().Be(expected);
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", true)]
    [InlineData("\"true\"", false)]
    public void Validate_Should_CheckBooleanType_ByValueKind(string json, bool expected)
    {
        var actual = _validator.Validate(
            JsonTestData.Parse("""{ "type": "boolean" }"""),
            JsonTestData.Parse(json));

        actual.IsValid.Should().Be(expected);
    }

    [Fact]
    public void Validate_Should_AcceptAnyValue_When_SchemaIsTrue()
    {
        var actual = _validator.Validate(JsonTestData.Parse("true"), JsonTestData.Parse("[1, \"x\"]"));

        actual.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Should_RejectAnyValue_When_SchemaIsFalse()
    {
        var actual = _validator.Validate(JsonTestData.Parse("false"), JsonTestData.Parse("1"));

        actual.Errors.Should().ContainSingle().Which.Should().Be("$: no value is allowed here.");
    }

    [Fact]
    public void Validate_Should_Throw_When_SchemaTypeIsUnsupported()
    {
        var act = () => _validator.Validate(
            JsonTestData.Parse("""{ "type": "date" }"""),
            JsonTestData.Parse("\"2026-01-01\""));

        act.Should().Throw<ArgumentException>().WithMessage("*'date'*");
    }

    [Fact]
    public void Validate_Should_Throw_When_SchemaIsNotObjectOrBoolean()
    {
        var act = () => _validator.Validate(JsonTestData.Parse("\"string\""), JsonTestData.Parse("1"));

        act.Should().Throw<ArgumentException>();
    }
}
