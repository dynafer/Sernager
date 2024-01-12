using Sernager.Resources;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Helpers;

namespace Sernager.Terminal.Prompts.Plugins.Utilities;

internal sealed class AutoComplete<TSearchable>
    where TSearchable : notnull
{
    private readonly IResourcePack mResourcePack = ResourceRetriever.UsePack("Terminal.Prompt.AutoComplete");
    internal string Prompt => $"{mResourcePack.GetString("Prompt")}: ";
    internal string NoResultText => mResourcePack.GetString("NoResult");
    internal string Input { get; set; } = string.Empty;
    internal int CursorPosition { get; private set; } = 0;

    internal AutoComplete()
    {
        TypeHelper.EnsureIsSearchable<TSearchable>();
    }

    internal void SetInitialInput(string input)
    {
        Input = input;
        CursorPosition = input.Length;
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

        if (items.Length == 0)
        {
            return Array.Empty<int>();
        }

        if (Input.Length == 0)
        {
            return items.Select((_, i) => i).ToArray();
        }

        IEnumerable<int> indexes = items
            .Where(item => item.ToSuggestItem().Contains(Input, StringComparison.OrdinalIgnoreCase))
            .Select(item => Array.IndexOf(items, item));

        return indexes.ToArray();
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
