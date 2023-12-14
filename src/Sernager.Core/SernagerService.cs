using Sernager.Core.Builders;

namespace Sernager.Core;

internal sealed class SernagerService : ISernagerService
{
    public ISettingBuilder Setting { get; init; } = new SettingBuilder();
}
