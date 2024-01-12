using Sernager.Core.Extensions;
using Sernager.Core.Options;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Flows.Extensions;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows;

[Flow]
internal sealed class SaveAsUserFriendlyFlow : IFlow
{
    void IFlow.Prompt()
    {
        (string, string)[] options = Enum.GetValues<EUserFriendlyConfigurationType>()
            .Select(value => (value.GetDescription(), value.ToString()))
            .ToArray();

        string typeString = Prompter.Prompt(
            new SelectionPlugin<string>()
                .UseResourcePack(FlowManager.GetResourceNamespace("Home"))
                .SetPrompt("SaveAsPrompt")
                .SetPageSize(FlowManager.PageSize)
                .UseAutoComplete()
                .AddOptions(options)
                .AddFlowCommonSelectionOptions()
        );

        if (FlowManager.TryHandleCommonSelectionResult(typeString))
        {
            return;
        }

        EUserFriendlyConfigurationType type = Enum.Parse<EUserFriendlyConfigurationType>(typeString);
        Program.Service.SaveAs(type);
        FlowManager.RunPreviousFlow();
    }

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
