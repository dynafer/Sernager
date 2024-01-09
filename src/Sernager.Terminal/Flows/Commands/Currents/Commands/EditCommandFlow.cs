using Sernager.Core.Models;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Flows.Extensions;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Commands.Currents.Commands;

[Flow(Alias = "Command.CurrentCommand.Manage")]
internal sealed class EditCommandFlow : IFlow
{
    private CommandModel mCommandModel;

    internal EditCommandFlow(CommandModel commandModel)
    {
        mCommandModel = commandModel;
    }

    void IFlow.Prompt()
    {
        string oldCommand = mCommandModel.Command switch
        {
            string[] commandArray => string.Join(" ", commandArray),
            string commandString => commandString,
            _ => throw new ArgumentException("Command must be a string or string[]")
        };

        string command = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt("Enter a command (Cancel: Same input)")
                .SetInitialInput(oldCommand)
        );

        if (command == oldCommand)
        {
            FlowManager.RunPreviousFlow();
            return;
        }

        if (!string.IsNullOrWhiteSpace(command))
        {
            bool bCommandArray = Prompter.Prompt(
                new ConfirmPlugin()
                    .SetPrompt("Do you want to save the command as an array?")
            );

            mCommandModel.Command = command;

            if (bCommandArray)
            {
                mCommandModel.ToCommandAsArray();
            }
        }
        else
        {
            mCommandModel.Command = string.Empty;
        }

        FlowManager.RunPreviousFlow();
    }

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
