using System.Diagnostics;
using Sernager.Core.Configs;
using Sernager.Core.Managers;
using Sernager.Core.Models;

namespace Sernager.Core.Tests.Fixtures;

public abstract class CommandManagerFixture : FailureFixture
{
    private protected ICommandManager setUpCommandManager(string prefix, int level, string caseName)
    {
        Configurator.Parse(CaseUtil.GetPath($"{prefix}.{caseName}", "json"));

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
    private protected GroupModel findMainGroupWithMostItems()
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
    private protected Guid findSubgroupIdWithMostItems(List<Guid> itemIds)
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
    private protected Guid getCurrentGroupId(ICommandManager commandManager)
    {
        if (commandManager.CurrentGroup == null)
        {
            throw new Exception("Current group not found.");
        }

        foreach (var pair in Configurator.Config.CommandSubgroups)
        {
            if (pair.Value == commandManager.CurrentGroup)
            {
                return pair.Key;
            }
        }

        throw new Exception("Current group not found.");
    }
}
