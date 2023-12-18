using Sernager.Core.Tests.Dummies;
using Sernager.Core.Tests.Fixtures;
using Sernager.Core.Utils;

namespace Sernager.Core.Tests.Units.Utils;

public class YamlWrapperTest : BaseFixture
{
    private NormalObject mNormalObject;
    private NormalObject[] mTestArray;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        mNormalObject = new NormalObject { Id = 1, Name = "Test" };
        mTestArray = [mNormalObject, new NormalObject { Id = 2, Name = "Test" }];
    }

    [Test]
    public void Serialize_NullObject_ReturnsEmptyString()
    {
        string result = YamlWrapper.Serialize(null!);

        Assert.That(result, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Serialize_ValidObject_ReturnsYamlString()
    {
        var testObject = new { Name = "Test", Value = 123 };
        string result = YamlWrapper.Serialize(testObject);

        Assert.That(result, Is.Not.Null.And.Not.Empty);
        Assert.That(result, Is.EqualTo("name: Test\r\nvalue: 123\r\n"));
    }

    [Test]
    public void Serialize_ListOfIntegers_ReturnsYamlSequence()
    {
        List<int> testObject = new List<int> { 1, 2, 3 };
        string result = YamlWrapper.Serialize(testObject);

        Assert.That(result, Is.EqualTo("- 1\r\n- 2\r\n- 3\r\n"));
    }

    [Test]
    public void Serialize_Dictionary_ReturnsYamlMapping()
    {
        Dictionary<string, int> testObject = new Dictionary<string, int> { { "TestName1", 1 }, { "TestName2", 2 }, { "TestName3", 3 } };
        string result = YamlWrapper.Serialize(testObject);

        Assert.That(result, Is.EqualTo("TestName1: 1\r\nTestName2: 2\r\nTestName3: 3\r\n"));
    }

    [Test]
    public void Serialize_Instance_ReturnsYamlString()
    {
        string result = YamlWrapper.Serialize(mNormalObject);

        Assert.That(result, Is.Not.Null.And.Not.Empty);
        Assert.That(result, Is.EqualTo("id: 1\r\nname: Test\r\n"));
    }

    [Test]
    public void Serialize_InstanceArray_ReturnsYamlSequence()
    {
        string result = YamlWrapper.Serialize(mTestArray);

        Assert.That(result, Is.Not.Null.And.Not.Empty);
        Assert.That(result, Is.EqualTo("- id: 1\r\n  name: Test\r\n- id: 2\r\n  name: Test\r\n"));
    }

    [Test]
    public void Serialize_OneIndentedObject_ReturnsNestedYaml()
    {
        var testObject = new { Parent = new { Child = "value" } };
        string result = YamlWrapper.Serialize(testObject);

        Assert.That(result, Is.EqualTo("parent:\r\n  child: value\r\n"));
    }

    [Test]
    public void Serialize_TwoIndentedObject_ReturnsNestedYaml()
    {
        var testObject = new { Parent = new { Child = new { GrandChild = "value" } } };
        string result = YamlWrapper.Serialize(testObject);

        Assert.That(result, Is.EqualTo("parent:\r\n  child:\r\n    grand_child: value\r\n"));
    }

    [Test]
    public void Deserialize_ValidYamlString_ReturnsObject()
    {
        string yaml = "Name: Test\r\nValue: 123\r\n";
        var result = YamlWrapper.Deserialize<dynamic>(yaml);

        Assert.That(result, Is.Not.Null);
        Assert.That(result["Name"], Is.EqualTo("Test"));
        Assert.That(result["Value"], Is.EqualTo("123"));
    }

    [Test]
    public void Deserialize_YamlSequence_ReturnsListOfIntegers()
    {
        string yaml = "- 1\r\n- 2\r\n- 3\r\n";
        List<int>? result = YamlWrapper.Deserialize<List<int>>(yaml);

        Assert.That(result, Is.EquivalentTo(new List<int> { 1, 2, 3 }));
    }

    [Test]
    public void Deserialize_YamlMapping_ReturnsDictionary()
    {
        string yaml = "one: 1\r\ntwo: 2\r\nthree: 3\r\n";
        Dictionary<string, int>? result = YamlWrapper.Deserialize<Dictionary<string, int>>(yaml);

        Assert.That(result, Is.EquivalentTo(new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } }));
    }

    [Test]
    public void Deserialize_YamlMapping_ReturnsInstance()
    {
        string yaml = "id: 1\r\nname: Test\r\n";
        var result = YamlWrapper.Deserialize<NormalObject>(yaml);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Name, Is.EqualTo("Test"));
    }

    [Test]
    public void Deserialize_YamlSequence_ReturnsInstanceArray()
    {
        string yaml = "- id: 1\r\n  name: Test\r\n- id: 2\r\n  name: Test\r\n";
        var result = YamlWrapper.Deserialize<NormalObject[]>(yaml);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Length, Is.EqualTo(2));
        Assert.That(result[0].Id, Is.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Test"));
        Assert.That(result[1].Id, Is.EqualTo(2));
        Assert.That(result[1].Name, Is.EqualTo("Test"));
    }

    [Test]
    public void Deserialize_OneIndentedYaml_ReturnsNestedObject()
    {
        string yaml = @"
Parent:
  Child: value
";
        var result = YamlWrapper.Deserialize<dynamic>(yaml);

        Assert.That(result, Is.Not.Null);
        Assert.That(result["Parent"]["Child"], Is.EqualTo("value"));
    }

    [Test]
    public void Deserialize_TwoIndentedYaml_ReturnsNestedObject()
    {
        string yaml = @"
Parent:
  Child:
    Grandchild: value
";
        var result = YamlWrapper.Deserialize<dynamic>(yaml);

        Assert.That(result, Is.Not.Null);
        Assert.That(result["Parent"]["Child"]["Grandchild"], Is.EqualTo("value"));
    }

    [Test]
    public void Deserialize_InvalidYaml_ReturnsDefault()
    {
        Assert.That(YamlWrapper.Deserialize<int>("invalid_yaml"), Is.Default);
        Assert.That(YamlWrapper.Deserialize<object>("name: Test\r\nvalue"), Is.Default);
    }
}
