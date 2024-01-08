using Sernager.Core.Managers;
using Sernager.Terminal.Attributes;

namespace Sernager.Terminal.Flows.Environments;

[Flow(Alias = "Environment", Name = "Manage")]
internal sealed class ManageFlow : IFlow
{
    private IEnvironmentManager mManager;

    internal ManageFlow(IEnvironmentManager manager)
    {
        mManager = manager;
    }

    void IFlow.Prompt()
    {
        // FIX ME: Implement this flow.
    }
}
