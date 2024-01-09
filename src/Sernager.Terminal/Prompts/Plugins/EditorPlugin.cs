using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Cursors;
using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Extensions.Components;
using Sernager.Terminal.Prompts.Helpers;
using Sernager.Terminal.Prompts.Plugins.Utilities;

namespace Sernager.Terminal.Prompts.Plugins;

internal sealed class EditorPlugin : IEnumerableResultBasePlugin<string>
{
    private List<string> mOriginalLines = new List<string>();
    private List<string> mLines = new List<string>();
    private int mCurrentEditorY = 0;
    private SCursor mCursor = new SCursor();
    private bool mbCommandMode = true;
    private string mCommandResult = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public List<string> Description { get; private init; } = new List<string>();
    private int mMaxEditorHeight => Console.WindowHeight - Description.Count - 2;

    internal EditorPlugin SetInitialLines(params string[] lines)
    {
        mOriginalLines = lines.ToList();
        mLines = lines.ToList();

        mCursor.X = mLines[^1].Length;
        mCursor.Y = mLines.Count - 1;

        return this;
    }

    bool IBasePlugin.Input(ConsoleKeyInfo keyInfo, out object result)
    {
        if (tryMoveCursor(keyInfo))
        {
            result = null!;

            return false;
        }
        else if (tryDeleteChar(keyInfo))
        {
            result = null!;

            return false;
        }
        else if (tryAddLineBreak(keyInfo))
        {
            result = null!;

            return false;
        }
        else if (tryAddChar(keyInfo))
        {
            result = null!;

            return false;
        }
        else if (tryToggleCommandMode(keyInfo))
        {
            result = null!;

            return false;
        }
        else if (tryCommand(keyInfo))
        {
            if (mCommandResult == "Q")
            {
                result = mOriginalLines;

                return true;
            }
            else if (mCommandResult == "S")
            {
                result = mLines;

                return true;
            }
            else
            {
                result = null!;

                return false;
            }
        }
        else
        {
            result = null!;

            return false;
        }
    }

    List<IPromptComponent> IBasePlugin.Render()
    {
        List<IPromptComponent> components =
        [
            new TextComponent()
                .SetDecoration(EDecorationFlags.Bold)
                .SetText(Prompt)
                .UseLineBreak(),
        ];

        if (Description.Count > 0)
        {
            components.AddRange(
                Description
                    .Select((string description) =>
                    {
                        return new TextComponent()
                            .SetTextColor(EColorFlags.BrightBlack)
                            .SetText(description)
                            .UseLineBreak();
                    })
            );
        }

        int countPrintableLines = Math.Min(mMaxEditorHeight, mLines.Count - mCurrentEditorY);

        components.AddRange(
            mLines
                .GetRange(mCurrentEditorY, countPrintableLines)
                .Select((string line, int _) =>
                {
                    return new TextComponent()
                        .SetText(line)
                        .SetTextColor(EColorFlags.Green)
                        .UseLineBreak();
                })
        );

        bool bNotFull = countPrintableLines < mMaxEditorHeight;

        if (bNotFull)
        {
            components.AddRange(
                Enumerable
                    .Range(0, mMaxEditorHeight - countPrintableLines)
                    .Select((int _) =>
                    {
                        return new TextComponent()
                            .UseLineBreak();
                    })
            );
        }

        components.AddRange([
            new TextComponent()
                .SetText(mbCommandMode ? "(Command Mode)" : "(Insert Mode)")
                .SetDecoration(EDecorationFlags.Bold),
            new TextComponent()
                .SetText($" | Esc: {(mbCommandMode ? "Insert" : "Command")} Mode")
                .SetDecoration(EDecorationFlags.Bold)
        ]);

        if (mbCommandMode)
        {
            components.AddRange([
                new TextComponent()
                    .SetText(" | Q: Exit | S : Save and Exit")
                    .SetDecoration(EDecorationFlags.Bold)
            ]);
        }

        components.AddRange([
            new TextComponent()
                .SetText($" | Ln {mCursor.Y + 1}, Col {mCursor.X + 1}")
                .SetDecoration(EDecorationFlags.Bold),
            new CursorComponent()
                .AddCursors(
                    new { Direction = ECursorDirection.HorizontalAbsolute, Count = mCursor.X + 1 },
                    new { Direction = ECursorDirection.Up, Count = mMaxEditorHeight }
                )
        ]);

        if (mCursor.Y > 0 && mCursor.Y != mCurrentEditorY)
        {
            components.Add(
                new CursorComponent()
                    .AddCursor(ECursorDirection.Down, mCursor.Y - mCurrentEditorY)
            );
        }

        return components;
    }

    List<IPromptComponent> IBasePlugin.RenderLast()
    {
        List<IPromptComponent> components =
        [
            new TextComponent()
                .SetDecoration(EDecorationFlags.Bold)
                .SetText(Prompt),
            new CursorComponent()
        ];

        if (Description.Count > 0)
        {
            components.Add(new LineBreakComponent());
            components.AddRange(
                Description
                    .Select((string description) =>
                    {
                        return new TextComponent()
                            .SetTextColor(EColorFlags.BrightBlack)
                            .SetText(description)
                            .UseLineBreak();
                    })
            );
        }

        return components;
    }

