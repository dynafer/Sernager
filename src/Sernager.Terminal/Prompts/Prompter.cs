using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Helpers;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Prompts;

internal static class Prompter
{
    internal readonly static TextWriter Writer = Console.Out;
    internal static int CountRemovableConsoleLines { get; private set; } = 0;

    internal static object Prompt(IBasePlugin plugin)
    {
        object result = startPrompt<object, object>(plugin);

        return result;
    }

    internal static TResult Prompt<TResult>(ITypePlugin<TResult> plugin)
        where TResult : notnull
    {
        TResult result = startPrompt<TResult, TResult>(plugin);

        return result;
    }

    internal static IEnumerable<TResult> Prompt<TResult>(IEnumerableResultBasePlugin<TResult> plugin)
        where TResult : notnull
    {
        IEnumerable<TResult> result = startPrompt<TResult, IEnumerable<TResult>>(plugin);

        return result;
    }

    private static TResult startPrompt<TOptionValue, TResult>(IBasePlugin plugin)
        where TOptionValue : notnull
        where TResult : notnull
    {
        TypeHelper.EnsureIsPluginResultType<TOptionValue>(plugin);

        object? result;

        using (Renderer renderer = new Renderer(Writer))
        {
            if (!plugin.ShouldShowCursor)
            {
                renderer.HideCursor();
            }

            while (true)
            {
                renderer.Render(plugin.Render());

                CountRemovableConsoleLines = renderer.CurrentY - 1;

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (plugin.Input(keyInfo, out result))
                {
                    break;
                }
            }

            renderer.Render(plugin.RenderLast());

            CountRemovableConsoleLines = 0;

            renderer.ShowCursor();
        }

        return result.ToResult<TResult>();
    }
}
