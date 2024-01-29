using Sernager.Core.Configs;
using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Core.Options;
using Sernager.Core.Tests.Fixtures;

namespace Sernager.Core.Tests.Units.Managers;

public class EnvironmentManagerSuccessTests : EnvironmentManagerFixture
{
    private static readonly string ENV_FILE_ALIAS = "Envs.ValidEnv";
    private static readonly string INVALID_ENV_FILE_ALIAS = "Envs.InvalidEnv";
    private static readonly string KEY_NAME_FOR_SUBSTITUTION = "SUBSTITUTION_TEST";

    [Test]
    public void Constructor_ShouldCreateGroup()
    {
        string name = "inexistentName";

        Assert.That(Configurator.Config.EnvironmentGroups.ContainsKey(name), Is.False);

        IEnvironmentManager manager = new EnvironmentManager(name);

        Assert.That(Configurator.Config.EnvironmentGroups.ContainsKey(name), Is.True);

        Assert.That(manager.Group.Name, Is.EqualTo(name));
    }

    [Test]
    public void Constructor_ShouldUseExistingGroup()
    {
        Assert.That(Configurator.Config.EnvironmentGroups.ContainsKey(mGroup.Name), Is.True);

        Assert.That(mManager.Group, Is.EqualTo(mGroup));
        Assert.That(mManager.Group.Name, Is.EqualTo(mGroup.Name));
    }

    [Test]
    public void RemoveGroup_ShouldRemoveGroup()
    {
        mManager.RemoveGroup();

        Assert.That(Configurator.Config.EnvironmentGroups.ContainsKey(mGroup.Name), Is.False);

        Assert.That(mManager.Group, Is.Null);
    }

    [Test]
    public void RemoveGroup_ShouldRemoveGroupAndInListFromCommand()
    {
        Guid commandId = Guid.NewGuid();
        CommandModel commandModel = new CommandModel()
        {
            Name = "commandName",
            UsedEnvironmentGroups = new List<string>() { mGroup.Name }
        };

        Configurator.Config.Commands.Add(commandId, commandModel);

        mManager.RemoveGroup();

        Assert.That(Configurator.Config.EnvironmentGroups.ContainsKey(mGroup.Name), Is.False);

        Assert.That(mManager.Group, Is.Null);
        Assert.That(commandModel.UsedEnvironmentGroups.Contains(mGroup.Name), Is.False);
    }

    [Theory]
    public void AddFromFile_ShouldAddVariablesFromFile(string testType, EAddDataOption additionMode)
    {
        Assume.That(testType, Is.AnyOf(TEST_TYPES));
        Assume.That(additionMode, Is.AnyOf(ADDITION_MODES));

        mManager.AdditionMode = additionMode;

        string path = CaseUtil.GetPath(ENV_FILE_ALIAS, "env");

        Dictionary<string, string> target = getVariableDictionary(mGroup, testType);

        string[] existingKeys = target.Keys.ToArray();
        string existingVar = target[EXISTING_KEY_NAME];

        switch (testType)
        {
            case "Subst":
                mManager.AddFromSubstFile(path);
                break;
            case "Normal":
                mManager.AddFromFile(path);
                break;
            default:
                throw new Exception("Invalid test type");
        }

        switch (additionMode)
        {
            case EAddDataOption.SkipIfExists:
                foreach (string key in existingKeys)
                {
                    Assert.That(target.ContainsKey(key), Is.True);
                }

                Assert.That(target[EXISTING_KEY_NAME], Is.EqualTo(existingVar));
                break;
            case EAddDataOption.OverwriteIfExists:
                foreach (string key in existingKeys)
                {
                    Assert.That(target.ContainsKey(key), Is.True);
                }

                Assert.That(target[EXISTING_KEY_NAME], Is.Not.EqualTo(existingVar));
                break;
            case EAddDataOption.OverwriteAll:
                foreach (string key in existingKeys)
                {
                    if (key == EXISTING_KEY_NAME)
                    {
                        continue;
                    }

                    Assert.That(target.ContainsKey(key), Is.False);
                }

                Assert.That(target[EXISTING_KEY_NAME], Is.Not.EqualTo(existingVar));

                break;
            default:
                throw new Exception("Invalid addition mode");
        }

        Assert.That(target[KEY_NAME_FOR_SUBSTITUTION].Contains(" "), Is.False);
        Assert.That(target[KEY_NAME_FOR_SUBSTITUTION].Contains("\t"), Is.False);
    }

