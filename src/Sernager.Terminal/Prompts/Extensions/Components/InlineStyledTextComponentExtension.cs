using Sernager.Terminal.Prompts.Components;

namespace Sernager.Terminal.Prompts.Extensions.Components;

internal static class InlineStyledTextComponentExtension
{
    internal static InlineStyledTextComponent SetText(this InlineStyledTextComponent component, string text)
    {
        component.Text = text;

        return component;
    }
}
