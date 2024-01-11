using System.Globalization;
using System.Resources;

namespace Sernager.Resources;

internal sealed class ResourcePack : IResourcePack
{
    private ResourceManager mManager;
    private string mResourcePath;
    public string LangCode { get; private set; }

    internal ResourcePack(string resourcePath)
    {
        LangCode = CultureInfo.CurrentCulture.Name;
        mResourcePath = resourcePath;
        mManager = new ResourceManager($"{resourcePath}.{LangCode.ToLowerInvariant()}", typeof(ResourceRetriever).Assembly);
    }

    internal ResourcePack(CultureInfo culture, string resourcePath)
    {
        LangCode = culture.Name;
        mResourcePath = resourcePath;
        mManager = new ResourceManager($"{resourcePath}.{LangCode.ToLowerInvariant()}", typeof(ResourceRetriever).Assembly);
    }

    IResourcePack IResourcePack.ChangeLanguage(CultureInfo culture)
    {
        if (LangCode == culture.Name)
        {
            return this;
        }

        LangCode = culture.Name;
        mManager = new ResourceManager($"{mResourcePath}.{LangCode.ToLowerInvariant()}", typeof(ResourceRetriever).Assembly);

        return this;
    }

    string IResourcePack.GetString(string name)
    {
        try
        {
            return mManager.GetString(name) ?? name;
        }
        catch
        {
            return name;
        }
    }
}
