using Sernager.Terminal.Prompts;

namespace Sernager.Terminal.Models;

internal sealed class HistoryModel
{
    internal Guid Id { get; private init; }
    private HistoryPromptPluginHandler mPluginHandler { get; init; }
    private HistoryResultHandler mResultHandler { get; init; }

    internal HistoryModel(HistoryPromptPluginHandler pluginHandler, HistoryResultHandler resultHandler)
    {
        Id = Guid.NewGuid();
        mPluginHandler = pluginHandler;
        mResultHandler = resultHandler;
    }

    internal void RunWithPrompt()
    {
        object result = Prompter.Prompt(mPluginHandler());
        mResultHandler(result);
    }

    internal void RunWithResult(object result)
    {
        mResultHandler(result);
    }
}
