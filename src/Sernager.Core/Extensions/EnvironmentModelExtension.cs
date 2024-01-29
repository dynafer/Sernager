using Sernager.Core.Models;
using System.Diagnostics;
using System.Text;

namespace Sernager.Core.Extensions;

public static class EnvironmentModelExtension
{
    public static EnvironmentModel RemoveWhitespacesInDeclaredVariables(this EnvironmentModel model, Dictionary<string, string> target, string key)
    {
        if (!target.ContainsKey(key) ||
            !target[key].Contains("${") ||
            !target[key].Contains("}"))
        {
            return model;
        }

        StringBuilder builder = new StringBuilder();

        bool bDollarSign = false;
        bool bBegin = false;

        foreach (char c in target[key])
        {
            if (c == '$')
            {
                if (bDollarSign || bBegin)
                {
                    bDollarSign = false;
                    bBegin = false;
                }
                else
                {
                    bDollarSign = true;
                }
            }
            else if (c == '{')
            {
                if (bBegin)
                {
                    bDollarSign = false;
                    bBegin = false;
                }
                else if (bDollarSign)
                {
                    bDollarSign = false;
                    bBegin = true;
                }
            }
            else if (c == '}')
            {
                bDollarSign = false;
                bBegin = false;
            }
            else
            {
                if (bDollarSign)
                {
                    bDollarSign = false;
                }
                else if (bBegin)
                {
                    if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
                    {
                        continue;
                    }
                }
            }

            builder.Append(c);
        }

        if (bBegin)
        {
            Debug.WriteLine($"Warning: Invalid variable declaration. ({key}={target[key]})");
        }

        target[key] = builder.ToString();

        return model;
    }

    internal static Dictionary<string, string> BuildVariables(this EnvironmentModel model)
    {
        Dictionary<string, string> variables = new Dictionary<string, string>();

        foreach (var pair in model.Variables)
        {
            if (variables.ContainsKey(pair.Key))
            {
                continue;
            }

            variables.Add(pair.Key, pair.Value);

            if (!pair.Value.Contains("${") || !pair.Value.Contains("}"))
            {
                continue;
            }

            foreach (var substPair in model.SubstVariables)
            {
                variables[pair.Key] = variables[pair.Key].Replace("${" + substPair.Key + "}", substPair.Value);
            }
        }

        return variables;
    }
}
