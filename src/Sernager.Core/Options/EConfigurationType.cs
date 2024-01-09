using System.ComponentModel;

namespace Sernager.Core.Options;

public enum EConfigurationType
{
    [Description("Yaml")]
    Yaml,
    [Description("Json")]
    Json,
    [Description("Sernager")]
    Sernager,
}
