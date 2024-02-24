using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Helpers;

namespace Sernager.Terminal.Tests.Units.Prompts.Helpers;

internal sealed class AnsiCodeHelperTests
{
    [DatapointSource]
    private readonly EDecorationFlags[] mDecorations;
    [DatapointSource]
    private readonly EColorFlags[] mColors;
    [DatapointSource]
    private readonly RgbColor?[] mRgbColors;

    public AnsiCodeHelperTests()
    {
        List<EDecorationFlags> decorationFlags = Enum.GetValues<EDecorationFlags>().ToList();
        decorationFlags.Add((EDecorationFlags)int.MaxValue);

        List<EColorFlags> colorFlags = Enum.GetValues<EColorFlags>().ToList();
        colorFlags.Add((EColorFlags)int.MaxValue);

        mDecorations = decorationFlags.ToArray();
        mColors = colorFlags.ToArray();
        mRgbColors =
        [
            null,
            new RgbColor(255, 0, 0),
            new RgbColor(255, 127, 0),
            new RgbColor(255, 255, 0),
            new RgbColor(0, 255, 0),
            new RgbColor(0, 0, 255),
            new RgbColor(75, 0, 130),
            new RgbColor(143, 0, 255),
        ];
    }

    [Theory]
    public void FromDecoration_ShouldReturnCode(EDecorationFlags decoration)
    {
        int actual = AnsiCodeHelper.FromDecoration(decoration);
        if (decoration == EDecorationFlags.None || decoration == (EDecorationFlags)int.MaxValue)
        {
            Assert.That(actual, Is.Zero);
        }
        else
        {
            Assert.That(actual, Is.GreaterThan(0));
        }
    }

    [Theory]
    public void FromTextColor_ShouldReturnCode(EColorFlags color)
    {
        int actual = AnsiCodeHelper.FromTextColor(color);
        if (color == EColorFlags.Default || color == (EColorFlags)int.MaxValue)
        {
            Assert.That(actual, Is.Zero);
        }
        else
        {
            Assert.That(actual, Is.GreaterThan(0));
        }
    }

    [Test]
    public void GetBeginRgbColor_ShouldReturnCodes()
    {
        int[] actual = AnsiCodeHelper.GetBeginRgbColor();

        Assert.That(actual, Is.Not.Empty);
        Assert.That(actual, Has.Length.EqualTo(2));
        Assert.That(actual[0], Is.EqualTo(38));
        Assert.That(actual[1], Is.EqualTo(2));
    }

    [Theory]
    public void FromTextRgbColor_ShouldReturnCodes(RgbColor? rgbColor)
    {
        int[] actual = AnsiCodeHelper.FromTextRgbColor(rgbColor);

        if (!rgbColor.HasValue)
        {
            Assert.That(actual, Is.Empty);
        }
        else
        {
            Assert.That(actual, Has.Length.EqualTo(5));
            Assert.That(actual[0], Is.EqualTo(38));
            Assert.That(actual[1], Is.EqualTo(2));
            Assert.That(actual[2], Is.EqualTo(rgbColor.Value.R));
            Assert.That(actual[3], Is.EqualTo(rgbColor.Value.G));
            Assert.That(actual[4], Is.EqualTo(rgbColor.Value.B));
        }
    }
}
