using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Tables;

namespace Sernager.Terminal.Prompts.Extensions.Components;

internal static class TableComponentExtension
{
    internal static TableComponent AddRows(this TableComponent component, params Row[] rows)
    {
        component.Rows.AddRange(rows);

        return component;
    }

    internal static TableComponent AddRows(this TableComponent component, params Column[][] rows)
    {
        foreach (Column[] row in rows)
        {
            Row tableRow = new Row(row);

            component.Rows.Add(tableRow);
        }

        return component;
    }

    internal static TableComponent AddRows(this TableComponent component, params TextComponent[][] rows)
    {
        foreach (TextComponent[] row in rows)
        {
            Row tableRow = new Row(row);

            component.Rows.Add(tableRow);
        }

        return component;
    }

    internal static TableComponent AddRows(this TableComponent component, params string[][] rows)
    {
        foreach (string[] row in rows)
        {
            Row tableRow = new Row(row);

            component.Rows.Add(tableRow);
        }

        return component;
    }
}
