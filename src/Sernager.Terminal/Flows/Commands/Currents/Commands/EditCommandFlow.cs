using Sernager.Core.Models;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Commands.Currents.Commands;

[Flow(Alias = "Command.CurrentCommand.Manage")]
internal sealed class EditCommandFlow : IFlow
{
    private readonly CommandModel mCommandModel;

    internal EditCommandFlow(CommandModel commandModel)
    {
        mCommandModel = commandModel;
    }

    void IFlow.Prompt()
    {
        List<string> initialLines = new List<string>();

        switch (mCommandModel.Command)
        {
            case string[] commandArray:
                if (commandArray.Length > 0)
                {
                    initialLines.AddRange(commandArray);
                }

                break;
            case string commandString:
                if (!string.IsNullOrWhiteSpace(commandString))
                {
                    initialLines.Add(commandString);
                }

                break;
            default:
                break;
        }

        string[] command = Prompter.Prompt(
            new EditorPlugin()
                .UseResourcePack(FlowManager.GetResourceNamespace("Command"))
                .SetPrompt("EditCommand")
                .SetInitialLines(initialLines.ToArray())
        )
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        if (Enumerable.SequenceEqual(initialLines, command))
        {
            FlowManager.RunPreviousFlow();
            return;
        }

        if (command.Length > 1 || (command.Length == 1 && !string.IsNullOrWhiteSpace(command[0])))
        {
            bool bCommandArray = Prompter.Prompt(
                new ConfirmPlugin()
                    .UseResourcePack(FlowManager.GetResourceNamespace("Command"))
                    .SetPrompt("AskSaveCommandAsArray")
            );

            if (bCommandArray)
            {
                mCommandModel.Command = command;
            }
            else
            {
                mCommandModel.Command = string.Join(" ", command);
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
