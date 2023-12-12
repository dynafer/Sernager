using Sernager.Core.Builders;

namespace Sernager.Core;

internal class Service : IService
{
    public ISettingBuilder Setting { get; init; } = new SettingBuilder();
}
