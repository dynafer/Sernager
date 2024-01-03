using Sernager.Core.Configs;
using Sernager.Core.Managers;

namespace Sernager.Core;

internal sealed class SernagerService : ISernagerService
{
    ISettingManager ISernagerService.ManageSetting(string settingName)
    {
        ISettingManager manager;

        if (CacheManager.TryGet($"Setting-{settingName}", out manager))
        {
            return manager;
        }

        return new SettingManager(settingName);
    }

    IGroupManager ISernagerService.ManageGroup(string groupName)
    {
        IGroupManager manager;

        if (CacheManager.TryGet($"Group-{groupName}", out manager))
        {
            return manager;
        }

        return new GroupManager(groupName);
    }

    string[] ISernagerService.GetGroupNames()
    {
        return Configurator.Config.Groups.Keys.ToArray();
    }
}
