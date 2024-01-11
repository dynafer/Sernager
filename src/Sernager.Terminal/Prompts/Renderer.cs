using Sernager.Terminal.Prompts.Components;

namespace Sernager.Terminal.Prompts;

internal sealed class Renderer : IDisposable
{
    private TextWriter mWriter;
    internal int CurrentX { get; private set; } = 0;
    internal int CurrentY { get; private set; } = 0;

    internal Renderer(TextWriter writer)
    {
        mWriter = writer;
    }

    public void Dispose()
    {
        mWriter = null!;
    }

    internal void ShowCursor()
    {
        mWriter.Write(AnsiCode.ShowCursor());
    }

    internal void HideCursor()
    {
        mWriter.Write(AnsiCode.HideCursor());
    }

    internal void Render(List<IPromptComponent> components)
    {
        if (CurrentX != 0)
        {
            mWriter.Write(AnsiCode.CursorLeft(CurrentX));
            CurrentX = 0;
        }

        if (CurrentY != 0)
        {
            mWriter.Write(AnsiCode.CursorUp(CurrentY));
            CurrentY = 0;
        }

        mWriter.Write(AnsiCode.EraseScreen());

        foreach (IPromptComponent component in components)
        {
            string text = component.Render();

            if (component is CursorComponent cursorComponent)
            {
                if (cursorComponent.IsLastXAbsolute)
                {
                    CurrentX = cursorComponent.FianlX;
                }
                else
                {
                    CurrentX += cursorComponent.FianlX;
                }

                CurrentY += cursorComponent.FianlY;
            }

            if (component.IsLineBreak)
            {
                mWriter.WriteLine(text);

                CurrentX = 0;
                ++CurrentY;
            }
            else
            {
                mWriter.Write(text);

                CurrentX += text.Length;
            }
        }

        components.Clear();

        mWriter.Flush();
    }

    internal void Render(IPromptComponent component)
    {
        Render(new List<IPromptComponent>() { component });
    }
}
