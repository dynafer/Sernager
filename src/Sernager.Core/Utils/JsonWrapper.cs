using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sernager.Core.Utils;

internal static class JsonWrapper
{
    internal static string Serialize(object obj, bool bIndented = false)
    {
        if (obj == null)
        {
            return string.Empty;
        }

        string json;

        json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            WriteIndented = bIndented,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        });

        return json;
    }

    internal static T? Deserialize<T>(string json)
    {
        if (!IsValid(json))
        {
            return default;
        }

        T? obj;

        try
        {
            obj = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });
        }
        catch
        {
            obj = default;
        }

        return obj;
    }

    internal static bool IsValid(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        if (json.StartsWith("{") && json.EndsWith("}")
            || json.StartsWith("[") && json.EndsWith("]"))
        {
            try
            {
                using (_ = JsonDocument.Parse(json))
                {
                    return true;
                }
            }
            catch (JsonException)
            {
                return false;
            }
        }

        return false;
    }
}
