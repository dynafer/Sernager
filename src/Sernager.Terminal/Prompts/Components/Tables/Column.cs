namespace Sernager.Terminal.Prompts.Components.Tables;

internal sealed class Column
{
    internal IPromptComponent Component { get; init; }
    internal int Length { get; init; } = 0;
    internal int Colspan { get; init; } = 1;

    internal Column(TextComponent component, int colspan = 1)
    {
        if (colspan < 1)
        {
            throw new ArgumentException("Colspan must be greater than 0.");
        }

        Component = component;
        Length = component.Text.Length;
        Colspan = colspan;
    }

    internal string Render()
    {
        return Component.Render();
    }
}
