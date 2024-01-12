using Sernager.Core.Models;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Commands.Currents.Commands;

[Flow(Alias = "Command.CurrentCommand.Manage")]
internal sealed class EditEnvironmentGroupFlow : IFlow
{
    private readonly CommandModel mCommandModel;

    internal EditEnvironmentGroupFlow(CommandModel commandModel)
    {
        mCommandModel = commandModel;
    }

    void IFlow.Prompt()
    {
        (string, string)[] environmentGroupOptions = Program.Service.GetEnvironmentGroupNames()
            .Select(x => (x, x))
            .ToArray();

        string[] environmentGroups = Prompter.Prompt(
            new MultiSelectionPlugin<string>()
                .UseResourcePack(FlowManager.GetResourceNamespace("Command"))
                .SetPrompt("SelectEnvironmentGroupPrompt")
                .SetPageSize(FlowManager.PageSize)
                .UseAutoComplete()
                .SetInitialSelections(mCommandModel.UsedEnvironmentGroups.ToArray())
                .AddOptions(environmentGroupOptions)
        ).ToArray();

        mCommandModel.UsedEnvironmentGroups.Clear();
        mCommandModel.UsedEnvironmentGroups.AddRange(environmentGroups);

        FlowManager.RunPreviousFlow();
    }

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
