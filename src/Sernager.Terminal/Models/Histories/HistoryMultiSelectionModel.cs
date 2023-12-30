using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Models.Histories;

internal sealed class HistoryMultiSelectionModel<TOptionValue> : IHistoryPromptModel
    where TOptionValue : notnull
{
    private MultiSelectionPlugin<TOptionValue> mPlugin { get; init; }

    internal HistoryMultiSelectionModel(MultiSelectionPlugin<TOptionValue> plugin)
    {
        mPlugin = plugin;
    }

    object IHistoryPromptModel.Prompt()
    {
        return Prompter.Prompt(mPlugin);
    }
}
