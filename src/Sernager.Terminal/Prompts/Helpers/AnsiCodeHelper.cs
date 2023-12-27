using Sernager.Terminal.Prompts.Components.Texts;

namespace Sernager.Terminal.Prompts.Helpers;

internal static class AnsiCodeHelper
{
    internal static int FromDecoration(EDecorationFlags decoration)
    {
        return decoration switch
        {
            EDecorationFlags.Bold => 1,
            EDecorationFlags.Dim => 2,
            EDecorationFlags.Italic => 3,
            EDecorationFlags.Underline => 4,
            EDecorationFlags.Hidden => 8,
            EDecorationFlags.Strikethrough => 9,
            _ => 0,
        };
    }

    internal static int FromTextColor(EColorFlags color)
    {
        return color switch
        {
            EColorFlags.Black => 30,
            EColorFlags.Red => 31,
            EColorFlags.Green => 32,
            EColorFlags.Yellow => 33,
            EColorFlags.Blue => 34,
            EColorFlags.Magenta => 35,
            EColorFlags.Cyan => 36,
            EColorFlags.White => 37,
            EColorFlags.BrightBlack => 90,
            EColorFlags.BrightRed => 91,
            EColorFlags.BrightGreen => 92,
            EColorFlags.BrightYellow => 93,
            EColorFlags.BrightBlue => 94,
            EColorFlags.BrightMagenta => 95,
            EColorFlags.BrightCyan => 96,
            EColorFlags.BrightWhite => 97,
            _ => 0,
        };
    }

    internal static int[] GetBeginRgbColor()
    {
        return
        [
            38,
            2,
        ];
    }

    internal static int[] FromTextRgbColor(RgbColor? rgbColor)
    {
        if (!rgbColor.HasValue)
        {
            return Array.Empty<int>();
        }

        int[] beginRgbColor = GetBeginRgbColor();
        int[] rgbColorValues = [
            rgbColor.Value.R,
            rgbColor.Value.G,
            rgbColor.Value.B,
        ];

        return beginRgbColor.Concat(rgbColorValues).ToArray();
    }
}
