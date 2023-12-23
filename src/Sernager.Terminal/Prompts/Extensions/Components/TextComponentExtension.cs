using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Texts;

namespace Sernager.Terminal.Prompts.Extensions.Components;

internal static class TextComponentExtension
{
    internal static TextComponent SetDecoration(this TextComponent component, EDecorationFlags decoration)
    {
        component.Decoration = decoration;

        return component;
    }

    internal static TextComponent SetTextColor(this TextComponent component, EColorFlags color)
    {
        component.TextColor = color;

        return component;
    }

    internal static TextComponent SetTextColor(this TextComponent component, int r, int g, int b)
    {
        component.RgbColor = new RgbColor(r, g, b);

        return component;
    }

    internal static TextComponent SetText(this TextComponent component, string text)
    {
        component.Text = text;

        return component;
    }
}
