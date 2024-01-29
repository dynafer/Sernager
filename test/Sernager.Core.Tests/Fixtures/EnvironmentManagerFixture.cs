using Sernager.Core.Configs;
using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Core.Options;
using System.Diagnostics;

namespace Sernager.Core.Tests.Fixtures;

public abstract class EnvironmentManagerFixture : FailureFixture
{
    private protected static readonly string CONFIG_ALIAS = "Configs.Defaults.Specifications.Environment";
    private protected static readonly string EXISTING_KEY_NAME = "TEST2";
    [DatapointSource]
    private protected static readonly string[] TEST_TYPES = ["Subst", "Normal"];
    [DatapointSource]
    private protected static readonly EAddDataOption[] ADDITION_MODES = Enum.GetValues<EAddDataOption>();
    private protected EnvironmentModel mGroup;
    private protected IEnvironmentManager mManager;

    [SetUp]
    public void Setup()
    {
        Configurator.Parse(CaseUtil.GetPath(CONFIG_ALIAS, "json"));
        mGroup = findGroupWithMostItems();
        mManager = new EnvironmentManager(mGroup.Name);
    }

    [TearDown]
    public void ResetConfigurator()
    {
        ResetUtil.ResetConfigurator();
    }

    [StackTraceHidden]
    private protected EnvironmentModel findGroupWithMostItems()
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

    [StackTraceHidden]
    private protected Dictionary<string, string> getVariableDictionary(EnvironmentModel group, string testType)
    {
        return testType switch
        {
            "Subst" => group.SubstVariables,
            "Normal" => group.Variables,
            _ => throw new Exception("Invalid test type")
        };
    }
}
