using Sernager.Resources;
using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Helpers;

namespace Sernager.Terminal.Prompts.Factories;

internal sealed class AutoComplete<TSearchable>
    where TSearchable : notnull
{
    private string? mPrompt = null;
    private string? mNoResult = null;
    internal string Input { get; private set; } = string.Empty;
    internal int CursorPosition { get; private set; } = 0;

    internal AutoComplete()
    {
        checkSearchableType();
    }

    internal string GetPrompt()
    {
        if (mPrompt == null)
        {
            mPrompt = $"{ResourceRetriever.GetString("AutoComplete", "Prompt")}: ";
        }

        return mPrompt;
    }

    internal string GetNoResult()
    {
        if (mNoResult == null)
        {
            mNoResult = $"{ResourceRetriever.GetString("AutoComplete", "NoResult")}";
        }

        return mNoResult;
    }

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

    internal void CompleteByTab(IEnumerable<TSearchable> searchableItems)
    {
        TSearchable[] items = searchableItems.ToArray();

        if (items.Length == 0 || Input.Length == 0)
        {
            return;
        }

        int index = GetFirstSuggestionIndex(items);
        if (index == -1)
        {
            return;
        }

        Input = items[index].ToSuggestItem();
        CursorPosition = Input.Length;
    }

    internal int GetFirstSuggestionIndex(IEnumerable<TSearchable> searchableItems)
    {
        TSearchable[] items = searchableItems.ToArray();

        if (items.Length == 0 || Input.Length == 0)
        {
            return -1;
        }

        for (int i = 0; i < items.Length; ++i)
        {
            if (items[i].ToSuggestItem().StartsWith(Input, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return -1;
    }

    internal int[] GetSuggestionIndexes(IEnumerable<TSearchable> searchableItems)
    {
        TSearchable[] items = searchableItems.ToArray();

        if (Input.Length == 0)
        {
            return items.Select((_, i) => i).ToArray();
        }

        if (items.Length == 0)
        {
            return Array.Empty<int>();
        }

        IEnumerable<int> indexes = items
            .Where(item => item.ToSuggestItem().Contains(Input, StringComparison.OrdinalIgnoreCase))
            .Select(item => Array.IndexOf(items, item));

        return indexes.ToArray();
    }

    private void checkSearchableType()
    {
        if (typeof(TSearchable) == typeof(string))
        {
            return;
        }
        else if (typeof(TSearchable) == typeof(OptionItem))
        {
            return;
        }

        throw new InvalidCastException($"Cannot cast {nameof(TSearchable)} to {typeof(string)} or {typeof(OptionItem)}.");
    }

    private void addChar(ConsoleKeyInfo keyInfo)
    {
        if (!KeyHelper.IsCharKey(keyInfo.Key))
        {
            return;
        }

        Input = Input.Insert(CursorPosition, keyInfo.KeyChar.ToString());

        ++CursorPosition;
    }

    private void deleteChar(ConsoleKey key)
    {
        bool bForward = key == ConsoleKey.Delete;

        if (bForward)
        {
            if (CursorPosition < Input.Length)
            {
                Input = Input.Remove(CursorPosition, 1);
            }
        }
        else
        {
            if (CursorPosition > 0)
            {
                --CursorPosition;
                Input = Input.Remove(CursorPosition, 1);
            }
        }
    }

    private void moveCursor(ConsoleKey key)
    {
        bool bForward = key == ConsoleKey.RightArrow;

        if (bForward)
        {
            if (CursorPosition < Input.Length)
            {
                ++CursorPosition;
            }
        }
        else
        {
            if (CursorPosition > 0)
            {
                --CursorPosition;
            }
        }
    }
}
