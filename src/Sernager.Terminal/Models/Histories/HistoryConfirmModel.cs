using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Models.Histories;

internal sealed class HistoryConfirmModel : IHistoryPromptModel
{
    private ConfirmPlugin mPlugin { get; init; }

    internal HistoryConfirmModel(ConfirmPlugin plugin)
    {
        mPlugin = plugin;
    }

    object IHistoryPromptModel.Prompt()
    {
        return Prompter.Prompt(mPlugin);
    }
}
