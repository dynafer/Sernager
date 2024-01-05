using Sernager.Core.Configs;

namespace Sernager.Core.Helpers;

public static class ManagerHelper
{
    public static bool CanUseCommandGroupName(string name)
    {
        return !Configurator.Config.CommandMainGroups.ContainsKey(name) &&
               !Configurator.Config.CommandMainGroups.Values.Where(x => x.ShortName == name).Any();
    }

    public static bool CanUseEnvironmentGroupName(string name)
    {
        return !Configurator.Config.EnvironmentGroups.ContainsKey(name);
    }
}
