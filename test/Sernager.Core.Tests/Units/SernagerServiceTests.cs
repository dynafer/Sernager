using Sernager.Core.Configs;
using Sernager.Core.Managers;
using Sernager.Core.Models;

namespace Sernager.Core.Tests.Units;

internal sealed class SernagerServiceTests
{
    private ISernagerService mService;

    [SetUp]
    public void SetUpBuilder()
    {
        Configurator.Init();
        mService = new SernagerService();
    }

    [TearDown]
    public void Reset()
    {
        ResetUtil.ResetConfigurator();
        mService = null!;
    }

    [Test]
    public void ManageCommandGroup_ShouldReturnCommandManager()
    {
        string groupName = "Test";
        string shortName = "T";
        string description = "Test";

        ICommandManager? manager;

        Assert.That(CacheManager.TryGet($"Command-Group-{groupName}", out manager), Is.False);
        Assert.That(manager, Is.Null);

        ICommandManager actual = mService.ManageCommandGroup(groupName, shortName, description);

        Assert.That(CacheManager.TryGet($"Command-Group-{groupName}", out manager), Is.True);
        Assert.That(manager, Is.Not.Null);
        Assert.That(actual, Is.EqualTo(manager));

        // To check if the method returns from cache
        actual = mService.ManageCommandGroup(groupName, shortName, description);

        Assert.That(actual, Is.EqualTo(manager));
    }

    [Test]
    public void ManageEnvironmentGroup_ShouldReturnEnvironmentManager()
    {
        string groupName = "Test";

        IEnvironmentManager? manager;

        Assert.That(CacheManager.TryGet($"Environment-Group-{groupName}", out manager), Is.False);
        Assert.That(manager, Is.Null);

        IEnvironmentManager actual = mService.ManageEnvironmentGroup(groupName);

        Assert.That(CacheManager.TryGet($"Environment-Group-{groupName}", out manager), Is.True);
        Assert.That(manager, Is.Not.Null);
        Assert.That(actual, Is.EqualTo(manager));

        // To check if the method returns from cache
        actual = mService.ManageEnvironmentGroup(groupName);

        Assert.That(actual, Is.EqualTo(manager));
    }

    [Test]
    public void GetExecutor_ShouldReturnExecutor()
    {
        Guid commandId = Guid.NewGuid();

        IExecutor? executor;

        Assert.That(CacheManager.TryGet($"Executor-{commandId}", out executor), Is.False);
        Assert.That(executor, Is.Null);

        Configurator.Config.Commands.Add(commandId, new CommandModel());

        IExecutor actual = mService.GetExecutor(commandId);

        Assert.That(CacheManager.TryGet($"Executor-{commandId}", out executor), Is.True);
        Assert.That(executor, Is.Not.Null);
        Assert.That(actual, Is.EqualTo(executor));

        // To check if the method returns from cache
        actual = mService.GetExecutor(commandId);

        Assert.That(actual, Is.EqualTo(executor));
    }

    [Test]
    public void GetCommandGroupNames_ShouldReturnCommandGroupNames()
    {
        (string, GroupModel)[] groups =
        [
            ("Test1", new GroupModel()),
            ("Test2", new GroupModel()),
            ("Test3", new GroupModel()),
        ];

        for (int i = 0; i < groups.Length; ++i)
        {
            (string name, GroupModel model) = groups[i];
            model.Name = name;
            model.ShortName = $"T{i + 1}";
            Configurator.Config.CommandMainGroups.Add(name, model);
        }

        string[] expected = groups.Select(x => x.Item1).ToArray();
        string[] actual = mService.GetCommandGroupNames();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetCommandGroupShortNames_ShouldReturnCommandGroupShortNames()
    {
        (string, GroupModel)[] groups =
        [
            ("Test1", new GroupModel()),
            ("Test2", new GroupModel()),
            ("Test3", new GroupModel()),
        ];

        for (int i = 0; i < groups.Length; ++i)
        {
            (string name, GroupModel model) = groups[i];
            model.Name = name;
            model.ShortName = $"T{i + 1}";
            Configurator.Config.CommandMainGroups.Add(name, model);
        }

        string[] expected = groups.Select(x => x.Item2.ShortName).ToArray();
        string[] actual = mService.GetCommandGroupShortNames();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetEnvironmentGroupNames_ShouldReturnEnvironmentGroupNames()
    {
        (string, EnvironmentModel)[] groups =
        [
            ("Test1", new EnvironmentModel()),
            ("Test2", new EnvironmentModel()),
            ("Test3", new EnvironmentModel()),
        ];

        for (int i = 0; i < groups.Length; ++i)
        {
            (string name, EnvironmentModel model) = groups[i];
            model.Name = name;
            Configurator.Config.EnvironmentGroups.Add(name, model);
        }

        string[] expected = groups.Select(x => x.Item1).ToArray();
        string[] actual = mService.GetEnvironmentGroupNames();

        Assert.That(actual, Is.EqualTo(expected));
    }
}
