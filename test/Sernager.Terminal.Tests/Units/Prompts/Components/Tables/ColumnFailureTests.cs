using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Tables;

namespace Sernager.Terminal.Tests.Units.Prompts.Components.Tables;

internal sealed class ColumnFailureTests
{
    [Test]
    public void Constructor_ShouldThrow_WhenColspanIsLessThanOne()
    {
        Assert.Throws<ArgumentException>(() => new Column("Test", 0));
    }

    [Test]
    public void Constructor_ShouldThrow_WhenColspanIsLessThanOneWithTextComponent()
    {
        Assert.Throws<ArgumentException>(() => new Column(new TextComponent { Text = "Test" }, 0));
    }
}