    [Theory]
    public void AddFromFile_ShouldNotAddAnyInvalidVariablesFromFile(string testType, EAddDataOption additionMode)
    {
        Assume.That(testType, Is.AnyOf(TEST_TYPES));
        Assume.That(additionMode, Is.AnyOf(ADDITION_MODES));

        mManager.AdditionMode = additionMode;

        string path = CaseUtil.GetPath(INVALID_ENV_FILE_ALIAS, "env");

        Dictionary<string, string> target = getVariableDictionary(mGroup, testType);

        int countBefore = target.Count;
        var existingVars = target.ToArray();

        switch (testType)
        {
            case "Subst":
                mManager.AddFromSubstFile(path);
                break;
            case "Normal":
                mManager.AddFromFile(path);
                break;
            default:
                throw new Exception("Invalid test type");
        }

        if (additionMode == EAddDataOption.OverwriteAll)
        {
            Assert.That(target.Count, Is.Zero);
            return;
        }
        else
        {
            Assert.That(target.Count, Is.EqualTo(countBefore));
        }

        foreach (var var in existingVars)
        {
            Assert.That(target.ContainsKey(var.Key), Is.True);
            Assert.That(target[var.Key], Is.EqualTo(var.Value));
        }
    }

    [Theory]
    public void AddLines_ShouldAddVariablesFromLines(string testType, EAddDataOption additionMode)
    {
        Assume.That(testType, Is.AnyOf(TEST_TYPES));
        Assume.That(additionMode, Is.AnyOf(ADDITION_MODES));

        mManager.AdditionMode = additionMode;

        Dictionary<string, string> target = getVariableDictionary(mGroup, testType);

        Dictionary<string, string> newLines = new Dictionary<string, string>()
        {
            { "new_key1", "value1" },
            { "new_key2", "value2" },
            { "new_key3", "value3" },
            { EXISTING_KEY_NAME, "value4" },
            { "substitution_key1", "${  value1}" },
            { "substitution_key2", "${value2  }" },
            { "substitution_key3", "${\tvalue3}" },
            { "substitution_key4", "${value4\t}" },
            { "substitution_key5", "${  value5   }" },
            { "substitution_key6", "${\tvalue6\t}" },
            { "substitution_key7", "${  value7\t}" },
            { "substitution_key8", "${\tvalue8  }" },
        };

        int countBefore = target.Count;
        string[] existingKeys = target.Keys.ToArray();
        string existingVar = target[EXISTING_KEY_NAME];
        string[] newLinesArr = newLines.Select(x => $"{x.Key}={x.Value}").ToArray();

        switch (testType)
        {
            case "Subst":
                mManager.AddSubstLines(newLinesArr);
                break;
            case "Normal":
                mManager.AddLines(newLinesArr);
                break;
            default:
                throw new Exception("Invalid test type");
        }

        Assert.That(target.Count, Is.Not.EqualTo(countBefore));

        foreach (var line in newLines)
        {
            if (line.Key == EXISTING_KEY_NAME || line.Key.Contains("substitution_key"))
            {
                continue;
            }

            Assert.That(target.ContainsKey(line.Key), Is.True);
            Assert.That(target[line.Key], Is.EqualTo(line.Value));
        }

        switch (additionMode)
        {
            case EAddDataOption.SkipIfExists:
                foreach (string key in existingKeys)
                {
                    Assert.That(target.ContainsKey(key), Is.True);
                }

                Assert.That(target[EXISTING_KEY_NAME], Is.EqualTo(existingVar));
                break;
            case EAddDataOption.OverwriteIfExists:
            case EAddDataOption.OverwriteAll:
                foreach (string key in existingKeys)
                {
                    Assert.That(target.ContainsKey(key), Is.True);
                }

                Assert.That(target[EXISTING_KEY_NAME], Is.Not.EqualTo(existingVar));
                break;
            default:
                throw new Exception("Invalid addition mode");
        }

        for (int i = 1; i <= 8; ++i)
        {
            string key = $"substitution_key{i}";
            Assert.That(target[key].Contains(" "), Is.False);
            Assert.That(target[key].Contains("\t"), Is.False);
        }
    }

