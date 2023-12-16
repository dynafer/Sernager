namespace Sernager.Terminal.Prompts.Styles;

internal sealed class PromptTextStyle
{
    private readonly record struct RGBColor(int R, int G, int B);

    private EPromptDecorationFlags mDecoration { get; set; }
    private ConsoleColor? mTextColor { get; set; }
    private RGBColor? mRgbColor { get; set; }
    private string mText { get; set; } = string.Empty;

    internal PromptTextStyle SetDecoration(EPromptDecorationFlags decoration)
    {
        mDecoration = decoration;

        return this;
    }

    internal PromptTextStyle SetTextColor(ConsoleColor color)
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
        return $"{getDecoration()}{getTextColor()}{mText}{getReset()}";
    }

    private string getDecoration()
    {
        return mDecoration switch
        {
            EPromptDecorationFlags.Bold => "\u001b[1m",
            EPromptDecorationFlags.Dim => "\u001b[2m",
            EPromptDecorationFlags.Italic => "\u001b[3m",
            EPromptDecorationFlags.Underline => "\u001b[4m",
            EPromptDecorationFlags.Hidden => "\u001b[8m",
            EPromptDecorationFlags.Strikethrough => "\u001b[9m",
            _ => string.Empty,
        };
    }

    private string getTextColor()
    {
        if (mRgbColor.HasValue)
        {
            return $"\u001b[38;2;{mRgbColor.Value.R};{mRgbColor.Value.G};{mRgbColor.Value.B}m";
        }

        return mTextColor switch
        {
            ConsoleColor.Black => "\u001b[30m",
            ConsoleColor.Red => "\u001b[31m",
            ConsoleColor.Green => "\u001b[32m",
            ConsoleColor.Yellow => "\u001b[33m",
            ConsoleColor.Blue => "\u001b[34m",
            ConsoleColor.Magenta => "\u001b[35m",
            ConsoleColor.Cyan => "\u001b[36m",
            ConsoleColor.White => "\u001b[37m",
            _ => string.Empty,
        };
    }

    private string getReset()
    {
        return "\u001b[0m";
    }
}
