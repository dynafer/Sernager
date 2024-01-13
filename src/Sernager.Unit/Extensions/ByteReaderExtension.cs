using Sernager.Core.Utils;
using System.Reflection;

namespace Sernager.Unit.Extensions;

internal static class ByteReaderExtension
{
    public static void ChangePosition(this ByteReader reader, int position)
    {
        Type type = reader.GetType();
        PropertyInfo? property = type.GetProperty("Position", BindingFlags.NonPublic | BindingFlags.Instance);

        if (property == null)
        {
            throw new NullReferenceException("Property 'Position' is null.");
        }

        property.SetValue(reader, position);
    }
}
