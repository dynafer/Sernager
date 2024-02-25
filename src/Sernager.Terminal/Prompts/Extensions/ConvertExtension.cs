using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Helpers;
using Sernager.Terminal.Prompts.Plugins;
using System.Reflection;

namespace Sernager.Terminal.Prompts.Extensions;

internal static class ConvertExtension
{
    internal static string ToSuggestItem<T>(this T item)
    {
        if (item is string strItem)
        {
            return strItem;
        }
        else if (TypeHelper.Is<T, OptionItem<object>>())
        {
            PropertyInfo nameProperty = typeof(T).GetProperty("Name", BindingFlags.NonPublic | BindingFlags.Instance)!;

            return nameProperty.GetValue(item, null)?.ToString() ?? string.Empty;
        }

        throw new NotSupportedException($"Cannot convert {nameof(T)} to suggest item.");
    }

    internal static TResult ToResult<TResult>(this object result)
        where TResult : notnull
    {
        if (result is TResult casted)
        {
            return casted;
        }

        throw new InvalidCastException($"Cannot cast {result.GetType().Name} to {typeof(TResult).Name}.");
    }

    internal static EOptionTypeFlags ToOptionType<TOptionValue>(this ListBasePlugin<TOptionValue> plugin)
        where TOptionValue : notnull
    {
        switch (plugin)
        {
            case MultiSelectionPlugin<TOptionValue> _:
                return EOptionTypeFlags.MultiSelect;
            case SelectionPlugin<TOptionValue> _:
                return EOptionTypeFlags.Select;
            default:
                throw new NotSupportedException($"{plugin.GetType().Name} isn't supported as option type.");
        }
    }
}
