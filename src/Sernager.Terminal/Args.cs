using Sernager.Terminal.Attributes;
using Sernager.Terminal.Models;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Tables;
using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Extensions.Components;
using System.Reflection;

namespace Sernager.Terminal;

internal static class Args
{
    internal static BuilderModel Model { get; private set; } = new BuilderModel();
    private static Dictionary<string, (PropertyInfo, ArgAttribute, object?)> mCommandAttributes = new Dictionary<string, (PropertyInfo, ArgAttribute, object?)>();
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

            mCommandAttributes.Add(argAttribute.Name, (property, argAttribute, property.GetValue(Model)));

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

        mCommandAttributes.Clear();
        mCommandAttributes = null!;

        mShortNames.Clear();
        mShortNames = null!;
    }

    internal static void Complete()
    {
        Model = null!;
    }

    private static void parseArg(string arg, string? value = null)
    {
        if (arg.Contains("="))
        {
            string[] argValue = arg.Split('=');
            arg = argValue[0];
            value = string.Join('=', argValue.Skip(1));
        }

        arg = arg.Replace("-", string.Empty);

        if (!mCommandAttributes.ContainsKey(arg) && !mShortNames.ContainsKey(arg))
        {
            throw new ArgumentException($"Unknown argument: {arg}");
        }

        if (mShortNames.ContainsKey(arg))
        {
            arg = mShortNames[arg];
        }

        (PropertyInfo property, ArgAttribute attribute, object? defaultValue) = mCommandAttributes[arg];

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

        foreach (KeyValuePair<string, (PropertyInfo, ArgAttribute, object?)> command in mCommandAttributes)
        {
            (PropertyInfo _, ArgAttribute attribute, object? _) = command.Value;

            string arguments = $"--{command.Key}";
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
