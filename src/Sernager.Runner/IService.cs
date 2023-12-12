using Sernager.Runner.Builders;

namespace Sernager.Runner;

public interface IService
{
    ISettingBuilder Setting { get; }
}
