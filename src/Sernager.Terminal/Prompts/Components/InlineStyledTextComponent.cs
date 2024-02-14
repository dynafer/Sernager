using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Helpers;
using System.Text.RegularExpressions;

namespace Sernager.Terminal.Prompts.Components;

internal sealed class InlineStyledTextComponent : IPromptComponent
{
    private bool mbPlainTextOnly = false;
    internal string Text { get; set; } = string.Empty;
    public bool IsLineBreak => false;

    internal InlineStyledTextComponent UsePlainTextOnly()
    {
        mbPlainTextOnly = true;

        return this;
    }

    string IPromptComponent.Render()
    {
        string pattern = @"\[\/?[a-zA-Z]*\(?[\d\s,]*\)?\]";
        MatchCollection matches = Regex.Matches(Text, pattern);

        List<SInlineStyle> styles = new List<SInlineStyle>();
        int lastCodesLength = 0;

        string replacedText = Regex.Replace(Text, pattern, match =>
        {
            string value = match.Value.ToUpperInvariant().Replace("[", "").Replace("]", "").Replace(" ", "");
            bool bClosed = value.StartsWith("/");

            int code;
            bool bReset = false;
            bool bParsed = false;

            if (tryParseDecoration(value.Replace("/", ""), out code))
            {
                if (mbPlainTextOnly)
                {
                    return string.Empty;
                }

                addOrDeleteDecoration(styles, code, bClosed, ref bReset);
                bParsed = true;
            }
            else if (tryParseColor(value.Replace("/", ""), out code))
            {
                if (mbPlainTextOnly)
                {
                    return string.Empty;
                }

                addOrDeleteColor(styles, code, bClosed, ref bReset);
                bParsed = true;
            }
            else if (isTextRgbColor(value))
            {
                if (mbPlainTextOnly)
                {
                    return string.Empty;
                }

                addOrDeleteRgbColor(styles, value.Replace("/", ""), bClosed, ref bReset);
                bParsed = true;
            }
            else if (value == "/")
            {
                if (mbPlainTextOnly)
                {
                    return string.Empty;
                }

                if (styles.Count > 0)
                {
                    styles.RemoveAt(styles.Count - 1);
                    bReset = true;
                }

                bParsed = true;
            }
            else if (value == "/RESET" || value == "RESET")
            {
                if (mbPlainTextOnly)
                {
                    return string.Empty;
                }

                styles.Clear();
                bReset = true;
                bParsed = true;
            }

            if (mbPlainTextOnly || !bParsed)
            {
                return match.Value;
            }

            List<int> codes = new List<int>();
            if (bReset)
            {
                codes.Add(0);
            }

            foreach (SInlineStyle style in styles)
            {
                if (style.Name == "Decoration")
                {
                    codes.Add((int)style.Value);
                }
                else if (style.Name == "Color")
                {
                    codes.Add((int)style.Value);
                }
                else if (style.Name == "RgbColor")
                {
                    codes.AddRange((int[])style.Value);
                }
            }

            if (codes.Count == lastCodesLength && codes.Count == 0)
            {
                return string.Empty;
            }

            lastCodesLength = codes.Count;

            if (codes.Count == 0)
            {
                return AnsiCode.ResetGraphicsMode();
            }

            return AnsiCode.GraphicsMode(codes.ToArray());
        });

        if (mbPlainTextOnly)
        {
            return replacedText;
        }

        if (!replacedText.EndsWith(AnsiCode.ResetGraphicsMode()))
        {
            replacedText += AnsiCode.ResetGraphicsMode();
        }

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

    private void addOrDeleteDecoration(List<SInlineStyle> styles, int code, bool bDelete, ref bool bReset)
    {
        if (code == 0)
        {
            return;
        }

        if (!bDelete)
        {
            styles.Add(new SInlineStyle("Decoration", code));
        }
        else
        {
            int existedIndex = styles.FindLastIndex(style => style.Name == "Decoration" && style.Value.Equals(code));
            if (existedIndex != -1)
            {
                styles.RemoveAt(existedIndex);
                bReset = true;
            }
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

    private void addOrDeleteColor(List<SInlineStyle> styles, int code, bool bDelete, ref bool bReset)
    {
        if (code == 0)
        {
            return;
        }

        if (!bDelete)
        {
            styles.Add(new SInlineStyle("Color", code));
        }
        else
        {
            int existedIndex = styles.FindLastIndex(style => style.Name == "Color" && style.Value.Equals(code));
            if (existedIndex != -1)
            {
                styles.RemoveAt(existedIndex);
                bReset = true;
            }
        }
    }

    private bool isTextRgbColor(string value)
    {
        return value.Contains("RGB", StringComparison.OrdinalIgnoreCase);
    }

    private int[]? toTextRgbColorOrNull(string value)
    {
        string name = value
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

    private void addOrDeleteRgbColor(List<SInlineStyle> styles, string value, bool bDelete, ref bool bReset)
    {
        int[]? textRgbColor = toTextRgbColorOrNull(value);

        if (!bDelete)
        {
            if (textRgbColor != null)
            {
                styles.Add(new SInlineStyle("RgbColor", textRgbColor));
            }

            return;
        }

        int existedIndex = styles.FindLastIndex(style =>
        {
            bool bRgbColor = style.Name == "RgbColor";

            if (textRgbColor != null)
            {
                int[] styleValue = (int[])style.Value;

                bRgbColor &= styleValue[0] == textRgbColor[0] &&
                             styleValue[1] == textRgbColor[1] &&
                             styleValue[2] == textRgbColor[2];
            }

            return bRgbColor;
        });

        if (existedIndex != -1)
        {
            styles.RemoveAt(existedIndex);
            bReset = true;
        }
    }
}
