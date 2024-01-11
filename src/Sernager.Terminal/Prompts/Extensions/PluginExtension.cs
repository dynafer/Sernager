using Sernager.Core;
using Sernager.Resources;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Prompts.Extensions;

internal static class PluginExtension
{
    internal static T SetPrompt<T>(this T plugin, string prompt)
        where T : IBasePlugin
    {
        plugin.Prompt = prompt;

        return plugin;
    }

    internal static T AddDescriptions<T>(this T plugin, params string[] description)
        where T : IBasePlugin
    {
        plugin.Description.AddRange(description);

        return plugin;
    }

    internal static T UseResourcePack<T>(this T plugin, string resourcePath)
        where T : IBasePlugin
    {
        if (plugin.ResourcePack != null)
        {
            throw new SernagerException("Resource pack already set.");
        }

        plugin.ResourcePack = ResourceRetriever.UsePack(resourcePath);

        return plugin;
    }
}
