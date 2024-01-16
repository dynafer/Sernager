using System.ComponentModel;
using System.Reflection;

namespace Sernager.Core.Extensions;

public static class EnumExtension
{
    public static string GetDescription(this Enum e)
    {
        Type type = e.GetType();
        string? name = Enum.GetName(type, e);

        if (name == null)
        {
            return string.Empty;
        }

        FieldInfo field = type.GetField(name)!;

        DescriptionAttribute? attr = field.GetCustomAttribute<DescriptionAttribute>();

        if (attr == null)
        {
            return string.Empty;
        }

        return attr.Description;
    }
}
