using Sernager.Core.Models;
using System.Text;

namespace Sernager.Terminal.Flows.Extensions;

internal static class FlowCommandModelExtension
{
    internal static void ToCommandAsArray(this CommandModel model, string delimiter = "\n")
    {
        string command = model.Command switch
        {
            string[] commandArray => string.Join(" ", commandArray),
            string commandString => commandString,
            _ => throw new ArgumentException("Command must be a string or string[]")
        };

        command = command.Replace(delimiter, " ").Trim();

        StringBuilder sb = new StringBuilder();
        bool bInsideSingleQuote = false;
        bool bInsideDoubleQuote = false;

        foreach (char c in command)
        {
            if (c == '\'')
            {
                bInsideSingleQuote = !bInsideSingleQuote;
            }
            else if (c == '"')
            {
                bInsideDoubleQuote = !bInsideDoubleQuote;
            }
            else if (c == ' ' && !bInsideSingleQuote && !bInsideDoubleQuote)
            {
                sb.Append('\n');
                continue;
            }

            sb.Append(c);
        }

        command = sb.ToString();

        model.Command = command.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .ToArray();
    }
}
