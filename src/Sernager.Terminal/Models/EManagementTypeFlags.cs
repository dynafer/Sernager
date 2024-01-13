namespace Sernager.Terminal.Models;

[Flags]
internal enum EManagementTypeFlags
{
    Command = 0,
    Environment = 1 << 0,
}
