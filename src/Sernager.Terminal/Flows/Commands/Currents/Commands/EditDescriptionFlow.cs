using Sernager.Core.Models;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Commands.Currents.Commands;

[Flow(Alias = "Command.CurrentCommand.Manage")]
internal sealed class EditDescriptionFlow : IFlow
{
    private CommandModel mCommandModel;

    internal EditDescriptionFlow(CommandModel commandModel)
    {
        mCommandModel = commandModel;
    }

    void IFlow.Prompt()
    {
        string description = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt("Enter a command description")
                .SetInitialInput(mCommandModel.Description)
        );

        mCommandModel.Description = description;

        FlowManager.RunPreviousFlow();
    }

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
