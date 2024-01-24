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

    internal static Configuration ToConfiguration(this UserFriendlyConfiguration ufConfig)
    {
        Configuration config = new Configuration();

        List<string> environmentNames = new List<string>();

        foreach (KeyValuePair<string, EnvironmentModel> pair in ufConfig.Environments)
        {
            if (!canUseName(pair.Value.Name, string.Empty, environmentNames))
            {
                return config;
            }

            config.EnvironmentGroups.Add(pair.Key, pair.Value);
            environmentNames.Add(pair.Value.Name);
        }

        environmentNames.Clear();

        List<string> mainGroupNames = new List<string>();

        foreach (KeyValuePair<string, UserFriendlyGroupModel> pair in ufConfig.Commands)
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

            List<string> subNames = new List<string>();

            foreach (object item in pair.Value.Items)
            {
                if (!tryFilterModel(config, groupModel.Items, subNames, item))
                {
                    return config;
                }
            }
        }

        mainGroupNames.Clear();

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
            if (name == shortName || names.Contains(shortName))
            {
                ExceptionManager.ThrowFail<SernagerException>($"Short name must be unique in the group: {shortName}");
                return false;
            }
        }

        return true;
    }

    private static bool tryFilterModel(Configuration config, List<Guid> parentItems, List<string> parentNames, object item)
    {
        CommandModel? commandModel;
        UserFriendlyGroupModel? ufGroupModel;

        if (tryToCommandModel(item, out commandModel))
        {
            if (!canUseName(commandModel.Name, commandModel.ShortName, parentNames))
            {
                return false;
            }

            Guid guid = Guid.NewGuid();

            config.Commands.Add(guid, commandModel);
            parentItems.Add(guid);
            parentNames.Add(commandModel.Name);
            if (!string.IsNullOrWhiteSpace(commandModel.ShortName))
            {
                parentNames.Add(commandModel.ShortName);
            }
        }
        else if (tryToUserFriendlyGroupModel(item, out ufGroupModel))
        {
            if (!canUseName(ufGroupModel.Name, ufGroupModel.ShortName, parentNames))
            {
                return false;
            }

            Guid groupGuid = Guid.NewGuid();
            GroupModel groupModel = new GroupModel
            {
                Name = ufGroupModel.Name,
                ShortName = ufGroupModel.ShortName,
                Description = ufGroupModel.Description,
            };

            config.CommandSubgroups.Add(groupGuid, groupModel);
            parentItems.Add(groupGuid);
            parentNames.Add(ufGroupModel.Name);
            if (!string.IsNullOrWhiteSpace(ufGroupModel.ShortName))
            {
                parentNames.Add(ufGroupModel.ShortName);
            }

            List<string> subNames = new List<string>();

            foreach (object subItem in ufGroupModel.Items)
            {
                if (!tryFilterModel(config, groupModel.Items, subNames, subItem))
                {
                    return false;
                }
            }
        }

        return true;
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
        else if (obj is Dictionary<object, object> dictionary)
        {
            bCommand = isCommand(dictionary);
            json = JsonWrapper.Serialize(dictionary);
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

    private static bool tryToUserFriendlyGroupModel(object obj, [NotNullWhen(true)] out UserFriendlyGroupModel? ufGroupModel)
    {
        bool bGroup;
        string json;

        if (obj is JsonElement jsonElement)
        {
            bGroup = isGroup(jsonElement);
            json = jsonElement.GetRawText();
        }
        else if (obj is Dictionary<object, object> dictionary)
        {
            bGroup = isGroup(dictionary);
            json = JsonWrapper.Serialize(dictionary);
        }
        else
        {
            bGroup = isGroup(obj);
            json = JsonWrapper.Serialize(obj);
        }

        if (!bGroup)
        {
            ufGroupModel = null;
            return false;
        }

        ufGroupModel = JsonWrapper.Deserialize<UserFriendlyGroupModel>(json);
        return ufGroupModel != null;
    }

    private static bool isCommand(JsonElement jsonElement)
    {
        if (jsonElement.TryGetProperty("command", out _))
        {
            return true;
        }

        if (jsonElement.TryGetProperty("usedEnvironmentGroups", out _))
        {
            return true;
        }

        return false;
    }

    private static bool isCommand(Dictionary<object, object> dictionary)
    {
        if (dictionary.ContainsKey("command"))
        {
            return true;
        }

        if (dictionary.ContainsKey("usedEnvironmentGroups"))
        {
            return true;
        }

        return false;
    }

    private static bool isCommand(object obj)
    {
        Type type = obj.GetType();
        PropertyInfo? propertyInfo = type.GetProperty("command", OBJECT_BINDING_FLAGS) ?? type.GetProperty("Command", OBJECT_BINDING_FLAGS);

        if (propertyInfo != null)
        {
            return true;
        }

        propertyInfo = type.GetProperty("usedEnvironmentGroups", OBJECT_BINDING_FLAGS) ?? type.GetProperty("UsedEnvironmentGroups", OBJECT_BINDING_FLAGS);

        return propertyInfo != null;
    }

    private static bool isGroup(JsonElement jsonElement)
    {
        if (jsonElement.TryGetProperty("items", out _))
        {
            return true;
        }

        return false;
    }

    private static bool isGroup(Dictionary<object, object> dictionary)
    {
        if (dictionary.ContainsKey("items"))
        {
            return true;
        }

        return false;
    }

    private static bool isGroup(object obj)
    {
        Type type = obj.GetType();
        PropertyInfo? propertyInfo = type.GetProperty("items", OBJECT_BINDING_FLAGS) ?? type.GetProperty("Items", OBJECT_BINDING_FLAGS);

        return propertyInfo != null;
    }
}
