using Sernager.Terminal.Prompts.Components.Tables;
using System.Text;

namespace Sernager.Terminal.Prompts.Components;

internal sealed class TableComponent : IPromptComponent
{
    internal readonly List<Row> Rows = new List<Row>();
    private bool mUseBorder = false;
    bool IPromptComponent.IsLineBreak => true;

    internal TableComponent UseBorder()
    {
        mUseBorder = true;

        return this;
    }

    string IPromptComponent.Render()
    {
        if (Rows.Count == 0)
        {
            return string.Empty;
        }

        int[] widths = measureWidths();
        bool[][] grid = createTableGrid();

        StringBuilder builder = new StringBuilder();

        addMostHorizontalEdgeBorder(builder, widths, true);

        for (int i = 0; i < Rows.Count; ++i)
        {
            if (i != 0)
            {
                addMiddleHorizontalEdgeBorder(builder, widths, grid, i);
            }

            int index = 0;
            foreach (Column column in Rows[i].Columns)
            {
                int padding = widths[index] - column.Length;
                for (int j = 1; j < column.Colspan; ++j)
                {
                    padding += widths[index + j] + (mUseBorder ? 1 : 0);
                }

                int rest = padding % 2;
                padding /= 2;

                if (mUseBorder)
                {
                    builder.Append('│');
                }
                else if (column.Colspan > 1)
                {
                    builder.Append(' ');
                }

                builder.Append(' ', padding);
                builder.Append(column.Render());
                builder.Append(' ', padding + rest);

                index += column.Colspan;
                if (index >= widths.Length)
                {
                    break;
                }
            }

            if (mUseBorder)
            {
                builder.Append('│');
            }

            builder.AppendLine();
        }

        addMostHorizontalEdgeBorder(builder, widths, false);

        return builder.ToString();
    }

    private void addMiddleHorizontalEdgeBorder(StringBuilder builder, int[] widths, bool[][] grid, int curRowIndex)
    {
        if (!mUseBorder)
        {
            return;
        }

        builder.Append("├");

        int index = 0;
        foreach (Column column in Rows[curRowIndex].Columns)
        {
            for (int j = 0; j < column.Colspan; ++j)
            {
                builder.Append('─', widths[index + j]);
                if (index + j < widths.Length - 1)
                {
                    builder.Append(selectMiddlePartition(grid, curRowIndex, index + j, column.Colspan - j - 1));
                }
            }

            index += column.Colspan;
            if (index >= widths.Length)
            {
                break;
            }
        }

        builder.Append("┤");
        builder.AppendLine();
    }

    private char selectMiddlePartition(bool[][] grid, int curRowIndex, int curColIndex, int remainingColspan = 0)
    {
        bool[] prevRow = grid[curRowIndex - 1];

        if (remainingColspan > 0)
        {
            return prevRow[curColIndex] ? '┴' : '─';
        }
        else
        {
            return curColIndex + 1 < prevRow.Length && prevRow[curColIndex + 1] ? '┼' : '┬';
        }
    }

    private void addMostHorizontalEdgeBorder(StringBuilder builder, int[] widths, bool bTop)
    {
        if (!mUseBorder)
        {
            return;
        }

        Row row = bTop ? Rows[0] : Rows[^1];
        string firstChar = bTop ? "┌" : "└";
        string middleChar = bTop ? "┬" : "┴";
        string lastChar = bTop ? "┐" : "┘";

        builder.Append(firstChar);

        int index = 0;
        foreach (Column column in row.Columns)
        {
            int width = widths[index];
            for (int j = 1; j < column.Colspan; ++j)
            {
                width += widths[index + j] + 1;
            }

            builder.Append('─', width);

            index += column.Colspan;
            if (index < widths.Length)
            {
                builder.Append(middleChar);
            }

            if (index >= widths.Length)
            {
                break;
            }
        }

        builder.Append(lastChar);
        if (bTop)
        {
            builder.AppendLine();
        }
    }

    private bool[][] createTableGrid()
    {
        bool[][] grid = new bool[Rows.Count][];

        for (int i = 0; i < Rows.Count; ++i)
        {
            grid[i] = Rows[i].RowGrid.ToArray();
        }

        return grid;
    }

    private int[] measureWidths()
    {
        int[] widths = new int[Rows[0].Widths.Count];

        Rows[0].Widths.CopyTo(widths);

        for (int i = 1; i < Rows.Count; ++i)
        {
            Row row = Rows[i];

            for (int j = 0; j < row.Widths.Count; ++j)
            {
                if (!row.RowGrid[j])
                {
                    continue;
                }

                int colspanWidth = widths[j];
                int colspan = 1;
                for (int k = j + 1; k < row.Widths.Count; ++k)
                {
                    if (row.RowGrid[k])
                    {
                        break;
                    }

                    colspanWidth += widths[k];
                    ++colspan;
                }

                if (colspan == 1)
                {
                    widths[j] = Math.Max(widths[j], row.Widths[j]);
                    continue;
                }

                if (colspanWidth < row.Widths[j])
                {
                    int restWidth = row.Widths[j] - colspanWidth;
                    restWidth = (int)Math.Ceiling((float)(restWidth / colspan)) + (restWidth % 2);

                    for (int k = j; k < j + colspan; ++k)
                    {
                        widths[k] += restWidth;
                    }
                }
            }
        }

        return widths.Where(width => width > 0).ToArray();
    }
}
