namespace Sernager.Core.Models;

public sealed class CommandModel
{
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> UsedEnvironmentGroups { get; init; } = new List<string>();
    private object mCommand = null!;
    public object Command
    {
        get
        {
            return mCommand;
        }
        set
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