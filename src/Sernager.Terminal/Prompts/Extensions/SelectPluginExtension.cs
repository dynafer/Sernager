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
            if (option is object optionObject)
            {
                string? optionName = optionObject.GetType().GetProperty("Name")?.GetValue(optionObject, null)?.ToString();
                T? optionValue = (T?)(optionObject.GetType().GetProperty("Value")?.GetValue(optionObject, null));

                if (optionName == null || optionValue == null)
                {
                    throw new ArgumentException("Option object must have Name and Value properties.");
                }

                plugin.Options.Add(new OptionItem<T>(EOptionTypeFlags.Select, optionName, optionValue));
            }
            else
            {
                throw new InvalidCastException($"Cannot cast {nameof(option)} to OptionItem.");
            }
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
