using Sernager.Terminal.Prompts.Extensions;

namespace Sernager.Terminal.Tests.Units.Prompts.Extensions;

internal sealed class ConvertExtensionFailureTests
{
    [Test]
    public void ToSuggestItem_ShouldThrow_WhenItemIsNotSupported()
    {
        object objItem = new object();
        int intItem = 1;
        bool boolItem = true;

        Assert.Throws<NotSupportedException>(() => objItem.ToSuggestItem());
        Assert.Throws<NotSupportedException>(() => intItem.ToSuggestItem());
        Assert.Throws<NotSupportedException>(() => boolItem.ToSuggestItem());
    }

    [Test]
    public void ToResult_ShouldThrow_WhenObjectCannotBeCasted()
    {
        object objItem = new object();
        object intItem = 1;
        object boolItem = true;

        Assert.Throws<InvalidCastException>(() => objItem.ToResult<string>());
        Assert.Throws<InvalidCastException>(() => intItem.ToResult<bool>());
        Assert.Throws<InvalidCastException>(() => boolItem.ToResult<int>());
    }

    [Test]
    public void ToOptionType_ShouldThrow_WhenPluginIsNotSupported()
    {
        // TODO: Add test for ToRestTextComponent after testing Plugins
    }
}
