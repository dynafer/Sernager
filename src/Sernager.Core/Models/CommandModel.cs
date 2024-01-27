using Sernager.Core.Extensions;
using System.Text.Json;

namespace Sernager.Core.Models;

public sealed class CommandModel
{
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> UsedEnvironmentGroups { get; init; } = new List<string>();
    private object mCommand = string.Empty;
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
            else if (value is JsonElement jsonElement)
            {
                string[]? commands;
                string? command;

                if (jsonElement.TryGetStringArray(out commands))
                {
                    mCommand = commands;
                }
                else if (jsonElement.TryGetString(out command))
                {
                    mCommand = command;
                }
                else
                {
                    throw new InvalidCastException("Command must be a string or string[].");
                }
            }
            else if (value is IEnumerable<object> objectEnumerable)
            {
                mCommand = objectEnumerable.Select(x => x.ToString()).ToArray();
            }
            else if (value is IEnumerable<string> stringEnumerable)
            {
                mCommand = stringEnumerable.ToArray();
            }
            else if (value is null)
            {
                mCommand = string.Empty;
            }
            else
            {
                throw new InvalidCastException("Command must be a string or string[].");
            }
        }
    }

    public string ToCommandString()
    {
        return Command switch
        {
            string[] commandArray => string.Join(" ", commandArray),
            string commandString => commandString,
            null => string.Empty,
            _ => throw new InvalidCastException("Command must be a string or string[].")
        };
    }
}