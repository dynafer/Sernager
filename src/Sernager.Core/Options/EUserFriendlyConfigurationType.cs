using System.ComponentModel;

namespace Sernager.Core.Options;

public enum EUserFriendlyConfigurationType
{
    [Description("Yaml")]
    Yaml,
    [Description("Json")]
    Json,
}
