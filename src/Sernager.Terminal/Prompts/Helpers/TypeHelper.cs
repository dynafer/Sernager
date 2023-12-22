using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Prompts.Helpers;

internal static class TypeHelper
{
    internal static bool Is<TTarget, TCompare>()
    {
        return typeof(TTarget) == typeof(TCompare) || typeof(TTarget).Namespace == typeof(TCompare).Namespace && typeof(TTarget).Name == typeof(TCompare).Name;
    }

    internal static void EnsureIsSearchable<T>()
        where T : notnull
    {
        if (typeof(T) == typeof(string))
        {
            return;
        }
        else if (Is<T, OptionItem<object>>())
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
            case MultiSelectionPlugin<TResult> _:
            case SelectionPlugin<TResult> _:
                return;
        }

        throw new NotSupportedException($"{plugin.GetType().Name} doesn't accept {typeof(TResult).Name} as result type.");
    }
}
