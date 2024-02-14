using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Helpers;

namespace Sernager.Terminal.Tests.Units.Prompts.Components;

internal sealed class InlineStyledTextComponentTests
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
    public void UsePlainTextOnly_ShouldUsePlainTextOnlyOption()
    {
        InlineStyledTextComponent component = new InlineStyledTextComponent();

        Assert.That(PrivateUtil.GetFieldValue<bool>(component, "mbPlainTextOnly"), Is.False);

        component.UsePlainTextOnly();

        Assert.That(PrivateUtil.GetFieldValue<bool>(component, "mbPlainTextOnly"), Is.True);
    }

    [Theory]
    public void Render_ShouldReturnRenderedText(EDecorationFlags decoration, EColorFlags textColor, RgbColor? rgbColor)
    {
        Assume.That(decoration, Is.AnyOf(DECORATIONS));
        Assume.That(textColor, Is.AnyOf(COLORS));
        Assume.That(rgbColor, Is.AnyOf(RGB_COLORS));

        (string, string)[] testCasesAndExpections = createAllCasesAndExpectations(decoration, textColor, rgbColor);

        for (int i = 0; i < testCasesAndExpections.Length; ++i)
        {
            (string testCase, string expected) = testCasesAndExpections[i];
            string expectation = expected + AnsiCode.ResetGraphicsMode();

            InlineStyledTextComponent component = new InlineStyledTextComponent
            {
                Text = testCase
            };

            IPromptComponent promptComponent = component;

            Assert.That(promptComponent.Render(), Is.EqualTo(expectation), $"Test case: {i}");

            component.UsePlainTextOnly();

            Assert.That(promptComponent.Render(), Is.EqualTo("Hello, World!").Or.EqualTo("[Unrelated]Hello, World!"), $"Test case: {i}");
        }
    }

    private (string, string)[] createAllCasesAndExpectations(EDecorationFlags decoration, EColorFlags textColor, RgbColor? rgbColor)
    {
        bool bDecoration = decoration != EDecorationFlags.None;
        bool bTextColor = textColor != EColorFlags.Default;
        bool bRgbColor = rgbColor != null;

        int? decorationResetCode = bDecoration ? 0 : null;
        int? textColorResetCode = bTextColor ? 0 : null;
        int? rgbColorResetCode = bRgbColor ? 0 : null;

        List<(string, string)> testCasesAndExpections =
        [
            (
                "Hello, World!",
                "Hello, World!"
            ),
            (
                "[Unrelated]Hello, World!",
                "[Unrelated]Hello, World!"
            ),
            (
                $"Hello, [{decoration}][{textColor}]World!",
                "Hello, "
                    + codesToString(decoration)
                    + codesToString(decoration, textColor)
                    + "World!"
            ),
            (
                $"Hello, [RGB(0, 0)]World!",
                "Hello, World!"
            ),
            (
                $"Hello, [{decoration}][{textColor}]World![/{textColor}][/{decoration}]",
                "Hello, "
                    + codesToString(decoration)
                    + codesToString(decoration, textColor)
                    + "World!"
                    + codesToString(
                        textColorResetCode,
                        bDecoration ? decoration : null
                    )
            ),
            (
                $"Hello, [{decoration}][{textColor}]World![/][/]",
                "Hello, "
                    + codesToString(decoration)
                    + codesToString(decoration, textColor)
                    + "World!"
                    + (bDecoration && bTextColor
                        ? codesToString(
                            textColorResetCode,
                            decoration
                        )
                        : (bDecoration || bTextColor
                            ? AnsiCode.ResetGraphicsMode()
                            : string.Empty
                        )
                    )
            ),
            (
                $"Hello, [{decoration}]Wor[{textColor}]ld[/{textColor}]![/{decoration}]",
                "Hello, "
                    + codesToString(decoration)
                    + "Wor"
                    + codesToString(decoration, textColor)
                    + "ld"
                    + codesToString(
                        textColorResetCode,
                        bDecoration ? decoration : null
                    )
                    + "!"
            ),
            (
                $"Hello, [{decoration}]Wor[{textColor}]ld[/]![/]",
                "Hello, "
                    + codesToString(decoration)
                    + "Wor"
                    + codesToString(decoration, textColor)
                    + "ld"
                    + (bDecoration && bTextColor
                        ? codesToString(textColorResetCode, decoration)
                        : (bDecoration || bTextColor
                            ? AnsiCode.ResetGraphicsMode()
                            : string.Empty
                        )
                    )
                    + "!"
            ),
            (
                $"Hello, [{decoration}]Wor[{textColor}]ld[/{decoration}]![/{textColor}]",
                "Hello, "
                    + codesToString(decoration)
                    + "Wor"
                    + codesToString(decoration, textColor)
                    + "ld"
                    + codesToString(
                        decorationResetCode,
                        bTextColor ? textColor : null
                    )
                    + "!"
            ),
            (
                $"Hello, [{decoration}]Wor{rgbColorToString(rgbColor, false)}ld{rgbColorToString(rgbColor, true)}![/{decoration}]",
                "Hello, "
                    + codesToString(decoration)
                    + "Wor"
                    + (bRgbColor ? codesToString(decoration, rgbColor) : string.Empty)
                    + "ld"
                    + (bRgbColor
                        ? codesToString(
                            rgbColorResetCode,
                            bDecoration ? decoration : null
                        )
                        : string.Empty
                    )
                    + "!"
            ),
            (
                $"Hello, [{decoration}]Wor{rgbColorToString(rgbColor, false)}ld[/]![/]",
                "Hello, "
                    + codesToString(decoration)
                    + "Wor"
                    + (bRgbColor ? codesToString(decoration, rgbColor) : string.Empty)
                    + "ld"
                    + (bDecoration && bRgbColor
                        ? codesToString(rgbColorResetCode, decoration)
                        : (bDecoration || bRgbColor
                            ? AnsiCode.ResetGraphicsMode()
                            : string.Empty
                          )
                      )
                    + "!"
            ),
            (
                $"Hello, [{decoration}]Wor{rgbColorToString(rgbColor, false)}ld[/{decoration}]!{rgbColorToString(rgbColor, true)}",
                "Hello, "
                    + codesToString(decoration)
                    + "Wor"
                    + (bRgbColor ? codesToString(decoration, rgbColor) : string.Empty)
                    + "ld"
                    + codesToString(
                        decorationResetCode,
                        bRgbColor ? rgbColor : null
                    )
                    + "!"
            ),
        ];

        testCasesAndExpections.AddRange(createDefaultCasesAndExpectations("Hello, ", "World!", decoration));
        testCasesAndExpections.AddRange(createDefaultCasesAndExpectations("Hello, ", "World!", textColor));
        testCasesAndExpections.AddRange(createDefaultCasesAndExpectations("Hello, ", "World!", rgbColor));

        return testCasesAndExpections.ToArray();
    }

    private (string, string)[] createDefaultCasesAndExpectations(string prefix, string text, object? code)
    {
        List<(string, string)> wrappedCasesAndExpectations = new List<(string, string)>();

        if (code == null)
        {
            return wrappedCasesAndExpectations.ToArray();
        }

        if (code is EDecorationFlags decoration)
        {
            wrappedCasesAndExpectations.AddRange([
                ($"{prefix}[{decoration}]{text}", $"{prefix}{codesToString(code)}{text}"),
                ($"{prefix}[{decoration}]{text}[/{decoration}]", $"{prefix}{codesToString(code)}{text}"),
                ($"{prefix}[{decoration}]{text}[/]", $"{prefix}{codesToString(code)}{text}"),
                ($"{prefix}[{decoration}]{text}[Reset]", $"{prefix}{codesToString(code)}{text}"),
                ($"{prefix}[{decoration}]{text}[/Reset]", $"{prefix}{codesToString(code)}{text}"),
            ]);
        }
        else if (code is EColorFlags color)
        {
            wrappedCasesAndExpectations.AddRange([
                ($"{prefix}[{color}]{text}", $"{prefix}{codesToString(code)}{text}"),
                ($"{prefix}[{color}]{text}[/{color}]", $"{prefix}{codesToString(code)}{text}"),
                ($"{prefix}[{color}]{text}[/]", $"{prefix}{codesToString(code)}{text}"),
                ($"{prefix}[{color}]{text}[Reset]", $"{prefix}{codesToString(code)}{text}"),
                ($"{prefix}[{color}]{text}[/Reset]", $"{prefix}{codesToString(code)}{text}"),
            ]);
        }
        else if (code is RgbColor rgbColor)
        {
            wrappedCasesAndExpectations.AddRange([
                ($"{prefix}{rgbColorToString(rgbColor, false)}{text}", $"{prefix}{codesToString(code)}{text}"),
                ($"{prefix}{rgbColorToString(rgbColor, false)}{text}{rgbColorToString(rgbColor, true)}", $"{prefix}{codesToString(code)}{text}"),
                ($"{prefix}{rgbColorToString(rgbColor, false)}{text}[/]", $"{prefix}{codesToString(code)}{text}"),
                ($"{prefix}{rgbColorToString(rgbColor, false)}{text}[Reset]", $"{prefix}{codesToString(code)}{text}"),
                ($"{prefix}{rgbColorToString(rgbColor, false)}{text}[/Reset]", $"{prefix}{codesToString(code)}{text}"),
            ]);
        }
        else
        {
            throw new ArgumentException($"Invalid code type: {code.GetType()}");
        }

        return wrappedCasesAndExpectations.ToArray();
    }

    private string rgbColorToString(RgbColor? rgbColor, bool bClosed)
    {
        if (rgbColor == null)
        {
            return string.Empty;
        }

        string colorString = $"RGB({rgbColor.Value.R}, {rgbColor.Value.G}, {rgbColor.Value.B})";

        if (bClosed)
        {
            colorString = $"/{colorString}";
        }

        return $"[{colorString}]";
    }

    private string codesToString(params object?[] codes)
    {
        List<int> parsedCodes = new List<int>();

        foreach (object? code in codes)
        {
            if (code == null)
            {
                continue;
            }

            if (code is EDecorationFlags decoration)
            {
                if (decoration == EDecorationFlags.None)
                {
                    continue;
                }

                parsedCodes.Add(AnsiCodeHelper.FromDecoration(decoration));
            }
            else if (code is EColorFlags color)
            {
                if (color == EColorFlags.Default)
                {
                    continue;
                }

                parsedCodes.Add(AnsiCodeHelper.FromTextColor(color));
            }
            else if (code is RgbColor rgbColor)
            {
                parsedCodes.AddRange(AnsiCodeHelper.FromTextRgbColor(rgbColor));
            }
            else if (code is int codeValue)
            {
                parsedCodes.Add(codeValue);
            }
            else
            {
                throw new ArgumentException($"Invalid code type: {code.GetType()}");
            }
        }

        if (parsedCodes.Count == 0)
        {
            return string.Empty;
        }

        return AnsiCode.GraphicsMode(parsedCodes.ToArray());
    }
}
