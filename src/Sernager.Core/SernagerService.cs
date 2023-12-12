using Sernager.Core.Builders;

namespace Sernager.Core;

internal class SernagerService : ISernagerService
{
    public ISettingBuilder Setting { get; init; } = new SettingBuilder();
}
