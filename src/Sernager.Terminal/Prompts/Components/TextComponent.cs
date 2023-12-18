namespace Sernager.Terminal.Prompts.Components;

internal sealed class TextComponent : IPromptComponent
{
    private readonly record struct RGBColor(int R, int G, int B);
    private EDecorationFlags mDecoration = EDecorationFlags.None;
    private EColorFlags mTextColor = EColorFlags.Default;
    private RGBColor? mRgbColor;
    private string mText = string.Empty;
    public bool IsLineBreak { get; private set; } = false;

    internal TextComponent SetDecoration(EDecorationFlags decoration)
    {
        mDecoration = decoration;

        return this;
    }

    internal TextComponent SetTextColor(EColorFlags color)
    {
        mTextColor = color;

        return this;
    }

    internal TextComponent SetTextColor(int r, int g, int b)
    {
        mRgbColor = new RGBColor(r, g, b);

        return this;
    }

    internal TextComponent SetText(string text)
    {
        mText = text;

        return this;
    }

    internal TextComponent UseLineBreak()
    {
        IsLineBreak = true;

        return this;
    }

    string IPromptComponent.Render()
    {
        List<int> codes = new List<int>();

        if (mDecoration != EDecorationFlags.None)
        {
            codes.Add(getDecorationCode());
        }

        if (mTextColor != EColorFlags.Default)
        {
            codes.Add(getTextColorCode());
        }

        if (mRgbColor.HasValue)
        {
            codes.AddRange(getTextRgbColorCode());
        }

        return $"{AnsiCode.GraphicsMode(codes.ToArray())}{mText}{AnsiCode.ResetGraphicsMode()}";
    }

    private int getDecorationCode()
    {
        return mDecoration switch
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
        return mTextColor switch
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
        if (!mRgbColor.HasValue)
        {
            return Array.Empty<int>();
        }

        return
        [
            38,
            2,
            mRgbColor.Value.R,
            mRgbColor.Value.G,
            mRgbColor.Value.B,
        ];
    }
}
