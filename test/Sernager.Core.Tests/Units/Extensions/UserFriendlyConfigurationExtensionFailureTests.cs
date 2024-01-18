using Sernager.Core.Configs;
using Sernager.Core.Extensions;
using Sernager.Core.Models;
using Sernager.Core.Utils;
using System.Diagnostics;
using System.Text.Json;

namespace Sernager.Core.Tests.Units.Extensions;

public class UserFriendlyConfigurationExtensionFailureTests : FailureFixture
{
    private readonly struct FoundItem<TItem>
    {
        public TItem Item { get; init; }
        public int Index { get; init; }

        public FoundItem(TItem item, int index)
        {
            Item = item;
            Index = index;
        }
    }

    [DatapointSource]
    private static readonly (int, string)[] LEVEL_CASE_PAIRS =
    [
        (1, "OneLevel"),
        (2, "TwoLevels"),
        (3, "ThreeLevels")
    ];

    [Test]
    public void ToConfiguration_ShouldThrow_WhenEnvironmentGroupNameIsEmpty()
    {
        UserFriendlyConfiguration ufConfig = CaseUtil.ReadJson<UserFriendlyConfiguration>("Extensions.UserFriendlyConfigurations.Environment");
        EnvironmentModel environment = findItems<EnvironmentModel>(ufConfig.Environments.Values.Cast<object>().ToList(), 1)[0].Item;
        environment.Name = string.Empty;

        TestNoneLevel(() => ufConfig.ToConfiguration());
        TestExceptionLevel<SernagerException>(() => ufConfig.ToConfiguration());
    }

    [Test]
    public void ToConfiguration_ShouldThrow_WhenEnvironmentGroupNameIsNotUnique()
    {
        UserFriendlyConfiguration ufConfig = CaseUtil.ReadJson<UserFriendlyConfiguration>("Extensions.UserFriendlyConfigurations.Environment");
        FoundItem<EnvironmentModel>[] environments = findItems<EnvironmentModel>(ufConfig.Environments.Values.Cast<object>().ToList(), 2);
        environments[0].Item.Name = "Environment1";
        environments[1].Item.Name = "Environment1";

        TestNoneLevel(ufConfig.ToConfiguration, Is.TypeOf<Configuration>());
        TestExceptionLevel<SernagerException>(() => ufConfig.ToConfiguration());
    }

    [Theory]
    public void ToConfiguration_ShouldThrow_WhenCommandGroupNameIsEmpty((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseLevelName) = pair;

        UserFriendlyConfiguration ufConfig = CaseUtil.ReadJson<UserFriendlyConfiguration>($"Extensions.UserFriendlyConfigurations.Commands.{caseLevelName}");
        UserFriendlyGroupModel mainGroup = findItems(ufConfig.Commands.Values.Cast<object>().ToList(), 1)[0].Item;
        string mainGroupName = mainGroup.Name;
        mainGroup.Name = string.Empty;

        TestNoneLevel(ufConfig.ToConfiguration, Is.TypeOf<Configuration>());
        TestExceptionLevel<SernagerException>(() => ufConfig.ToConfiguration());

        if (level == 1)
        {
            return;
        }

        mainGroup.Name = mainGroupName;

        UserFriendlyGroupModel groupModel = mainGroup;
        for (int i = 1; i < level; ++i)
        {
            FoundItem<UserFriendlyGroupModel> subgroup = findItems(groupModel.Items, 1)[0];
            groupModel.Items[subgroup.Index] = subgroup.Item;
            groupModel = subgroup.Item;

            string subGroupName = groupModel.Name;
            groupModel.Name = string.Empty;

            TestNoneLevel(ufConfig.ToConfiguration, Is.TypeOf<Configuration>());
            TestExceptionLevel<SernagerException>(() => ufConfig.ToConfiguration());

            groupModel.Name = subGroupName;
        }
    }

