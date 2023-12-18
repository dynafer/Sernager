using Sernager.Core.Tests.Dummies;
using Sernager.Core.Tests.Fixtures;
using Sernager.Core.Utils;
using System.Text.Json;

namespace Sernager.Core.Tests.Units.Utils;

public class JsonWrapperTest : BaseFixture
{
    private NormalObject mNormalObject;
    private NormalObject[] mTestArray;
    private UnserializableObject mUnserializableObject;
    private string mValidObjectJson;
    private string mValidArrayJson;
    private string?[] mInvalidCases;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        mNormalObject = new NormalObject { Id = 1, Name = "Test" };
        mTestArray = [mNormalObject, new NormalObject { Id = 2, Name = "Test" }];
        mUnserializableObject = new UnserializableObject();
        mValidObjectJson = "{\"id\":1,\"name\":\"Test\"}";
        mValidArrayJson = "[{\"id\":1,\"name\":\"Test\"},{\"id\":2,\"name\":\"Test\"}]";
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
        string result = JsonWrapper.Serialize(mNormalObject);

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
            NormalObject? result = JsonWrapper.Deserialize<NormalObject>(invalidCase!);

            Assert.That(result, Is.Null);
        }
    }

    [Test]
    public void Deserialize_WithValidObjectJson_ShouldReturnCorrectObject()
    {
        NormalObject? result = JsonWrapper.Deserialize<NormalObject>(mValidObjectJson);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(mNormalObject.Id));
        Assert.That(result.Name, Is.EqualTo(mNormalObject.Name));
    }

    [Test]
    public void Deserialize_WithValidArrayJson_ShouldReturnCorrectArray()
    {
        NormalObject[]? result = JsonWrapper.Deserialize<NormalObject[]>(mValidArrayJson);

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
        string result = JsonWrapper.Serialize(new NormalObject());

        Assert.That(result, Is.EqualTo("{\"id\":0,\"name\":\"\"}"));
    }

    [Test]
    public void Serialize_WithEmptyArray_ShouldReturnEmptyJson()
    {
        string result = JsonWrapper.Serialize(new NormalObject[2]);

        Assert.That(result, Is.EqualTo("[null,null]"));
    }

    [Test]
    public void Serialize_WithEmptyObjectArray_ShouldReturnEmptyJson()
    {
        string result = JsonWrapper.Serialize(new NormalObject[2] { new NormalObject(), new NormalObject() });

        Assert.That(result, Is.EqualTo("[{\"id\":0,\"name\":\"\"},{\"id\":0,\"name\":\"\"}]"));
    }

    [Test]
    public void Deserialize_WithEmptyObjectJson_ShouldReturnDefault()
    {
        NormalObject? result = JsonWrapper.Deserialize<NormalObject>("{}");

        Assert.That(result, Is.EqualTo(new NormalObject()));
    }

    [Test]
    public void Deserialize_WithEmptyArrayJson_ShouldReturnDefault()
    {
        NormalObject[]? result = JsonWrapper.Deserialize<NormalObject[]>("[]");

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