    [Theory]
    public void GetVariableOrNull_ShouldReturnValueOrNull(string testType)
    {
        Assume.That(testType, Is.AnyOf(TEST_TYPES));

        Dictionary<string, string> target = getVariableDictionary(mGroup, testType);

        string key = target.Keys.First();
        string value = target[key];
        string inexistentKey = "inexistent_key";

        switch (testType)
        {
            case "Subst":
                Assert.That(mManager.GetSubstVariableOrNull(key), Is.EqualTo(value));
                Assert.That(mManager.GetSubstVariableOrNull(inexistentKey), Is.Null);
                break;
            case "Normal":
                Assert.That(mManager.GetVariableOrNull(key), Is.EqualTo(value));
                Assert.That(mManager.GetVariableOrNull(inexistentKey), Is.Null);
                break;
            default:
                throw new Exception("Invalid test type");
        }
    }

    [Theory]
    public void SetVariable_ShouldSetVariable(string testType, EAddDataOption additionMode)
    {
        Assume.That(testType, Is.AnyOf(TEST_TYPES));
        Assume.That(additionMode, Is.AnyOf(ADDITION_MODES));

        mManager.AdditionMode = additionMode;

        string key = "new_key";
        string value = "${ \t  TEST1\t   \t}";

        Dictionary<string, string> target = getVariableDictionary(mGroup, testType);

        int countBefore = target.Count;

        Assert.That(target.ContainsKey(key), Is.False);

        switch (testType)
        {
            case "Subst":
                mManager.SetSubstVariable(key, value);
                break;
            case "Normal":
                mManager.SetVariable(key, value);
                break;
            default:
                throw new Exception("Invalid test type");
        }

        Assert.That(target.Count, Is.Not.EqualTo(countBefore));

        Assert.That(target.ContainsKey(key), Is.True);
        Assert.That(target[key], Is.EqualTo(value.Replace(" ", "").Replace("\t", "")));

        countBefore = target.Count;
        value = "new_value2";

        switch (testType)
        {
            case "Subst":
                mManager.SetSubstVariable(key, value);
                break;
            case "Normal":
                mManager.SetVariable(key, value);
                break;
            default:
                throw new Exception("Invalid test type");
        }

        Assert.That(target.Count, Is.EqualTo(countBefore));

        switch (additionMode)
        {
            case EAddDataOption.SkipIfExists:
                Assert.That(target[key], Is.Not.EqualTo(value));
                break;
            case EAddDataOption.OverwriteIfExists:
            case EAddDataOption.OverwriteAll:
                Assert.That(target[key], Is.EqualTo(value));
                break;
            default:
                throw new Exception("Invalid addition mode");
        }
    }

