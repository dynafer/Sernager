using Sernager.Core.Tests.Fakes;
using Sernager.Core.Utils;

namespace Sernager.Core.Tests.Units.Utils;

public class YamlWrapperTests
{
    [Test]
    public void Serialize_ShouldReturnEmptyString_WhenPassedNullObject()
    {
        string yaml = YamlWrapper.Serialize(null!);

        Assert.That(yaml, Is.Not.Null);
        Assert.That(yaml, Is.Empty);
    }

    [Test]
    public void Serialize_ShouldReturnEmptyYamlString_WhenPassedEmptyObject()
    {
        string yaml = YamlWrapper.Serialize(new object()).Trim();

        Assert.That(yaml, Is.Not.Null);
        Assert.That(yaml, Is.EqualTo("{}"));
    }

    [Test]
    public void Serialize_ShouldReturnYamlString_WhenPassedValidObject()
    {
        string yaml = YamlWrapper.Serialize(new { Name = "Sernager" }).Trim();

        Assert.That(yaml, Is.Not.Null);
        Assert.That(yaml, Is.EqualTo("name: Sernager"));
    }

    [Test]
    public void Serialize_ShouldReturnYamlString_WhenPassedValidClass()
    {
        FakeModel fakeModel = new FakeModel()
        {
            Name = "Sernager",
            Number = 1,
            IsEnabled = true,
            CaseName = "sernager"
        };

        string yaml = YamlWrapper.Serialize(fakeModel).Replace("\r\n", "\n").Trim();

        Assert.That(yaml, Is.Not.Null);
        Assert.That(yaml, Is.EqualTo("name: Sernager\nnumber: 1\nisEnabled: true\ncaseName: sernager\nnullName:"));
    }

    [Test]
    public void Serialize_ShouldReturnYamlString_WhenPassedValidClassWithSomeEmptyProperties()
    {
        FakeModel fakeModel = new FakeModel()
        {
            Name = "Sernager",
            Number = 1,
            IsEnabled = true
        };

        string yaml = YamlWrapper.Serialize(fakeModel).Replace("\r\n", "\n").Trim();

        Assert.That(yaml, Is.Not.Null);
        Assert.That(yaml, Is.EqualTo("name: Sernager\nnumber: 1\nisEnabled: true\ncaseName: \"\"\nnullName:"));

        Assert.That(YamlWrapper.Deserialize<FakeModel>(yaml), Is.EqualTo(fakeModel));

        fakeModel = new FakeModel()
        {
            Name = "Sernager",
            Number = 1,
            CaseName = "sernager"
        };

        yaml = YamlWrapper.Serialize(fakeModel).Replace("\r\n", "\n").Trim();

        Assert.That(yaml, Is.Not.Null);
        Assert.That(yaml, Is.EqualTo("name: Sernager\nnumber: 1\nisEnabled: false\ncaseName: sernager\nnullName:"));

        fakeModel = new FakeModel()
        {
            Name = "Sernager",
            IsEnabled = true,
            CaseName = "sernager"
        };

        yaml = YamlWrapper.Serialize(fakeModel).Replace("\r\n", "\n").Trim();

        Assert.That(yaml, Is.Not.Null);
        Assert.That(yaml, Is.EqualTo("name: Sernager\nnumber: 0\nisEnabled: true\ncaseName: sernager\nnullName:"));

        fakeModel = new FakeModel()
        {
            Name = "Sernager",
        };

        yaml = YamlWrapper.Serialize(fakeModel).Replace("\r\n", "\n").Trim();

        Assert.That(yaml, Is.Not.Null);
        Assert.That(yaml, Is.EqualTo("name: Sernager\nnumber: 0\nisEnabled: false\ncaseName: \"\"\nnullName:"));
    }

    public void Deserialize_ShouldReturDefault_WhenPassedNull()
    {
        Assert.That(YamlWrapper.Deserialize<int>(null!), Is.Default);
        Assert.That(YamlWrapper.Deserialize<string>(null!), Is.Default);
        Assert.That(YamlWrapper.Deserialize<object>(null!), Is.Default);
        Assert.That(YamlWrapper.Deserialize<FakeModel>(null!), Is.Default);
    }

