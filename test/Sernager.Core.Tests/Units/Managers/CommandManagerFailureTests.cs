using Sernager.Core.Configs;
using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Core.Options;
using System.Diagnostics;

namespace Sernager.Core.Tests.Units.Managers;

public class CommandManagerFailureTests : FailureFixture
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
        ICommandManager commandManager = setUpCommandManager(level, caseName);

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

        ICommandManager commandManager = setUpCommandManager(level, caseName);

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

        ICommandManager commandManager = setUpCommandManager(level, caseName);

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

        ICommandManager commandManager = setUpCommandManager(level, caseName);

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

        ICommandManager commandManager = setUpCommandManager(level, caseName);

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

        ICommandManager commandManager = setUpCommandManager(level, caseName);

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

        ICommandManager commandManager = setUpCommandManager(level, caseName);

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

        ICommandManager commandManager = setUpCommandManager(level, caseName);

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

        ICommandManager commandManager = setUpCommandManager(level, caseName);

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

        ICommandManager commandManager = setUpCommandManager(level, caseName);
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

        ICommandManager commandManager = setUpCommandManager(level, caseName);

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

        ICommandManager commandManager = setUpCommandManager(level, caseName);

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

        ICommandManager commandManager = setUpCommandManager(level, caseName);

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

        ICommandManager commandManager = setUpCommandManager(level, caseName);

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

        ICommandManager commandManager = setUpCommandManager(level, caseName);

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

        ICommandManager commandManager = setUpCommandManager(level, caseName);

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

        ICommandManager commandManager = setUpCommandManager(level, caseName);

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

    private ICommandManager setUpCommandManager(int level, string caseName)
    {
        Configurator.Parse(CaseUtil.GetPath($"{PREFIX_ALIAS}.{caseName}", "json"));

        GroupModel mainGroup = findMainGroupWithMostItems();
        ICommandManager commandManager = new CommandManager(mainGroup.Name, mainGroup.ShortName, mainGroup.Description);

        for (int i = 1; i < level; ++i)
        {
            Guid subgroupId = findSubgroupIdWithMostItems(mainGroup.Items);
            commandManager.UseItem(subgroupId);
            mainGroup = commandManager.CurrentGroup;
        }

        return commandManager;
    }

    [StackTraceHidden]
    private GroupModel findMainGroupWithMostItems()
    {
        GroupModel? groupModel = null;

        foreach (GroupModel group in Configurator.Config.CommandMainGroups.Values)
        {
            if (groupModel == null || group.Items.Count > groupModel.Items.Count)
            {
                groupModel = group;
            }
        }

        if (groupModel == null || !Configurator.Config.CommandMainGroups.Values.Contains(groupModel))
        {
            throw new Exception("Main group not found.");
        }

        return groupModel;
    }

    [StackTraceHidden]
    private Guid findSubgroupIdWithMostItems(List<Guid> itemIds)
    {
        Guid? subgroupId = null;
        GroupModel? groupModel = null;

        foreach (Guid itemId in itemIds)
        {
            if (!Configurator.Config.CommandSubgroups.ContainsKey(itemId))
            {
                continue;
            }

            GroupModel group = Configurator.Config.CommandSubgroups[itemId];

            if (subgroupId == null || groupModel == null || group.Items.Count > groupModel.Items.Count)
            {
                subgroupId = itemId;
                groupModel = group;
            }
        }

        if (subgroupId == null || groupModel == null || !Configurator.Config.CommandSubgroups.ContainsKey(subgroupId.Value))
        {
            throw new Exception("Subgroup not found.");
        }

        return subgroupId.Value;
    }
}
