using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sernager.Resources.Tests")]
[assembly: InternalsVisibleTo("Sernager.Unit")]
namespace Sernager.Resources;

public static class ResourceRetriever
{
    private static readonly string BASE_NAMESPACE = "Sernager.Resources";
    private static readonly Dictionary<string, IResourcePack> mResourcePacks;
    public static IResourcePack Shared { get; }
    public static string LangCode { get; private set; }

    static ResourceRetriever()
    {
        mResourcePacks = new Dictionary<string, IResourcePack>();
        LangCode = CultureInfo.CurrentCulture.Name;
        Shared = UsePack("Shared");
    }

    public static void ChangeLanguage(CultureInfo culture)
    {
        if (LangCode == culture.Name)
        {
            return;
        }

        LangCode = culture.Name;

        foreach (IResourcePack pack in mResourcePacks.Values)
        {
            pack.ChangeLanguage(culture);
        }
    }

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
        return getString(LangCode, resourcePath, name);
    }

    public static string GetString(CultureInfo culture, string resourcePath, string name)
    {
        return getString(culture.Name, resourcePath, name);
    }

    public static string FormatString(string resourcePath, string name, params object[] args)
    {
        string str = getString(LangCode, resourcePath, name);

        return string.Format(str, args);
    }

    public static string FormatString(CultureInfo culture, string resourcePath, string name, params object[] args)
    {
        string str = getString(culture.Name, resourcePath, name);

        return string.Format(str, args);
    }

    private static string getString(string langCode, string resourcePath, string name)
    {
        try
        {
            ResourceManager manager = createResourceManager(langCode, resourcePath);

            return manager.GetString(name) ?? name;
        }
        catch
        {
            return name;
        }
    }

    private static ResourceManager createResourceManager(string langCode, string resourcePath)
    {
        return new ResourceManager($"{BASE_NAMESPACE}.{resourcePath}.{langCode.ToLowerInvariant()}", typeof(ResourceRetriever).Assembly);
    }
}
