using Sernager.Core.Extensions;
using Sernager.Core.Managers;
using Sernager.Core.Options;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Environments.Manages;

[Flow(Alias = "Environment.Manage")]
internal sealed class ChangeAdditionModeFlow : IFlow
{
    private readonly IEnvironmentManager mManager;

    internal ChangeAdditionModeFlow(IEnvironmentManager manager)
    {
        mManager = manager;
    }

    void IFlow.Prompt()
    {
        EAddDataOption result = Prompter.Prompt(
            new SelectionPlugin<EAddDataOption>()
                .SetPrompt(FlowManager.CommonResourcePack.GetString("ChooseOptionPrompt"))
                .AddOptions(
                    (EAddDataOption.SkipIfExists.GetDescription(), EAddDataOption.SkipIfExists),
                    (EAddDataOption.OverwriteIfExists.GetDescription(), EAddDataOption.OverwriteIfExists),
                    (EAddDataOption.OverwriteAll.GetDescription(), EAddDataOption.OverwriteAll)
                )
        );

        mManager.UseMode(result);

        FlowManager.RunPreviousFlow();
    }

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
