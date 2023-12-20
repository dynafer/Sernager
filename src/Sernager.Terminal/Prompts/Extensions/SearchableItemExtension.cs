using Sernager.Terminal.Prompts.Helpers;
using System.Reflection;

namespace Sernager.Terminal.Prompts.Extensions;

internal static class SearchableItemExtension
{
    internal static string ToSuggestItem<T>(this T item)
    {
        if (typeof(T) == typeof(string))
        {
            return item?.ToString() ?? string.Empty;
        }
        else if (TypeHelper.IsOptionItem<T>())
        {
            PropertyInfo? nameProperty = typeof(T).GetProperty("Name");

            if (nameProperty != null)
            {
                return nameProperty.GetValue(item, null)?.ToString() ?? string.Empty;
            }
        }

        throw new InvalidCastException($"Cannot cast {nameof(T)} to suggest item.");
    }
}
