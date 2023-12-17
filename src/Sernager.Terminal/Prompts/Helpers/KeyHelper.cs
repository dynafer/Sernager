namespace Sernager.Terminal.Prompts.Helpers;

internal static class KeyHelper
{
    internal static bool IsCharKey(ConsoleKey key)
    {
        return key is >= ConsoleKey.A and <= ConsoleKey.Z
            || key is >= ConsoleKey.D0 and <= ConsoleKey.D9
            || key is >= ConsoleKey.NumPad0 and <= ConsoleKey.NumPad9
            || key is ConsoleKey.Spacebar
            || key is ConsoleKey.Oem1
            || key is ConsoleKey.Oem2
            || key is ConsoleKey.Oem3
            || key is ConsoleKey.Oem4
            || key is ConsoleKey.Oem5
            || key is ConsoleKey.Oem6
            || key is ConsoleKey.Oem7
            || key is ConsoleKey.Oem8
            || key is ConsoleKey.Oem102
            || key is ConsoleKey.OemComma
            || key is ConsoleKey.OemMinus
            || key is ConsoleKey.OemPeriod
            || key is ConsoleKey.OemPlus;
    }
}
