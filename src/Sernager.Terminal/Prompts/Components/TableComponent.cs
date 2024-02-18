using Sernager.Terminal.Prompts.Components.Tables;
using System.Text;

namespace Sernager.Terminal.Prompts.Components;

internal sealed class TableComponent : IPromptComponent
{
    internal readonly List<Row> Rows = new List<Row>();
    private bool mbUseBorder = false;
    private bool mbUseHeader = false;
    bool IPromptComponent.IsLineBreak => true;

    internal TableComponent UseBorder()
    {
        mbUseBorder = true;

        return this;
    }

    internal TableComponent UseHeader()
    {
        mbUseHeader = true;

        return this;
    }

    string IPromptComponent.Render()
    {
        if (Rows.Count == 0)
        {
            return string.Empty;
        }

        ensureRowCount();

        int[] widths = measureWidths();

        StringBuilder builder = new StringBuilder();

        addMostHorizontalEdgeBorder(builder, widths, true);

        for (int i = 0; i < Rows.Count; ++i)
        {
            if (i != 0)
            {
                addMiddleHorizontalEdgeBorder(builder, widths, i);
            }

            bool bHeaderRow = mbUseHeader && i == 0;
            int index = 0;

            foreach (Column column in Rows[i].Columns)
            {
                int padding = widths[index] - column.Length;
                for (int j = 1; j < column.Colspan; ++j)
                {
                    padding += widths[index + j] + (mbUseBorder ? 1 : 0);
                }

                int rest = padding % 2;
                padding /= 2;

                if (mbUseBorder)
                {
                    builder.Append('│');
                }

                builder.Append(' ', bHeaderRow
                    ? padding
                    : 1);
                builder.Append(column.Render());
                builder.Append(' ', bHeaderRow
                    ? (padding + rest)
                    : (padding * 2 + rest - 1));

                index += column.Colspan;
                if (index >= widths.Length)
                {
                    break;
                }
            }

            if (mbUseBorder)
            {
                builder.Append('│');
            }

            builder.AppendLine();
        }

        addMostHorizontalEdgeBorder(builder, widths, false);

        return builder.ToString();
    }

    private void ensureRowCount()
    {
        int columnCount = Rows[0].TotalColumns;

        for (int i = 1; i < Rows.Count; ++i)
        {
            if (Rows[i].TotalColumns != columnCount)
            {
                throw new InvalidOperationException("All rows must have the same number of columns or colspan.");
            }
        }
    }

    private void addMostHorizontalEdgeBorder(StringBuilder builder, int[] widths, bool bTop)
    {
        if (!mbUseBorder)
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
            for (int i = 1; i < column.Colspan; ++i)
            {
                width += widths[index + i] + 1;
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

    private void addMiddleHorizontalEdgeBorder(StringBuilder builder, int[] widths, int curRowIndex)
    {
        if (!mbUseBorder)
        {
            return;
        }

        builder.Append("├");

        int index = 0;
        foreach (Column column in Rows[curRowIndex].Columns)
        {
            for (int i = 0; i < column.Colspan; ++i)
            {
                builder.Append('─', widths[index + i]);
                if (index + i < widths.Length - 1)
                {
                    builder.Append(selectMiddlePartition(curRowIndex, index + i, column.Colspan - i - 1));
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

    private char selectMiddlePartition(int curRowIndex, int curColIndex, int remainingColspan = 0)
    {
        List<bool> prevRowGrid = Rows[curRowIndex - 1].RowGrid;

        if (remainingColspan > 0)
        {
            return prevRowGrid[curColIndex] ? '┴' : '─';
        }
        else
        {
            return curColIndex + 1 < prevRowGrid.Count && prevRowGrid[curColIndex + 1] ? '┼' : '┬';
        }
    }

    private int[] measureWidths()
    {
        int[] widths = new int[Rows[0].Widths.Count];

        foreach (Row row in Rows)
        {
            for (int i = 0; i < row.Widths.Count; ++i)
            {
                if (!row.RowGrid[i])
                {
                    continue;
                }

                int colspanWidth = widths[i];
                int colspan = 1;
                for (int j = i + 1; j < row.Widths.Count && !row.RowGrid[j]; ++j)
                {
                    colspanWidth += widths[j];
                    ++colspan;
                }

                if (colspan == 1)
                {
                    widths[i] = Math.Max(widths[i], row.Widths[i]);
                }
                else if (colspanWidth < row.Widths[i])
                {
                    int averageWidth = row.Widths[i] - colspanWidth;
                    averageWidth = (int)Math.Ceiling((float)(averageWidth / colspan)) + (averageWidth % 2);

                    for (int j = i; j < i + colspan; ++j)
                    {
                        widths[j] += averageWidth;
                    }
                }
            }
        }

        return widths.Where(width => width > 0).ToArray();
    }
}
