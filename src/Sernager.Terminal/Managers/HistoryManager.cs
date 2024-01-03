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

    internal static void PrevTo(HistoryModel history)
    {
        if (mHistoryStacks.Count == 0)
        {
            return;
        }

        Guid prevHistoryId = mHistoryStacks.Pop();

        while (prevHistoryId != history.Id)
        {
            if (mHistoryStacks.Count == 0)
            {
                return;
            }

            prevHistoryId = mHistoryStacks.Pop();
        }

        mHistories[prevHistoryId].RunWithPrompt();
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
