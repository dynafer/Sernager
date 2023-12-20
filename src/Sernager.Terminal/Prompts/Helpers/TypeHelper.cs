using Sernager.Terminal.Prompts.Components;

namespace Sernager.Terminal.Prompts.Helpers;

internal static class TypeHelper
{
    internal static bool IsOptionItem<T>()
    {
        // FIX ME: This is a hacky way to check if T is OptionItem<T>.
        return typeof(T).IsClass && typeof(T).IsInstanceOfType(typeof(OptionItem<>));
    }

    internal static void EnsureIsSearchable<T>()
    {
        if (typeof(T) == typeof(string))
        {
            return;
        }
        else if (IsOptionItem<T>())
        {
            return;
        }

        throw new InvalidCastException($"Cannot cast {typeof(T)} to searchable type.");
    }
}
