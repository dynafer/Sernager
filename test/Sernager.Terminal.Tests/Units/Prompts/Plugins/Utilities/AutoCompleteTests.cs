using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Plugins.Utilities;

namespace Sernager.Terminal.Tests.Units.Prompts.Plugins.Utilities;

internal sealed class AutoCompleteTests
{
    [DatapointSource]
    private readonly object[][] mItemsArrays;

    public AutoCompleteTests()
    {
        List<string> stringItems = new List<string>();
        List<OptionItem<string>> noneOptionItems = new List<OptionItem<string>>();
        List<OptionItem<string>> selectOptionItems = new List<OptionItem<string>>();
        List<OptionItem<string>> multiSelectOptionItems = new List<OptionItem<string>>();

        for (int i = 0; i < 20; ++i)
        {
            stringItems.Add($"item{i}");
            noneOptionItems.Add(new OptionItem<string>(EOptionTypeFlags.None, $"itemName{i}", $"itemValue{i}", false));
            selectOptionItems.Add(new OptionItem<string>(EOptionTypeFlags.Select, $"itemName{i}", $"itemValue{i}", false));
            multiSelectOptionItems.Add(new OptionItem<string>(EOptionTypeFlags.MultiSelect, $"itemName{i}", $"itemValue{i}", false));
        }

        mItemsArrays = [stringItems.ToArray(), noneOptionItems.ToArray(), selectOptionItems.ToArray(), multiSelectOptionItems.ToArray()];
    }

    [Test]
    public void Constructor_ShouldThrow_WhenTypeIsNotSearchable()
    {
        Assert.Throws<NotSupportedException>(() => new AutoComplete<object>());
        Assert.Throws<NotSupportedException>(() => new AutoComplete<int>());
        Assert.Throws<NotSupportedException>(() => new AutoComplete<bool>());
    }

    [Test]
    public void SetInitialInput_ShouldSetInputAndCursorPosition_WhenInputIsNotEmpty()
    {
        AutoComplete<string> autoComplete = new AutoComplete<string>();

        Assert.That(autoComplete.Input, Is.EqualTo(string.Empty));
        Assert.That(autoComplete.CursorPosition, Is.Zero);

        autoComplete.SetInitialInput("test");

        Assert.That(autoComplete.Input, Is.EqualTo("test"));
        Assert.That(autoComplete.CursorPosition, Is.EqualTo(4));
    }

    [Test]
    public void InterceptInput_ShouldAddChar_WhenKeyIsNotFunctionKey()
    {
        AutoComplete<string> autoComplete = new AutoComplete<string>();

        string appendedStr;
        addAlphabets(autoComplete, out appendedStr);

        Assert.That(autoComplete.Input, Is.EqualTo(appendedStr));
        Assert.That(autoComplete.CursorPosition, Is.EqualTo(appendedStr.Length));
    }

    [Test]
    public void InterceptInput_ShouldDeleteChar_WhenKeyIsOneOfDeletionKeys()
    {
        AutoComplete<string> autoComplete = new AutoComplete<string>();

        string appendedStr;
        addAlphabets(autoComplete, out appendedStr);

        for (int i = 0; i < appendedStr.Length; ++i)
        {
            autoComplete.InterceptInput(new ConsoleKeyInfo('\0', ConsoleKey.Backspace, false, false, false));
            appendedStr = appendedStr.Remove(appendedStr.Length - 1);

            Assert.That(autoComplete.Input, Is.EqualTo(appendedStr));
            Assert.That(autoComplete.CursorPosition, Is.EqualTo(appendedStr.Length));
        }

        PrivateUtil.SetPropertyValue(autoComplete, "CursorPosition", 0);

        for (int i = 0; i < appendedStr.Length; ++i)
        {
            autoComplete.InterceptInput(new ConsoleKeyInfo('\0', ConsoleKey.Delete, false, false, false));
            appendedStr = appendedStr.Remove(0, 1);

            Assert.That(autoComplete.Input, Is.EqualTo(appendedStr));
            Assert.That(autoComplete.CursorPosition, Is.Zero);
        }
    }

    [Test]
    public void InterceptInput_ShouldMoveCursor_WhenKeyIsOneOfHorizontalMovementKeys()
    {
        AutoComplete<string> autoComplete = new AutoComplete<string>();

        string appendedStr;
        addAlphabets(autoComplete, out appendedStr);

        for (int i = 0; i < appendedStr.Length; ++i)
        {
            autoComplete.InterceptInput(new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false));

            Assert.That(autoComplete.CursorPosition, Is.EqualTo(appendedStr.Length - i - 1));
        }

