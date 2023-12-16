using System.Resources;

namespace Sernager.Resources;

public class ResourceRetriever : IDisposable
{
    private static readonly string BASE_NAMESPACE = "Sernager.Resources";
    private readonly string mLangCode = "EN";

    public ResourceRetriever(string langCode)
    {
        mLangCode = langCode;
    }

    public void Dispose()
    {
    }

    public string GetString(string resourcePath)
    {
        try
        {
            ResourceManager manager = new ResourceManager($"{BASE_NAMESPACE}.{resourcePath}", typeof(ResourceRetriever).Assembly);

            return manager.GetString(mLangCode) ?? string.Empty;
        }
        catch (MissingManifestResourceException)
        {
            return string.Empty;
        }
    }
}
