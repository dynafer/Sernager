using ServiceRunner.Runner.Utils;
using System.Text.Json;

namespace ServiceRunner.Runner.Tests.Units.Utils;

public class JsonWrapperTest
{
    private class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private class UnserializableObject
    {
        public UnserializableObject Self { get; set; }

        public UnserializableObject()
        {
            Self = this;
        }
    }

    private TestObject mTestObject { get; set; }
    private TestObject[] mTestArray { get; set; }
    private UnserializableObject mUnserializableObject { get; set; }
    private string mValidObjectJson { get; set; }
    private string mValidArrayJson { get; set; }
    private string?[] mInvalidCases { get; set; }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        mTestObject = new TestObject { Id = 1, Name = "Test" };
        mTestArray = [mTestObject, new TestObject { Id = 2, Name = "Test" }];
        mUnserializableObject = new UnserializableObject();
        mValidObjectJson = "{\"Id\":1,\"Name\":\"Test\"}";
        mValidArrayJson = "[{\"Id\":1,\"Name\":\"Test\"},{\"Id\":2,\"Name\":\"Test\"}]";
        mInvalidCases = [
            null,
            "",
            "null",
            "{]",
            "[}",
            "{{}",
            "{}}",
            "[}]",
            "[{]",
            "{'a':1}",
            "{\"a':1}",
            "{\"a':1,}",
            "{'a\":1}",
            "{'a\":1,}",
            "['a':1]",
            "[\"a\":1]",
            "[\"a\":1,]",
            "[{'a':1}]",
            "[{\"a':1}]",
            "[{\"a':1,}]",
            "[{'a\":1}]",
            "[{'a\":1,}]",
        ];
    }

    [Test]
    public void Serialize_WithNull_ShouldReturnEmptyString()
    {
        string result = JsonWrapper.Serialize(null!);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Serialize_WithValidObject_ShouldReturnCorrectJson()
    {
        string result = JsonWrapper.Serialize(mTestObject);

        Assert.That(result, Is.EqualTo(mValidObjectJson));
    }

    [Test]
    public void Serialize_WithValidArray_ShouldReturnCorrectJson()
    {
        string result = JsonWrapper.Serialize(mTestArray);

        Assert.That(result, Is.EqualTo(mValidArrayJson));
    }

    [Test]
    public void Deserialize_WithInvalidJsons_ShouldReturnDefault()
    {
        for (int i = 0; i < mInvalidCases.Length; ++i)
        {
            string? invalidCase = mInvalidCases[i];
            TestObject? result = JsonWrapper.Deserialize<TestObject>(invalidCase!);

            Assert.That(result, Is.Null);
        }
    }

    [Test]
    public void Deserialize_WithValidObjectJson_ShouldReturnCorrectObject()
    {
        TestObject? result = JsonWrapper.Deserialize<TestObject>(mValidObjectJson);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(mTestObject.Id));
        Assert.That(result.Name, Is.EqualTo(mTestObject.Name));
    }

    [Test]
    public void Deserialize_WithValidArrayJson_ShouldReturnCorrectArray()
    {
        TestObject[]? result = JsonWrapper.Deserialize<TestObject[]>(mValidArrayJson);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Length, Is.EqualTo(mTestArray.Length));

        for (int i = 0; i < result.Length; ++i)
        {
            Assert.That(result[i].Id, Is.EqualTo(mTestArray[i].Id));
            Assert.That(result[i].Name, Is.EqualTo(mTestArray[i].Name));
        }
    }

    [Test]
    public void IsValid_WithNull_ShouldReturnFalse()
    {
        bool result = JsonWrapper.IsValid(null!);

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsValid_WithEmptyString_ShouldReturnFalse()
    {
        bool result = JsonWrapper.IsValid(string.Empty);

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsValid_WithInvalidJsons_ShouldReturnFalse()
    {
        for (int i = 0; i < mInvalidCases.Length; ++i)
        {
            string? invalidCase = mInvalidCases[i];
            bool result = JsonWrapper.IsValid(invalidCase!);

            Assert.That(result, Is.False);
        }
    }

    [Test]
    public void IsValid_WithValidObjectJson_ShouldReturnTrue()
    {
        bool result = JsonWrapper.IsValid(mValidObjectJson);

        Assert.That(result, Is.True);
    }

    [Test]
    public void IsValid_WithValidArrayJson_ShouldReturnTrue()
    {
        bool result = JsonWrapper.IsValid(mValidArrayJson);

        Assert.That(result, Is.True);
    }

    [Test]
    public void Serialize_WithEmptyObject_ShouldReturnEmptyJson()
    {
        string result = JsonWrapper.Serialize(new TestObject());

        Assert.That(result, Is.EqualTo("{\"Id\":0,\"Name\":\"\"}"));
    }

    [Test]
    public void Serialize_WithEmptyArray_ShouldReturnEmptyJson()
    {
        string result = JsonWrapper.Serialize(new TestObject[2]);

        Assert.That(result, Is.EqualTo("[null,null]"));
    }

    [Test]
    public void Serialize_WithEmptyObjectArray_ShouldReturnEmptyJson()
    {
        string result = JsonWrapper.Serialize(new TestObject[2] { new TestObject(), new TestObject() });

        Assert.That(result, Is.EqualTo("[{\"Id\":0,\"Name\":\"\"},{\"Id\":0,\"Name\":\"\"}]"));
    }

    [Test]
    public void Deserialize_WithEmptyObjectJson_ShouldReturnDefault()
    {
        TestObject? result = JsonWrapper.Deserialize<TestObject>("{}");

        Assert.That(result, Is.EqualTo(new TestObject()));
    }

    [Test]
    public void Deserialize_WithEmptyArrayJson_ShouldReturnDefault()
    {
        TestObject[]? result = JsonWrapper.Deserialize<TestObject[]>("[]");

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void IsValid_WithEmptyObjectJson_ShouldReturnTrue()
    {
        bool result = JsonWrapper.IsValid("{}");

        Assert.That(result, Is.True);
    }

    [Test]
    public void IsValid_WithEmptyArrayJson_ShouldReturnTrue()
    {
        bool result = JsonWrapper.IsValid("[]");

        Assert.That(result, Is.True);
    }

    [Test]
    public void Serialize_WithUnserializableObject_ShouldThrowException()
    {
        Assert.Throws<JsonException>(() => JsonWrapper.Serialize(mUnserializableObject));
    }
}