        for (int i = 0; i < appendedStr.Length; ++i)
        {
            autoComplete.InterceptInput(new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false));

            Assert.That(autoComplete.CursorPosition, Is.EqualTo(i + 1));
        }

        autoComplete.InterceptInput(new ConsoleKeyInfo('\0', ConsoleKey.Home, false, false, false));

        Assert.That(autoComplete.CursorPosition, Is.Zero);

        autoComplete.InterceptInput(new ConsoleKeyInfo('\0', ConsoleKey.End, false, false, false));

        Assert.That(autoComplete.CursorPosition, Is.EqualTo(appendedStr.Length));
    }

    [Theory]
    public void CompleteByTab_ShouldNotCompleteInput_WhenInputOrItemsAreEmptyOrItemsDoNotHaveSimilarPrefix(object[] itemsObj)
    {
        AutoComplete<string> stringAutoComplete = new AutoComplete<string>();
        AutoComplete<OptionItem<string>> optionAutoComplete = new AutoComplete<OptionItem<string>>();

        if (itemsObj is string[] strItems)
        {
            stringAutoComplete.CompleteByTab(strItems);

            Assert.That(stringAutoComplete.Input, Is.EqualTo(string.Empty));

            stringAutoComplete.CompleteByTab(Array.Empty<string>());

            Assert.That(stringAutoComplete.Input, Is.EqualTo(string.Empty));

            stringAutoComplete.SetInitialInput("test");
            stringAutoComplete.CompleteByTab(strItems);

            Assert.That(stringAutoComplete.Input, Is.EqualTo("test"));
            Assert.That(stringAutoComplete.CursorPosition, Is.EqualTo(4));
        }
        else if (itemsObj is OptionItem<string>[] optionItems)
        {
            optionAutoComplete.CompleteByTab(optionItems);

            Assert.That(optionAutoComplete.Input, Is.EqualTo(string.Empty));

            optionAutoComplete.CompleteByTab(Array.Empty<OptionItem<string>>());

            Assert.That(optionAutoComplete.Input, Is.EqualTo(string.Empty));

            optionAutoComplete.SetInitialInput("test");
            optionAutoComplete.CompleteByTab(optionItems);

            Assert.That(optionAutoComplete.Input, Is.EqualTo("test"));
            Assert.That(optionAutoComplete.CursorPosition, Is.EqualTo(4));
        }
    }

    [Theory]
    public void CompleteByTab_ShouldCompleteInput_WhenOneOfItemsHasSimilarPrefix(object[] itemsObj)
    {
        AutoComplete<string> stringAutoComplete = new AutoComplete<string>();
        AutoComplete<OptionItem<string>> optionAutoComplete = new AutoComplete<OptionItem<string>>();

        stringAutoComplete.SetInitialInput("it");
        optionAutoComplete.SetInitialInput("it");

        if (itemsObj is string[] strItems)
        {
            stringAutoComplete.CompleteByTab(strItems);
            string expected = strItems[0];

            Assert.That(stringAutoComplete.Input, Is.EqualTo(expected));
            Assert.That(stringAutoComplete.CursorPosition, Is.EqualTo(expected.Length));
        }
        else if (itemsObj is OptionItem<string>[] optionItems)
        {
            optionAutoComplete.CompleteByTab(optionItems);
            string expected = optionItems[0].Name;

            Assert.That(optionAutoComplete.Input, Is.EqualTo(expected));
            Assert.That(optionAutoComplete.CursorPosition, Is.EqualTo(expected.Length));
        }
    }

    [Theory]
    public void GetFirstSuggestionIndex_ShouldReturnSentinelValue_WhenInputOrItemsAreEmptyOrItemsDoNotHaveSimilarPrefix(object[] itemsObj)
    {
        AutoComplete<string> stringAutoComplete = new AutoComplete<string>();
        AutoComplete<OptionItem<string>> optionAutoComplete = new AutoComplete<OptionItem<string>>();

        int actualInputEmpty = -1;
        int actualItemsEmpty = -1;
        int actualItemsNoSimilarPrefix = -1;

        if (itemsObj is string[] strItems)
        {
            actualInputEmpty = stringAutoComplete.GetFirstSuggestionIndex(strItems);
            actualItemsEmpty = stringAutoComplete.GetFirstSuggestionIndex(Array.Empty<string>());

            stringAutoComplete.SetInitialInput("test");

            actualItemsNoSimilarPrefix = stringAutoComplete.GetFirstSuggestionIndex(strItems);
        }
        else if (itemsObj is OptionItem<string>[] optionItems)
        {
            actualInputEmpty = optionAutoComplete.GetFirstSuggestionIndex(optionItems);
            actualItemsEmpty = optionAutoComplete.GetFirstSuggestionIndex(Array.Empty<OptionItem<string>>());

            optionAutoComplete.SetInitialInput("test");

            actualItemsNoSimilarPrefix = optionAutoComplete.GetFirstSuggestionIndex(optionItems);
        }

        Assert.That(actualInputEmpty, Is.EqualTo(-1));
        Assert.That(actualItemsEmpty, Is.EqualTo(-1));
        Assert.That(actualItemsNoSimilarPrefix, Is.EqualTo(-1));
    }

    [Theory]
    public void GetFirstSuggestionIndex_ShouldReturnIndex_WhenOneOfItemsHasSimilarPrefix(object[] itemsObj)
    {
        AutoComplete<string> stringAutoComplete = new AutoComplete<string>();
        AutoComplete<OptionItem<string>> optionAutoComplete = new AutoComplete<OptionItem<string>>();

        stringAutoComplete.SetInitialInput("item5");
        optionAutoComplete.SetInitialInput("itemName5");

        int actual = -1;

        if (itemsObj is string[] strItems)
        {
            actual = stringAutoComplete.GetFirstSuggestionIndex(strItems);
        }
        else if (itemsObj is OptionItem<string>[] optionItems)
        {
            actual = optionAutoComplete.GetFirstSuggestionIndex(optionItems);
        }

        Assert.That(actual, Is.EqualTo(5));
    }

    [Test]
    public void GetSuggestionIndexes_ShouldReturnEmptyArray_WhenItemsAreEmpty()
    {
        AutoComplete<string> autoComplete = new AutoComplete<string>();

        int[] actual = autoComplete.GetSuggestionIndexes(Array.Empty<string>());

        Assert.That(actual, Is.Empty);
    }

    [Theory]
    public void GetSuggestionIndexes_ShouldReturnAllIndexes_WhenInputIsEmpty(object[] itemsObj)
    {
        AutoComplete<string> stringAutoComplete = new AutoComplete<string>();
        AutoComplete<OptionItem<string>> optionAutoComplete = new AutoComplete<OptionItem<string>>();

        int[] actual = Array.Empty<int>();
        int[] expected = new int[20];

        for (int i = 0; i < 20; ++i)
        {
            expected[i] = i;
        }

        if (itemsObj is string[] strItems)
        {
            actual = stringAutoComplete.GetSuggestionIndexes(strItems);
        }
        else if (itemsObj is OptionItem<string>[] optionItems)
        {
            actual = optionAutoComplete.GetSuggestionIndexes(optionItems);
        }

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Theory]
    public void GetSuggestionIndexes_ShouldReturnIndexes_WhenSomeItemsAreSimilarToInput(object[] itemsObj)
    {
        AutoComplete<string> stringAutoComplete = new AutoComplete<string>();
        AutoComplete<OptionItem<string>> optionAutoComplete = new AutoComplete<OptionItem<string>>();

        stringAutoComplete.SetInitialInput("0");
        optionAutoComplete.SetInitialInput("0");

        int[] actual = Array.Empty<int>();
        int[] expected = [0, 10];

        if (itemsObj is string[] strItems)
        {
            actual = stringAutoComplete.GetSuggestionIndexes(strItems);
        }
        else if (itemsObj is OptionItem<string>[] optionItems)
        {
            actual = optionAutoComplete.GetSuggestionIndexes(optionItems);
        }

        Assert.That(actual, Is.EqualTo(expected));
    }

    private void addAlphabets(AutoComplete<string> autoComplete, out string outStr)
    {
        string appendedStr = string.Empty;

        for (char sl = 'a', cl = 'A'; sl <= 'z' && cl <= 'Z'; ++sl, ++cl)
        {
            ConsoleKey consoleKey = Enum.Parse<ConsoleKey>(cl.ToString());
            autoComplete.InterceptInput(new ConsoleKeyInfo(sl, consoleKey, false, false, false));
            autoComplete.InterceptInput(new ConsoleKeyInfo(cl, consoleKey, false, false, false));
            appendedStr += sl.ToString() + cl.ToString();
        }

        outStr = appendedStr;
    }
}
