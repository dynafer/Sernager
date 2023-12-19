namespace Sernager.Terminal.Prompts.Factories;

internal sealed class Pagination
{
    internal int PageSize { get; set; } = 5;
    internal int Total { get; set; } = 0;
    internal int Offset { get; private set; } = 0;

    internal (int, int, int, int) GetRange()
    {
        int halfPageSize = PageSize / 2;
        int start = Math.Max(0, Offset - halfPageSize);
        int end = Math.Min(Total, start + PageSize);

        int prevRest = 0;
        if (PageSize < Total && start == 0)
        {
            prevRest = halfPageSize - Offset;
            end -= prevRest;
        }

        int nextRest = 0;
        if (PageSize < Total && end > Total - halfPageSize)
        {
            nextRest = Math.Max(0, PageSize - (end - start));
        }

        return (start, end, prevRest, nextRest);
    }

    internal void Home()
    {
        Offset = 0;
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
