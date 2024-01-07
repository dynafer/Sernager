using Sernager.Terminal.Attributes;
using Sernager.Terminal.Flows;
using System.Reflection;

namespace Sernager.Terminal.Managers;

internal static class FlowManager
{
    private static readonly BindingFlags BINDING_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    private static Dictionary<string, Type> mFlowTypes = new Dictionary<string, Type>();
    private static Stack<IFlow> mFlowStack = new Stack<IFlow>();
    private static IFlow mHomeFlow = new HomeFlow();
    internal static int PageSize
    {
        get
        {
            int maxPromptSize = 4;
            if (Console.WindowHeight - maxPromptSize < 1)
            {
                return 1;
            }
            else
            {
                return Console.WindowHeight - maxPromptSize;
            }
        }
    }
    internal static bool IsManagementMode { get; set; } = false;

    static FlowManager()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        foreach (Type type in assembly.GetTypes())
        {
            if (type.GetCustomAttribute<FlowAttribute>() is FlowAttribute attribute)
            {
                if (string.IsNullOrEmpty(attribute.Name))
                {
                    attribute.Name = type.Name.Replace("Flow", string.Empty);
                }

                string attachedName = attribute.Name;
                if (!string.IsNullOrEmpty(attribute.Alias))
                {
                    attachedName = $"{attribute.Alias}.{attribute.Name}";
                }

                mFlowTypes.Add(attachedName, type);
            }
        }
    }

    internal static void Start(string[] commands)
    {
        mHomeFlow.Prompt();
    }

    internal static void RunFlow(string flowName)
    {
        tryCreateFlow(flowName);

        runLastFlow();
    }

    internal static void RunFlow(string flowName, params object[] parameters)
    {
        tryCreateFlow(flowName, parameters);

        runLastFlow();
    }

    internal static void RunPreviousFlow()
    {
        if (mFlowStack.Count == 0)
        {
            mHomeFlow.Prompt();
            return;
        }

        mFlowStack.Pop();

        runLastFlow();
    }

    internal static void RunPreviousFlow(int count)
    {
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than 0.");
        }

        for (int i = 0; i < count; ++i)
        {
            if (mFlowStack.Count == 0)
            {
                break;
            }

            mFlowStack.Pop();
        }

        runLastFlow();
    }

    internal static void GoHome()
    {
        mFlowStack.Clear();

        mHomeFlow.Prompt();
    }

    internal static bool TryHandleCommonSelectionResult(string result)
    {
        switch (result)
        {
            case "Back":
                RunPreviousFlow();
                return true;
            case "Home":
                GoHome();
                return true;
            case "Exit":
                Environment.Exit(0);
                return true;
            default:
                return false;
        }
    }

    internal static bool TryHandleCommonSelectionResult<TOptionValue>(TOptionValue result, TOptionValue backOption, TOptionValue homeOption, TOptionValue exitOption)
        where TOptionValue : notnull
    {
        if (EqualityComparer<TOptionValue>.Default.Equals(result, exitOption))
        {
            Environment.Exit(0);
            return true;
        }
        else if (EqualityComparer<TOptionValue>.Default.Equals(result, homeOption))
        {
            GoHome();
            return true;
        }
        else if (EqualityComparer<TOptionValue>.Default.Equals(result, backOption))
        {
            RunPreviousFlow();
            return true;
        }
        else
        {
            return false;
        }
    }

    private static bool tryCreateFlow(string flowName, object[]? paramters = null)
    {
        Type? flowType;

        if (!mFlowTypes.TryGetValue(flowName, out flowType))
        {
            return false;
        }

        IFlow? flow;

        if (paramters == null)
        {
            flow = (IFlow?)flowType.GetConstructor(BINDING_FLAGS, Type.EmptyTypes)?.Invoke(null);
        }
        else
        {
            flow = (IFlow?)flowType.GetConstructor(BINDING_FLAGS, paramters.Select(parameter => parameter.GetType()).ToArray())?.Invoke(paramters);
        }

        if (flow == null)
        {
            return false;
        }

        mFlowStack.Push(flow);

        return true;
    }

    private static void runLastFlow()
    {
        if (mFlowStack.Count == 0)
        {
            mHomeFlow.Prompt();
            return;
        }

        mFlowStack
            .Peek()
            .Prompt();
    }
}
