using Sernager.Core;
using Sernager.Core.Managers;
using Sernager.Resources;
using Sernager.Terminal.Attributes;
using System.Reflection;

namespace Sernager.Terminal.Managers;

internal static class FlowManager
{
    private static readonly BindingFlags BINDING_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    private static readonly Dictionary<string, Type> mFlowTypes = new Dictionary<string, Type>();
    private static readonly Stack<IFlow> mFlowStack = new Stack<IFlow>();
    private static Queue<string> mCommandQueue = null!;
    private static readonly IFlow mHomeFlow;
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
            if (type.GetCustomAttribute<FlowAttribute>() is not FlowAttribute attribute)
            {
                continue;
            }

            if (string.IsNullOrEmpty(attribute.Name))
            {
                attribute.Name = type.Name.Replace("Flow", string.Empty);
            }

            string attachedName = attribute.Name;
            if (!string.IsNullOrEmpty(attribute.Alias))
            {
                attachedName = $"{attribute.Alias}.{attribute.Name}";
            }

            if (mFlowTypes.ContainsKey(attachedName))
            {
                throw new SernagerException($"Flow name '{attachedName}' is already registered.");
            }

            mFlowTypes.Add(attachedName, type);
        }

        tryCreateFlow("Home");

        mHomeFlow = mFlowStack.Pop();
    }

    internal static string GetResourceString(string type, string key)
    {
        IResourcePack resourcePack;
        if (CacheManager.TryGet($"FlowManager.Resources.{type}", out resourcePack))
        {
            return resourcePack.GetString(key);
        }
        else
        {
            resourcePack = ResourceRetriever.UsePack($"Terminal.Flow.{type}");
            CacheManager.Set($"FlowManager.Resources.{type}", resourcePack);

            return resourcePack.GetString(key);
        }
    }

    internal static void Start(string[] commands)
    {
        if (commands.Length == 0)
        {
            mHomeFlow.Prompt();
            return;
        }

        mCommandQueue = new Queue<string>(commands);

        mHomeFlow.TryJump(string.Empty, true);
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

    internal static void JumpFlow(string flowName)
    {
        tryCreateFlow(flowName);

        jumpLastFlow();
    }

    internal static void JumpFlow(string flowName, params object[] parameters)
    {
        tryCreateFlow(flowName, parameters);

        jumpLastFlow();
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

    internal static void RunLastFlow()
    {
        runLastFlow();
    }

    internal static void GoHome()
    {
        mFlowStack.Clear();

        mHomeFlow.Prompt();
    }

    internal static bool TryHandleCommonSelectionResult(string result)
    {
        return tryHandleCommonSelectionResult(result);
    }

    internal static bool TryHandleCommonSelectionResult(string result, Action beforeBackAction, Action beforeHomeAction, Action beforeExitAction)
    {
        switch (result)
        {
            case "Back":
                beforeBackAction();
                break;
            case "Home":
                beforeHomeAction();
                break;
            case "Exit":
                beforeExitAction();
                break;
            default:
                return false;
        }

        return tryHandleCommonSelectionResult(result);
    }

    internal static bool TryHandleCommonSelectionResult<TOptionValue>(TOptionValue result, TOptionValue backOption, TOptionValue homeOption, TOptionValue exitOption)
        where TOptionValue : notnull
    {
        if (EqualityComparer<TOptionValue>.Default.Equals(result, backOption))
        {
            return tryHandleCommonSelectionResult("Back");
        }
        else if (EqualityComparer<TOptionValue>.Default.Equals(result, homeOption))
        {
            return tryHandleCommonSelectionResult("Home");
        }
        else if (EqualityComparer<TOptionValue>.Default.Equals(result, exitOption))
        {
            return tryHandleCommonSelectionResult("Exit");
        }
        else
        {
            return false;
        }
    }

    internal static bool TryHandleCommonSelectionResult<TOptionValue>(
        TOptionValue result,
        (TOptionValue, Action?) backOption,
        (TOptionValue, Action?) homeOption,
        (TOptionValue, Action?) exitOption
    )
        where TOptionValue : notnull
    {
        if (EqualityComparer<TOptionValue>.Default.Equals(result, backOption.Item1))
        {
            if (backOption.Item2 != null)
            {
                backOption.Item2();
            }

            return tryHandleCommonSelectionResult("Back");
        }
        else if (EqualityComparer<TOptionValue>.Default.Equals(result, homeOption.Item1))
        {
            if (homeOption.Item2 != null)
            {
                homeOption.Item2();
            }

            return tryHandleCommonSelectionResult("Home");
        }
        else if (EqualityComparer<TOptionValue>.Default.Equals(result, exitOption.Item1))
        {
            if (exitOption.Item2 != null)
            {
                exitOption.Item2();
            }

            return tryHandleCommonSelectionResult("Exit");
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
            string[] possibleFlows = PossibilityManager.Find(flowName, mFlowTypes.Keys.ToArray());

            ExceptionManager.Throw<SernagerException>($"Unknown flow name: {flowName}. Did you mean one of [{string.Join(", ", possibleFlows)}]?");

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
            ExceptionManager.Throw<SernagerException>($"Flow {flowName} could not be created. Check if it implements {nameof(IFlow)} interface.");

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

    private static void jumpLastFlow()
    {
        if (mCommandQueue == null || mCommandQueue.Count == 0)
        {
            mCommandQueue = null!;
            runLastFlow();
            return;
        }

        string command = mCommandQueue.Dequeue();

        if (!mFlowStack.Peek().TryJump(command, mCommandQueue.Count != 0))
        {
            mCommandQueue = null!;
            runLastFlow();
        }
    }

    private static bool tryHandleCommonSelectionResult(string result)
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
}
