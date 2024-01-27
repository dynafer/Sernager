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

    public static TValue? GetMethodResult<TClass, TValue>(string methodName, params object[] parameters)
    {
        Type type = typeof(TClass);
        MethodInfo? method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
        if (method == null)
        {
            throw new InvalidOperationException($"{nameof(MethodInfo)} '{methodName}' is null.");
        }

        object? value = method.Invoke(null, parameters);

        if (value == null)
        {
            return default;
        }

        return (TValue)value;
    }

    public static TValue? GetMethodResult<TValue>(Type classType, string methodName, params object[] parameters)
    {
        MethodInfo? method = classType.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
        if (method == null)
        {
            throw new InvalidOperationException($"{nameof(MethodInfo)} '{methodName}' is null.");
        }

        object? value = method.Invoke(null, parameters);

        if (value == null)
        {
            return default;
        }

        return (TValue)value;
    }

    public static TValue? GetMethodResult<TValue>(object obj, string methodName, params object[] parameters)
    {
        Type type = obj.GetType();
        MethodInfo? method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (method == null)
        {
            throw new InvalidOperationException($"{nameof(MethodInfo)} '{methodName}' is null.");
        }

        object? value = method.Invoke(obj, parameters);

        if (value == null)
        {
            return default;
        }

        return (TValue)value;
    }

    public static void SetFieldValue<TClass, TValue>(string fieldName, TValue value)
    {
        Type type = typeof(TClass);
        FieldInfo? field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        if (field == null)
        {
            throw new InvalidOperationException($"{nameof(FieldInfo)} '{fieldName}' is null.");
        }

        field.SetValue(null, value);
    }

    public static void SetFieldValue<TValue>(Type classType, string fieldName, TValue value)
    {
        FieldInfo? field = classType.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        if (field == null)
        {
            throw new InvalidOperationException($"{nameof(FieldInfo)} '{fieldName}' is null.");
        }

        field.SetValue(null, value);
    }

    public static void SetFieldValue<TValue>(object obj, string fieldName, TValue value)
    {
        Type type = obj.GetType();
        FieldInfo? field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null)
        {
            throw new InvalidOperationException($"{nameof(FieldInfo)} '{fieldName}' is null.");
        }

        field.SetValue(obj, value);
    }

    public static void SetPropertyValue<TClass, TValue>(string propertyName, TValue value)
    {
        Type type = typeof(TClass);
        PropertyInfo? property = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.NonPublic);
        if (property == null)
        {
            throw new InvalidOperationException($"{nameof(PropertyInfo)} '{propertyName}' is null.");
        }

        property.SetValue(null, value);
    }

    public static void SetPropertyValue<TValue>(Type classType, string propertyName, TValue value)
    {
        PropertyInfo? property = classType.GetProperty(propertyName, BindingFlags.Static | BindingFlags.NonPublic);
        if (property == null)
        {
            throw new InvalidOperationException($"{nameof(PropertyInfo)} '{propertyName}' is null.");
        }

        property.SetValue(null, value);
    }

    public static void SetPropertyValue<TValue>(object obj, string propertyName, TValue value)
    {
        Type type = obj.GetType();
        PropertyInfo? property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (property == null)
        {
            throw new InvalidOperationException($"{nameof(PropertyInfo)} '{propertyName}' is null.");
        }

        property.SetValue(obj, value);
    }
}
