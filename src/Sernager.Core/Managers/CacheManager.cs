namespace Sernager.Core.Managers;

internal static class CacheManager
{
    private static readonly Dictionary<string, object> mCache = new Dictionary<string, object>();

    internal static bool TryGet<T>(string key, out T value)
        where T : class
    {
        value = null!;

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

    internal static void Set<T>(string key, T value)
        where T : class
    {
        mCache[key] = value;
    }

    internal static void Remove(string key)
    {
        mCache.Remove(key);
    }

    internal static void Clear()
    {
        mCache.Clear();
    }
}
