using Sernager.Terminal.Attributes;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Tables;
using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Extensions.Components;
using System.Reflection;

namespace Sernager.Terminal;

internal static class Args
{
    private static Dictionary<string, object> mValues = new Dictionary<string, object>();
    private static Dictionary<string, ArgAttribute> mCommandAttributes = new Dictionary<string, ArgAttribute>();
    private static Dictionary<string, string> mShortNames = new Dictionary<string, string>();

    internal static void Init()
    {
        Assembly currentAssembly = Assembly.GetExecutingAssembly();

        string currentNamespace = currentAssembly.GetName().Name!;

        Type[] types = currentAssembly.GetTypes()
            .Where(type => type.Namespace != null
                        && type.Namespace != currentNamespace
                        && !type.Namespace.StartsWith($"{currentNamespace}.Attributes")
                        && !type.Namespace.StartsWith($"{currentNamespace}.Prompts")
            )
            .ToArray();

        foreach (Type type in types)
        {
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                ArgAttribute? argAttribute = propertyInfo.GetCustomAttribute<ArgAttribute>();

                if (argAttribute == null)
                {
                    continue;
                }

                mCommandAttributes.Add(argAttribute.Name, argAttribute);

                if (!string.IsNullOrWhiteSpace(argAttribute.ShortName))
                {
                    mShortNames.Add(argAttribute.ShortName, argAttribute.Name);
                }
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

    internal static T? GetValue<T>(string arg)
        where T : notnull
    {
        if (!mValues.ContainsKey(arg))
        {
            return default;
        }

        return (T)mValues[arg];
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

        if (!mCommandAttributes.ContainsKey(arg))
        {
            throw new ArgumentException($"Unknown argument: {arg}");
        }

        if (mCommandAttributes[arg].IsBool)
        {
            if (value != null)
            {
                if (bool.TryParse(value, out bool result))
                {
                    mValues.Add(arg, result);
                    return;
                }
                else if (value == "1" || value == "0")
                {
                    mValues.Add(arg, value == "1");
                    return;
                }
            }

            mValues.Add(arg, true);
            return;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"Argument {arg} requires {mCommandAttributes[arg].Value}");
        }

        mValues.Add(arg, value);
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

        foreach (KeyValuePair<string, ArgAttribute> command in mCommandAttributes)
        {
            string arguments = $"--{command.Key}";
            if (!string.IsNullOrWhiteSpace(command.Value.ShortName))
            {
                arguments += $", -{command.Value.ShortName}";
            }

            table.AddRows(
                new Row(
                    new TextComponent().SetText(arguments),
                    new TextComponent().SetText(command.Value.Value),
                    new TextComponent().SetText(command.Value.Description)
                )
            );
        }

        using (Renderer renderer = new Renderer(Prompter.Writer))
        {
            renderer.Render(table);
        }
    }
}
