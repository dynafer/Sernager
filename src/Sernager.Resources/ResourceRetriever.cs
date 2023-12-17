using System.Globalization;
using System.Resources;

namespace Sernager.Resources;

public static class ResourceRetriever
{
    private static readonly string BASE_NAMESPACE = "Sernager.Resources";
    public static string LangCode { get; set; } = CultureInfo.CurrentCulture.Name;

    public static string GetString(string resourcePath, string name)
    {
        try
        {
            ResourceManager manager = new ResourceManager($"{BASE_NAMESPACE}.{resourcePath}.{LangCode.ToLowerInvariant()}", typeof(ResourceRetriever).Assembly);

            return manager.GetString(name) ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    public static string GetString(CultureInfo culture, string resourcePath, string name)
    {
        try
        {
            ResourceManager manager = new ResourceManager($"{BASE_NAMESPACE}.{resourcePath}.{culture.Name.ToLowerInvariant()}", typeof(ResourceRetriever).Assembly);

            return manager.GetString(name) ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}
