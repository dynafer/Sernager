using Sernager.Core.Extensions;
using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Core.Options;
using Sernager.Core.Tests.Fixtures;

namespace Sernager.Core.Tests.Units.Extensions;

public class CommandManagerExtensionFailureTests : CommandManagerFixture
{
    private static readonly string PREFIX_ALIAS = "Configs.Defaults.Specifications.Commands";
    [DatapointSource]
    private static readonly (int, string)[] LEVEL_CASE_PAIRS =
    [
        (1, "OneLevel"),
        (2, "TwoLevels"),
        (3, "ThreeLevels")
    ];

    [TearDown]
    public void ResetConfigurator()
    {
        ResetUtil.ResetConfigurator();
    }

    [Theory]
    public void AddSubgroup_ShouldThrow_WhenMainGroupAlreadyRemoved(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        manager.RemoveMainGroup();

        TestNoneLevel(() => manager.AddSubgroup("Test", "Test", "Test"), Is.EqualTo(manager));
        TestExceptionLevel<SernagerException>(() => manager.AddSubgroup("Test", "Test", "Test"));
    }

    [Theory]
    public void AddCommand_ShouldThrow_WhenMainGroupAlreadyRemoved(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        manager.RemoveMainGroup();

        TestNoneLevel(() => manager.AddCommand(new CommandModel()), Is.EqualTo(manager));
        TestExceptionLevel<SernagerException>(() => manager.AddCommand(new CommandModel()));
    }

    [Theory]
    public void TryChangeCurrentGroupName_ShouldThrow_WhenMainGroupAlreadyRemoved(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        manager.RemoveMainGroup();

        TestNoneLevel(() => manager.TryChangeCurrentGroupName("Test"), Is.False);
        TestExceptionLevel<SernagerException>(() => manager.TryChangeCurrentGroupName("Test"));
    }

    [Theory]
    public void TryChangeCurrentGroupShortName_ShouldThrow_WhenMainGroupAlreadyRemoved(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        manager.RemoveMainGroup();

        TestNoneLevel(() => manager.TryChangeCurrentGroupShortName("Test"), Is.False);
        TestExceptionLevel<SernagerException>(() => manager.TryChangeCurrentGroupShortName("Test"));
    }

    [Theory]
    public void ChangeCurrentGroupDescription_ShouldThrow_WhenMainGroupAlreadyRemoved(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        manager.RemoveMainGroup();

        TestNoneLevel(() => manager.ChangeCurrentGroupDescription("Test"));
        TestExceptionLevel<SernagerException>(() => manager.ChangeCurrentGroupDescription("Test"));
    }

    [Theory]
    public void CanUseName_ShouldThrow_WhenMainGroupAlreadyRemoved(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        manager.RemoveMainGroup();

        TestNoneLevel(() => manager.CanUseName("Test", true), Is.False);
        TestExceptionLevel<SernagerException>(() => manager.CanUseName("Test", true));

        TestNoneLevel(() => manager.CanUseName("Test", false), Is.False);
        TestExceptionLevel<SernagerException>(() => manager.CanUseName("Test", false));
    }
}
