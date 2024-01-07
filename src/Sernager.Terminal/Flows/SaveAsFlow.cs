using Sernager.Core.Options;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Flows.Extensions;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows;

[Flow]
internal sealed class SaveAsFlow : IFlow
{
    void IFlow.Prompt()
    {
        (string, string)[] options = Enum.GetNames<EConfigurationType>()
            .Select(x => (x, x))
            .ToArray();

        string typeString = Prompter.Prompt(
            new SelectionPlugin<string>()
                .SetPrompt("Choose a configuration type:")
                .SetPageSize(FlowManager.PageSize)
                .UseAutoComplete()
                .AddOptions(options)
                .AddFlowCommonSelectionOptions()
        );

        if (FlowManager.TryHandleCommonSelectionResult(typeString))
        {
            return;
        }

        EConfigurationType type = Enum.Parse<EConfigurationType>(typeString);
        Program.Service.SaveAs(type);
        FlowManager.RunPreviousFlow();
    }
}
