using Sernager.Core.Extensions;
using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Core.Tests.Fixtures;

namespace Sernager.Core.Tests.Units.Extensions;

public class CommandManagerExtensionSuccessTests : CommandManagerFixture
{
    [Theory]
    public void AddSubgroup_ShouldAdd((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        string groupName = "Test";
        string shortName = "Test";
        string description = "Test";

        manager.AddSubgroup(groupName, shortName, description);

        List<GroupItemModel> groupItems = manager.GetItems();

        foreach (GroupItemModel groupItem in groupItems)
        {
            if (groupItem.Item is GroupModel groupModel && groupModel.Name == groupName)
            {
                Assert.That(groupModel.Name, Is.EqualTo(groupName));
                Assert.That(groupModel.ShortName, Is.EqualTo(shortName));
                Assert.That(groupModel.Description, Is.EqualTo(description));
            }
        }
    }

    [Theory]
    public void AddCommand_ShouldAdd((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        string name = "Test";
        string shortName = "Test";
        string description = "Test";

        manager.AddCommand(new CommandModel
        {
            Name = name,
            ShortName = shortName,
            Description = description,
        });

        List<GroupItemModel> groupItems = manager.GetItems();

        foreach (GroupItemModel groupItem in groupItems)
        {
            if (groupItem.Item is CommandModel commandModel && commandModel.Name == name)
            {
                Assert.That(commandModel.Name, Is.EqualTo(name));
                Assert.That(commandModel.ShortName, Is.EqualTo(shortName));
                Assert.That(commandModel.Description, Is.EqualTo(description));
            }
        }
    }

    [Theory]
    public void TryChangeCurrentGroupName_ShouldChange((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        string name = "Test";

        Assert.That(manager.TryChangeCurrentGroupName(name), Is.True);
        Assert.That(manager.CurrentGroup.Name, Is.EqualTo(name));
    }

    [Theory]
    public void TryChangeCurrentGroupShortName_ShouldChange((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        string shortName = "Test";

        Assert.That(manager.TryChangeCurrentGroupShortName(shortName), Is.True);
        Assert.That(manager.CurrentGroup.ShortName, Is.EqualTo(shortName));
    }

    [Theory]
    public void ChangeCurrentGroupDescription_ShouldChange((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        string previousDescription = manager.CurrentGroup.Description;
        string description = $"{previousDescription}Test";

        manager.ChangeCurrentGroupDescription(description);

        Assert.That(manager.CurrentGroup.Description, Is.Not.EqualTo(previousDescription));
        Assert.That(manager.CurrentGroup.Description, Is.EqualTo(description));
    }

    [Theory]
    public void CanUseName_ShouldReturnTrue_WhenNameIsUnique((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        string name = "TestName";
        string subItemName = "TestSubItemName";

        Assert.That(manager.CanUseName(name, false), Is.True);
        Assert.That(manager.CanUseName(subItemName, true), Is.True);
    }

    [Theory]
    public void CanUseName_ShouldReturnFalse_WhenNameIsNotUnique((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        string name = "TestName";
        string subItemName = "TestSubItemName";

        manager.PrevGroup()
            .AddSubgroup(name, name, name)
            .UseItem(findSubgroupIdWithMostItems(manager.CurrentGroup.Items))
            .AddSubgroup(subItemName, subItemName, subItemName);

        Assert.That(manager.CanUseName(name, false), Is.False);
        Assert.That(manager.CanUseName(subItemName, true), Is.False);
    }

    [Theory]
    public void CanUseName_ShouldReturnTrue_WhenShortNameIsUnique((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        string shortName = "TST";
        string subItemShortName = "SUBTST";

        Assert.That(manager.CanUseName(shortName, false), Is.True);
        Assert.That(manager.CanUseName(subItemShortName, true), Is.True);
    }

    [Theory]
    public void CanUseName_ShouldReturnFalse_WhenShortNameIsNotUnique((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseName) = pair;
        ICommandManager manager = setUpCommandManager(PREFIX_ALIAS, level, caseName);

        string shortName = "TST";
        string subItemShortName = "SUBTST";

        manager.PrevGroup()
            .AddSubgroup(shortName, shortName, shortName)
            .UseItem(findSubgroupIdWithMostItems(manager.CurrentGroup.Items))
            .AddSubgroup(subItemShortName, subItemShortName, subItemShortName);

        Assert.That(manager.CanUseName(shortName, false), Is.False);
        Assert.That(manager.CanUseName(subItemShortName, true), Is.False);
    }
}
