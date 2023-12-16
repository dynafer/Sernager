namespace Sernager.Terminal.Prompts;

internal sealed class PromptPagination
{
    internal int PageSize { get; set; } = 5;
    internal int Total { get; set; } = 0;
    internal int Offset { get; set; } = 0;

    internal (int, int, int) GetRange()
    {
        int start = Offset - PageSize / 2;
        if (start < 0)
        {
            start = 0;
        }

        int end = start + PageSize;
        if (end > Total)
        {
            end = Total;
        }

        int rest = 0;
        if (Total > PageSize)
        {
            rest = PageSize - (end - start);
        }

        return (start, end, rest);
    }

    internal void Prev()
    {
        if (Offset > 0)
        {
            --Offset;
        }
        else
        {
            Offset = Total - 1;
        }
    }

    internal void Next()
    {
        if (Offset < Total - 1)
        {
            ++Offset;
        }
        else
        {
            Offset = 0;
        }
    }
}