    private void updateEditorY()
    {
        if (mCursor.Y < mCurrentEditorY)
        {
            mCurrentEditorY = mCursor.Y;
        }
        else if (mCursor.Y >= mCurrentEditorY + mMaxEditorHeight)
        {
            mCurrentEditorY = mCursor.Y - mMaxEditorHeight + 1;
        }
    }

    private bool tryAddChar(ConsoleKeyInfo keyInfo)
    {
        if (mbCommandMode || !KeyHelper.IsCharKey(keyInfo.Key))
        {
            return false;
        }

        mLines[mCursor.Y] = mLines[mCursor.Y].Insert(mCursor.X, keyInfo.KeyChar.ToString());
        ++mCursor.X;

        return true;
    }

    private bool tryAddLineBreak(ConsoleKeyInfo keyInfo)
    {
        if (mbCommandMode || keyInfo.Key != ConsoleKey.Enter)
        {
            return false;
        }

        mLines.Insert(mCursor.Y + 1, string.Empty);

        if (mCursor.X < mLines[mCursor.Y].Length)
        {
            mLines[mCursor.Y + 1] = mLines[mCursor.Y].Substring(mCursor.X);
            mLines[mCursor.Y] = mLines[mCursor.Y].Substring(0, mCursor.X);
        }

        mCursor.X = 0;
        ++mCursor.Y;

        updateEditorY();

        return true;
    }

    private bool tryDeleteChar(ConsoleKeyInfo keyInfo)
    {
        if (mbCommandMode || (keyInfo.Key != ConsoleKey.Backspace && keyInfo.Key != ConsoleKey.Delete))
        {
            return false;
        }

        switch (keyInfo.Key)
        {
            case ConsoleKey.Backspace:
                if (mCursor.X > 0)
                {
                    mLines[mCursor.Y] = mLines[mCursor.Y].Remove(mCursor.X - 1, 1);
                    --mCursor.X;
                }
                else if (mCursor.Y > 0)
                {
                    --mCursor.Y;
                    mCursor.X = mLines[mCursor.Y].Length;
                    mLines[mCursor.Y] += mLines[mCursor.Y + 1];
                    mLines.RemoveAt(mCursor.Y + 1);

                    updateEditorY();
                }

                break;
            case ConsoleKey.Delete:
                if (mCursor.X < mLines[mCursor.Y].Length)
                {
                    mLines[mCursor.Y] = mLines[mCursor.Y].Remove(mCursor.X, 1);
                }
                else if (mCursor.Y < mLines.Count - 1)
                {
                    mLines[mCursor.Y] += mLines[mCursor.Y + 1];
                    mLines.RemoveAt(mCursor.Y + 1);

                    updateEditorY();
                }

                break;
            default:
                return false;
        }

        return true;
    }

    private bool tryMoveCursor(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.LeftArrow:
                if (mCursor.X > 0)
                {
                    --mCursor.X;
                }
                else if (mCursor.Y > 0)
                {
                    --mCursor.Y;
                    mCursor.X = mLines[mCursor.Y].Length;

                    updateEditorY();
                }

                break;
            case ConsoleKey.RightArrow:
                if (mCursor.X < mLines[mCursor.Y].Length)
                {
                    ++mCursor.X;
                }
                else if (mCursor.Y < mLines.Count - 1)
                {
                    ++mCursor.Y;
                    mCursor.X = 0;

                    updateEditorY();
                }

                break;
            case ConsoleKey.UpArrow:
                if (mCursor.Y > 0)
                {
                    --mCursor.Y;
                }

                if (mCursor.X > mLines[mCursor.Y].Length)
                {
                    mCursor.X = mLines[mCursor.Y].Length;
                }

                updateEditorY();

                break;
            case ConsoleKey.DownArrow:
                if (mCursor.Y < mLines.Count - 1)
                {
                    ++mCursor.Y;
                }

                if (mCursor.X > mLines[mCursor.Y].Length)
                {
                    mCursor.X = mLines[mCursor.Y].Length;
                }

                updateEditorY();

                break;
            case ConsoleKey.Home:
                mCursor.X = 0;

                break;
            case ConsoleKey.End:
                mCursor.X = mLines[mCursor.Y].Length;

                break;
            case ConsoleKey.PageUp:
                mCurrentEditorY = Math.Max(0, mCurrentEditorY - mMaxEditorHeight);
                mCursor.Y = Math.Max(0, mCursor.Y - mMaxEditorHeight);

                break;
            case ConsoleKey.PageDown:
                mCurrentEditorY = Math.Min(mLines.Count - mMaxEditorHeight, mCurrentEditorY + mMaxEditorHeight);
                mCurrentEditorY = Math.Max(mCurrentEditorY, 0);
                mCursor.Y = Math.Min(mLines.Count - 1, mCursor.Y + mMaxEditorHeight);
                mCursor.Y = Math.Max(mCursor.Y, 0);

                break;
            default:
                return false;
        }

        return true;
    }

    private bool tryToggleCommandMode(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key != ConsoleKey.Escape)
        {
            return false;
        }

        mbCommandMode = !mbCommandMode;

        return true;
    }

    private bool tryCommand(ConsoleKeyInfo keyInfo)
    {
        if (!mbCommandMode)
        {
            return false;
        }

        if (keyInfo.Key == ConsoleKey.Q)
        {
            mCommandResult = "Q";
        }
        else if (keyInfo.Key == ConsoleKey.S)
        {
            mCommandResult = "S";
        }
        else
        {
            return false;
        }

        return true;
    }
}
