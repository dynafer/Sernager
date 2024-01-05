using System.Text.Json;

namespace Sernager.Core.Extensions;

internal static class JsonElementExtension
{
    internal static bool TryGetString(this JsonElement jsonElement, out string result)
    {
        try
        {
            result = jsonElement.GetString() ?? string.Empty;
            return true;
        }
        catch
        {
            result = null!;
            return false;
        }
    }

    internal static bool TryGetStringArray(this JsonElement jsonElement, out string[] result)
    {
        try
        {
            result = jsonElement.EnumerateArray()
                .Select(x => x.GetString())
                .Where(x => x != null)
                .Select(x => x!)
                .ToArray();

            return true;
        }
        catch
        {
            result = null!;
            return false;
        }
    }
}
