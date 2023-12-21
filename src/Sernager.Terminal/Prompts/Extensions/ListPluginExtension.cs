using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Factories.Plugins;

namespace Sernager.Terminal.Prompts.Extensions;

internal static class ListPluginExtension
{
    internal static TPlugin AddOption<TPlugin, TOptionValue>(this TPlugin plugin, string name, TOptionValue value)
        where TPlugin : ListBasePlugin<TOptionValue>
        where TOptionValue : notnull
    {
        EOptionTypeFlags optionType = plugin.ToOptionType();

        plugin.Options.Add(new OptionItem<TOptionValue>(optionType, name, value));

        return plugin;
    }

    internal static TPlugin AddOptions<TPlugin, TOptionValue>(this TPlugin plugin, params (string, TOptionValue)[] options)
        where TPlugin : ListBasePlugin<TOptionValue>
        where TOptionValue : notnull
    {
        EOptionTypeFlags optionType = plugin.ToOptionType();

        foreach ((string name, TOptionValue value) in options)
        {
            plugin.Options.Add(new OptionItem<TOptionValue>(optionType, name, value));
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
