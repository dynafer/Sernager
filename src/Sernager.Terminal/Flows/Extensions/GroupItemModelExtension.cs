using Sernager.Core.Models;
using Sernager.Terminal.Managers;

namespace Sernager.Terminal.Flows.Extensions;

internal static class GroupItemExtension
{
    internal static string GetTypeString(this GroupItemModel item)
    {
        return item.Item switch
        {
            CommandModel => FlowManager.CommonResourcePack.GetString("Command"),
            GroupModel => FlowManager.CommonResourcePack.GetString("Group"),
            _ => FlowManager.CommonResourcePack.GetString("Unknown")
        };
    }

    internal static string GetNameString(this GroupItemModel item)
    {
        return item.Item switch
        {
            CommandModel command => command.Name,
            GroupModel group => group.Name,
            _ => FlowManager.CommonResourcePack.GetString("Unknown")
        };
    }
}
