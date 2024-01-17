using Sernager.Core.Extensions;
using System.Text.Json;

namespace Sernager.Core.Tests.Units.Extensions;

public class JsonElementExtensionFailureTests : FailureFixture
{
    [Test]
    public void TryGetString_ShouldReturnFalseAndOutNull_WhenJsonElementIsNotString()
    {
        string json = "{\"int\": 42, \"object\": {\"key\": \"value\"}, \"array\": [1, 2, 3], \"boolean\": true}";
        string[] testCases = { "int", "object", "array", "boolean" };
        JsonElement jsonElement = JsonDocument.Parse(json).RootElement;
        string? value;

        foreach (string testCase in testCases)
        {
            JsonElement element = jsonElement.GetProperty(testCase);
            bool result = element.TryGetString(out value);

            Assert.That(result, Is.False, $"Expected {testCase} to be false");
            Assert.That(value, Is.Null, $"Expected {testCase} to be null");
        }
    }

    [Test]
    public void TryGetStringArray_ShouldReturnFalseAndOutNull_WhenJsonElementIsNotStringArray()
    {
        string json = "{\"int\": 42, \"object\": {\"key\": \"value\"}, \"string\": \"value\", \"boolean\": true}";
        string[] testCases = { "int", "object", "string", "boolean" };
        JsonElement jsonElement = JsonDocument.Parse(json).RootElement;
        string[]? value;

        foreach (string testCase in testCases)
        {
            JsonElement element = jsonElement.GetProperty(testCase);
            bool result = element.TryGetStringArray(out value);

            Assert.That(result, Is.False, $"Expected {testCase} to be false");
            Assert.That(value, Is.Null, $"Expected {testCase} to be null");
        }
    }
}
