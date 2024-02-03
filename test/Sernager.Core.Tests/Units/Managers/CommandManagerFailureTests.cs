using Sernager.Core.Managers;
using Sernager.Core.Options;
using Sernager.Core.Tests.Fixtures;

namespace Sernager.Core.Tests.Units.Managers;

internal sealed class CommandManagerFailureTests : CommandManagerFixture
{
    [Theory]
    public void RemoveMainGroup_ShouldThrow_WhenMainGroupAlreadyRemoved(EErrorLevel errorLevel, (int, string) pair)
    {
        Assume.That(errorLevel, Is.AnyOf(TESTABLE_LEVELS));
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        manager.RemoveMainGroup();

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(manager.RemoveMainGroup);
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(manager.RemoveMainGroup);
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

        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        Guid id = Guid.NewGuid();

        manager.CurrentGroup.Items.Add(id);

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(manager.RemoveMainGroup);
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(manager.RemoveMainGroup);
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

        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        manager.RemoveMainGroup();

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(manager.RemoveCurrentGroup);
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(manager.RemoveCurrentGroup);
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

        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        Guid id = Guid.NewGuid();

        manager.CurrentGroup.Items.Add(id);

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(manager.RemoveCurrentGroup);
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(manager.RemoveCurrentGroup);
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

        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        manager.RemoveMainGroup();

        Guid id = Guid.NewGuid();

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(() => manager.RemoveItem(id));
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => manager.RemoveItem(id));
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

        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        Guid id = Guid.NewGuid();

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(() => manager.RemoveItem(id));
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => manager.RemoveItem(id));
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

        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        Guid id = Guid.NewGuid();

        manager.CurrentGroup.Items.Add(id);

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(() => manager.RemoveItem(id));
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => manager.RemoveItem(id));
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

        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        manager.RemoveMainGroup();

        Guid id = Guid.NewGuid();

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(() => manager.UseItem(id), Is.EqualTo(manager));
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => manager.UseItem(id));
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

        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        Guid id = Guid.NewGuid();

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(() => manager.UseItem(id), Is.EqualTo(manager));
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => manager.UseItem(id));
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

        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);
        Stack<Guid>? parents = PrivateUtil.GetFieldValue<Stack<Guid>>(manager, "mParents");
        if (parents == null)
        {
            throw new Exception("Parents is null.");
        }

        Guid subgroupId = parents.Peek();

        manager.CurrentGroup.Items.Add(subgroupId);

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(() => manager.UseItem(subgroupId), Is.EqualTo(manager));
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => manager.UseItem(subgroupId));
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

        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        Guid id = Guid.NewGuid();

        manager.CurrentGroup.Items.Add(id);

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(() => manager.UseItem(id));
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => manager.UseItem(id));
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

        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        manager.RemoveMainGroup();

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(manager.GetBreadcrumb, Is.Empty);
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => manager.GetBreadcrumb());
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

        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        manager.RemoveMainGroup();

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(manager.PrevGroup, Is.EqualTo(manager));
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => manager.PrevGroup());
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

        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        manager.RemoveMainGroup();

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(manager.GoMainGroup, Is.EqualTo(manager));
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => manager.GoMainGroup());
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

        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        manager.RemoveMainGroup();

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(manager.GetPrevGroup, Is.Null);
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => manager.GetPrevGroup());
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

        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        manager.RemoveMainGroup();

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(manager.GetItems, Is.Empty);
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => manager.GetItems());
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

        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        Guid id = Guid.NewGuid();

        manager.CurrentGroup.Items.Add(id);

        switch (errorLevel)
        {
            case EErrorLevel.None:
                TestNoneLevel(() => manager.GetItems().Count, Is.EqualTo(manager.CurrentGroup.Items.Count - 1));
                break;
            case EErrorLevel.Exception:
                TestExceptionLevel<SernagerException>(() => manager.GetItems());
                break;
            default:
                throw new Exception($"Error level: {errorLevel} doesn't exist or isn't available to test.");
        }
    }
}
