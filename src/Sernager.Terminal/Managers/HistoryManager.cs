using Sernager.Core.Managers;
using Sernager.Terminal.Models;

namespace Sernager.Terminal.Managers;

internal static class HistoryManager
{
    private static readonly Dictionary<Guid, HistoryModel> mHistories = new Dictionary<Guid, HistoryModel>();
    private static readonly Stack<Guid> mHistoryStacks = new Stack<Guid>();

    internal static void Run(HistoryModel history, bool bSkip = false)
    {
        if (!mHistories.ContainsKey(history.Id))
        {
            mHistories.Add(history.Id, history);
        }

        mHistoryStacks.Push(history.Id);

        if (bSkip)
        {
            return;
        }

        history.RunWithPrompt();
    }

    internal static void Run(Guid historyId, bool bSkip = false)
    {
        if (!mHistories.ContainsKey(historyId))
        {
            return;
        }

        mHistoryStacks.Push(historyId);

        if (bSkip)
        {
            return;
        }

        mHistories[historyId].RunWithPrompt();
    }

    internal static void Prev()
    {
        if (mHistoryStacks.Count == 0)
        {
            return;
        }

        mHistoryStacks.Pop();

        Guid historyId = mHistoryStacks.Peek();
        mHistories[historyId].RunWithPrompt();
    }

    internal static void Prev(int count)
    {
        if (count < 1)
        {
            ExceptionManager.ThrowFail<ArgumentException>("Count must be greater than 0.");
            return;
        }

        if (mHistoryStacks.Count == 0)
        {
            return;
        }

        for (int i = 0; i < count; ++i)
        {
            if (mHistoryStacks.Count <= 1)
            {
                break;
            }

            mHistoryStacks.Pop();
        }

        Guid historyId = mHistoryStacks.Peek();
        mHistories[historyId].RunWithPrompt();
    }

    internal static void GoHome()
    {
        if (mHistoryStacks.Count == 0)
        {
            return;
        }

        Guid historyId = mHistoryStacks.First();
        mHistoryStacks.Clear();
        Run(mHistories[historyId]);
    }
}
