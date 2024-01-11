using System.Globalization;

namespace Sernager.Resources;

public interface IResourcePack
{
    string LangCode { get; }
    IResourcePack ChangeLanguage(CultureInfo culture);
    string GetString(string name);
}
