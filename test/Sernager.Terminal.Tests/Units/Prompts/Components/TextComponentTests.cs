using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Helpers;

namespace Sernager.Terminal.Tests.Units.Prompts.Components;

internal sealed class TextComponentTests
{
    [DatapointSource]
    private static readonly EDecorationFlags[] DECORATIONS = Enum.GetValues<EDecorationFlags>();
    [DatapointSource]
    private static readonly EColorFlags[] COLORS = Enum.GetValues<EColorFlags>();
    [DatapointSource]
    private static readonly RgbColor?[] RGB_COLORS =
    [
        null,
        new RgbColor(255, 0, 0),
        new RgbColor(255, 127, 0),
        new RgbColor(255, 255, 0),
        new RgbColor(0, 255, 0),
        new RgbColor(0, 0, 255),
        new RgbColor(75, 0, 130),
        new RgbColor(143, 0, 255)
    ];

    [Test]
    public void UseLineBreak_ShouldSetIsLineBreakToTrue()
    {
        TextComponent component = new TextComponent();

        Assert.That(component.IsLineBreak, Is.False);

        component.UseLineBreak();

        Assert.That(component.IsLineBreak, Is.True);
    }

    [Theory]
    public void Render_ShouldReturnRenderedText(EDecorationFlags decoration, EColorFlags textColor, RgbColor? rgbColor)
    {
        Assume.That(decoration, Is.AnyOf(DECORATIONS));
        Assume.That(textColor, Is.AnyOf(COLORS));
        Assume.That(rgbColor, Is.AnyOf(RGB_COLORS));

        IPromptComponent component = new TextComponent
        {
            Decoration = decoration,
            TextColor = textColor,
            RgbColor = rgbColor,
            Text = "Hello, World!"
        };

        List<int> codes = new List<int>();

        if (decoration != EDecorationFlags.None)
        {
            codes.Add(AnsiCodeHelper.FromDecoration(decoration));
        }

        if (textColor != EColorFlags.Default)
        {
            codes.Add(AnsiCodeHelper.FromTextColor(textColor));
        }

        if (rgbColor.HasValue)
        {
            codes.AddRange(AnsiCodeHelper.FromTextRgbColor(rgbColor));
        }

        if (codes.Count == 0)
        {
            Assert.That(component.Render(), Is.EqualTo("Hello, World!"));
            return;
        }

        string expected = $"{AnsiCode.GraphicsMode(codes.ToArray())}Hello, World!{AnsiCode.ResetGraphicsMode()}";
        string actual = component.Render();

        Assert.That(actual, Is.EqualTo(expected));
    }
}
