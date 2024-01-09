namespace Sernager.Terminal.Prompts.Plugins.Utilities;

internal sealed class Pagination
{
    internal int PageSize { get; set; } = 5;
    internal int Total { get; set; } = 0;
    internal int Offset { get; private set; } = 0;

    internal (int, int, int, int) GetRange()
    {
        if (PageSize >= Total)
        {
            return (0, Total, 0, 0);
        }

        int halfPageSize = (int)Math.Floor((float)(PageSize / 2));
        int start = Math.Max(0, Offset - halfPageSize);
        int end = Math.Min(Total, Offset + halfPageSize);
        int prev = 0;
        int next = 0;

        if (Offset - halfPageSize < 0)
        {
            prev = halfPageSize - Offset;
        }

        if (Offset + halfPageSize > Total)
        {
            next = Offset + halfPageSize - Total;
        }

        return (start, end, prev, next);
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
