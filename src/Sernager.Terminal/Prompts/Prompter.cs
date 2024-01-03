using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;
using Sernager.Terminal.Prompts.Helpers;

namespace Sernager.Terminal.Prompts;

internal static class Prompter
{
    internal readonly static TextWriter Writer = Console.Out;

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

        object result;

        using (Renderer renderer = new Renderer(Writer))
        {
            while (true)
            {
                renderer.Render(plugin.Render());

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                bool bBreak = plugin.Input(keyInfo, out result);

                if (bBreak)
                {
                    break;
                }
            }

            if (plugin.ShouldContinueToNextLine)
            {
                Writer.WriteLine();
            }
        }

        return result.ToResult<TResult>();
    }
}
