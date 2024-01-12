using Sernager.Core.Tests.Fakes;
using Sernager.Core.Utils;

namespace Sernager.Core.Tests.Units.Utils;

public class JsonWrapperTests
{
    [Test]
    public void Serialize_ShouldReturnEmptyString_WhenPassedNull()
    {
        string json = JsonWrapper.Serialize(null!);

        Assert.That(json, Is.Not.Null);
        Assert.That(json, Is.Empty);
    }

    [Test]
    public void Serialize_ShouldReturnEmptyJsonString_WhenPassedEmptyObject()
    {
        string json = JsonWrapper.Serialize(new object());

        Assert.That(json, Is.Not.Null);
        Assert.That(json, Is.EqualTo("{}"));
    }

    [Test]
    public void Serialize_ShouldReturnEmptyJsonString_WhenPassedEmptyArray()
    {
        string json = JsonWrapper.Serialize(new object[0]);

        Assert.That(json, Is.Not.Null);
        Assert.That(json, Is.EqualTo("[]"));
    }

    [Test]
    public void Serialize_ShouldReturnEmptyJsonString_WhenPassedEmptyString()
    {
        string json = JsonWrapper.Serialize(string.Empty);

        Assert.That(json, Is.Not.Null);
        Assert.That(json, Is.EqualTo("\"\""));
    }

    [Test]
    public void Serialize_ShouldReturnJsonString_WhenPassedObject()
    {
        string json = JsonWrapper.Serialize(new { Name = "Sernager" });

        Assert.That(json, Is.Not.Null);
        Assert.That(json, Is.EqualTo("{\"name\":\"Sernager\"}"));
    }

    [Test]
    public void Serialize_ShouldReturnIndentedJsonString_WhenPassedObject()
    {
        string json = JsonWrapper.Serialize(new { Name = "Sernager" }, true).Replace("\r\n", "\n");

        Assert.That(json, Is.Not.Null);
        Assert.That(json, Is.EqualTo("{\n  \"name\": \"Sernager\"\n}"));
    }

    [Test]
    public void Serialize_ShouldReturnJsonString_WhenPassedClass()
    {
        FakeModel model = new()
        {
            Name = "Sernager",
            Number = 1,
            IsEnabled = true,
            CamelName = "Sernager",
        };

        string json = JsonWrapper.Serialize(model);

        Assert.That(json, Is.Not.Null);
        Assert.That(json, Is.EqualTo("{\"name\":\"Sernager\",\"number\":1,\"isEnabled\":true,\"camelName\":\"Sernager\"}"));
    }

    [Test]
    public void Serialize_ShouldReturnIndentedJsonString_WhenPassedClass()
    {
        FakeModel model = new()
        {
            Name = "Sernager",
            Number = 1,
            IsEnabled = true,
            CamelName = "Sernager",
        };

        string json = JsonWrapper.Serialize(model, true).Replace("\r\n", "\n");

        Assert.That(json, Is.Not.Null);
        Assert.That(json, Is.EqualTo("{\n  \"name\": \"Sernager\",\n  \"number\": 1,\n  \"isEnabled\": true,\n  \"camelName\": \"Sernager\"\n}"));
    }

    [Test]
    public void Deserialize_ShouldReturnDefault_WhenPassedNull()
    {
        Assert.That(JsonWrapper.Deserialize<int>(null!), Is.Default);
        Assert.That(JsonWrapper.Deserialize<string>(null!), Is.Default);
        Assert.That(JsonWrapper.Deserialize<object>(null!), Is.Default);
        Assert.That(JsonWrapper.Deserialize<FakeModel>(null!), Is.Default);
    }

    [Test]
    public void Deserialize_ShouldReturnDefault_WhenPassedEmptyString()
    {
        Assert.That(JsonWrapper.Deserialize<int>(string.Empty), Is.Default);
        Assert.That(JsonWrapper.Deserialize<string>(string.Empty), Is.Default);
        Assert.That(JsonWrapper.Deserialize<object>(string.Empty), Is.Default);
        Assert.That(JsonWrapper.Deserialize<FakeModel>(string.Empty), Is.Default);
    }

    [Test]
    public void Deserialize_ShouldReturnDefault_WhenPassedInvalidJsonString()
    {
        string[] testCases = [
            "1",
            "\"\"",
            "\"Sernager\"",
            "[}",
            "{]",
            "[1,2,3",
            "1,2,3]",
            "{1,2,3]",
            "[1,2,3}",
            "{[1,2,3}",
            "{1,2,3]}",
            "{[1,2,3]}",
            "[1,2,3}]",
            "[{1,2,3]",
            "[{1,2,3}]",
            "{\"name\":\"Sernager\"]",
            "[\"name\":\"Sernager\"}",
            "{\"name\":\"Sernager\"}]",
            "[{\"name\":\"Sernager\"}",
        ];

        foreach (string testCase in testCases)
        {
            Assert.That(JsonWrapper.Deserialize<int>(testCase), Is.Default);
            Assert.That(JsonWrapper.Deserialize<string>(testCase), Is.Default);
            Assert.That(JsonWrapper.Deserialize<object>(testCase), Is.Default);
            Assert.That(JsonWrapper.Deserialize<FakeModel>(testCase), Is.Default);
        }
    }

