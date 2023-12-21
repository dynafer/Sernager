using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Factories.Plugins;
using Sernager.Terminal.Prompts.Helpers;

namespace Sernager.Terminal.Prompts;

internal static class Prompter
{
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

        Renderer renderer = new Renderer();

        object result;

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
            Renderer.Writer.WriteLine();
        }

        return result.ToResult<TResult>();
    }
}
