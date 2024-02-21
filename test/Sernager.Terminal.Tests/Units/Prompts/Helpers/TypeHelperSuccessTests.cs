using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Helpers;

namespace Sernager.Terminal.Tests.Units.Prompts.Helpers;

internal sealed class TypeHelperSuccessTests
{
    [Test]
    public void EnsureIsSearchable_ShouldPass_WhenTypeIsSupported()
    {
        Assert.DoesNotThrow(TypeHelper.EnsureIsSearchable<string>);
        Assert.DoesNotThrow(TypeHelper.EnsureIsSearchable<OptionItem<string>>);
        Assert.DoesNotThrow(TypeHelper.EnsureIsSearchable<OptionItem<int>>);
        Assert.DoesNotThrow(TypeHelper.EnsureIsSearchable<OptionItem<bool>>);
        Assert.DoesNotThrow(TypeHelper.EnsureIsSearchable<OptionItem<object>>);
    }

    [Test]
    public void EnsureIsPluginResultType_ShouldThrow_WhenTypeIsNotSupported()
    {
        // TODO: Add test for ToTextComponent after testing Plugins
    }
}
