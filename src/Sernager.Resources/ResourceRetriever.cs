using System.Globalization;
using System.Resources;

namespace Sernager.Resources;

public static class ResourceRetriever
{
    private static readonly string BASE_NAMESPACE = "Sernager.Resources";
    private static readonly Dictionary<string, IResourcePack> mResourcePacks = new Dictionary<string, IResourcePack>();
    public static string LangCode { get; set; } = CultureInfo.CurrentCulture.Name;

    public static IResourcePack UsePack(string resourcePath)
    {
        if (!mResourcePacks.ContainsKey(resourcePath))
        {
            mResourcePacks.Add(resourcePath, new ResourcePack($"{BASE_NAMESPACE}.{resourcePath}"));
        }

        return mResourcePacks[resourcePath];
    }

    public static IResourcePack UsePack(CultureInfo culture, string resourcePath)
    {
        if (!mResourcePacks.ContainsKey(resourcePath))
        {
            mResourcePacks.Add(resourcePath, new ResourcePack(culture, $"{BASE_NAMESPACE}.{resourcePath}"));
        }

        return mResourcePacks[resourcePath].ChangeLanguage(culture);
    }

    public static string GetString(string resourcePath, string name)
    {
        try
        {
            ResourceManager manager = new ResourceManager($"{BASE_NAMESPACE}.{resourcePath}.{LangCode.ToLowerInvariant()}", typeof(ResourceRetriever).Assembly);

            return manager.GetString(name) ?? name;
        }
        catch
        {
            return name;
        }
    }

    public static string GetString(CultureInfo culture, string resourcePath, string name)
    {
        try
        {
            ResourceManager manager = new ResourceManager($"{BASE_NAMESPACE}.{resourcePath}.{culture.Name.ToLowerInvariant()}", typeof(ResourceRetriever).Assembly);

            return manager.GetString(name) ?? name;
        }
        catch
        {
            return name;
        }
    }
}
