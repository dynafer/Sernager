using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Models;

internal sealed class HistoryModel
{
    internal Guid Id { get; private init; }
    private IBasePlugin mPlugin { get; init; }
    private HistoryResultHandler mResultHandler { get; init; }

    internal HistoryModel(IBasePlugin plugin, HistoryResultHandler resultHandler)
    {
        Id = Guid.NewGuid();
        mPlugin = plugin;
        mResultHandler = resultHandler;
    }

    internal void RunWithPrompt()
    {
        object result = Prompter.Prompt(mPlugin);
        mResultHandler(result);
    }

    internal void RunWithResult(object result)
    {
        mResultHandler(result);
    }
}
