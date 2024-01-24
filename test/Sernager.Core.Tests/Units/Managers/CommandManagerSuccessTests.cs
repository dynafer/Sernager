using Sernager.Core.Configs;
using Sernager.Core.Managers;
using Sernager.Core.Models;
using System.Diagnostics;

namespace Sernager.Core.Tests.Units.Managers;

public class CommandManagerSuccessTests
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
    public void Constructor_ShouldCreateMainGroup((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (_, string caseName) = pair;
        Configurator.Parse(CaseUtil.GetPath($"{PREFIX_ALIAS}.{caseName}", "json"));

        string name = "inexistentName";
        string shortName = "iet";
        string description = "This is a description.";

        ICommandManager commandManager = new CommandManager(name, shortName, description);

        Assert.That(Configurator.Config.CommandMainGroups.ContainsKey(name), Is.True);

        Assert.That(commandManager.MainGroup.Name, Is.EqualTo(name));
        Assert.That(commandManager.MainGroup.ShortName, Is.EqualTo(shortName));
        Assert.That(commandManager.MainGroup.Description, Is.EqualTo(description));
    }

    [Theory]
    public void Constructor_ShouldUseExistingMainGroup((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (_, string caseName) = pair;
        Configurator.Parse(CaseUtil.GetPath($"{PREFIX_ALIAS}.{caseName}", "json"));

        GroupModel groupModel = findMainGroupWithMostItems();

        ICommandManager commandManager = new CommandManager(groupModel.Name, groupModel.ShortName, groupModel.Description);

        Assert.That(Configurator.Config.CommandMainGroups.ContainsKey(groupModel.Name), Is.True);

        Assert.That(commandManager.MainGroup.Name, Is.EqualTo(groupModel.Name));
        Assert.That(commandManager.MainGroup.ShortName, Is.EqualTo(groupModel.ShortName));
        Assert.That(commandManager.MainGroup.Description, Is.EqualTo(groupModel.Description));
    }

    [Theory]
    public void RemoveMainGroup_ShouldRemoveMainGroup((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (_, string caseName) = pair;
        Configurator.Parse(CaseUtil.GetPath($"{PREFIX_ALIAS}.{caseName}", "json"));

        GroupModel groupModel = findMainGroupWithMostItems();

        ICommandManager commandManager = new CommandManager(groupModel.Name, groupModel.ShortName, groupModel.Description);

        commandManager.RemoveMainGroup();

        Assert.That(Configurator.Config.CommandMainGroups.ContainsKey(groupModel.Name), Is.False);

        Assert.That(commandManager.MainGroup, Is.Null);
        Assert.That(commandManager.CurrentGroup, Is.Null);
    }

    [Theory]
    public void RemoveCurrentGroup_ShouldRemoveCurrentGroup((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        Configurator.Parse(CaseUtil.GetPath($"{PREFIX_ALIAS}.{caseName}", "json"));

        GroupModel groupModel = findMainGroupWithMostItems();

        ICommandManager commandManager = new CommandManager(groupModel.Name, groupModel.ShortName, groupModel.Description);

        Guid subgroupId = Guid.Empty;

        for (int i = 1; i < level; ++i)
        {
            subgroupId = findSubgroupIdWithMostItems(groupModel.Items);

            commandManager.UseItem(subgroupId);

            groupModel = commandManager.CurrentGroup;
        }

        GroupModel currentGroup = commandManager.CurrentGroup;

        commandManager.RemoveCurrentGroup();

        if (level == 0)
        {
            Assert.That(Configurator.Config.CommandMainGroups.ContainsKey(groupModel.Name), Is.False);

            Assert.That(commandManager.MainGroup, Is.Null);
            Assert.That(commandManager.CurrentGroup, Is.Null);
        }
        else
        {
            Assert.That(Configurator.Config.CommandSubgroups.ContainsKey(subgroupId), Is.False);

            Assert.That(commandManager.GetPrevGroup(), Is.Not.EqualTo(currentGroup));
        }
    }

    [Theory]
    public void RemoveItem_ShouldRemoveItem((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        Configurator.Parse(CaseUtil.GetPath($"{PREFIX_ALIAS}.{caseName}", "json"));

        if (level == 1)
        {
            Assert.Pass();
            return;
        }

        GroupModel groupModel = findMainGroupWithMostItems();

        ICommandManager commandManager = new CommandManager(groupModel.Name, groupModel.ShortName, groupModel.Description);

        Guid subgroupId;

        for (int i = 1; i < level; ++i)
        {
            subgroupId = findSubgroupIdWithMostItems(groupModel.Items);

            commandManager.UseItem(subgroupId);

            groupModel = commandManager.CurrentGroup;
        }

        for (int i = 0; i < 2; ++i)
        {
            commandManager.PrevGroup();
            groupModel = commandManager.CurrentGroup;

            subgroupId = findSubgroupIdWithMostItems(groupModel.Items);
            Guid commandId = findCommandIdWithMostItems(groupModel.Items);

            int countBefore = groupModel.Items.Count;

            commandManager.RemoveItem(subgroupId);
            commandManager.RemoveItem(commandId);

            Assert.That(commandManager.CurrentGroup.Items.Contains(subgroupId), Is.False);
            Assert.That(commandManager.CurrentGroup.Items.Contains(commandId), Is.False);
            Assert.That(commandManager.CurrentGroup.Items.Count, Is.EqualTo(countBefore - 2));
            Assert.That(Configurator.Config.CommandSubgroups.ContainsKey(subgroupId), Is.False);
            Assert.That(Configurator.Config.Commands.ContainsKey(commandId), Is.False);
        }
    }

    [Theory]
    public void GetCommand_ShouldReturnCommandModel((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        Configurator.Parse(CaseUtil.GetPath($"{PREFIX_ALIAS}.{caseName}", "json"));

        if (level == 1)
        {
            Assert.Pass();
            return;
        }

        GroupModel groupModel = findMainGroupWithMostItems();

        ICommandManager commandManager = new CommandManager(groupModel.Name, groupModel.ShortName, groupModel.Description);

        for (int i = 1; i < level; ++i)
        {
            Guid commandId = findCommandIdWithMostItems(groupModel.Items);

            Assert.That(commandManager.GetCommand(commandId), Is.Not.Null);
            Assert.That(commandManager.GetCommand(commandId), Is.InstanceOf<CommandModel>());

            Guid subgroupId = findSubgroupIdWithMostItems(groupModel.Items);

            commandManager.UseItem(subgroupId);

            groupModel = commandManager.CurrentGroup;
        }
    }

    [Theory]
    public void UseItem_ShouldChangeCurrentGroup((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        Configurator.Parse(CaseUtil.GetPath($"{PREFIX_ALIAS}.{caseName}", "json"));

        GroupModel groupModel = findMainGroupWithMostItems();

        ICommandManager commandManager = new CommandManager(groupModel.Name, groupModel.ShortName, groupModel.Description);

        Assert.That(commandManager.CurrentGroup.Name, Is.EqualTo(groupModel.Name));
        Assert.That(commandManager.CurrentGroup.ShortName, Is.EqualTo(groupModel.ShortName));
        Assert.That(commandManager.CurrentGroup.Description, Is.EqualTo(groupModel.Description));

        for (int i = 1; i < level; ++i)
        {
            Guid subgroupId = findSubgroupIdWithMostItems(groupModel.Items);
            GroupModel subgroupModel = Configurator.Config.CommandSubgroups[subgroupId];

            commandManager.UseItem(subgroupId);

            Assert.That(commandManager.CurrentGroup.Name, Is.EqualTo(subgroupModel.Name));
            Assert.That(commandManager.CurrentGroup.ShortName, Is.EqualTo(subgroupModel.ShortName));
            Assert.That(commandManager.CurrentGroup.Description, Is.EqualTo(subgroupModel.Description));

            groupModel = subgroupModel;
        }
    }

    [Theory]
    public void UseItem_ShouldDoNothing_WhenItemIsCommand((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        Configurator.Parse(CaseUtil.GetPath($"{PREFIX_ALIAS}.{caseName}", "json"));

        if (level == 1)
        {
            Assert.Pass();
            return;
        }

        GroupModel groupModel = findMainGroupWithMostItems();

        ICommandManager commandManager = new CommandManager(groupModel.Name, groupModel.ShortName, groupModel.Description);

        for (int i = 1; i < level; ++i)
        {
            Guid commandId = findCommandIdWithMostItems(groupModel.Items);

            commandManager.UseItem(commandId);

            Assert.That(commandManager.CurrentGroup.Name, Is.EqualTo(groupModel.Name));
            Assert.That(commandManager.CurrentGroup.ShortName, Is.EqualTo(groupModel.ShortName));
            Assert.That(commandManager.CurrentGroup.Description, Is.EqualTo(groupModel.Description));

            Guid subgroupId = findSubgroupIdWithMostItems(groupModel.Items);
            GroupModel subgroupModel = Configurator.Config.CommandSubgroups[subgroupId];

            commandManager.UseItem(subgroupId);

            Assert.That(commandManager.CurrentGroup.Name, Is.EqualTo(subgroupModel.Name));
            Assert.That(commandManager.CurrentGroup.ShortName, Is.EqualTo(subgroupModel.ShortName));
            Assert.That(commandManager.CurrentGroup.Description, Is.EqualTo(subgroupModel.Description));

            groupModel = subgroupModel;
        }
    }

    [Theory]
    public void GetBreadcrumb_ShouldReturnBreadcrumb((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        Configurator.Parse(CaseUtil.GetPath($"{PREFIX_ALIAS}.{caseName}", "json"));

        GroupModel groupModel = findMainGroupWithMostItems();

        ICommandManager commandManager = new CommandManager(groupModel.Name, groupModel.ShortName, groupModel.Description);
        string[] breadcrumb = commandManager.GetBreadcrumb();

        Assert.That(breadcrumb.Length, Is.EqualTo(1));
        Assert.That(breadcrumb[0], Is.EqualTo(groupModel.Name));

        for (int i = 1; i < level; ++i)
        {
            Guid subgroupId = findSubgroupIdWithMostItems(groupModel.Items);
            GroupModel subgroupModel = Configurator.Config.CommandSubgroups[subgroupId];

            commandManager.UseItem(subgroupId);
            breadcrumb = commandManager.GetBreadcrumb();

            Assert.That(breadcrumb.Length, Is.EqualTo(i + 1));
            Assert.That(breadcrumb[i], Is.EqualTo(subgroupModel.Name));

            groupModel = subgroupModel;
        }
    }

    [Theory]
    public void PrevGroup_ShouldChangeCurrentGroup((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        Configurator.Parse(CaseUtil.GetPath($"{PREFIX_ALIAS}.{caseName}", "json"));

        GroupModel groupModel = findMainGroupWithMostItems();

        ICommandManager commandManager = new CommandManager(groupModel.Name, groupModel.ShortName, groupModel.Description);

        commandManager.PrevGroup();

        Assert.That(commandManager.CurrentGroup.Name, Is.EqualTo(groupModel.Name));
        Assert.That(commandManager.CurrentGroup.ShortName, Is.EqualTo(groupModel.ShortName));
        Assert.That(commandManager.CurrentGroup.Description, Is.EqualTo(groupModel.Description));

        for (int i = 1; i < level; ++i)
        {
            Guid subgroupId = findSubgroupIdWithMostItems(groupModel.Items);
            GroupModel subgroupModel = Configurator.Config.CommandSubgroups[subgroupId];

            commandManager.UseItem(subgroupId);

            Assert.That(commandManager.CurrentGroup.Name, Is.EqualTo(subgroupModel.Name));
            Assert.That(commandManager.CurrentGroup.ShortName, Is.EqualTo(subgroupModel.ShortName));
            Assert.That(commandManager.CurrentGroup.Description, Is.EqualTo(subgroupModel.Description));

            if (i != level - 1)
            {
                groupModel = subgroupModel;
            }
        }

        commandManager.PrevGroup();

        Assert.That(commandManager.CurrentGroup.Name, Is.EqualTo(groupModel.Name));
        Assert.That(commandManager.CurrentGroup.ShortName, Is.EqualTo(groupModel.ShortName));
        Assert.That(commandManager.CurrentGroup.Description, Is.EqualTo(groupModel.Description));
    }

    [Theory]
    public void GoMainGroup_ShouldChangeCurrentGroupToMainGroup((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        Configurator.Parse(CaseUtil.GetPath($"{PREFIX_ALIAS}.{caseName}", "json"));

        GroupModel groupModel = findMainGroupWithMostItems();

        ICommandManager commandManager = new CommandManager(groupModel.Name, groupModel.ShortName, groupModel.Description);

        for (int i = 1; i < level; ++i)
        {
            Guid subgroupId = findSubgroupIdWithMostItems(groupModel.Items);
            GroupModel subgroupModel = Configurator.Config.CommandSubgroups[subgroupId];

            commandManager.UseItem(subgroupId);

            Assert.That(commandManager.CurrentGroup.Name, Is.EqualTo(subgroupModel.Name));
            Assert.That(commandManager.CurrentGroup.ShortName, Is.EqualTo(subgroupModel.ShortName));
            Assert.That(commandManager.CurrentGroup.Description, Is.EqualTo(subgroupModel.Description));

            groupModel = subgroupModel;
        }

        commandManager.GoMainGroup();

        Assert.That(commandManager.CurrentGroup.Name, Is.EqualTo(commandManager.MainGroup.Name));
        Assert.That(commandManager.CurrentGroup.ShortName, Is.EqualTo(commandManager.MainGroup.ShortName));
        Assert.That(commandManager.CurrentGroup.Description, Is.EqualTo(commandManager.MainGroup.Description));
    }

    [Theory]
    public void GetPrevGroup_ShouldReturnPrevGroup((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        Configurator.Parse(CaseUtil.GetPath($"{PREFIX_ALIAS}.{caseName}", "json"));

        GroupModel groupModel = findMainGroupWithMostItems();

        ICommandManager commandManager = new CommandManager(groupModel.Name, groupModel.ShortName, groupModel.Description);
        GroupModel prevGroup = commandManager.GetPrevGroup();

        Assert.That(prevGroup.Name, Is.EqualTo(groupModel.Name));
        Assert.That(prevGroup.ShortName, Is.EqualTo(groupModel.ShortName));
        Assert.That(prevGroup.Description, Is.EqualTo(groupModel.Description));

        for (int i = 1; i < level; ++i)
        {
            Guid subgroupId = findSubgroupIdWithMostItems(groupModel.Items);
            GroupModel subgroupModel = Configurator.Config.CommandSubgroups[subgroupId];

            commandManager.UseItem(subgroupId);

            Assert.That(commandManager.CurrentGroup.Name, Is.EqualTo(subgroupModel.Name));
            Assert.That(commandManager.CurrentGroup.ShortName, Is.EqualTo(subgroupModel.ShortName));
            Assert.That(commandManager.CurrentGroup.Description, Is.EqualTo(subgroupModel.Description));

            prevGroup = commandManager.GetPrevGroup();

            Assert.That(prevGroup.Name, Is.EqualTo(groupModel.Name));
            Assert.That(prevGroup.ShortName, Is.EqualTo(groupModel.ShortName));
            Assert.That(prevGroup.Description, Is.EqualTo(groupModel.Description));

            groupModel = subgroupModel;
        }
    }

    [Theory]
    public void GetItems_ShouldReturnItems((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        Configurator.Parse(CaseUtil.GetPath($"{PREFIX_ALIAS}.{caseName}", "json"));

        GroupModel groupModel = findMainGroupWithMostItems();

        ICommandManager commandManager = new CommandManager(groupModel.Name, groupModel.ShortName, groupModel.Description);
        List<GroupItemModel> items = commandManager.GetItems();

        Assert.That(items.Count, Is.EqualTo(groupModel.Items.Count));

        foreach (GroupItemModel item in items)
        {
            Assert.That(groupModel.Items.Contains(item.Id), Is.True);
            Assert.That(item.Item, Is.InstanceOf<GroupModel>().Or.InstanceOf<CommandModel>());
        }

        for (int i = 1; i < level; ++i)
        {
            Guid subgroupId = findSubgroupIdWithMostItems(groupModel.Items);
            GroupModel subgroupModel = Configurator.Config.CommandSubgroups[subgroupId];

            commandManager.UseItem(subgroupId);

            Assert.That(commandManager.CurrentGroup.Name, Is.EqualTo(subgroupModel.Name));
            Assert.That(commandManager.CurrentGroup.ShortName, Is.EqualTo(subgroupModel.ShortName));
            Assert.That(commandManager.CurrentGroup.Description, Is.EqualTo(subgroupModel.Description));

            items = commandManager.GetItems();

            Assert.That(items.Count, Is.EqualTo(subgroupModel.Items.Count));

            foreach (GroupItemModel item in items)
            {
                Assert.That(subgroupModel.Items.Contains(item.Id), Is.True);
                Assert.That(item.Item, Is.InstanceOf<GroupModel>().Or.InstanceOf<CommandModel>());
            }

            groupModel = subgroupModel;
        }
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

    [StackTraceHidden]
    private Guid findCommandIdWithMostItems(List<Guid> itemIds)
    {
        foreach (Guid itemId in itemIds)
        {
            if (!Configurator.Config.Commands.ContainsKey(itemId))
            {
                continue;
            }

            return itemId;
        }

        throw new Exception("Command not found.");
    }
}
