using Sernager.Core.Builders;

namespace Sernager.Core;

public interface IService
{
    ISettingBuilder Setting { get; }
}