    [Test]
    public void Deserialize_ShouldReturnDefault_WhenPassedEmptyString()
    {
        Assert.That(YamlWrapper.Deserialize<int>(string.Empty), Is.Default);
        Assert.That(YamlWrapper.Deserialize<string>(string.Empty), Is.Default);
        Assert.That(YamlWrapper.Deserialize<object>(string.Empty), Is.Default);
        Assert.That(YamlWrapper.Deserialize<FakeModel>(string.Empty), Is.Default);
    }

    [Test]
    public void Deserialize_ShouldReturnDefault_WhenPassedWrongYaml()
    {
        Assert.That(YamlWrapper.Deserialize<int>("name: Sernager"), Is.Default);
        Assert.That(YamlWrapper.Deserialize<string>("name: Sernager"), Is.Default);
    }

    [Test]
    public void Deserialize_ShouldReturnDefault_WhenPassedInvalidCasesYaml()
    {
        string snake_case = "name: Sernager\nnumber: 1\nis_enabled: true\ncase_name: sernager";

        Assert.That(YamlWrapper.Deserialize<FakeModel>(snake_case), Is.Default);

        string PascalCase = "Name: Sernager\nNumber: 1\nIsEnabled: true\nCaseName: sernager";

        Assert.That(YamlWrapper.Deserialize<FakeModel>(PascalCase), Is.Default);
    }

    [Test]
    public void Deserialize_ShouldReturnObject_WhenPassedValidYaml()
    {
        string yaml = "name: Sernager\ncamelCase: Sernager\nsnake_case: Sernager\nPascalCase: Sernager";
        object? obj = YamlWrapper.Deserialize<object>(yaml);

        Assert.That(obj, Is.Not.Null);
        Assert.That(obj, Is.TypeOf<Dictionary<object, object>>());
        Assert.That(obj, Is.EqualTo(new Dictionary<object, object>
        {
            { "name", "Sernager" },
            { "camelCase", "Sernager" },
            { "snake_case", "Sernager" },
            { "PascalCase", "Sernager" },
        }));
    }

    [Test]
    public void Deserialize_ShouldReturnClass_WhenPassedValidYaml()
    {
        string yaml = "name: Sernager\nnumber: 1\nisEnabled: true\ncaseName: sernager";
        FakeModel? fakeModel = YamlWrapper.Deserialize<FakeModel>(yaml);

        Assert.That(fakeModel, Is.Not.Null);
        Assert.That(fakeModel, Is.TypeOf<FakeModel>());
        Assert.That(fakeModel, Is.EqualTo(new FakeModel
        {
            Name = "Sernager",
            Number = 1,
            IsEnabled = true,
            CaseName = "sernager"
        }));
    }

    [Test]
    public void Deserialize_ShouldReturnClass_WhenPassValidYamlWithSomeEmptyProperties()
    {
        string yaml = "name: Sernager";
        FakeModel? fakeModel = YamlWrapper.Deserialize<FakeModel>(yaml);

        Assert.That(fakeModel, Is.Not.Null);
        Assert.That(fakeModel, Is.TypeOf<FakeModel>());
        Assert.That(fakeModel, Is.EqualTo(new FakeModel
        {
            Name = "Sernager",
            Number = 0,
            IsEnabled = false,
            CaseName = string.Empty,
            NullName = null
        }));

        yaml = "name: Sernager\nnumber: 1\nisEnabled: true\ncaseName: sernager\nnullName: null";
        fakeModel = YamlWrapper.Deserialize<FakeModel>(yaml);

        Assert.That(fakeModel, Is.Not.Null);
        Assert.That(fakeModel, Is.TypeOf<FakeModel>());
        Assert.That(fakeModel, Is.EqualTo(new FakeModel
        {
            Name = "Sernager",
            Number = 1,
            IsEnabled = true,
            CaseName = "sernager",
            NullName = null
        }));

        yaml = "name: Sernager\nnumber: 1\nisEnabled: true\ncaseName: sernager\nnullName: \"\"";
        fakeModel = YamlWrapper.Deserialize<FakeModel>(yaml);

        Assert.That(fakeModel, Is.Not.Null);
        Assert.That(fakeModel, Is.TypeOf<FakeModel>());
        Assert.That(fakeModel, Is.EqualTo(new FakeModel
        {
            Name = "Sernager",
            Number = 1,
            IsEnabled = true,
            CaseName = "sernager",
            NullName = string.Empty
        }));
    }
}
