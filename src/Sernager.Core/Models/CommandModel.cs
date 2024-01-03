namespace Sernager.Core.Models;

internal sealed class CommandModel
{
    public string Name { get; init; } = string.Empty;
    public string ShortName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    private object mCommand = null!;
    public object Command
    {
        get
        {
            return mCommand;
        }
        init
        {
            if (value is string)
            {
                mCommand = value;
            }
            else if (value is string[])
            {
                mCommand = value;
            }
            else
            {
                throw new ArgumentException("Command must be a string or string[]");
            }
        }
    }
}