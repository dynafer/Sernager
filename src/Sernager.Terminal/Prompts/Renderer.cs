using Sernager.Terminal.Prompts.Components;

namespace Sernager.Terminal.Prompts;

internal sealed class Renderer
{
    public readonly static TextWriter Writer = Console.Out;
    private int mCurrentX = 0;
    private int mCurrentY = 0;

    internal void Render(List<IPromptComponent> components)
    {
        if (mCurrentX != 0)
        {
            Writer.Write(AnsiCode.CursorLeft(mCurrentX));
            mCurrentX = 0;
        }

        if (mCurrentY != 0)
        {
            Writer.Write(AnsiCode.CursorUp(mCurrentY));
            mCurrentY = 0;
        }

        Writer.Write(AnsiCode.EraseScreen());

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
                Writer.WriteLine(text);

                mCurrentX = 0;
                ++mCurrentY;
            }
            else
            {
                Writer.Write(text);

                mCurrentX += text.Length;
            }
        }

        Writer.Flush();
    }
}