    [Theory]
    public void ToConfiguration_ShouldThrow_WhenCommandGroupNameIsSameAsShortName((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseLevelName) = pair;

        UserFriendlyConfiguration ufConfig = CaseUtil.ReadJson<UserFriendlyConfiguration>($"Extensions.UserFriendlyConfigurations.Commands.{caseLevelName}");
        UserFriendlyGroupModel mainGroupModel = findItems(ufConfig.Commands.Values.Cast<object>().ToList(), 1)[0].Item;
        string mainGroupShortName = mainGroupModel.ShortName;
        mainGroupModel.ShortName = mainGroupModel.Name;

        TestNoneLevel(ufConfig.ToConfiguration, Is.TypeOf<Configuration>());
        TestExceptionLevel<SernagerException>(() => ufConfig.ToConfiguration());

        if (level == 1)
        {
            return;
        }

        mainGroupModel.ShortName = mainGroupShortName;

        UserFriendlyGroupModel groupModel = mainGroupModel;
        for (int i = 1; i < level; ++i)
        {
            FoundItem<UserFriendlyGroupModel> subgroup = findItems(groupModel.Items, 1)[0];
            groupModel.Items[subgroup.Index] = subgroup.Item;
            groupModel = subgroup.Item;

            string subGroupShortName = groupModel.ShortName;
            groupModel.ShortName = groupModel.Name;

            TestNoneLevel(ufConfig.ToConfiguration, Is.TypeOf<Configuration>());
            TestExceptionLevel<SernagerException>(() => ufConfig.ToConfiguration());

            groupModel.ShortName = subGroupShortName;
        }
    }

    [Theory]
    public void ToConfiguration_ShouldThrow_WhenCommandGroupNameIsNotUnique((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseLevelName) = pair;

        UserFriendlyConfiguration ufConfig = CaseUtil.ReadJson<UserFriendlyConfiguration>($"Extensions.UserFriendlyConfigurations.Commands.{caseLevelName}");
        FoundItem<UserFriendlyGroupModel>[] mainGroupModels = findItems(ufConfig.Commands.Values.Cast<object>().ToList(), 2);
        string mainGroupName1 = mainGroupModels[0].Item.Name;
        string mainGroupName2 = mainGroupModels[1].Item.Name;
        mainGroupModels[0].Item.Name = "CommandMainGroup1";
        mainGroupModels[1].Item.Name = "CommandMainGroup1";

        TestNoneLevel(ufConfig.ToConfiguration, Is.TypeOf<Configuration>());
        TestExceptionLevel<SernagerException>(() => ufConfig.ToConfiguration());

        if (level == 1)
        {
            return;
        }

        mainGroupModels[0].Item.Name = mainGroupName1;
        mainGroupModels[1].Item.Name = mainGroupName2;

        UserFriendlyGroupModel groupModel = getTestableGroupModel(mainGroupModels[0].Item, mainGroupModels[1].Item);

        for (int i = 1; i < level; ++i)
        {
            FoundItem<UserFriendlyGroupModel>[] subGroupModels = findItems(groupModel.Items, 2);
            groupModel.Items[subGroupModels[0].Index] = subGroupModels[0].Item;
            groupModel.Items[subGroupModels[1].Index] = subGroupModels[1].Item;

            string subGroupName1 = subGroupModels[0].Item.Name;
            string subGroupName2 = subGroupModels[1].Item.Name;
            subGroupModels[0].Item.Name = "CommandSubGroup1";
            subGroupModels[1].Item.Name = "CommandSubGroup1";

            TestNoneLevel(ufConfig.ToConfiguration, Is.TypeOf<Configuration>());
            TestExceptionLevel<SernagerException>(() => ufConfig.ToConfiguration());

            subGroupModels[0].Item.Name = subGroupName1;
            subGroupModels[1].Item.Name = subGroupName2;

            if (level > 2 && i < level - 1)
            {
                groupModel = getTestableGroupModel(subGroupModels[0].Item, subGroupModels[1].Item);
            }
        }
    }

