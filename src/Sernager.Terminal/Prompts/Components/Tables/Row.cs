using Sernager.Terminal.Prompts.Extensions.Components.Tables;

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
        this.AddColumns(columns);
    }

    internal Row(params TextComponent[] columns)
    {
        this.AddColumns(columns);
    }

    internal Row(params string[] columns)
    {
        this.AddColumns(columns);
    }
}
