using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Tables;

namespace Sernager.Terminal.Prompts.Extensions.Components.Tables;

internal static class RowExtension
{
    internal static Row AddColumn(this Row row, Column column)
    {
        row.TotalColumns += column.Colspan;

        row.Widths.Add(column.Length + Row.PaddingOfTwoSides);
        row.RowGrid.Add(true);

        if (column.Colspan > 1)
        {
            for (int i = 1; i < column.Colspan; ++i)
            {
                row.Widths.Add(0);
                row.RowGrid.Add(false);
            }
        }

        row.Columns.Add(column);

        return row;
    }

    internal static Row AddColumn(this Row row, TextComponent column)
    {
        row.AddColumn(new Column(column));

        return row;
    }

    internal static Row AddColumn(this Row row, string column)
    {
        row.AddColumn(new Column(new TextComponent().SetText(column)));

        return row;
    }

    internal static Row AddColumns(this Row row, params Column[] columns)
    {
        foreach (Column column in columns)
        {
            row.AddColumn(column);
        }

        return row;
    }

    internal static Row AddColumns(this Row row, params TextComponent[] columns)
    {
        foreach (TextComponent column in columns)
        {
            row.AddColumn(column);
        }

        return row;
    }

    internal static Row AddColumns(this Row row, params string[] columns)
    {
        foreach (string column in columns)
        {
            row.AddColumn(column);
        }

        return row;
    }
}
