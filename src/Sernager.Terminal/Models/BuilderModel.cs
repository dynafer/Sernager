using Sernager.Core.Options;
using Sernager.Terminal.Attributes;

namespace Sernager.Terminal.Models;

internal sealed class BuilderModel
{
    [Arg(Name = "config", ShortName = "c", Value = "<config path>", DescriptionResourceName = "ConfigPathDescription")]
    internal string? ConfigPath { get; set; } = null;
    [Arg(Name = "autosave", ShortName = "aus", DescriptionResourceName = "IsAutoSaveDescription")]
    internal bool IsAutoSave { get; set; } = false;
    [Arg(Name = "errorLevel", ShortName = "el", DescriptionResourceName = "ExceptionLevelDescription")]
    internal EErrorLevel ExceptionLevel { get; set; } = EErrorLevel.None;
}
