using System.Reflection;

namespace Sernager.Unit.Utils;

public static class PrivateUtil
{
    public static TValue? GetFieldValue<TClass, TValue>(string fieldName)
    {
        Type type = typeof(TClass);
        FieldInfo? field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        if (field == null)
        {
            throw new InvalidOperationException($"{nameof(FieldInfo)} '{fieldName}' is null.");
        }

        object? value = field.GetValue(null);

        if (value == null)
        {
            return default;
        }

        return (TValue)value;
    }

    public static TValue? GetFieldValue<TValue>(Type classType, string fieldName)
    {
        FieldInfo? field = classType.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        if (field == null)
        {
            throw new InvalidOperationException($"{nameof(FieldInfo)} '{fieldName}' is null.");
        }

        object? value = field.GetValue(null);

        if (value == null)
        {
            return default;
        }

        return (TValue)value;
    }

    public static TValue? GetFieldValue<TValue>(object obj, string fieldName)
    {
        Type type = obj.GetType();
        FieldInfo? field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null)
        {
            throw new InvalidOperationException($"{nameof(FieldInfo)} '{fieldName}' is null.");
        }

        object? value = field.GetValue(obj);

        if (value == null)
        {
            return default;
        }

        return (TValue)value;
    }

    public static TValue? GetPropertyValue<TClass, TValue>(string propertyName)
    {
        Type type = typeof(TClass);
        PropertyInfo? property = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.NonPublic);
        if (property == null)
        {
            throw new InvalidOperationException($"{nameof(PropertyInfo)} '{propertyName}' is null.");
        }

        object? value = property.GetValue(null);

        if (value == null)
        {
            return default;
        }

        return (TValue)value;
    }

    public static TValue? GetPropertyValue<TValue>(Type classType, string propertyName)
    {
        PropertyInfo? property = classType.GetProperty(propertyName, BindingFlags.Static | BindingFlags.NonPublic);
        if (property == null)
        {
            throw new InvalidOperationException($"{nameof(PropertyInfo)} '{propertyName}' is null.");
        }

        object? value = property.GetValue(null);

        if (value == null)
        {
            return default;
        }

        return (TValue)value;
    }

    public static TValue? GetPropertyValue<TValue>(object obj, string propertyName)
    {
        Type type = obj.GetType();
        PropertyInfo? property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (property == null)
        {
            throw new InvalidOperationException($"{nameof(PropertyInfo)} '{propertyName}' is null.");
        }

        object? value = property.GetValue(obj);

        if (value == null)
        {
            return default;
        }

        return (TValue)value;
    }
}
