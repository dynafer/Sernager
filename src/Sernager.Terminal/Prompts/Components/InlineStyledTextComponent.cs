using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Helpers;
using System.Text.RegularExpressions;

namespace Sernager.Terminal.Prompts.Components;

internal sealed class InlineStyledTextComponent : IPromptComponent
{
    internal string Text { get; set; } = string.Empty;
    public bool IsLineBreak => false;

    string IPromptComponent.Render()
    {
        string pattern = @"\[(\/?\w+)\]";
        MatchCollection matches = Regex.Matches(Text, pattern);

        int decoration = 0;
        int color = 0;
        int[]? textRgbColor = null;

        string replacedText = Regex.Replace(Text, pattern, match =>
        {
            string value = match.Value.Replace("[", "").Replace("]", "");
            bool bClosed = value.StartsWith("/");

            int code;
            if (tryParseDecoration(value.Replace("/", ""), out code))
            {
                decoration = bClosed
                    ? 0
                    : code;
            }
            else if (tryParseColor(value.Replace("/", ""), out code))
            {
                color = bClosed
                    ? 0
                    : code;
            }
            else if (isTextRgbColor(value))
            {
                textRgbColor = bClosed
                    ? null
                    : toTextRgbColorOrNull(value.Replace("/", ""));
            }
            else if (value == "/")
            {
                decoration = 0;
                color = 0;
                textRgbColor = null;
            }

            List<int> codes = new List<int>() { 0 };

            if (decoration != 0)
            {
                codes.Add(decoration);
            }

            if (color != 0)
            {
                codes.Add(color);
            }

            if (textRgbColor != null)
            {
                codes.AddRange(textRgbColor);
            }

            return AnsiCode.GraphicsMode(codes.ToArray());
        });

        replacedText += AnsiCode.ResetGraphicsMode();

        return replacedText;
    }

    private bool tryParseDecoration(string value, out int code)
    {
        EDecorationFlags decoration;

        if (Enum.TryParse(value, true, out decoration))
        {
            code = AnsiCodeHelper.FromDecoration(decoration);

            return true;
        }
        else
        {
            code = -1;

            return false;
        }
    }

    private bool tryParseColor(string value, out int code)
    {
        EColorFlags color;

        if (Enum.TryParse(value, true, out color))
        {
            code = AnsiCodeHelper.FromTextColor(color);

            return true;
        }
        else
        {
            code = -1;

            return false;
        }
    }

    private bool isTextRgbColor(string value)
    {
        return value.Contains("RGB", StringComparison.OrdinalIgnoreCase);
    }

    private int[]? toTextRgbColorOrNull(string value)
    {
        string name = value.ToUpperInvariant()
            .Replace("RGB", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace(" ", "");

        string[] rgb = name.Split(',');

        if (rgb.Length != 3)
        {
            return null;
        }

        int r;
        int g;
        int b;

        if (int.TryParse(rgb[0], out r) &&
            int.TryParse(rgb[1], out g) &&
            int.TryParse(rgb[2], out b))
        {
            return AnsiCodeHelper.FromTextRgbColor(new RgbColor(r, g, b));
        }
        else
        {
            return null;
        }
    }
}
