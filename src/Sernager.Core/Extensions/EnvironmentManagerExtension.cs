using Sernager.Core.Configs;
using Sernager.Core.Helpers;
using Sernager.Core.Managers;
using Sernager.Core.Options;

namespace Sernager.Core.Extensions;

public static class EnvironmentManagerExtension
{
    public static IEnvironmentManager UseMode(this IEnvironmentManager manager, EAddDataOption mode)
    {
        manager.AdditionMode = mode;

        return manager;
    }

    public static bool ChangeGroupName(this IEnvironmentManager manager, string name)
    {
        if (!ManagerHelper.CanUseEnvironmentGroupName(name))
        {
            return false;
        }

        Configurator.Config.EnvironmentGroups.Remove(manager.Group.Name);
        Configurator.Config.EnvironmentGroups.Add(name, manager.Group);

        manager.Group.Name = name;

        return true;
    }
}
