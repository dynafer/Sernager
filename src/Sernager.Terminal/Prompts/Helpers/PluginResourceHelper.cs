using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Prompts.Helpers;

internal static class PluginResourceHelper
{
    internal static string GetString(IBasePlugin plugin, string key)
    {
        if (plugin.ResourcePack == null)
        {
            return key;
        }

        return plugin.ResourcePack.GetString(key);
    }
}
