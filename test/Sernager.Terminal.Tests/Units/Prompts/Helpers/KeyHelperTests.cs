using Sernager.Terminal.Prompts.Helpers;

namespace Sernager.Terminal.Tests.Units.Prompts.Helpers;

internal sealed class KeyHelperTests
{
    private static readonly ConsoleKey[] KEYS = Enum.GetValues<ConsoleKey>();

    [Theory]
    public void IsCharKey_ShouldReturnBool(ConsoleKey key)
    {
        Assume.That(key, Is.AnyOf(KEYS));

        bool result = KeyHelper.IsCharKey(key);
        bool expected = key >= ConsoleKey.A && key <= ConsoleKey.Z
            || key >= ConsoleKey.D0 && key <= ConsoleKey.D9
            || key >= ConsoleKey.NumPad0 && key <= ConsoleKey.NumPad9
            || key == ConsoleKey.Spacebar
            || key == ConsoleKey.Oem1
            || key == ConsoleKey.Oem2
            || key == ConsoleKey.Oem3
            || key == ConsoleKey.Oem4
            || key == ConsoleKey.Oem5
            || key == ConsoleKey.Oem6
            || key == ConsoleKey.Oem7
            || key == ConsoleKey.Oem8
            || key == ConsoleKey.Oem102
            || key == ConsoleKey.OemComma
            || key == ConsoleKey.OemMinus
            || key == ConsoleKey.OemPeriod
            || key == ConsoleKey.OemPlus;

        Assert.That(result, Is.EqualTo(expected));
    }
}
