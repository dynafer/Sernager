using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.Text;

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
        StringBuilder builder = new StringBuilder();

        bool bTagging = false;
        bool bClose = false;
        string tagName = string.Empty;

        List<SInlineStyle> styles = new List<SInlineStyle>();
        bool bAppliedStyle = true;

        foreach (char c in Text)
        {
            if (c == '[')
            {
                tagName = string.Empty;
                bTagging = true;
                bClose = false;
            }
            else if (c == '/' && bTagging)
            {
                bClose = true;
            }
            else if (c == ']' && bTagging)
            {
                if (!tryTag(styles, tagName, bClose))
                {
                    builder.Append('[');

                    if (bClose)
                    {
                        builder.Append('/');
                    }

                    builder.Append(tagName);
                    builder.Append(']');
                }
                else
                {
                    bAppliedStyle = false;
                }

                bTagging = false;
                bClose = false;
                tagName = string.Empty;
            }
            else if (bTagging)
            {
                tagName += c;
            }
            else
            {
                if (!bAppliedStyle && !mbPlainTextOnly)
                {
                    if (styles.Count == 0)
                    {
                        builder.Append(AnsiCode.ResetGraphicsMode());
                    }
                    else
                    {
                        builder.Append(applyStyles(styles));
                    }

                    bTagging = false;
                    bClose = false;
                    tagName = string.Empty;
                    bAppliedStyle = true;
                }

                builder.Append(c);
            }
        }

        styles.Clear();

        string result = builder.ToString();

        if (!result.EndsWith(AnsiCode.ResetGraphicsMode()) && !mbPlainTextOnly)
        {
            result += AnsiCode.ResetGraphicsMode();
        }

        return result;
    }

    private bool tryTag(List<SInlineStyle> styles, string tagName, bool bClose)
    {
        string filteredTag = tagName.ToUpperInvariant().Replace(" ", "");

        if (filteredTag == "RESET")
        {
            styles.Clear();

            return true;
        }

        if (bClose)
        {
            if (filteredTag.Length == 0)
            {
                if (styles.Count > 0)
                {
                    styles.RemoveAt(styles.Count - 1);
                }

                return true;
            }
            else if (filteredTag == "DECO" || filteredTag == "DECORATION")
            {
                removeStyle(styles, SInlineStyle.DECORATION_STYLE_NAME);
                return true;
            }
            else if (filteredTag == "COLOR")
            {
                removeStyle(styles, SInlineStyle.COLOR_STYLE_NAME);
                return true;
            }
            else if (filteredTag == "RGB")
            {
                removeStyle(styles, SInlineStyle.RGB_COLOR_STYLE_NAME);
                return true;
            }
        }

        string? styleName;
        int code;

        if (tryParseDecoration(filteredTag, out styleName, out code) ||
            tryParseColor(filteredTag, out styleName, out code))
        {
            if (code == 0)
            {
                return false;
            }

            if (bClose)
            {
                removeStyle(styles, styleName, code);
            }
            else
            {
                styles.Add(new SInlineStyle(styleName, code));
            }

            return true;
        }
        else if (isTextRgbColor(filteredTag))
        {
            int[]? rgbColor;

            if (tryParseRgbColor(filteredTag, out styleName, out rgbColor))
            {
                if (bClose)
                {
                    removeStyle(styles, styleName, rgbColor);
                }
                else
                {
                    styles.Add(new SInlineStyle(styleName, rgbColor));
                }

                return true;
            }
        }

        return false;
    }

    private string applyStyles(List<SInlineStyle> styles)
    {
        if (styles.Count == 0)
        {
            return string.Empty;
        }

        List<int> codes = [0];

        foreach (SInlineStyle style in styles)
        {
            if (style.Name == SInlineStyle.DECORATION_STYLE_NAME || style.Name == SInlineStyle.COLOR_STYLE_NAME)
            {
                codes.Add((int)style.Value);
            }
            else if (style.Name == SInlineStyle.RGB_COLOR_STYLE_NAME)
            {
                int[] rgbColor = (int[])style.Value;

                codes.AddRange(rgbColor);
            }
        }

        return AnsiCode.GraphicsMode(codes.ToArray());
    }

    private bool tryParseDecoration(string value, [NotNullWhen(true)] out string? styleName, out int code)
    {
        EDecorationFlags decoration;

        if (Enum.TryParse(value, true, out decoration))
        {
            styleName = SInlineStyle.DECORATION_STYLE_NAME;
            code = AnsiCodeHelper.FromDecoration(decoration);

            return true;
        }
        else
        {
            styleName = null;
            code = -1;

            return false;
        }
    }

    private bool tryParseColor(string value, [NotNullWhen(true)] out string? styleName, out int code)
    {
        EColorFlags color;

        if (Enum.TryParse(value, true, out color))
        {
            styleName = SInlineStyle.COLOR_STYLE_NAME;
            code = AnsiCodeHelper.FromTextColor(color);

            return true;
        }
        else
        {
            styleName = null;
            code = -1;

            return false;
        }
    }

    private bool isTextRgbColor(string value)
    {
        return value.Contains("RGB", StringComparison.OrdinalIgnoreCase);
    }

    private bool tryParseRgbColor(string value, [NotNullWhen(true)] out string? styleName, [NotNullWhen(true)] out int[]? codes)
    {
        string name = value
            .Replace("RGB", "")
            .Replace("(", "")
            .Replace(")", "");

        string[] rgb = name.Split(',');

        if (rgb.Length != 3)
        {
            styleName = null;
            codes = null;

            return false;
        }

        uint r;
        uint g;
        uint b;

        if (uint.TryParse(rgb[0], out r) &&
            uint.TryParse(rgb[1], out g) &&
            uint.TryParse(rgb[2], out b))
        {
            RgbColor rgbColor = new RgbColor((int)r, (int)g, (int)b);

            styleName = SInlineStyle.RGB_COLOR_STYLE_NAME;
            codes = AnsiCodeHelper.FromTextRgbColor(rgbColor);

            return true;
        }
        else
        {
            styleName = null;
            codes = null;

            return false;
        }
    }

    private void removeStyle(List<SInlineStyle> styles, string name)
    {
        findAndRemoveStyle(styles, style => style.Name == name);
    }

    private void removeStyle(List<SInlineStyle> styles, string name, int value)
    {
        findAndRemoveStyle(styles, style => style.Name == name && style.Value.Equals(value));
    }

    private void removeStyle(List<SInlineStyle> styles, string name, int[] rgbColor)
    {
        findAndRemoveStyle(styles, style =>
        {
            if (style.Name != name)
            {
                return false;
            }

            int[] styleValue = (int[])style.Value;

            int startIndex = 0;

            if (rgbColor.Length == 5)
            {
                startIndex = 2;
            }

            return styleValue[startIndex] == rgbColor[startIndex] &&
                   styleValue[startIndex + 1] == rgbColor[startIndex + 1] &&
                   styleValue[startIndex + 2] == rgbColor[startIndex + 2];
        });
    }

    private void findAndRemoveStyle(List<SInlineStyle> styles, Predicate<SInlineStyle> match)
    {
        int existedIndex = styles.FindLastIndex(match);
        if (existedIndex != -1)
        {
            styles.RemoveAt(existedIndex);
        }
    }
}
