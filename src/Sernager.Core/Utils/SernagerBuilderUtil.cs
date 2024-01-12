using Sernager.Core.Options;

namespace Sernager.Core.Utils;

public static class SernagerBuilderUtil
{
    public static EConfigurationType? GetConfigurationTypeOrNull(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return null;
        }

        string extension = Path.GetExtension(filePath).ToLowerInvariant();
        EConfigurationType? type = extension switch
        {
            ".yml" => EConfigurationType.Yaml,
            ".yaml" => EConfigurationType.Yaml,
            ".json" => EConfigurationType.Json,
            ".srn" => EConfigurationType.Sernager,
            _ => null,
        };

        return type;
    }
}
