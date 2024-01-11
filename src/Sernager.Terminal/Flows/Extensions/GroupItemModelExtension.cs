using Sernager.Core.Models;
using Sernager.Terminal.Managers;

namespace Sernager.Terminal.Flows.Extensions;

internal static class GroupItemExtension
{
    internal static string GetTypeString(this GroupItemModel item)
    {
        return item.Item switch
        {
            CommandModel => FlowManager.GetResourceString("Command", "Command"),
            GroupModel => FlowManager.GetResourceString("Command", "Group"),
            _ => FlowManager.GetResourceString("Command", "Unknown")
        };
    }

    internal static string GetNameString(this GroupItemModel item)
    {
        return item.Item switch
        {
            CommandModel command => command.Name,
            GroupModel group => group.Name,
            _ => FlowManager.GetResourceString("Command", "Unknown")
        };
    }
}
