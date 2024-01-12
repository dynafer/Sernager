using Sernager.Core.Configs;
using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Core.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;

namespace Sernager.Core.Extensions;

internal static class UserFriendlyConfigurationExtension
{
    private static readonly BindingFlags OBJECT_BINDING_FLAGS = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    internal static Configuration ToConfiguration(this UserFriendlyConfiguration userFriendlyConfig)
    {
        Configuration config = new Configuration();

        foreach (KeyValuePair<string, EnvironmentModel> pair in userFriendlyConfig.Environments)
        {
            config.EnvironmentGroups.Add(pair.Key, pair.Value);
        }

        List<string> mainGroupNames = [];

        foreach (KeyValuePair<string, UserFriendlyGroupModel> pair in userFriendlyConfig.Commands)
        {
            if (!canUseName(pair.Value.Name, pair.Value.ShortName, mainGroupNames))
            {
                return config;
            }

            GroupModel groupModel = new GroupModel
            {
                Name = pair.Value.Name,
                ShortName = pair.Value.ShortName,
                Description = pair.Value.Description,
            };

            config.CommandMainGroups.Add(pair.Value.Name, groupModel);
            mainGroupNames.Add(pair.Value.Name);
            if (!string.IsNullOrWhiteSpace(pair.Value.ShortName))
            {
                mainGroupNames.Add(pair.Value.ShortName);
            }

            foreach (object item in pair.Value.Items)
            {
                List<string> subNames = new List<string>();

                filterModel(config, groupModel.Items, subNames, item);
            }
        }

        return config;
    }

    private static bool canUseName(string name, string shortName, List<string> names)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ExceptionManager.ThrowFail<SernagerException>($"Name cannot be empty: {name}");
            return false;
        }

        if (names.Contains(name))
        {
            ExceptionManager.ThrowFail<SernagerException>($"Name must be unique in the group: {name}");
            return false;
        }

        if (!string.IsNullOrWhiteSpace(shortName))
        {
            if (names.Contains(shortName))
            {
                ExceptionManager.ThrowFail<SernagerException>($"Short name must be unique in the group: {shortName}");
                return false;
            }
        }

        return true;
    }

    private static void filterModel(Configuration config, List<Guid> groupItems, List<string> groupNames, object item)
    {
        CommandModel? commandModel;
        UserFriendlyGroupModel? userFriendlyGroupModel;

        if (tryToCommandModel(item, out commandModel))
        {
            if (!canUseName(commandModel.Name, commandModel.ShortName, groupNames))
            {
                return;
            }

            Guid guid = Guid.NewGuid();

            config.Commands.Add(guid, commandModel);
            groupItems.Add(guid);
            groupNames.Add(commandModel.Name);
            if (!string.IsNullOrWhiteSpace(commandModel.ShortName))
            {
                groupNames.Add(commandModel.ShortName);
            }
        }
        else if (tryToUserFriendlyGroupModel(item, out userFriendlyGroupModel))
        {
            if (!canUseName(userFriendlyGroupModel.Name, userFriendlyGroupModel.ShortName, groupNames))
            {
                return;
            }

            groupNames.Add(userFriendlyGroupModel.Name);
            if (!string.IsNullOrWhiteSpace(userFriendlyGroupModel.ShortName))
            {
                groupNames.Add(userFriendlyGroupModel.ShortName);
            }

            traverseSubgroup(config, groupItems, userFriendlyGroupModel);
        }
    }

    private static void traverseSubgroup(Configuration config, List<Guid> parentItems, UserFriendlyGroupModel model)
    {
        List<string> subNames = new List<string>();

        Guid groupGuid = Guid.NewGuid();
        GroupModel groupModel = new GroupModel
        {
            Name = model.Name,
            ShortName = model.ShortName,
            Description = model.Description,
        };

        config.CommandSubgroups.Add(groupGuid, groupModel);
        parentItems.Add(groupGuid);

        foreach (object item in model.Items)
        {
            filterModel(config, groupModel.Items, subNames, item);
        }
    }

    private static bool tryToCommandModel(object obj, [NotNullWhen(true)] out CommandModel? commandModel)
    {
        bool bCommand;
        string json;

        if (obj is JsonElement jsonElement)
        {
            bCommand = isCommand(jsonElement);
            json = jsonElement.GetRawText();
        }
        else
        {
            bCommand = isCommand(obj);
            json = JsonWrapper.Serialize(obj);
        }

        if (!bCommand)
        {
            commandModel = null;
            return false;
        }

        commandModel = JsonWrapper.Deserialize<CommandModel>(json);
        return commandModel != null;
    }

    private static bool tryToUserFriendlyGroupModel(object obj, [NotNullWhen(true)] out UserFriendlyGroupModel? userFriendlyGroupModel)
    {
        bool bGroup;
        string json;

        if (obj is JsonElement jsonElement)
        {
            bGroup = isGroup(jsonElement);
            json = jsonElement.GetRawText();
        }
        else
        {
            bGroup = isGroup(obj);
            json = JsonWrapper.Serialize(obj);
        }

        if (!bGroup)
        {
            userFriendlyGroupModel = null;
            return false;
        }

        userFriendlyGroupModel = JsonWrapper.Deserialize<UserFriendlyGroupModel>(json);
        return userFriendlyGroupModel != null;
    }

    private static bool isCommand(JsonElement jsonElement)
    {
        if (jsonElement.TryGetProperty("command", out JsonElement _))
        {
            return true;
        }

        if (jsonElement.TryGetProperty("usedEnvironmentGroups", out JsonElement _))
        {
            return true;
        }

        return false;
    }

    private static bool isCommand(object obj)
    {
        Type type = obj.GetType();
        PropertyInfo? propertyInfo = type.GetProperty("command", OBJECT_BINDING_FLAGS);

        if (propertyInfo != null)
        {
            return true;
        }

        propertyInfo = type.GetProperty("usedEnvironmentGroups", OBJECT_BINDING_FLAGS);

        return propertyInfo != null;
    }

    private static bool isGroup(JsonElement jsonElement)
    {
        if (jsonElement.TryGetProperty("items", out JsonElement _))
        {
            return true;
        }

        return false;
    }

    private static bool isGroup(object obj)
    {
        Type type = obj.GetType();
        PropertyInfo? propertyInfo = type.GetProperty("items", OBJECT_BINDING_FLAGS);

        return propertyInfo != null;
    }
}
