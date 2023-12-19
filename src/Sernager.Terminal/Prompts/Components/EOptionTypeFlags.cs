namespace Sernager.Terminal.Prompts.Components;

[Flags]
public enum EOptionTypeFlags
{
    None = 0,
    Select = 1 << 0,
    MultiSelect = 1 << 1,
    Hint = 1 << 2,
}
