using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Extensions.Components;

namespace Sernager.Terminal.Tests.Units.Prompts.Extensions.Components;

internal sealed class TextComponentExtensionTests
{
    [DatapointSource]
    private static readonly EDecorationFlags[] DECORATIONS = Enum.GetValues<EDecorationFlags>();
    [DatapointSource]
    private static readonly EColorFlags[] COLORS = Enum.GetValues<EColorFlags>();
    [DatapointSource]
    private static readonly RgbColor[] RGB_COLORS =
    [
        new RgbColor(255, 0, 0),
        new RgbColor(255, 127, 0),
        new RgbColor(255, 255, 0),
        new RgbColor(0, 255, 0),
        new RgbColor(0, 0, 255),
        new RgbColor(75, 0, 130),
        new RgbColor(143, 0, 255)
    ];

    [Theory]
    public void SetDecoration_ShouldSetDecoration(EDecorationFlags decoration)
    {
        Assume.That(decoration, Is.AnyOf(DECORATIONS));

        TextComponent component = new TextComponent();
        component.SetDecoration(decoration);

        Assert.That(component.Decoration, Is.EqualTo(decoration));
    }

    [Theory]
    public void SetTextColor_ShouldSetTextColor(EColorFlags color)
    {
        Assume.That(color, Is.AnyOf(COLORS));

        TextComponent component = new TextComponent();
        component.SetTextColor(color);

        Assert.That(component.TextColor, Is.EqualTo(color));
    }

    [Theory]
    public void SetTextColor_ShouldSetRgbColor(RgbColor rgbColor)
    {
        Assume.That(rgbColor, Is.AnyOf(RGB_COLORS));

        TextComponent component = new TextComponent();
        component.SetTextColor(rgbColor.R, rgbColor.G, rgbColor.B);

        Assert.That(component.RgbColor, Is.EqualTo(rgbColor));
    }

    [Test]
    public void SetText_ShouldSetText()
    {
        TextComponent component = new TextComponent();
        string text = "Hello, World!";
        component.SetText(text);

        Assert.That(component.Text, Is.EqualTo(text));

        text = "Bye, World!";
        component.SetText(text);

        Assert.That(component.Text, Is.EqualTo(text));
    }
}
