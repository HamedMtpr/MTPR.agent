using System.Text.Json;

namespace Mtpr.TestUtilities;

public static class JsonTestData
{
    public static JsonElement Parse(string json) => JsonDocument.Parse(json).RootElement;
}
