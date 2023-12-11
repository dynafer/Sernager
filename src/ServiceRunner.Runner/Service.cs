using ServiceRunner.Runner.Builders;

namespace ServiceRunner.Runner;

internal class Service : IService
{
    public ISettingBuilder Setting { get; init; } = new SettingBuilder();
}