    [Theory]
    public void SetVariables_ShouldSetVariables(string testType, EAddDataOption additionMode)
    {
        Assume.That(testType, Is.AnyOf(TEST_TYPES));
        Assume.That(additionMode, Is.AnyOf(ADDITION_MODES));

        mManager.AdditionMode = additionMode;

        Dictionary<string, string> newLines = new Dictionary<string, string>()
        {
            { "new_key1", "value1" },
            { "new_key2", "value2" },
            { "new_key3", "value3" },
            { EXISTING_KEY_NAME, "value4" },
            { "substitution_key1", "${  value1}" },
            { "substitution_key2", "${value2  }" },
            { "substitution_key3", "${\tvalue3}" },
            { "substitution_key4", "${value4\t}" },
            { "substitution_key5", "${  value5   }" },
            { "substitution_key6", "${\tvalue6\t}" },
            { "substitution_key7", "${  value7\t}" },
            { "substitution_key8", "${\tvalue8  }" },
        };

        Dictionary<string, string> target = getVariableDictionary(mGroup, testType);

        int countBefore = target.Count;
        string[] existingKeys = target.Keys.ToArray();
        string existingVar = target[EXISTING_KEY_NAME];

        switch (testType)
        {
            case "Subst":
                mManager.SetSubstVariables(newLines);
                break;
            case "Normal":
                mManager.SetVariables(newLines);
                break;
            default:
                throw new Exception("Invalid test type");
        }

        Assert.That(target.Count, Is.Not.EqualTo(countBefore));

        foreach (var line in newLines)
        {
            if (line.Key == EXISTING_KEY_NAME || line.Key.Contains("substitution_key"))
            {
                continue;
            }

            Assert.That(target.ContainsKey(line.Key), Is.True);
            Assert.That(target[line.Key], Is.EqualTo(line.Value));
        }

        switch (additionMode)
        {
            case EAddDataOption.SkipIfExists:
                foreach (string key in existingKeys)
                {
                    Assert.That(target.ContainsKey(key), Is.True);
                }

                Assert.That(target[EXISTING_KEY_NAME], Is.EqualTo(existingVar));
                break;
            case EAddDataOption.OverwriteIfExists:
            case EAddDataOption.OverwriteAll:
                foreach (string key in existingKeys)
                {
                    Assert.That(target.ContainsKey(key), Is.True);
                }

                Assert.That(target[EXISTING_KEY_NAME], Is.Not.EqualTo(existingVar));
                break;
            default:
                throw new Exception("Invalid addition mode");
        }

        for (int i = 1; i <= 8; ++i)
        {
            string key = $"substitution_key{i}";
            Assert.That(target[key].Contains(" "), Is.False, $"key: {key}, value: {target[key]}");
            Assert.That(target[key].Contains("\t"), Is.False);
        }
    }

    [Theory]
    public void RemoveVariable_ShouldRemoveVariableOrSkip(string testType)
    {
        Assume.That(testType, Is.AnyOf(TEST_TYPES));

        Dictionary<string, string> target = getVariableDictionary(mGroup, testType);

        string key = target.Keys.First();
        string inexistentKey = "inexistent_key";

        Assert.That(target.ContainsKey(key), Is.True);

        switch (testType)
        {
            case "Subst":
                mManager.RemoveSubstVariable(key);
                mManager.RemoveSubstVariable(inexistentKey);
                break;
            case "Normal":
                mManager.RemoveVariable(key);
                mManager.RemoveVariable(inexistentKey);
                break;
            default:
                throw new Exception("Invalid test type");
        }

        Assert.That(target.ContainsKey(key), Is.False);
    }

    [Theory]
    public void RemoveVariables_ShouldRemoveVariablesOrSkip(string testType)
    {
        Assume.That(testType, Is.AnyOf(TEST_TYPES));

        Dictionary<string, string> target = getVariableDictionary(mGroup, testType);

        string[] keys = target.Keys.ToArray();
        string[] inexistentKeys = ["inexistent_key1", "inexistent_key2"];

        Assert.That(keys.Length, Is.GreaterThan(0));

        switch (testType)
        {
            case "Subst":
                mManager.RemoveSubstVariables(keys);
                mManager.RemoveSubstVariables(inexistentKeys);
                break;
            case "Normal":
                mManager.RemoveVariables(keys);
                mManager.RemoveVariables(inexistentKeys);
                break;
            default:
                throw new Exception("Invalid test type");
        }

        foreach (string key in keys)
        {
            Assert.That(target.ContainsKey(key), Is.False);
        }
    }
}
