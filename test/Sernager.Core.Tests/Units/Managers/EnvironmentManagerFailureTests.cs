using Sernager.Core.Configs;
using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Core.Options;
using System.Diagnostics;

namespace Sernager.Core.Tests.Units.Managers;

public class EnvironmentManagerFailureTests : FailureFixture
{
    private static readonly string ALIAS = "Configs.Defaults.Specifications.Environment";
    private static readonly string ENV_FILE_ALIAS = "Envs.InvalidEnv";
    private static readonly string EXISTING_KEY_NAME = "TEST2";
    [DatapointSource]
    private static readonly string[] TEST_TYPES = ["Subst", "Normal"];
    [DatapointSource]
    private static readonly EAddDataOption[] ADDITION_MODES = Enum.GetValues<EAddDataOption>();
    private EnvironmentModel mGroup;
    private IEnvironmentManager mManager;

    [SetUp]
    public void SetUpConfigurator()
    {
        Configurator.Parse(CaseUtil.GetPath(ALIAS, "json"));
        mGroup = findGroupWithMostItems();
        mManager = new EnvironmentManager(mGroup.Name);
    }

    [TearDown]
    public void ResetConfigurator()
    {
        ResetUtil.ResetConfigurator();
    }

    [Test]
    public void RemoveGroup_ShouldThrow_WhenGroupAlreadyRemoved()
    {
        mManager.RemoveGroup();

        TestNoneLevel(mManager.RemoveGroup);
        TestExceptionLevel<SernagerException>(mManager.RemoveGroup);
    }

    [Theory]
    public void AddFromFile_ShouldThrow_WhenGroupAlreadyRemoved(string testType, EAddDataOption additionMode)
    {
        Assume.That(testType, Is.AnyOf(TEST_TYPES));
        Assume.That(additionMode, Is.AnyOf(ADDITION_MODES));

        mManager.AdditionMode = additionMode;

        string path = CaseUtil.GetPath(ENV_FILE_ALIAS, "env");

        mManager.RemoveGroup();

        switch (testType)
        {
            case "Subst":
                TestNoneLevel(() => mManager.AddFromSubstFile(path), Is.EqualTo(mManager));
                TestExceptionLevel<SernagerException>(() => mManager.AddFromSubstFile(path));
                break;
            case "Normal":
                TestNoneLevel(() => mManager.AddFromFile(path), Is.EqualTo(mManager));
                TestExceptionLevel<SernagerException>(() => mManager.AddFromFile(path));
                break;
            default:
                throw new Exception("Invalid test type");
        }
    }

    [Theory]
    public void AddFromFile_ShouldThrow_WhenFileDoesNotExist(string testType, EAddDataOption additionMode)
    {
        Assume.That(testType, Is.AnyOf(TEST_TYPES));
        Assume.That(additionMode, Is.AnyOf(ADDITION_MODES));

        mManager.AdditionMode = additionMode;

        string path = "./InvalidPath.env";

        switch (testType)
        {
            case "Subst":
                TestNoneLevel(() => mManager.AddFromSubstFile(path), Is.EqualTo(mManager));
                TestExceptionLevel<FileNotFoundException>(() => mManager.AddFromSubstFile(path));
                break;
            case "Normal":
                TestNoneLevel(() => mManager.AddFromFile(path), Is.EqualTo(mManager));
                TestExceptionLevel<FileNotFoundException>(() => mManager.AddFromFile(path));
                break;
            default:
                throw new Exception("Invalid test type");
        }
    }

    [Theory]
    public void AddLines_ShouldThrow_WhenGroupAlreadyRemoved(string testType, EAddDataOption additionMode)
    {
        Assume.That(testType, Is.AnyOf(TEST_TYPES));
        Assume.That(additionMode, Is.AnyOf(ADDITION_MODES));

        mManager.AdditionMode = additionMode;

        string[] lines =
        [
            "ENV1=ENV1",
            "ENV2=ENV2",
            "ENV3=ENV3",
        ];

        mManager.RemoveGroup();

        switch (testType)
        {
            case "Subst":
                TestNoneLevel(() => mManager.AddSubstLines(lines), Is.EqualTo(mManager));
                TestExceptionLevel<SernagerException>(() => mManager.AddSubstLines(lines));
                break;
            case "Normal":
                TestNoneLevel(() => mManager.AddLines(lines), Is.EqualTo(mManager));
                TestExceptionLevel<SernagerException>(() => mManager.AddLines(lines));
                break;
            default:
                throw new Exception("Invalid test type");
        }
    }