    [Theory]
    public void ToConfiguration_ShouldThrow_WhenCommandGroupShortNameIsNotUnique((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseLevelName) = pair;

        UserFriendlyConfiguration ufConfig = CaseUtil.ReadJson<UserFriendlyConfiguration>($"Extensions.UserFriendlyConfigurations.Commands.{caseLevelName}");
        FoundItem<UserFriendlyGroupModel>[] mainGroupModels = findItems(ufConfig.Commands.Values.Cast<object>().ToList(), 2);
        string mainGroupShortName1 = mainGroupModels[0].Item.ShortName;
        string mainGroupShortName2 = mainGroupModels[1].Item.ShortName;
        mainGroupModels[0].Item.ShortName = "CommandMainGroup1";
        mainGroupModels[1].Item.ShortName = "CommandMainGroup1";

        TestNoneLevel(ufConfig.ToConfiguration, Is.TypeOf<Configuration>());
        TestExceptionLevel<SernagerException>(() => ufConfig.ToConfiguration());

        if (level == 1)
        {
            return;
        }

        mainGroupModels[0].Item.ShortName = mainGroupShortName1;
        mainGroupModels[1].Item.ShortName = mainGroupShortName2;

        UserFriendlyGroupModel groupModel = getTestableGroupModel(mainGroupModels[0].Item, mainGroupModels[1].Item);

        for (int i = 1; i < level; ++i)
        {
            FoundItem<UserFriendlyGroupModel>[] subGroupModels = findItems(groupModel.Items, 2);
            groupModel.Items[subGroupModels[0].Index] = subGroupModels[0].Item;
            groupModel.Items[subGroupModels[1].Index] = subGroupModels[1].Item;

            string subGroupShortName1 = subGroupModels[0].Item.ShortName;
            string subGroupShortName2 = subGroupModels[1].Item.ShortName;
            subGroupModels[0].Item.ShortName = "CommandSubGroup1";
            subGroupModels[1].Item.ShortName = "CommandSubGroup1";

            TestNoneLevel(ufConfig.ToConfiguration, Is.TypeOf<Configuration>());
            TestExceptionLevel<SernagerException>(() => ufConfig.ToConfiguration());

            subGroupModels[0].Item.ShortName = subGroupShortName1;
            subGroupModels[1].Item.ShortName = subGroupShortName2;

            if (level > 2 && i < level - 1)
            {
                groupModel = getTestableGroupModel(subGroupModels[0].Item, subGroupModels[1].Item);
            }
        }
    }

    [Theory]
    public void ToConfiguration_ShouldThrow_WhenCommandNameIsEmpty((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseLevelName) = pair;

        if (level == 1)
        {
            return;
        }

        UserFriendlyConfiguration ufConfig = CaseUtil.ReadJson<UserFriendlyConfiguration>($"Extensions.UserFriendlyConfigurations.Commands.{caseLevelName}");

        UserFriendlyGroupModel groupModel = findItems(ufConfig.Commands.Values.Cast<object>().ToList(), 1)[0].Item;
        for (int i = 1; i < level; ++i)
        {
            FoundItem<CommandModel> command = findItems<CommandModel>(groupModel.Items, 1)[0];
            groupModel.Items[command.Index] = command.Item;

            string commandName = command.Item.Name;
            command.Item.Name = string.Empty;

            TestNoneLevel(ufConfig.ToConfiguration, Is.TypeOf<Configuration>());
            TestExceptionLevel<SernagerException>(() => ufConfig.ToConfiguration());

            command.Item.Name = commandName;

            FoundItem<UserFriendlyGroupModel> subgroup = findItems(groupModel.Items, 1)[0];
            groupModel.Items[subgroup.Index] = subgroup.Item;
            groupModel = subgroup.Item;
        }
    }

