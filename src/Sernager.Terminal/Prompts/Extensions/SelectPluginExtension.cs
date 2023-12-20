using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Factories.Plugins;

namespace Sernager.Terminal.Prompts.Extensions;

internal static class SelectPluginExtension
{
    internal static SelectionPlugin<T> AddOption<T>(this SelectionPlugin<T> plugin, string name, T value)
        where T : notnull
    {
        plugin.Options.Add(new OptionItem<T>(EOptionTypeFlags.Select, name, value));

        return plugin;
    }

    internal static SelectionPlugin<T> AddOptions<T>(this SelectionPlugin<T> plugin, params object[] options)
        where T : notnull
    {
        foreach (object option in options)
        {
            plugin.Options.Add(option.ToOptionItem<T>(EOptionTypeFlags.Select));
        }

        return plugin;
    }

    internal static SelectionPlugin<T> SetPageSize<T>(this SelectionPlugin<T> plugin, int pageSize)
        where T : notnull
    {
        plugin.Pagination.PageSize = pageSize;

        return plugin;
    }
}
