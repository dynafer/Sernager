using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Factories.Plugins;

namespace Sernager.Terminal.Prompts.Extensions;

internal static class SelectPluginExtension
{
    internal static SelectionPlugin AddOption(this SelectionPlugin plugin, string name)
    {
        plugin.Options.Add(new OptionItem(EOptionTypeFlags.Select, name));

        return plugin;
    }

    internal static SelectionPlugin AddOption(this SelectionPlugin plugin, string name, string value)
    {
        plugin.Options.Add(new OptionItem(EOptionTypeFlags.Select, name, value));

        return plugin;
    }

    internal static SelectionPlugin AddOptions(this SelectionPlugin plugin, params object[] options)
    {
        foreach (object option in options)
        {
            if (option is string optionString)
            {
                plugin.Options.Add(new OptionItem(EOptionTypeFlags.Select, optionString));
            }
            else if (option is object optionObject)
            {
                string? optionName = optionObject.GetType().GetProperty("Name")?.GetValue(optionObject, null)?.ToString();
                string? optionValue = optionObject.GetType().GetProperty("Value")?.GetValue(optionObject, null)?.ToString();

                if (optionName == null || optionValue == null)
                {
                    throw new ArgumentException("Option object must have Name and Value properties.");
                }

                plugin.Options.Add(new OptionItem(EOptionTypeFlags.Select, optionName, optionValue));
            }
            else
            {
                throw new InvalidCastException($"Cannot cast {nameof(option)} to {typeof(string)} or {typeof(OptionItem)}.");
            }
        }

        return plugin;
    }

    internal static SelectionPlugin SetPageSize(this SelectionPlugin plugin, int pageSize)
    {
        plugin.Pagination.PageSize = pageSize;

        return plugin;
    }
}
