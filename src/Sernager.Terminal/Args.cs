using Sernager.Resources;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Managers;
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

    static Args()
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

    internal static string[] GetCommands(string[] args)
    {
        List<string> commandArgs = new List<string>();

        foreach (string arg in args)
        {
            if (arg.StartsWith("--") || arg.StartsWith("-"))
            {
                break;
            }

            commandArgs.Add(arg);
        }

        return commandArgs.ToArray();
    }

    internal static EManagementTypeFlags? GetManagementType()
    {
        return Model.ManagementType;
    }

    internal static void Parse(string[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        if (args[0] == "help" || args[0] == "-h" || args[0] == "--help")
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
            if (value != null && (value.StartsWith("--") || value.StartsWith("-")))
            {
                value = null;
            }

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
            string[] possibleArgs = PossibilityManager.Find(
                arg,
                bShort
                    ? mShortNames.Keys.ToArray()
                    : mArgInfos.Keys.ToArray()
                );

            Logger.ErrorWithExit($"Unknown argument: [Bold]{arg}[/Bold]. Did you mean one of [Bold]({string.Join(", ", possibleArgs)})[/Bold]?");
            return;
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

        Type underlyingType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

        switch (underlyingType)
        {
            case Type stringType when stringType == typeof(string):
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Logger.ErrorWithExit($"Argument [Bold]{arg}[/Bold] requires [Bold]{attribute.Value}[/Bold]");
                    return;
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

                bool result;
                if (bool.TryParse(value, out result))
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
            case Type enumType when enumType.IsEnum:
            {
                string[] names = Enum.GetNames(enumType);
                object? result;

                if (string.IsNullOrWhiteSpace(value))
                {
                    Logger.ErrorWithExit($"Argument [Bold]{arg}[/Bold] requires one of [Bold]({string.Join(", ", names)})[/Bold]");
                    return;
                }

                if (!Enum.TryParse(enumType, value, true, out result))
                {
                    string[] possibleArgs = PossibilityManager.Find(value, names);
                    Logger.ErrorWithExit($"Unknown value: [Bold]{value}[/Bold] of [Bold]{arg}[/Bold]. Did you mean one of [Bold]({string.Join(", ", possibleArgs)})[/Bold]?");
                    return;
                }

                property.SetValue(Model, result);
                return;
            }
            default:
                break;
        }
    }

    private static void help()
    {
        IResourcePack resourcePack = ResourceRetriever.UsePack("Terminal.Model.Builder");

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
            (PropertyInfo property, ArgAttribute attribute, _) = argInfo.Value;

            string arguments = $"--{argInfo.Key}";
            if (!string.IsNullOrWhiteSpace(attribute.ShortName))
            {
                arguments += $", -{attribute.ShortName}";
            }

            Type underlyingType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            if (underlyingType.IsEnum)
            {
                string[] enumNames = Enum.GetNames(underlyingType);
                if (enumNames.Length > 1)
                {
                    enumNames[enumNames.Length - 1] = $"or {enumNames[enumNames.Length - 1]}";
                }

                table.AddRows(
                    new Row(
                        new TextComponent().SetText(arguments),
                        new TextComponent().SetText(string.Join(", ", enumNames)),
                        new TextComponent().SetText(resourcePack.GetString(attribute.DescriptionResourceName))
                    )
                );

                continue;
            }

            table.AddRows(
                new Row(
                    new TextComponent().SetText(arguments),
                    new TextComponent().SetText(attribute.Value),
                    new TextComponent().SetText(resourcePack.GetString(attribute.DescriptionResourceName))
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
