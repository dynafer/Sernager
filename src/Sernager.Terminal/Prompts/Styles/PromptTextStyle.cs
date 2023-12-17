namespace Sernager.Terminal.Prompts.Styles;

internal sealed class PromptTextStyle
{
    private readonly record struct RGBColor(int R, int G, int B);
    private EPromptDecorationFlags mDecoration { get; set; } = EPromptDecorationFlags.None;
    private EPromptColorFlags mTextColor { get; set; } = EPromptColorFlags.Default;
    private RGBColor? mRgbColor { get; set; }
    private string mText { get; set; } = string.Empty;

    internal PromptTextStyle SetDecoration(EPromptDecorationFlags decoration)
    {
        mDecoration = decoration;

        return this;
    }

    internal PromptTextStyle SetTextColor(EPromptColorFlags color)
    {
        mTextColor = color;

        return this;
    }

    internal PromptTextStyle SetTextColor(int r, int g, int b)
    {
        mRgbColor = new RGBColor(r, g, b);

        return this;
    }

    internal PromptTextStyle SetText(string text)
    {
        mText = text;

        return this;
    }

    internal string Apply()
    {
        List<int> codes = new List<int>();

        if (mDecoration != EPromptDecorationFlags.None)
        {
            codes.Add(getDecorationCode());
        }

        if (mTextColor != EPromptColorFlags.Default)
        {
            codes.Add(getTextColorCode());
        }

        if (mRgbColor.HasValue)
        {
            codes.Add(38);
            codes.Add(2);
            codes.Add(mRgbColor.Value.R);
            codes.Add(mRgbColor.Value.G);
            codes.Add(mRgbColor.Value.B);
        }

        return $"{AnsiCode.GraphicsMode(codes.ToArray())}{mText}{AnsiCode.ResetGraphicsMode()}";
    }

    private int getDecorationCode()
    {
        return mDecoration switch
        {
            EPromptDecorationFlags.Bold => 1,
            EPromptDecorationFlags.Dim => 2,
            EPromptDecorationFlags.Italic => 3,
            EPromptDecorationFlags.Underline => 4,
            EPromptDecorationFlags.Hidden => 8,
            EPromptDecorationFlags.Strikethrough => 9,
            _ => 0,
        };
    }

    private int getTextColorCode()
    {
        return mTextColor switch
        {
            EPromptColorFlags.Black => 30,
            EPromptColorFlags.Red => 31,
            EPromptColorFlags.Green => 32,
            EPromptColorFlags.Yellow => 33,
            EPromptColorFlags.Blue => 34,
            EPromptColorFlags.Magenta => 35,
            EPromptColorFlags.Cyan => 36,
            EPromptColorFlags.White => 37,
            EPromptColorFlags.BrightBlack => 90,
            EPromptColorFlags.BrightRed => 91,
            EPromptColorFlags.BrightGreen => 92,
            EPromptColorFlags.BrightYellow => 93,
            EPromptColorFlags.BrightBlue => 94,
            EPromptColorFlags.BrightMagenta => 95,
            EPromptColorFlags.BrightCyan => 96,
            EPromptColorFlags.BrightWhite => 97,
            _ => 0,
        };
    }
}
