namespace Sernager.Terminal.Prompts;

internal static class PromptCursor
{
    private static EPromptCursorDirection? mSavedDirection { get; set; } = null;
    private static int mSavedAmount { get; set; } = 0;

    internal static void Save(EPromptCursorDirection direction, int amount = 1)
    {
        mSavedDirection = direction;
        mSavedAmount = amount;
    }

    internal static void Restore()
    {
        if (mSavedDirection == null)
        {
            return;
        }

        Move(mSavedDirection.Value, mSavedAmount);

        mSavedDirection = null;
        mSavedAmount = 0;
    }

    internal static void Move(EPromptCursorDirection direction, int amount = 1)
    {
        if (amount == 0)
        {
            return;
        }

        string ansiCode;
        switch (direction)
        {
            case EPromptCursorDirection.Up:
                ansiCode = AnsiCode.CursorUp(amount);
                break;
            case EPromptCursorDirection.Down:
                ansiCode = AnsiCode.CursorDown(amount);
                break;
            case EPromptCursorDirection.Right:
                ansiCode = AnsiCode.CursorRight(amount);
                break;
            case EPromptCursorDirection.Left:
                ansiCode = AnsiCode.CursorLeft(amount);
                break;
            default:
                throw new NotImplementedException();
        }

        PromptRenderer.Write(ansiCode);
    }

    internal static void MoveHorizontal(int x)
    {
        string ansiCode = AnsiCode.CursorHorizontalAbsolute(x);

        PromptRenderer.Write(ansiCode);
    }
}
