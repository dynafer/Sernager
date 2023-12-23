using Sernager.Terminal.Prompts.Components.Texts;

namespace Sernager.Terminal.Prompts.Components;

internal sealed class TextComponent : IPromptComponent
{
    internal EDecorationFlags Decoration { get; set; } = EDecorationFlags.None;
    internal EColorFlags TextColor { get; set; } = EColorFlags.Default;
    internal RgbColor? RgbColor { get; set; } = null;
    internal string Text { get; set; } = string.Empty;
    public bool IsLineBreak { get; set; } = false;

    internal TextComponent UseLineBreak()
    {
        IsLineBreak = true;

        return this;
    }

    string IPromptComponent.Render()
    {
        List<int> codes = new List<int>();

        if (Decoration != EDecorationFlags.None)
        {
            codes.Add(getDecorationCode());
        }

        if (TextColor != EColorFlags.Default)
        {
            codes.Add(getTextColorCode());
        }

        if (RgbColor.HasValue)
        {
            codes.AddRange(getTextRgbColorCode());
        }

        return $"{AnsiCode.GraphicsMode(codes.ToArray())}{Text}{AnsiCode.ResetGraphicsMode()}";
    }

    private int getDecorationCode()
    {
        return Decoration switch
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

    private int getTextColorCode()
    {
        return TextColor switch
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

    private int[] getTextRgbColorCode()
    {
        if (!RgbColor.HasValue)
        {
            return Array.Empty<int>();
        }

        return
        [
            38,
            2,
            RgbColor.Value.R,
            RgbColor.Value.G,
            RgbColor.Value.B,
        ];
    }
}
