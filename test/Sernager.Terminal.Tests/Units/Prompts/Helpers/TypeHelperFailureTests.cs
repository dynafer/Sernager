using Sernager.Terminal.Prompts.Helpers;

namespace Sernager.Terminal.Tests.Units.Prompts.Helpers;

internal sealed class TypeHelperFailureTests
{
    [Test]
    public void EnsureIsSearchable_ShouldThrow_WhenTypeIsNotSupported()
    {
        Assert.Throws<NotSupportedException>(TypeHelper.EnsureIsSearchable<int>);
        Assert.Throws<NotSupportedException>(TypeHelper.EnsureIsSearchable<bool>);
        Assert.Throws<NotSupportedException>(TypeHelper.EnsureIsSearchable<object>);
    }

    [Test]
    public void EnsureIsPluginResultType_ShouldThrow_WhenTypeIsNotSupported()
    {
        // TODO: Add test for ToTextComponent after testing Plugins
    }
}
