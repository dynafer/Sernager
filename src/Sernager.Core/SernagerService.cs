using Sernager.Core.Managers;

namespace Sernager.Core;

internal sealed class SernagerService : ISernagerService
{
    public ISettingManager ManageSetting(string settingName)
    {
        ISettingManager manager;

        if (CacheManager.TryGet(settingName, out manager))
        {
            return manager;
        }

        return new SettingManager(settingName);
    }
}