    [Test]
    public void Deserialize_ShouldReturnDefault_WhenPassedInvalidType()
    {
        Assert.That(JsonWrapper.Deserialize<int>("{}"), Is.Default);
        Assert.That(JsonWrapper.Deserialize<string>("{}"), Is.Default);
    }

    [Test]
    public void Deserialize_ShouldReturnClass_WhenPassedValidJsonString()
    {
        string[] testCases =
        [
            "{\"name\":\"Sernager\",\"number\":1,\"isEnabled\":true,\"camelName\":\"Sernager\"}",
            "{\"Name\":\"Sernager\",\"Number\":1,\"IsEnabled\":true,\"CamelName\":\"Sernager\"}",
            "{\"NAME\":\"Sernager\",\"NUMBER\":1,\"ISENABLED\":true,\"CAMELNAME\":\"Sernager\"}",
            "{\"nAmE\":\"Sernager\",\"nUmBeR\":1,\"iSeNaBlEd\":true,\"cAmElNaMe\":\"Sernager\"}",
            "{\n  \"name\": \"Sernager\",\n  \"number\": 1,\n  \"isEnabled\": true,\n  \"camelName\": \"Sernager\"\n}",
            "{\n  \"Name\": \"Sernager\",\n  \"Number\": 1,\n  \"IsEnabled\": true,\n  \"CamelName\": \"Sernager\"\n}",
            "{\n  \"NAME\": \"Sernager\",\n  \"NUMBER\": 1,\n  \"ISENABLED\": true,\n  \"CAMELNAME\": \"Sernager\"\n}",
            "{\n  \"nAmE\": \"Sernager\",\n  \"nUmBeR\": 1,\n  \"iSeNaBlEd\": true,\n  \"cAmElNaMe\": \"Sernager\"\n}",
        ];

        foreach (string testCase in testCases)
        {
            FakeModel? model = JsonWrapper.Deserialize<FakeModel>(testCase);

            Assert.That(model, Is.Not.Null);
            Assert.That(model.Name, Is.EqualTo("Sernager"));
            Assert.That(model.Number, Is.EqualTo(1));
            Assert.That(model.IsEnabled, Is.True);
            Assert.That(model.CamelName, Is.EqualTo("Sernager"));
        }

        string[] someEmptyPropertiesCases =
        [
            "{\"name\":\"Sernager\",\"number\":1}",
            "{\"name\":\"Sernager\",\"number\":1}",
            "{\"name\":\"Sernager\",\"number\":1}",
            "{\"name\":\"Sernager\",\"number\":1}",
            "{\"name\":\"Sernager\",\"number\":1}",
            "{\"name\":\"Sernager\",\"number\":1}",
            "{\"name\":\"Sernager\",\"number\":1}",
            "{\"name\":\"Sernager\",\"number\":1}",
            "{\"name\":\"Sernager\",\"number\":1}",
            "{\"name\":\"Sernager\",\"number\":1}",
            "{\"name\":\"Sernager\",\"number\":1}",
            "{\"name\":\"Sernager\",\"number\":1}",
            "{\"name\":\"Sernager\",\"number\":1}",
            "{\"name\":\"Sernager\",\"number\":1}",
            "{\"name\":\"Sernager\",\"number\":1}",
            "{\"name\":\"Sernager\",\"number\":1}",
        ];

        foreach (string testCase in someEmptyPropertiesCases)
        {
            FakeModel? model = JsonWrapper.Deserialize<FakeModel>(testCase);

            Assert.That(model, Is.Not.Null);
            Assert.That(model.Name, Is.EqualTo("Sernager"));
            Assert.That(model.Number, Is.EqualTo(1));
            Assert.That(model.IsEnabled, Is.Default);
            Assert.That(model.CamelName, Is.Empty);
        }
    }

    [Test]
    public void IsValid_ShouldReturnFalse_WhenPassedNull()
    {
        Assert.That(JsonWrapper.IsValid(null!), Is.False);
    }

    [Test]
    public void IsValid_ShouldReturnFalse_WhenPassedEmptyString()
    {
        Assert.That(JsonWrapper.IsValid(string.Empty), Is.False);
    }

    [Test]
    public void IsValid_ShouldReturnFalse_WhenPassedInvalidJsonString()
    {
        string[] testCases = [
            "1",
            "\"\"",
            "\"Sernager\"",
            "[}",
            "{]",
            "[1,2,3",
            "1,2,3]",
            "{1,2,3]",
            "[1,2,3}",
            "{[1,2,3}",
            "{1,2,3]}",
            "{[1,2,3]}",
            "[1,2,3}]",
            "[{1,2,3]",
            "[{1,2,3}]",
            "{\"name\":\"Sernager\"]",
            "[\"name\":\"Sernager\"}",
            "{\"name\":\"Sernager\"}]",
            "[{\"name\":\"Sernager\"}",
        ];

        foreach (string testCase in testCases)
        {
            Assert.That(JsonWrapper.IsValid(testCase), Is.False);
        }
    }

    [Test]
    public void IsValid_ShouldReturnTrue_WhenPassedValidJsonString()
    {
        string[] testCases =
        [
            "{}",
            "[]",
            "[1,2,3]",
            "[\"Sernager\"]",
            "[{\"name\":\"Sernager\"}]",
            "{\"name\":\"Sernager\"}",
            "{\n  \"name\": \"Sernager\"\n}",
        ];

        foreach (string testCase in testCases)
        {
            Assert.That(JsonWrapper.IsValid(testCase), Is.True);
        }
    }
}
