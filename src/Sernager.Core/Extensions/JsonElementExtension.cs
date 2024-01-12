using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Sernager.Core.Extensions;

internal static class JsonElementExtension
{
    internal static bool TryGetString(this JsonElement jsonElement, [NotNullWhen(true)] out string? result)
    {
        try
        {
            result = jsonElement.GetString() ?? string.Empty;
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    internal static bool TryGetStringArray(this JsonElement jsonElement, [NotNullWhen(true)] out string[]? result)
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
            result = null;
            return false;
        }
    }
}
