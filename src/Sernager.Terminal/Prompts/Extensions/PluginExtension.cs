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

    internal static T SetDescription<T>(this T plugin, string description)
        where T : IBasePlugin
    {
        plugin.Description = description;

        return plugin;
    }
}
