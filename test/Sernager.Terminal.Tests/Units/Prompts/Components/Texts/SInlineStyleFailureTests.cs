using Sernager.Terminal.Prompts.Components.Texts;

namespace Sernager.Terminal.Tests.Units.Prompts.Components.Texts;

internal sealed class SInlineStyleFailureTests
{
    [Test]
    public void Constructor_ShouldThrow_WhenRgbValuesAreOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SInlineStyle(SInlineStyle.RGB_COLOR_STYLE_NAME, [0, 0, 256]));
        Assert.Throws<ArgumentOutOfRangeException>(() => new SInlineStyle(SInlineStyle.RGB_COLOR_STYLE_NAME, [0, -1, 0]));
    }
}
