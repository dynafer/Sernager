using Sernager.Core.Options;
using Sernager.Terminal.Attributes;

namespace Sernager.Terminal.Models;

internal sealed class BuilderModel
{
    [Arg(Name = "config", ShortName = "c", Value = "<config path>", DescriptionResourceName = "ConfigPathDescription")]
    internal string? ConfigPath { get; set; } = null;
    [Arg(Name = "autoSave", ShortName = "asav", DescriptionResourceName = "AutoSaveDescription")]
    internal EConfigurationType? AutoSaveType { get; set; } = null;
    [Arg(Name = "autoSaveUF", ShortName = "asavuf", DescriptionResourceName = "AutoSaveUserFriendlyTypeDescription")]
    internal EUserFriendlyConfigurationType? AutoSaveUserFriendlyType { get; set; } = null;
    [Arg(Name = "errorLevel", ShortName = "el", DescriptionResourceName = "ErrorLevelDescription")]
    internal EErrorLevel ErrorLevel { get; set; } = EErrorLevel.None;
}