    [Theory]
    public void ToConfiguration_ShouldThrow_WhenCommandNameIsSameAsShortName((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseLevelName) = pair;

        if (level == 1)
        {
            return;
        }

        UserFriendlyConfiguration ufConfig = CaseUtil.ReadJson<UserFriendlyConfiguration>($"Extensions.UserFriendlyConfigurations.Commands.{caseLevelName}");

        UserFriendlyGroupModel groupModel = findItems(ufConfig.Commands.Values.Cast<object>().ToList(), 1)[0].Item;
        for (int i = 1; i < level; ++i)
        {
            FoundItem<CommandModel> command = findItems<CommandModel>(groupModel.Items, 1)[0];
            groupModel.Items[command.Index] = command.Item;

            string commandShortName = command.Item.ShortName;
            command.Item.ShortName = command.Item.Name;

            TestNoneLevel(ufConfig.ToConfiguration, Is.TypeOf<Configuration>());
            TestExceptionLevel<SernagerException>(() => ufConfig.ToConfiguration());

            command.Item.ShortName = commandShortName;

            FoundItem<UserFriendlyGroupModel> subgroup = findItems(groupModel.Items, 1)[0];
            groupModel.Items[subgroup.Index] = subgroup.Item;
            groupModel = subgroup.Item;
        }
    }

    [Theory]
    public void ToConfiguration_ShouldThrow_WhenCommandNameIsNotUnique((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseLevelName) = pair;

        if (level == 1)
        {
            return;
        }

        UserFriendlyConfiguration ufConfig = CaseUtil.ReadJson<UserFriendlyConfiguration>($"Extensions.UserFriendlyConfigurations.Commands.{caseLevelName}");

        UserFriendlyGroupModel groupModel = findItems(ufConfig.Commands.Values.Cast<object>().ToList(), 1)[0].Item;
        for (int i = 1; i < level; ++i)
        {
            FoundItem<CommandModel>[] commandModels = findItems<CommandModel>(groupModel.Items, 2);
            groupModel.Items[commandModels[0].Index] = commandModels[0].Item;
            groupModel.Items[commandModels[1].Index] = commandModels[1].Item;

            string commandName1 = commandModels[0].Item.Name;
            string commandName2 = commandModels[1].Item.Name;
            commandModels[0].Item.Name = "Command1";
            commandModels[1].Item.Name = "Command1";

            TestNoneLevel(ufConfig.ToConfiguration, Is.TypeOf<Configuration>());
            TestExceptionLevel<SernagerException>(() => ufConfig.ToConfiguration());

            commandModels[0].Item.Name = commandName1;
            commandModels[1].Item.Name = commandName2;

            FoundItem<UserFriendlyGroupModel> subgroup = findItems(groupModel.Items, 1)[0];
            groupModel.Items[subgroup.Index] = subgroup.Item;
            groupModel = subgroup.Item;
        }
    }

    [Theory]
    public void ToConfiguration_ShouldThrow_WhenCommandShortNameIsNotUnique((int, string) pair)
    {
        Assume.That(pair, Is.AnyOf(LEVEL_CASE_PAIRS));

        (int level, string caseLevelName) = pair;

        if (level == 1)
        {
            return;
        }

        UserFriendlyConfiguration ufConfig = CaseUtil.ReadJson<UserFriendlyConfiguration>($"Extensions.UserFriendlyConfigurations.Commands.{caseLevelName}");

        UserFriendlyGroupModel groupModel = findItems(ufConfig.Commands.Values.Cast<object>().ToList(), 1)[0].Item;
        for (int i = 1; i < level; ++i)
        {
            FoundItem<CommandModel>[] commandModels = findItems<CommandModel>(groupModel.Items, 2);
            groupModel.Items[commandModels[0].Index] = commandModels[0].Item;
            groupModel.Items[commandModels[1].Index] = commandModels[1].Item;

            string commandShortName1 = commandModels[0].Item.ShortName;
            string commandShortName2 = commandModels[1].Item.ShortName;
            commandModels[0].Item.ShortName = "Command1";
            commandModels[1].Item.ShortName = "Command1";

            TestNoneLevel(ufConfig.ToConfiguration, Is.TypeOf<Configuration>());
            TestExceptionLevel<SernagerException>(() => ufConfig.ToConfiguration());

            commandModels[0].Item.ShortName = commandShortName1;
            commandModels[1].Item.ShortName = commandShortName2;

            FoundItem<UserFriendlyGroupModel> subgroup = findItems(groupModel.Items, 1)[0];
            groupModel.Items[subgroup.Index] = subgroup.Item;
            groupModel = subgroup.Item;
        }
    }

