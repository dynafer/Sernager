using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Models.Histories;

internal sealed class HistoryInputModel : IHistoryPromptModel
{
    private InputPlugin mPlugin { get; init; }

    internal HistoryInputModel(InputPlugin plugin)
    {
        mPlugin = plugin;
    }

    object IHistoryPromptModel.Prompt()
    {
        return Prompter.Prompt(mPlugin);
    }
}
