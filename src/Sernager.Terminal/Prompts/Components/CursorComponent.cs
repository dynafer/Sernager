using Sernager.Terminal.Prompts.Components.Cursors;
using System.Text;

namespace Sernager.Terminal.Prompts.Components;

internal sealed class CursorComponent : IPromptComponent
{
    internal List<PromptCursor> Cursors { get; init; } = new List<PromptCursor>();
    internal int FianlX { get; private set; } = 0;
    internal int FianlY { get; private set; } = 0;
    internal bool IsLastXAbsolute { get; private set; } = false;
    bool IPromptComponent.IsLineBreak => false;

    internal CursorComponent AddCursor(ECursorDirection direction, int count)
    {
        PromptCursor cursor = new PromptCursor(direction, count);
        Cursors.Add(cursor);
        updateFinalPosition(cursor);

        return this;
    }

    internal CursorComponent AddCursors(params object[] cursors)
    {
        foreach (object cursor in cursors)
        {
            ECursorDirection? direction = (ECursorDirection?)cursor.GetType().GetProperty("Direction")?.GetValue(cursor, null);
            int? count = (int?)cursor.GetType().GetProperty("Count")?.GetValue(cursor, null);

            if (direction == null || count == null)
            {
                throw new ArgumentException($"Object must have Direction and Count properties to be converted to {nameof(PromptCursor)}.");
            }

            AddCursor(direction.Value, count.Value);
        }

        return this;
    }

    string IPromptComponent.Render()
    {
        StringBuilder builder = new StringBuilder();

        foreach (PromptCursor cursor in Cursors)
        {
            builder.Append(cursor.Direction switch
            {
                ECursorDirection.Up => AnsiCode.CursorUp(cursor.Count),
                ECursorDirection.Down => AnsiCode.CursorDown(cursor.Count),
                ECursorDirection.Left => AnsiCode.CursorLeft(cursor.Count),
                ECursorDirection.Right => AnsiCode.CursorRight(cursor.Count),
                ECursorDirection.HorizontalAbsolute => AnsiCode.CursorHorizontalAbsolute(cursor.Count),
                _ => string.Empty
            });
        }

        return builder.ToString();
    }

    private void updateFinalPosition(PromptCursor cursor)
    {
        IsLastXAbsolute = cursor.Direction == ECursorDirection.HorizontalAbsolute;

        switch (cursor.Direction)
        {
            case ECursorDirection.Up:
                FianlY -= cursor.Count;
                break;
            case ECursorDirection.Down:
                FianlY += cursor.Count;
                break;
            case ECursorDirection.Left:
                FianlX -= cursor.Count;
                break;
            case ECursorDirection.Right:
                FianlX += cursor.Count;
                break;
            case ECursorDirection.HorizontalAbsolute:
                FianlX = cursor.Count;
                break;
        }
    }
}
