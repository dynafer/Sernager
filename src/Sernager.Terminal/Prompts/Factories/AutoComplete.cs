using Sernager.Terminal.Prompts.Helpers;

namespace Sernager.Terminal.Prompts.Factories;

internal class AutoComplete
{
    private string mInput { get; set; } = string.Empty;
    private int mCursorX { get; set; } = 0;

    internal void InterceptInput(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Backspace:
            case ConsoleKey.Delete:
                deleteChar(keyInfo.Key);
                break;
            case ConsoleKey.LeftArrow:
            case ConsoleKey.RightArrow:
                moveCursor(keyInfo.Key);
                break;
            default:
                addChar(keyInfo);
                break;
        }
    }

    private void addChar(ConsoleKeyInfo keyInfo)
    {
        if (!KeyHelper.IsCharKey(keyInfo.Key))
        {
            return;
        }

        mInput = mInput.Insert(mCursorX, keyInfo.KeyChar.ToString());

        ++mCursorX;
    }

    private void deleteChar(ConsoleKey key)
    {
        bool bForward = key == ConsoleKey.Delete;

        if (bForward)
        {
            if (mCursorX < mInput.Length)
            {
                mInput = mInput.Remove(mCursorX, 1);
            }
        }
        else
        {
            if (mCursorX > 0)
            {
                mInput = mInput.Remove(mCursorX, 1);
                --mCursorX;
            }
        }
    }

    private void moveCursor(ConsoleKey key)
    {
        bool bForward = key == ConsoleKey.RightArrow;

        if (bForward)
        {
            if (mCursorX < mInput.Length)
            {
                ++mCursorX;
            }
        }
        else
        {
            if (mCursorX > 0)
            {
                --mCursorX;
            }
        }
    }
}
