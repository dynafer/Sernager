using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Cursors;
using Sernager.Terminal.Prompts.Helpers;
using System.Reflection;

namespace Sernager.Terminal.Prompts.Extensions;

internal static class ConvertExtension
{
    internal static OptionItem<T> ToOptionItem<T>(this object item, EOptionTypeFlags type)
        where T : notnull
    {
        string? name = item.GetType().GetProperty("Name")?.GetValue(item, null)?.ToString();
        T? value = (T?)(item.GetType().GetProperty("Value")?.GetValue(item, null));

        if (name == null || value == null)
        {
            throw new ArgumentException($"Object must have Name and Value properties to be converted to {nameof(OptionItem<T>)}.");
        }

        return new OptionItem<T>(type, name, value);
    }

    internal static PromptCursor ToPromptCursor(this object cursor)
    {
        ECursorDirection? direction = (ECursorDirection?)cursor.GetType().GetProperty("Direction")?.GetValue(cursor, null);
        int? count = (int?)cursor.GetType().GetProperty("Count")?.GetValue(cursor, null);

        if (direction == null || count == null)
        {
            throw new ArgumentException($"Object must have Direction and Count properties to be converted to {nameof(PromptCursor)}.");
        }

        return new PromptCursor(direction.Value, count.Value);
    }

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
}
