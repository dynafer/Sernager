using Sernager.Terminal.Attributes;

namespace Sernager.Terminal.Models;

internal sealed class BuilderModel
{
    [Arg(Name = "config", ShortName = "c", Value = "<config path>", Description = "Path to config file")]
    internal string? ConfigPath { get; set; } = null;
    [Arg(Name = "autosave", ShortName = "aus", Description = "Enable auto save when exit")]
    internal bool IsAutoSave { get; set; } = false;
}
