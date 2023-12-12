using Sernager.Runner.Builders;

namespace Sernager.Runner;

internal class Service : IService
{
    public ISettingBuilder Setting { get; init; } = new SettingBuilder();
}
