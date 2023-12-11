using System.Text.Json;

namespace ServiceRunner.Runner.Utils;

internal static class JsonWrapper
{
    internal static string Serialize(object obj)
    {
        if (obj == null)
        {
            return string.Empty;
        }

        return JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            WriteIndented = false
        });
    }

    internal static T? Deserialize<T>(string json)
    {
        if (!IsValid(json))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            WriteIndented = false
        });
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
