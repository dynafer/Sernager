using System.Diagnostics.CodeAnalysis;

namespace Sernager.Core.Managers;

public static class CacheManager
{
    private static readonly Dictionary<string, object> mCache = new Dictionary<string, object>();

    public static bool TryGet<T>(string key, [NotNullWhen(true)] out T? value)
        where T : class
    {
        value = null;

        if (!mCache.ContainsKey(key))
        {
            return false;
        }

        object? obj = mCache[key];

        if (obj is not T)
        {
            ExceptionManager.ThrowFail<InvalidCastException>($"Cannot cast {obj.GetType().Name} to {typeof(T).Name}");
            return false;
        }

        value = (T)obj;
        return true;
    }

    public static void Set<T>(string key, T value)
        where T : class
    {
        mCache[key] = value;
    }

    public static void Remove(string key)
    {
        mCache.Remove(key);
    }

    public static void Clear()
    {
        mCache.Clear();
    }
}
