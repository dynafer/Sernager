using Sernager.Terminal.Attributes;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Commands.Mains;

[Flow(Alias = "Command.Main", Name = "RemoveGroup")]
internal sealed class RemoveGroupFlow : IFlow
{
    void IFlow.Prompt()
    {
        (string, string)[] options = Program.Service.GetCommandGroupNames()
            .Select(x => (x, x))
            .ToArray();

        IEnumerable<string> selectedGroups = Prompter.Prompt(
            new MultiSelectionPlugin<string>()
                .SetPrompt("Select a command group(s) to remove (Cancel: No selection):")
                .SetPageSize(FlowManager.PageSize)
                .UseAutoComplete()
                .AddOptions(options)
        );

        foreach (string group in selectedGroups)
        {
            Program.Service.ManageCommandGroup(group).RemoveMainGroup();
        }

        FlowManager.RunPreviousFlow();
    }
}
