namespace Sernager.Terminal.Prompts.Extensions;

internal static class SearchableItemExtension
{
    internal static string ToSuggestItem<T>(this T item)
    {
        if (item is string str)
        {
            return str;
        }

        throw new InvalidCastException($"Cannot cast {nameof(T)} to {typeof(string)}.");
    }
}
