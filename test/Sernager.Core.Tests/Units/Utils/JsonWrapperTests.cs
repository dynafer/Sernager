using Sernager.Core.Tests.Fakes;
using Sernager.Core.Utils;

namespace Sernager.Core.Tests.Units.Utils;

internal sealed class JsonWrapperTests
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
    public void Serialize_ShouldReturnEmptyJsonString_WhenPassedNullProperty()
    {
        string json = JsonWrapper.Serialize(new { Name = (string?)null });

        Assert.That(json, Is.Not.Null);
        Assert.That(json, Is.EqualTo("{}"));
    }

    [Test]
    public void Serialize_ShouldReturnJsonString_WhenPassedClass()
    {
        FakeModel model = new FakeModel()
        {
            Name = "Sernager",
            Number = 1,
            IsEnabled = true,
            CaseName = "Sernager",
            NullName = null,
        };

        string json = JsonWrapper.Serialize(model);

        Assert.That(json, Is.Not.Null);
        Assert.That(json, Is.EqualTo("{\"name\":\"Sernager\",\"number\":1,\"isEnabled\":true,\"caseName\":\"Sernager\"}"));
    }

    [Test]
    public void Serialize_ShouldReturnIndentedJsonString_WhenPassedClass()
    {
        FakeModel model = new FakeModel()
        {
            Name = "Sernager",
            Number = 1,
            IsEnabled = true,
            CaseName = "Sernager",
            NullName = null,
        };

        string json = JsonWrapper.Serialize(model, true).Replace("\r\n", "\n");

        Assert.That(json, Is.Not.Null);
        Assert.That(json, Is.EqualTo("{\n  \"name\": \"Sernager\",\n  \"number\": 1,\n  \"isEnabled\": true,\n  \"caseName\": \"Sernager\"\n}"));
    }

    [Test]
    public void Deserialize_ShouldReturnNull_WhenPassedNull()
    {
        Assert.That(JsonWrapper.Deserialize<string>(null!), Is.Null);
        Assert.That(JsonWrapper.Deserialize<object>(null!), Is.Null);
        Assert.That(JsonWrapper.Deserialize<FakeModel>(null!), Is.Null);
    }

    [Test]
    public void Deserialize_ShouldReturnNull_WhenPassedEmptyString()
    {
        Assert.That(JsonWrapper.Deserialize<string>(string.Empty), Is.Null);
        Assert.That(JsonWrapper.Deserialize<object>(string.Empty), Is.Null);
        Assert.That(JsonWrapper.Deserialize<FakeModel>(string.Empty), Is.Null);
    }

    [Test]
    public void Deserialize_ShouldReturnNull_WhenPassedInvalidJsonString()
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
            Assert.That(JsonWrapper.Deserialize<string>(testCase), Is.Null);
            Assert.That(JsonWrapper.Deserialize<object>(testCase), Is.Null);
            Assert.That(JsonWrapper.Deserialize<FakeModel>(testCase), Is.Null);
        }
    }

    [Test]
    public void Deserialize_ShouldReturnNull_WhenPassedInvalidType()
    {
        Assert.That(JsonWrapper.Deserialize<string>("{}"), Is.Null);
    }

    [Test]
    public void Deserialize_ShouldReturnClass_WhenPassedValidJsonString()
    {
        string[] testCases =
        [
            "{\"name\":\"Sernager\",\"number\":1,\"isEnabled\":true,\"caseName\":\"Sernager\"}",
            "{\"Name\":\"Sernager\",\"Number\":1,\"IsEnabled\":true,\"CaseName\":\"Sernager\"}",
            "{\"NAME\":\"Sernager\",\"NUMBER\":1,\"ISENABLED\":true,\"CASENAME\":\"Sernager\"}",
            "{\"nAmE\":\"Sernager\",\"nUmBeR\":1,\"iSeNaBlEd\":true,\"cAsEnAmE\":\"Sernager\"}",
            "{\n  \"name\": \"Sernager\",\n  \"number\": 1,\n  \"isEnabled\": true,\n  \"caseName\": \"Sernager\"\n}",
            "{\n  \"Name\": \"Sernager\",\n  \"Number\": 1,\n  \"IsEnabled\": true,\n  \"CaseName\": \"Sernager\"\n}",
            "{\n  \"NAME\": \"Sernager\",\n  \"NUMBER\": 1,\n  \"ISENABLED\": true,\n  \"CASENAME\": \"Sernager\"\n}",
            "{\n  \"nAmE\": \"Sernager\",\n  \"nUmBeR\": 1,\n  \"iSeNaBlEd\": true,\n  \"cAsEnAmE\": \"Sernager\"\n}",
        ];

        foreach (string testCase in testCases)
        {
            FakeModel? model = JsonWrapper.Deserialize<FakeModel>(testCase);

            Assert.That(model, Is.Not.Null);
            Assert.That(model.Name, Is.EqualTo("Sernager"));
            Assert.That(model.Number, Is.EqualTo(1));
            Assert.That(model.IsEnabled, Is.True);
            Assert.That(model.CaseName, Is.EqualTo("Sernager"));
        }

        string[] someEmptyPropertiesCases =
        [
            "{\"name\":\"Sernager1\",\"number\":1,\"caseName\":\"Sernager1\"}",
            "{\"name\":\"Sernager2\",\"number\":2,\"isEnabled\":true}",
            "{\"name\":\"Sernager3\",\"number\":3,\"caseName\":\"Sernager3\"}",
            "{\"name\":\"Sernager4\",\"number\":4,\"isEnabled\":true}",
            "{\"name\":\"Sernager5\",\"number\":5,\"caseName\":\"Sernager5\"}",
            "{\"name\":\"Sernager6\",\"number\":6,\"isEnabled\":true}",
            "{\"name\":\"Sernager7\",\"number\":7,\"caseName\":\"Sernager7\"}",
            "{\"name\":\"Sernager8\",\"number\":8,\"isEnabled\":true}",
            "{\"name\":\"Sernager9\",\"number\":9,\"caseName\":\"Sernager9\"}",
        ];

        for (int i = 1; i < 10; ++i)
        {
            FakeModel? model = JsonWrapper.Deserialize<FakeModel>(someEmptyPropertiesCases[i - 1]);

            Assert.That(model, Is.Not.Null);
            Assert.That(model.Name, Is.EqualTo($"Sernager{i}"));
            Assert.That(model.Number, Is.EqualTo(i));
            Assert.That(model.IsEnabled, i % 2 == 0 ? Is.True : Is.Default);
            Assert.That(model.CaseName, i % 2 != 0 ? Is.EqualTo($"Sernager{i}") : Is.Empty);
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
