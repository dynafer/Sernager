using System.Globalization;

namespace Sernager.Resources;

public interface IResourcePack
{
    IResourcePack ChangeLanguage(CultureInfo culture);
    string GetString(string name);
}
