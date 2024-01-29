using Sernager.Core.Configs;
using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Core.Tests.Fixtures;
using System.Diagnostics;

namespace Sernager.Core.Tests.Units.Managers;

public class CommandManagerSuccessTests : CommandManagerFixture
{
    [Theory]
    public void Constructor_ShouldCreateMainGroup((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (_, string caseName) = pair;
        Configurator.Parse(CaseUtil.GetPath($"{PREFIX_ALIAS}.{caseName}", "json"));

        string name = "inexistentName";
        string shortName = "iet";
        string description = "This is a description.";

        Assert.That(Configurator.Config.CommandMainGroups.ContainsKey(name), Is.False);

        ICommandManager manager = new CommandManager(name, shortName, description);

        Assert.That(Configurator.Config.CommandMainGroups.ContainsKey(name), Is.True);

        Assert.That(manager.MainGroup.Name, Is.EqualTo(name));
        Assert.That(manager.MainGroup.ShortName, Is.EqualTo(shortName));
        Assert.That(manager.MainGroup.Description, Is.EqualTo(description));
    }

    [Theory]
    public void Constructor_ShouldUseExistingMainGroup((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (_, string caseName) = pair;
        Configurator.Parse(CaseUtil.GetPath($"{PREFIX_ALIAS}.{caseName}", "json"));

        GroupModel groupModel = findMainGroupWithMostItems();
        ICommandManager manager = new CommandManager(groupModel.Name, groupModel.ShortName, groupModel.Description);

        Assert.That(Configurator.Config.CommandMainGroups.ContainsKey(groupModel.Name), Is.True);

        Assert.That(manager.MainGroup, Is.EqualTo(groupModel));
        Assert.That(manager.MainGroup.Name, Is.EqualTo(groupModel.Name));
        Assert.That(manager.MainGroup.ShortName, Is.EqualTo(groupModel.ShortName));
        Assert.That(manager.MainGroup.Description, Is.EqualTo(groupModel.Description));
    }

    [Theory]
    public void RemoveMainGroup_ShouldRemoveMainGroup((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;

        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);
        string name = manager.MainGroup.Name;

        manager.RemoveMainGroup();

        Assert.That(Configurator.Config.CommandMainGroups.ContainsKey(name), Is.False);

        Assert.That(manager.MainGroup, Is.Null);
        Assert.That(manager.CurrentGroup, Is.Null);
    }

    [Theory]
    public void RemoveCurrentGroup_ShouldRemoveCurrentGroup((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;

        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);
        string name = manager.CurrentGroup.Name;
        GroupModel currentModel = manager.CurrentGroup;
        Guid subgroupId = level == 1
            ? Guid.Empty
            : getCurrentGroupId(manager);

        manager.RemoveCurrentGroup();

        if (level == 1)
        {
            Assert.That(Configurator.Config.CommandMainGroups.ContainsKey(name), Is.False);

            Assert.That(manager.MainGroup, Is.Null);
            Assert.That(manager.CurrentGroup, Is.Null);
        }
        else
        {
            Assert.That(Configurator.Config.CommandSubgroups.ContainsKey(subgroupId), Is.False);

            Assert.That(manager.GetPrevGroup(), Is.Not.EqualTo(currentModel));
        }
    }

    [Theory]
    public void RemoveItem_ShouldRemoveItem((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        if (level == 1)
        {
            Assert.Pass();
            return;
        }

        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        for (int i = 0; i < 2; ++i)
        {
            manager.PrevGroup();

            Guid subgroupId = findSubgroupIdWithMostItems(manager.CurrentGroup.Items);
            Guid commandId = findCommandIdWithMostItems(manager.CurrentGroup.Items);

            int countBefore = manager.CurrentGroup.Items.Count;

            manager.RemoveItem(subgroupId);
            manager.RemoveItem(commandId);

            Assert.That(manager.CurrentGroup.Items.Contains(subgroupId), Is.False);
            Assert.That(manager.CurrentGroup.Items.Contains(commandId), Is.False);
            Assert.That(manager.CurrentGroup.Items.Count, Is.EqualTo(countBefore - 2));
            Assert.That(Configurator.Config.CommandSubgroups.ContainsKey(subgroupId), Is.False);
            Assert.That(Configurator.Config.Commands.ContainsKey(commandId), Is.False);
        }
    }

    [Theory]
    public void UseItem_ShouldChangeCurrentGroup((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        Configurator.Parse(CaseUtil.GetPath($"{PREFIX_ALIAS}.{caseName}", "json"));

        GroupModel groupModel = findMainGroupWithMostItems();

        ICommandManager manager = new CommandManager(groupModel.Name, groupModel.ShortName, groupModel.Description);

        Assert.That(manager.CurrentGroup.Name, Is.EqualTo(groupModel.Name));
        Assert.That(manager.CurrentGroup.ShortName, Is.EqualTo(groupModel.ShortName));
        Assert.That(manager.CurrentGroup.Description, Is.EqualTo(groupModel.Description));

        for (int i = 1; i < level; ++i)
        {
            Guid subgroupId = findSubgroupIdWithMostItems(groupModel.Items);
            GroupModel subgroupModel = Configurator.Config.CommandSubgroups[subgroupId];

            manager.UseItem(subgroupId);

            Assert.That(manager.CurrentGroup.Name, Is.EqualTo(subgroupModel.Name));
            Assert.That(manager.CurrentGroup.ShortName, Is.EqualTo(subgroupModel.ShortName));
            Assert.That(manager.CurrentGroup.Description, Is.EqualTo(subgroupModel.Description));

            groupModel = subgroupModel;
        }
    }

    [Theory]
    public void UseItem_ShouldDoNothing_WhenItemIsCommand((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        if (level == 1)
        {
            Assert.Pass();
            return;
        }

        Configurator.Parse(CaseUtil.GetPath($"{PREFIX_ALIAS}.{caseName}", "json"));

        GroupModel groupModel = findMainGroupWithMostItems();

        ICommandManager manager = new CommandManager(groupModel.Name, groupModel.ShortName, groupModel.Description);

        for (int i = 1; i < level; ++i)
        {
            Guid commandId = findCommandIdWithMostItems(groupModel.Items);

            manager.UseItem(commandId);

            Assert.That(manager.CurrentGroup.Name, Is.EqualTo(groupModel.Name));
            Assert.That(manager.CurrentGroup.ShortName, Is.EqualTo(groupModel.ShortName));
            Assert.That(manager.CurrentGroup.Description, Is.EqualTo(groupModel.Description));

            Guid subgroupId = findSubgroupIdWithMostItems(groupModel.Items);
            GroupModel subgroupModel = Configurator.Config.CommandSubgroups[subgroupId];

            manager.UseItem(subgroupId);

            Assert.That(manager.CurrentGroup.Name, Is.EqualTo(subgroupModel.Name));
            Assert.That(manager.CurrentGroup.ShortName, Is.EqualTo(subgroupModel.ShortName));
            Assert.That(manager.CurrentGroup.Description, Is.EqualTo(subgroupModel.Description));

            groupModel = subgroupModel;
        }
    }

    [Theory]
    public void GetBreadcrumb_ShouldReturnBreadcrumb((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;

        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        string[] breadcrumb = manager.GetBreadcrumb();

        Assert.That(breadcrumb[0], Is.EqualTo(manager.MainGroup.Name));
        Assert.That(breadcrumb.Length, Is.EqualTo(level));
    }

    [Theory]
    public void PrevGroup_ShouldChangeCurrentGroup((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;

        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        for (int i = 0; i < level; ++i)
        {
            GroupModel prevGroup = manager.GetPrevGroup();

            manager.PrevGroup();

            Assert.That(prevGroup, Is.EqualTo(manager.CurrentGroup), $"i: {i}, name: {prevGroup.Name}, name: {manager.CurrentGroup.Name}");
            Assert.That(prevGroup.Name, Is.EqualTo(manager.CurrentGroup.Name));
            Assert.That(prevGroup.ShortName, Is.EqualTo(manager.CurrentGroup.ShortName));
            Assert.That(prevGroup.Description, Is.EqualTo(manager.CurrentGroup.Description));
        }
    }

    [Theory]
    public void GoMainGroup_ShouldChangeCurrentGroupToMainGroup((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;

        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);
        GroupModel currentGroup = manager.CurrentGroup;

        manager.GoMainGroup();

        Assert.That(manager.CurrentGroup, Is.EqualTo(manager.MainGroup));
        Assert.That(manager.CurrentGroup.Name, Is.EqualTo(manager.MainGroup.Name));
        Assert.That(manager.CurrentGroup.ShortName, Is.EqualTo(manager.MainGroup.ShortName));
        Assert.That(manager.CurrentGroup.Description, Is.EqualTo(manager.MainGroup.Description));

        if (level > 1)
        {
            Assert.That(manager.CurrentGroup, Is.Not.EqualTo(currentGroup));
            Assert.That(manager.CurrentGroup.Name, Is.Not.EqualTo(currentGroup.Name));
            Assert.That(manager.CurrentGroup.ShortName, Is.Not.EqualTo(currentGroup.ShortName));
        }
    }

    [Theory]
    public void GetPrevGroup_ShouldReturnPrevGroup((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        Configurator.Parse(CaseUtil.GetPath($"{PREFIX_ALIAS}.{caseName}", "json"));

        GroupModel groupModel = findMainGroupWithMostItems();

        ICommandManager manager = new CommandManager(groupModel.Name, groupModel.ShortName, groupModel.Description);
        GroupModel prevGroup = manager.GetPrevGroup();

        Assert.That(prevGroup.Name, Is.EqualTo(groupModel.Name));
        Assert.That(prevGroup.ShortName, Is.EqualTo(groupModel.ShortName));
        Assert.That(prevGroup.Description, Is.EqualTo(groupModel.Description));

        for (int i = 1; i < level; ++i)
        {
            Guid subgroupId = findSubgroupIdWithMostItems(groupModel.Items);
            GroupModel subgroupModel = Configurator.Config.CommandSubgroups[subgroupId];

            manager.UseItem(subgroupId);

            Assert.That(manager.CurrentGroup.Name, Is.EqualTo(subgroupModel.Name));
            Assert.That(manager.CurrentGroup.ShortName, Is.EqualTo(subgroupModel.ShortName));
            Assert.That(manager.CurrentGroup.Description, Is.EqualTo(subgroupModel.Description));

            prevGroup = manager.GetPrevGroup();

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

        ICommandManager manager = new CommandManager(groupModel.Name, groupModel.ShortName, groupModel.Description);
        List<GroupItemModel> items = manager.GetItems();

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

            manager.UseItem(subgroupId);

            Assert.That(manager.CurrentGroup.Name, Is.EqualTo(subgroupModel.Name));
            Assert.That(manager.CurrentGroup.ShortName, Is.EqualTo(subgroupModel.ShortName));
            Assert.That(manager.CurrentGroup.Description, Is.EqualTo(subgroupModel.Description));

            items = manager.GetItems();

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
