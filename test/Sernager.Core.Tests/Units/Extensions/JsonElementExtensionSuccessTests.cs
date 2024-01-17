using Sernager.Core.Extensions;
using System.Text.Json;

namespace Sernager.Core.Tests.Units.Extensions;

public class JsonElementExtensionSuccessTests
{
    [Test]
    public void TryGetString_ShouldReturnEmptyString()
    {
        string json = "{\"string\": \"\", \"null\": null}";
        string[] testCases = { "string", "null" };
        JsonElement jsonElement = JsonDocument.Parse(json).RootElement;
        string? value;

        foreach (string testCase in testCases)
        {
            JsonElement element = jsonElement.GetProperty(testCase);
            bool result = element.TryGetString(out value);

            Assert.That(result, Is.True, $"Expected {testCase} to be true");
            Assert.That(value, Is.EqualTo(string.Empty), $"Expected {testCase} to be empty string");
        }
    }

    [Test]
    public void TryGetString_ShouldReturnString()
    {
        string json = "{\"string\": \"value\"}";
        JsonElement jsonElement = JsonDocument.Parse(json).RootElement;
        string? value;

        JsonElement element = jsonElement.GetProperty("string");
        bool result = element.TryGetString(out value);

        Assert.That(result, Is.True);
        Assert.That(value, Is.EqualTo("value"));
    }

    [Test]
    public void TryGetStringArray_ShouldReturnEmptyStringArray()
    {
        string json = "{\"array\": [null, null, null]}";
        JsonElement jsonElement = JsonDocument.Parse(json).RootElement;
        string[]? value;

        JsonElement element = jsonElement.GetProperty("array");
        bool result = element.TryGetStringArray(out value);

        Assert.That(result, Is.True);
        Assert.That(value, Is.Empty);
    }

    [Test]
    public void TryGetStringArray_ShouldReturnStringArray()
    {
        string json = "{\"array\": [\"value1\", null, \"value2\", null, \"value3\"]}";
        JsonElement jsonElement = JsonDocument.Parse(json).RootElement;
        string[]? value;

        JsonElement element = jsonElement.GetProperty("array");
        bool result = element.TryGetStringArray(out value);

        Assert.That(result, Is.True);
        Assert.That(value, Is.EqualTo(new string[] { "value1", "value2", "value3" }));
    }
}
