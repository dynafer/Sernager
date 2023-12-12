using Sernager.Core.Builders;

namespace Sernager.Core;

public interface ISernagerService
{
    ISettingBuilder Setting { get; }
}
