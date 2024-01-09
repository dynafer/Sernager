using Sernager.Terminal.Prompts.Components;

namespace Sernager.Terminal.Prompts;

internal sealed class Renderer : IDisposable
{
    private TextWriter mWriter;
    private int mCurrentX = 0;
    private int mCurrentY = 0;

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
        if (mCurrentX != 0)
        {
            mWriter.Write(AnsiCode.CursorLeft(mCurrentX));
            mCurrentX = 0;
        }

        if (mCurrentY != 0)
        {
            mWriter.Write(AnsiCode.CursorUp(mCurrentY));
            mCurrentY = 0;
        }

        mWriter.Write(AnsiCode.EraseScreen());

        foreach (IPromptComponent component in components)
        {
            string text = component.Render();

            if (component is CursorComponent cursorComponent)
            {
                if (cursorComponent.IsLastXAbsolute)
                {
                    mCurrentX = cursorComponent.FianlX;
                }
                else
                {
                    mCurrentX += cursorComponent.FianlX;
                }

                mCurrentY += cursorComponent.FianlY;
            }

            if (component.IsLineBreak)
            {
                mWriter.WriteLine(text);

                mCurrentX = 0;
                ++mCurrentY;
            }
            else
            {
                mWriter.Write(text);

                mCurrentX += text.Length;
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
