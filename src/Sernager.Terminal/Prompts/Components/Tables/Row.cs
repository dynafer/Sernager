using Sernager.Terminal.Prompts.Extensions.Components;

namespace Sernager.Terminal.Prompts.Components.Tables;

internal sealed class Row
{
    internal const int PADDING_TWO_SIDES = 2;
    internal readonly List<Column> Columns = new List<Column>();
    internal readonly List<int> Widths = new List<int>();
    internal readonly List<bool> RowGrid = new List<bool>();
    internal int TotalColumns { get; set; } = 0;

    internal Row()
    {
    }

    internal Row(params Column[] columns)
    {
        AddColumns(columns);
    }

    internal Row(params TextComponent[] columns)
    {
        AddColumns(columns);
    }

    internal Row(params string[] columns)
    {
        AddColumns(columns);
    }

    internal Row AddColumn(Column column)
    {
        TotalColumns += column.Colspan;

        Widths.Add(column.Length + PADDING_TWO_SIDES);
        RowGrid.Add(true);

        if (column.Colspan > 1)
        {
            for (int i = 1; i < column.Colspan; ++i)
            {
                Widths.Add(0);
                RowGrid.Add(false);
            }
        }

        Columns.Add(column);

        return this;
    }

    internal Row AddColumn(TextComponent column)
    {
        AddColumn(new Column(column));

        return this;
    }

    internal Row AddColumn(string column)
    {
        AddColumn(new Column(new TextComponent().SetText(column)));

        return this;
    }

    internal Row AddColumns(params Column[] columns)
    {
        foreach (Column column in columns)
        {
            AddColumn(column);
        }

        return this;
    }

    internal Row AddColumns(params TextComponent[] columns)
    {
        foreach (TextComponent column in columns)
        {
            AddColumn(column);
        }

        return this;
    }

    internal Row AddColumns(params string[] columns)
    {
        foreach (string column in columns)
        {
            AddColumn(column);
        }

        return this;
    }
}
