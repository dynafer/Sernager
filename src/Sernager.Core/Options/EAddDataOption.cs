using System.ComponentModel;

namespace Sernager.Core.Options;

public enum EAddDataOption
{
    [Description("Skip if exists")]
    SkipIfExists,
    [Description("Overwrite if exists")]
    OverwriteIfExists,
    [Description("Overwrite all (File-related commands only)")]
    OverwriteAll,
}
