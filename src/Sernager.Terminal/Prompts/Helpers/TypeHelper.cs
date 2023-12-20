using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Factories.Plugins;

namespace Sernager.Terminal.Prompts.Helpers;

internal static class TypeHelper
{
    internal static bool IsOptionItem<T>()
    {
        return typeof(T).IsClass && typeof(T).Namespace == typeof(OptionItem<>).Namespace && typeof(T).Name == typeof(OptionItem<>).Name;
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

        throw new NotSupportedException($"{typeof(T).Name} isn't supported as searchable type.");
    }

    internal static void EnsureIsPluginResultType<TResult>(IBasePlugin plugin)
        where TResult : notnull
    {
        switch (plugin)
        {
            case ConfirmPlugin _:
                if (typeof(TResult) == typeof(bool))
                {
                    return;
                }

                break;
            case InputPlugin _:
                if (typeof(TResult) == typeof(string))
                {
                    return;
                }

                break;
            case SelectionPlugin<TResult> _:
                return;
        }

        throw new NotSupportedException($"{plugin.GetType().Name} doesn't accept {typeof(TResult).Name} as result type.");
    }
}
