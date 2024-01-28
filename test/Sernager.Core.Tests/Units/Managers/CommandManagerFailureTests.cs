using Sernager.Core.Managers;
using Sernager.Core.Options;
using Sernager.Core.Tests.Fixtures;

namespace Sernager.Core.Tests.Units.Managers;

public class CommandManagerFailureTests : CommandManagerFixture
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
    public void RemoveMainGroup_ShouldThrow_WhenMainGroupAlreadyRemoved(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        ICommandManager commandManager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        commandManager.RemoveMainGroup();

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(commandManager.RemoveMainGroup);
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(commandManager.RemoveMainGroup);
                break;
            default:
                throw new Exception($"Error level: {errorLevel} doesn't exist or isn't available to test.");
        }
    }

    [Theory]
    public void RemoveMainGroup_ShouldThrow_WhenItemIdIsNotGroupOrCommand(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        if (level == 1)
        {
            Assert.Pass();
            return;
        }

        ICommandManager commandManager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        Guid id = Guid.NewGuid();

        commandManager.CurrentGroup.Items.Add(id);

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(commandManager.RemoveMainGroup);
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(commandManager.RemoveMainGroup);
                break;
            default:
                throw new Exception($"Error level: {errorLevel} doesn't exist or isn't available to test.");
        }
    }

    [Theory]
    public void RemoveCurrentGroup_ShouldThrow_WhenMainGroupAlreadyRemoved(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;

        ICommandManager commandManager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        commandManager.RemoveMainGroup();

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(commandManager.RemoveCurrentGroup);
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(commandManager.RemoveCurrentGroup);
                break;
            default:
                throw new Exception($"Error level: {errorLevel} doesn't exist or isn't available to test.");
        }
    }

    [Theory]
    public void RemoveCurrentGroup_ShouldThrow_WhenItemIdIsNotGroupOrCommand(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;

        ICommandManager commandManager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        Guid id = Guid.NewGuid();

        commandManager.CurrentGroup.Items.Add(id);

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(commandManager.RemoveCurrentGroup);
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(commandManager.RemoveCurrentGroup);
                break;
            default:
                throw new Exception($"Error level: {errorLevel} doesn't exist or isn't available to test.");
        }
    }

    [Theory]
    public void RemoveItem_ShouldThrow_WhenMainGroupAlreadyRemoved(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;

        ICommandManager commandManager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        commandManager.RemoveMainGroup();

        Guid id = Guid.NewGuid();

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(() => commandManager.RemoveItem(id));
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => commandManager.RemoveItem(id));
                break;
            default:
                throw new Exception($"Error level: {errorLevel} doesn't exist or isn't available to test.");
        }
    }

    [Theory]
    public void RemoveItem_ShouldThrow_WhenItemIdDoesNotExistInCurrentGroup(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;

        ICommandManager commandManager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        Guid id = Guid.NewGuid();

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(() => commandManager.RemoveItem(id));
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => commandManager.RemoveItem(id));
                break;
            default:
                throw new Exception($"Error level: {errorLevel} doesn't exist or isn't available to test.");
        }
    }

    [Theory]
    public void RemoveItem_ShouldThrow_WhenItemIdExistsButNotGroupOrCommand(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;

        ICommandManager commandManager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        Guid id = Guid.NewGuid();

        commandManager.CurrentGroup.Items.Add(id);

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(() => commandManager.RemoveItem(id));
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => commandManager.RemoveItem(id));
                break;
            default:
                throw new Exception($"Error level: {errorLevel} doesn't exist or isn't available to test.");
        }
    }

    [Theory]
    public void UseItem_ShouldThrow_WhenMainGroupAlreadyRemoved(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;

        ICommandManager commandManager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        commandManager.RemoveMainGroup();

        Guid id = Guid.NewGuid();

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(() => commandManager.UseItem(id), Is.EqualTo(commandManager));
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => commandManager.UseItem(id));
                break;
            default:
                throw new Exception($"Error level: {errorLevel} doesn't exist or isn't available to test.");
        }
    }

    [Theory]
    public void UseItem_ShouldThrow_WhenItemIdDoesNotExistInCurrentGroup(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;

        ICommandManager commandManager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        Guid id = Guid.NewGuid();

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(() => commandManager.UseItem(id), Is.EqualTo(commandManager));
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => commandManager.UseItem(id));
                break;
            default:
                throw new Exception($"Error level: {errorLevel} doesn't exist or isn't available to test.");
        }
    }

    [Theory]
    public void UseItem_ShouldThrow_WhenGroupHasItsIdAsItemId(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        if (level == 1)
        {
            Assert.Pass();
            return;
        }

        ICommandManager commandManager = setUpCommandManager(PREFIX_ALIAS, level, caseName);
        Stack<Guid>? parents = PrivateUtil.GetFieldValue<Stack<Guid>>(commandManager, "mParents");
        if (parents == null)
        {
            throw new Exception("Parents is null.");
        }

        Guid subgroupId = parents.Peek();

        commandManager.CurrentGroup.Items.Add(subgroupId);

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(() => commandManager.UseItem(subgroupId), Is.EqualTo(commandManager));
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => commandManager.UseItem(subgroupId));
                break;
            default:
                throw new Exception($"Error level: {errorLevel} doesn't exist or isn't available to test.");
        }
    }

    [Theory]
    public void UseItem_ShouldThrow_WhenItemIdExistsButNotGroupOrCommand(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        if (level == 1)
        {
            Assert.Pass();
            return;
        }

        ICommandManager commandManager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        Guid id = Guid.NewGuid();

        commandManager.CurrentGroup.Items.Add(id);

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(() => commandManager.UseItem(id));
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => commandManager.UseItem(id));
                break;
            default:
                throw new Exception($"Error level: {errorLevel} doesn't exist or isn't available to test.");
        }
    }

    [Theory]
    public void GetBreadcrumb_ShouldThrow_WhenMainGroupAlreadyRemoved(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;

        ICommandManager commandManager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        commandManager.RemoveMainGroup();

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(commandManager.GetBreadcrumb, Is.Empty);
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => commandManager.GetBreadcrumb());
                break;
            default:
                throw new Exception($"Error level: {errorLevel} doesn't exist or isn't available to test.");
        }
    }

    [Theory]
    public void PrevGroup_ShouldThrow_WhenMainGroupAlreadyRemoved(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;

        ICommandManager commandManager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        commandManager.RemoveMainGroup();

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(commandManager.PrevGroup, Is.EqualTo(commandManager));
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => commandManager.PrevGroup());
                break;
            default:
                throw new Exception($"Error level: {errorLevel} doesn't exist or isn't available to test.");
        }
    }

    [Theory]
    public void GoMainGroup_ShouldThrow_WhenMainGroupAlreadyRemoved(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;

        ICommandManager commandManager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        commandManager.RemoveMainGroup();

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(commandManager.GoMainGroup, Is.EqualTo(commandManager));
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => commandManager.GoMainGroup());
                break;
            default:
                throw new Exception($"Error level: {errorLevel} doesn't exist or isn't available to test.");
        }
    }

    [Theory]
    public void GetPrevGroup_ShouldThrow_WhenMainGroupAlreadyRemoved(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;

        ICommandManager commandManager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        commandManager.RemoveMainGroup();

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(commandManager.GetPrevGroup, Is.Null);
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => commandManager.GetPrevGroup());
                break;
            default:
                throw new Exception($"Error level: {errorLevel} doesn't exist or isn't available to test.");
        }
    }

    [Theory]
    public void GetItems_ShouldThrow_WhenMainGroupAlreadyRemoved(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;

        ICommandManager commandManager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        commandManager.RemoveMainGroup();

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(commandManager.GetItems, Is.Empty);
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => commandManager.GetItems());
                break;
            default:
                throw new Exception($"Error level: {errorLevel} doesn't exist or isn't available to test.");
        }
    }

    [Theory]
    public void GetItems_ShouldThrow_WhenItemIdIsNotGroupOrCommand(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;

        ICommandManager commandManager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        Guid id = Guid.NewGuid();

        commandManager.CurrentGroup.Items.Add(id);

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(() => commandManager.GetItems().Count, Is.EqualTo(commandManager.CurrentGroup.Items.Count - 1));
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => commandManager.GetItems());
                break;
            default:
                throw new Exception($"Error level: {errorLevel} doesn't exist or isn't available to test.");
        }
    }
}
