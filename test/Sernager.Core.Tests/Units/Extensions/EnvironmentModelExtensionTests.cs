using Sernager.Core.Extensions;
using Sernager.Core.Models;
using Sernager.Core.Options;

namespace Sernager.Core.Tests.Units.Extensions;

internal sealed class EnvironmentModelExtensionTests
{
    [DatapointSource]
    private static readonly EEnvironmentType[] TEST_TYPES = Enum.GetValues<EEnvironmentType>();
    private EnvironmentModel mModel;

    [SetUp]
    public void SetUp()
    {
        mModel = new EnvironmentModel();
    }

    [TearDown]
    public void TearDown()
    {
        mModel.Variables.Clear();
        mModel.SubstVariables.Clear();
    }

    [Test]
    public void RemoveWhitespacesInDeclaredVariables_ShouldThrow_WhenTypeIsInvalid()
    {
        Assert.Throws<NotImplementedException>(() => mModel.RemoveWhitespacesInDeclaredVariables((EEnvironmentType)int.MaxValue, "TEST"));
    }

    [Theory]
    public void RemoveWhitespacesInDeclaredVariables_ShouldRemoveWhitespaces(EEnvironmentType testType)
    {
        Assume.That(testType, Is.AnyOf(TEST_TYPES));

        Dictionary<string, string> target = getVariableDictionary(testType);

        Dictionary<string, string> testCases = new Dictionary<string, string>()
        {
            { "TEST1", "TEST ${TestVar}" },
            { "TEST2", "TEST ${ TestVar }" },
            { "TEST3", "TEST ${TestVar }" },
            { "TEST4", "TEST ${ TestVar}" },
            { "TEST5", "TEST ${\tTestVar\t}" },
            { "TEST6", "TEST ${TestVar\t}" },
            { "TEST7", "TEST ${\tTestVar}" },
            { "TEST8", "TEST ${\nTestVar\n}" },
            { "TEST9", "TEST ${TestVar\n}" },
            { "TEST10", "TEST ${\nTestVar}" },
            { "TEST11", "TEST ${\rTestVar\r}" },
            { "TEST12", "TEST ${TestVar\r}" },
            { "TEST13", "TEST ${\rTestVar}" },
            { "TEST14", "TEST ${\t\n\rTestVar\t\n\r}" },
            { "TEST15", "TEST ${TestVar\t\n\r}" },
            { "TEST16", "TEST ${\t\n\rTestVar}" },
        };

        foreach (var testCase in testCases)
        {
            target.Add(testCase.Key, testCase.Value);

            mModel.RemoveWhitespacesInDeclaredVariables(testType, testCase.Key);
        }

        foreach (var variable in target)
        {
            Assert.That(variable.Value, Is.EqualTo("TEST ${TestVar}"));
        }
    }

    [Theory]
    public void RemoveWhitespacesInDeclaredVariables_ShouldNotRemoveWhitespaces(EEnvironmentType testType)
    {
        Assume.That(testType, Is.AnyOf(TEST_TYPES));

        Dictionary<string, string> target = getVariableDictionary(testType);

        (string, string)[] cases =
        [
            ("TEST $VAR",                "TEST $VAR"),
            ("TEST $$VAR",               "TEST $$VAR"),
            ("TEST ${  $ VAR  }",        "TEST ${$ VAR  }"),
            ("TEST ${ { VAR }",          "TEST ${{ VAR }"),
            ("TEST $ {V$AR  }",          "TEST $ {V$AR  }"),
            ("TEST $ {  $VAR}",          "TEST $ {  $VAR}"),
            ("TEST $ {  VA$R  }",        "TEST $ {  VA$R  }"),
            ("TEST ${VAR$  }",           "TEST ${VAR$  }"),
            ("TEST ${  VA$R}",           "TEST ${VA$R}"),
            ("TEST ${  V$AR  }",         "TEST ${V$AR  }"),
        ];

        Dictionary<string, (string, string)> testCases = new Dictionary<string, (string, string)>();
        for (int i = 0; i < cases.Length; ++i)
        {
            testCases.Add($"INVALID_TEST{i}", cases[i]);
        }

        foreach (var testCase in testCases)
        {
            target.Add(testCase.Key, testCase.Value.Item1);

            mModel.RemoveWhitespacesInDeclaredVariables(testType, testCase.Key);
        }

        foreach (var variable in target)
        {
            Assert.That(variable.Value, Is.EqualTo(testCases[variable.Key].Item2));
        }
    }

    [Test]
    public void BuildVariables_ShouldReturnDictionary_ThatSubstitutedVariablesAreApplied()
    {
        Dictionary<string, string> substVariables = new Dictionary<string, string>()
        {
            { "SUBST_VAR1", "SUBST_VAR1_VALUE" },
            { "SUBST_VAR2", "SUBST_VAR2_VALUE" },
            { "SUBST_VAR3", "SUBST_VAR3_VALUE" },
        };

        foreach (var variable in substVariables)
        {
            mModel.SubstVariables.Add(variable.Key, variable.Value);
        }

        Dictionary<string, string> variables = new Dictionary<string, string>()
        {
            { "TEST1", "TEST ${SUBST_VAR1}" },
            { "TEST2", "TEST ${SUBST_VAR2}" },
            { "TEST3", "TEST ${SUBST_VAR3}" },
            { "TEST4", "TEST VARIABLE"},
        };

        foreach (var variable in variables)
        {
            mModel.Variables.Add(variable.Key, variable.Value);
        }

        Dictionary<string, string> expected = new Dictionary<string, string>()
        {
            { "TEST1", "TEST SUBST_VAR1_VALUE" },
            { "TEST2", "TEST SUBST_VAR2_VALUE" },
            { "TEST3", "TEST SUBST_VAR3_VALUE" },
            { "TEST4", "TEST VARIABLE"},
        };

        Dictionary<string, string> actual = mModel.BuildVariables();

        foreach (var variable in expected)
        {
            Assert.That(actual[variable.Key], Is.EqualTo(variable.Value));
        }
    }

    private Dictionary<string, string> getVariableDictionary(EEnvironmentType testType)
    {
        return testType switch
        {
            EEnvironmentType.Normal => mModel.Variables,
            EEnvironmentType.Substitution => mModel.SubstVariables,
            _ => throw new NotImplementedException(),
        };
    }
}
