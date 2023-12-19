using Sernager.Terminal.Prompts.Components;

namespace Sernager.Terminal.Prompts.Extensions;

internal static class SearchableItemExtension
{
    internal static string ToSuggestItem<T>(this T item)
    {
        if (item is string str)
        {
            return str;
        }
        else if (item is OptionItem component)
        {
            return component.Name;
        }

        throw new InvalidCastException($"Cannot cast {nameof(T)} to {typeof(string)}.");
    }
}
