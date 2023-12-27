using Sernager.Terminal.Attributes;
using Sernager.Terminal.Models;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Tables;
using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Extensions.Components;
using System.Reflection;

using TArgInfo = (System.Reflection.PropertyInfo, Sernager.Terminal.Attributes.ArgAttribute, object?);

namespace Sernager.Terminal;

internal static class Args
{
    internal static BuilderModel Model { get; private set; } = new BuilderModel();
    private static Dictionary<string, TArgInfo> mArgInfos = new Dictionary<string, TArgInfo>();
    private static Dictionary<string, string> mShortNames = new Dictionary<string, string>();

    internal static void Init()
    {
        PropertyInfo[] properties = typeof(BuilderModel).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Where(property => property.GetCustomAttribute<ArgAttribute>() != null)
            .ToArray();

        foreach (PropertyInfo property in properties)
        {
            ArgAttribute? argAttribute = property.GetCustomAttribute<ArgAttribute>();

            if (argAttribute == null)
            {
                continue;
            }

            mArgInfos.Add(argAttribute.Name, (property, argAttribute, property.GetValue(Model)));

            if (!string.IsNullOrWhiteSpace(argAttribute.ShortName))
            {
                mShortNames.Add(argAttribute.ShortName, argAttribute.Name);
            }
        }
    }

    internal static void Parse(string[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        if (args.Contains("help") || args.Contains("-h") || args.Contains("--help"))
        {
            help();
            return;
        }

        for (int i = 0; i < args.Length; ++i)
        {
            string arg = args[i];

            if (!arg.StartsWith("--") && !arg.StartsWith("-"))
            {
                continue;
            }

            string? value = args.ElementAtOrDefault(i + 1);
            parseArg(arg, value);
        }

        mArgInfos.Clear();
        mArgInfos = null!;

        mShortNames.Clear();
        mShortNames = null!;
    }

    internal static void Complete()
    {
        Model = null!;
    }

    private static void parseArg(string arg, string? value = null)
    {
        bool bShort = !arg.StartsWith("--");

        if (arg.Contains("="))
        {
            string[] argValue = arg.Split('=');
            arg = argValue[0];
            value = string.Join('=', argValue.Skip(1));
        }

        arg = arg.Replace("-", string.Empty);

        if (!mArgInfos.ContainsKey(arg) && !mShortNames.ContainsKey(arg))
        {
            string[] possibleArgs = findPossibleArgs(
                bShort
                    ? mShortNames.Keys.ToArray()
                    : mArgInfos.Keys.ToArray(),
                arg);

            IPromptComponent component = new InlineStyledTextComponent()
                .SetText($"Unknown argument: {arg}. Did you mean one of [Bold]({string.Join(", ", possibleArgs)})[/Bold]?");

            throw new ArgumentException(component.Render());
        }

        if (mShortNames.ContainsKey(arg))
        {
            arg = mShortNames[arg];
        }

        (PropertyInfo property, ArgAttribute attribute, object? defaultValue) = mArgInfos[arg];

        if (property.GetValue(Model)?.Equals(defaultValue) == false)
        {
            return;
        }

        switch (property.PropertyType)
        {
            case Type stringType when stringType == typeof(string):
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"Argument {arg} requires {attribute.Value}");
                }

                property.SetValue(Model, value);
                return;
            }
            case Type boolType when boolType == typeof(bool):
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    property.SetValue(Model, true);
                    return;
                }

                if (bool.TryParse(value, out bool result))
                {
                    property.SetValue(Model, result);
                    return;
                }

                if (value == "1" || value == "0")
                {
                    property.SetValue(Model, value == "1");
                }

                return;
            }
            default:
                break;
        }
    }

    private static string[] findPossibleArgs(string[] argNames, string arg)
    {
        List<string> possibleArgs = new List<string>();
        List<string> alternativeArgs = new List<string>();
        List<byte> possibilities = new List<byte>();

        const byte BEST_POSSIBILITY = 90;
        const byte ALTERNATIVE_POSSIBILITY = 50;

        foreach (string argName in argNames)
        {
            byte possibility = 0;

            for (int i = 0; i < arg.Length; ++i)
            {
                if (argName.Length <= i)
                {
                    break;
                }

                if (argName[i] == arg[i])
                {
                    ++possibility;
                }
            }

            possibility = (byte)(possibility / arg.Length * 100);
            possibilities.Add(possibility);

            if (possibility >= BEST_POSSIBILITY)
            {
                possibleArgs.Add(argName);
            }

            if (possibility >= ALTERNATIVE_POSSIBILITY)
            {
                alternativeArgs.Add(argName);
            }
        }

        if (possibleArgs.Count == 0)
        {
            if (alternativeArgs.Count > 0)
            {
                return alternativeArgs.ToArray();
            }

            return argNames;
        }

        return possibleArgs.ToArray();
    }

    private static void help()
    {
        Assembly currentAssembly = Assembly.GetExecutingAssembly();

        string title = currentAssembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? string.Empty;
        AssemblyName metadata = currentAssembly.GetName();

        Prompter.Writer.WriteLine($"{title} v{metadata.Version}");

        TableComponent table = new TableComponent()
            .UseBorder()
            .UseHeader()
            .AddRows(
                new Row(
                    new TextComponent().SetDecoration(EDecorationFlags.Bold).SetText("Argument, Short"),
                    new TextComponent().SetDecoration(EDecorationFlags.Bold).SetText("Value"),
                    new TextComponent().SetDecoration(EDecorationFlags.Bold).SetText("Description")
                )
            );

        foreach (KeyValuePair<string, TArgInfo> argInfo in mArgInfos)
        {
            (PropertyInfo _, ArgAttribute attribute, object? _) = argInfo.Value;

            string arguments = $"--{argInfo.Key}";
            if (!string.IsNullOrWhiteSpace(attribute.ShortName))
            {
                arguments += $", -{attribute.ShortName}";
            }

            table.AddRows(
                new Row(
                    new TextComponent().SetText(arguments),
                    new TextComponent().SetText(attribute.Value),
                    new TextComponent().SetText(attribute.Description)
                )
            );
        }

        using (Renderer renderer = new Renderer(Prompter.Writer))
        {
            renderer.Render(table);
        }

        Environment.Exit(1);
    }
}
