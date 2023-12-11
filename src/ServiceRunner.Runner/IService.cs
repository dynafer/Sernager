using ServiceRunner.Runner.Builders;

namespace ServiceRunner.Runner;

public interface IService
{
    ISettingBuilder Setting { get; }
}
