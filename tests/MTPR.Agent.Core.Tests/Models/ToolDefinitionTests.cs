using System.Text.Json;
using MTPR.Agent.Core.Models;
using Mtpr.TestUtilities;

namespace MTPR.Agent.Core.Tests.Models;

public sealed class ToolDefinitionTests : UnitTest
{
    [Fact]
    public void Constructor_ShouldExposeNameDescriptionAndSchema()
    {
        var schema = JsonDocument.Parse("{\"type\":\"object\"}").RootElement;

        var actual = new ToolDefinition("file_read", "Reads a file", schema);

        actual.Name.Should().Be("file_read");
        actual.Description.Should().Be("Reads a file");
        actual.ParametersSchema.GetProperty("type").GetString().Should().Be("object");
    }
}