    [StackTraceHidden]
    private FoundItem<UserFriendlyGroupModel>[] findItems(List<object> items, int count)
    {
        List<FoundItem<UserFriendlyGroupModel>> groups = new List<FoundItem<UserFriendlyGroupModel>>();

        List<(FoundItem<UserFriendlyGroupModel>, int)> groupPairs = new List<(FoundItem<UserFriendlyGroupModel>, int)>();

        for (int i = 0; i < items.Count; ++i)
        {
            object item = items[i];

            if (item is not UserFriendlyGroupModel group)
            {
                if (item is not JsonElement jsonElement || !canJsonElementCaseTo<UserFriendlyGroupModel>(jsonElement))
                {
                    continue;
                }

                UserFriendlyGroupModel? obj = JsonWrapper.Deserialize<UserFriendlyGroupModel>(jsonElement.GetRawText());

                if (obj == null)
                {
                    continue;
                }

                group = obj;
            }

            groupPairs.Add((new FoundItem<UserFriendlyGroupModel>(group, i), group.Items.Count));
        }

        groupPairs.Sort((pair1, pair2) => pair2.Item2.CompareTo(pair1.Item2));

        foreach ((FoundItem<UserFriendlyGroupModel> item, _) in groupPairs)
        {
            groups.Add(item);

            if (groups.Count == count)
            {
                break;
            }
        }

        if (groups.Count != count)
        {
            throw new Exception("Not enough groups found.");
        }

        return groups.ToArray();
    }

    [StackTraceHidden]
    private FoundItem<TItem>[] findItems<TItem>(List<object> items, int count)
        where TItem : class
    {
        List<FoundItem<TItem>> foundItems = new List<FoundItem<TItem>>();

        for (int i = 0; i < items.Count; ++i)
        {
            object item = items[i];

            if (item is not TItem foundItem)
            {
                if (item is not JsonElement jsonElement || !canJsonElementCaseTo<TItem>(jsonElement))
                {
                    continue;
                }

                TItem? obj = JsonWrapper.Deserialize<TItem>(jsonElement.GetRawText());

                if (obj == null)
                {
                    continue;
                }

                foundItem = obj;
            }

            foundItems.Add(new FoundItem<TItem>(foundItem, i));

            if (foundItems.Count == count)
            {
                break;
            }
        }

        if (foundItems.Count != count)
        {
            throw new Exception("Not enough items found.");
        }

        return foundItems.ToArray();
    }

    [StackTraceHidden]
    private bool canJsonElementCaseTo<TCast>(JsonElement jsonElement)
    {
        if (typeof(TCast) == typeof(CommandModel))
        {
            return jsonElement.TryGetProperty("command", out JsonElement _) &&
                jsonElement.TryGetProperty("usedEnvironmentGroups", out JsonElement _);
        }

        if (typeof(TCast) == typeof(UserFriendlyGroupModel))
        {
            return jsonElement.TryGetProperty("items", out JsonElement _);
        }

        return true;
    }

    [StackTraceHidden]
    private UserFriendlyGroupModel getTestableGroupModel(UserFriendlyGroupModel groupModel1, UserFriendlyGroupModel groupModel2)
    {
        if (groupModel1.Items.Count > 1)
        {
            return groupModel1;
        }
        else if (groupModel2.Items.Count > 1)
        {
            return groupModel2;
        }
        else
        {
            throw new Exception("Not enough items found.");
        }
    }
}
