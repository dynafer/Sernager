using Sernager.Core.Configs;
using Sernager.Core.Models;

namespace Sernager.Core.Extensions;

internal static class ConfigurationExtension
{
    internal static UserFriendlyConfiguration ToUserFriendlyConfiguration(this Configuration config)
    {
        UserFriendlyConfiguration userFriendlyConfig = new UserFriendlyConfiguration();

        foreach (KeyValuePair<string, EnvironmentModel> pair in config.EnvironmentGroups)
        {
            userFriendlyConfig.Environments.Add(pair.Key, pair.Value);
        }

        foreach (KeyValuePair<string, GroupModel> pair in config.CommandMainGroups)
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

            userFriendlyConfig.Commands.Add(pair.Value.Name, groupModel);
        }

        return userFriendlyConfig;
    }

    private static void traverseSubgroup(Configuration config, GroupModel model, List<object> parentItems)
    {
        List<object> subItems = new List<object>();

        foreach (Guid itemId in model.Items)
        {
            if (config.CommandSubgroups.ContainsKey(itemId))
            {
                traverseSubgroup(config, config.CommandSubgroups[itemId], subItems);
            }
            else if (config.Commands.ContainsKey(itemId))
            {
                subItems.Add(config.Commands[itemId]);
            }
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
