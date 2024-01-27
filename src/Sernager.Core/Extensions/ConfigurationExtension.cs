using Sernager.Core.Configs;
using Sernager.Core.Models;

namespace Sernager.Core.Extensions;

internal static class ConfigurationExtension
{
    internal static UserFriendlyConfiguration ToUserFriendlyConfiguration(this Configuration config)
    {
        UserFriendlyConfiguration ufConfig = new UserFriendlyConfiguration();

        foreach (var pair in config.EnvironmentGroups)
        {
            ufConfig.Environments.Add(pair.Key, pair.Value);
        }

        foreach (var pair in config.CommandMainGroups)
        {
            List<object> items = new List<object>();

            foreach (Guid itemId in pair.Value.Items)
            {
                filterModel(config, itemId, items);
            }

            UserFriendlyGroupModel groupModel = new UserFriendlyGroupModel
            {
                Name = pair.Value.Name,
                ShortName = pair.Value.ShortName,
                Description = pair.Value.Description,
                Items = items
            };

            ufConfig.Commands.Add(pair.Value.Name, groupModel);
        }

        return ufConfig;
    }

    private static void traverseSubgroup(Configuration config, GroupModel model, List<object> parentItems)
    {
        List<object> subItems = new List<object>();

        foreach (Guid itemId in model.Items)
        {
            filterModel(config, itemId, subItems);
        }

        UserFriendlyGroupModel groupModel = new UserFriendlyGroupModel
        {
            Name = model.Name,
            ShortName = model.ShortName,
            Description = model.Description,
            Items = subItems
        };

        parentItems.Add(groupModel);
    }

    private static void filterModel(Configuration config, Guid itemId, List<object> groupItems)
    {
        if (config.CommandSubgroups.ContainsKey(itemId))
        {
            traverseSubgroup(config, config.CommandSubgroups[itemId], groupItems);
        }
        else if (config.Commands.ContainsKey(itemId))
        {
            groupItems.Add(config.Commands[itemId]);
        }
    }
}