    [Theory]
    public void GetVariableOrNull_ShouldThrow_WhenGroupAlreadyRemoved(string testType)
    {
        Assume.That(testType, Is.AnyOf(TEST_TYPES));

        mManager.RemoveGroup();

        switch (testType)
        {
            case "Subst":
                TestNoneLevel(() => mManager.GetSubstVariableOrNull(EXISTING_KEY_NAME), Is.Null);
                TestExceptionLevel<SernagerException>(() => mManager.GetSubstVariableOrNull(EXISTING_KEY_NAME));
                break;
            case "Normal":
                TestNoneLevel(() => mManager.GetVariableOrNull(EXISTING_KEY_NAME), Is.Null);
                TestExceptionLevel<SernagerException>(() => mManager.GetVariableOrNull(EXISTING_KEY_NAME));
                break;
            default:
                throw new Exception("Invalid test type");
        }
    }

    [Theory]
    public void SetVariable_ShouldThrow_WhenGroupAlreadyRemoved(string testType)
    {
        Assume.That(testType, Is.AnyOf(TEST_TYPES));

        mManager.RemoveGroup();

        switch (testType)
        {
            case "Subst":
                TestNoneLevel(() => mManager.SetSubstVariable(EXISTING_KEY_NAME, "TEST"), Is.EqualTo(mManager));
                TestExceptionLevel<SernagerException>(() => mManager.SetSubstVariable(EXISTING_KEY_NAME, "TEST"));
                break;
            case "Normal":
                TestNoneLevel(() => mManager.SetVariable(EXISTING_KEY_NAME, "TEST"), Is.EqualTo(mManager));
                TestExceptionLevel<SernagerException>(() => mManager.SetVariable(EXISTING_KEY_NAME, "TEST"));
                break;
            default:
                throw new Exception("Invalid test type");
        }
    }

    [Theory]
    public void SetVariables_ShouldThrow_WhenGroupAlreadyRemoved(string testType)
    {
        Assume.That(testType, Is.AnyOf(TEST_TYPES));

        Dictionary<string, string> variables = new Dictionary<string, string>()
        {
            { "ENV1", "ENV1" },
            { "ENV2", "ENV2" },
            { "ENV3", "ENV3" },
            { EXISTING_KEY_NAME, "TEST" },
        };

        mManager.RemoveGroup();

        switch (testType)
        {
            case "Subst":
                TestNoneLevel(() => mManager.SetSubstVariables(variables), Is.EqualTo(mManager));
                TestExceptionLevel<SernagerException>(() => mManager.SetSubstVariables(variables));
                break;
            case "Normal":
                TestNoneLevel(() => mManager.SetVariables(variables), Is.EqualTo(mManager));
                TestExceptionLevel<SernagerException>(() => mManager.SetVariables(variables));
                break;
            default:
                throw new Exception("Invalid test type");
        }
    }

    [Theory]
    public void RemoveVariable_ShouldThrow_WhenGroupAlreadyRemoved(string testType)
    {
        Assume.That(testType, Is.AnyOf(TEST_TYPES));

        mManager.RemoveGroup();

        switch (testType)
        {
            case "Subst":
                TestNoneLevel(() => mManager.RemoveSubstVariable(EXISTING_KEY_NAME), Is.EqualTo(mManager));
                TestExceptionLevel<SernagerException>(() => mManager.RemoveSubstVariable(EXISTING_KEY_NAME));
                break;
            case "Normal":
                TestNoneLevel(() => mManager.RemoveVariable(EXISTING_KEY_NAME), Is.EqualTo(mManager));
                TestExceptionLevel<SernagerException>(() => mManager.RemoveVariable(EXISTING_KEY_NAME));
                break;
            default:
                throw new Exception("Invalid test type");
        }
    }

    [Theory]
    public void RemoveVariables_ShouldThrow_WhenGroupAlreadyRemoved(string testType)
    {
        Assume.That(testType, Is.AnyOf(TEST_TYPES));

        string[] keys =
        [
            EXISTING_KEY_NAME,
            "ENV1",
            "ENV2",
            "ENV3"
        ];

        mManager.RemoveGroup();

        switch (testType)
        {
            case "Subst":
                TestNoneLevel(() => mManager.RemoveSubstVariables(keys), Is.EqualTo(mManager));
                TestExceptionLevel<SernagerException>(() => mManager.RemoveSubstVariables(keys));
                break;
            case "Normal":
                TestNoneLevel(() => mManager.RemoveVariables(keys), Is.EqualTo(mManager));
                TestExceptionLevel<SernagerException>(() => mManager.RemoveVariables(keys));
                break;
            default:
                throw new Exception("Invalid test type");
        }
    }

    [StackTraceHidden]
    private EnvironmentModel findGroupWithMostItems()
    {
        int max = 0;
        string name = "";

        foreach (var group in Configurator.Config.EnvironmentGroups)
        {
            int cur = group.Value.SubstVariables.Count + group.Value.Variables.Count;
            if (cur > max)
            {
                max = cur;
                name = group.Key;
            }
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("No group found");
        }

        return Configurator.Config.EnvironmentGroups[name];
    }
}
