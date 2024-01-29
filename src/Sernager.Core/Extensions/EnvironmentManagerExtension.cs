using Sernager.Core.Configs;
using Sernager.Core.Helpers;
using Sernager.Core.Managers;
using Sernager.Core.Options;
using System.Diagnostics;

namespace Sernager.Core.Extensions;

public static class EnvironmentManagerExtension
{
    public static IEnvironmentManager UseMode(this IEnvironmentManager manager, EAddDataOption mode)
    {
        if (manager.Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return manager;
        }

        manager.AdditionMode = mode;

        return manager;
    }

    public static bool ChangeGroupName(this IEnvironmentManager manager, string name)
    {
        if (manager.Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return false;
        }

        Debug.Assert(ManagerHelper.CanUseEnvironmentGroupName(name), "Suppoed to be checked before.");

        Configurator.Config.EnvironmentGroups.Remove(manager.Group.Name);
        Configurator.Config.EnvironmentGroups.Add(name, manager.Group);

        manager.Group.Name = name;

        return true;
    }
}
