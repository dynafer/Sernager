using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Models.Histories;

internal sealed class HistorySelectionModel<TOptionValue> : IHistoryPromptModel
    where TOptionValue : notnull
{
    private SelectionPlugin<TOptionValue> mPlugin { get; init; }

    internal HistorySelectionModel(SelectionPlugin<TOptionValue> plugin)
    {
        mPlugin = plugin;
    }

    object IHistoryPromptModel.Prompt()
    {
        return Prompter.Prompt(mPlugin);
    }
}
