using Sernager.Terminal.Flows.Extensions;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Helpers;

internal static class FlowPromptHelper
{
    internal static bool TrySelectFile(string resourceType, string prompt, string extension, int pageSize, out string result)
    {
        (string, string)[] drives = DriveInfo.GetDrives()
            .Where(drive => drive.DriveType == DriveType.Fixed)
            .Select(drive =>
                (
                    string.Format(FlowManager.CommonResourcePack.GetString("GoTo"), drive.Name),
                    drive.RootDirectory.FullName
                )
            )
            .ToArray();
        string currentPath = Environment.CurrentDirectory;

        while (true)
        {
            List<(string, string)> options = [];

            string? parent = Directory.GetParent(currentPath)?.FullName;

            if (currentPath != Directory.GetDirectoryRoot(currentPath) && parent != null)
            {
                options.Add(("..", parent));
            }

            bool bBackslahedPath = currentPath.Contains("\\");

            options.AddRange(
                Directory.EnumerateFileSystemEntries(currentPath, "*")
                    .Where(path => Directory.Exists(path) || Path.GetExtension(path) == extension)
                    .Select(path =>
                    {
                        string name = Path.GetFileName(path);

                        if (Directory.Exists(path))
                        {
                            name = $"[Blue]{name}"
                                + (bBackslahedPath ? "\\" : "/")
                                + "[/Blue]";
                        }

                        return (name, path);
                    })
                    .ToArray()
            );

            options.AddRange(drives);

            string path = Prompter.Prompt(
                new SelectionPlugin<string>()
                    .UseResourcePack(FlowManager.GetResourceNamespace(resourceType))
                    .SetPrompt(prompt)
                    .AddDescriptions($"{FlowManager.CommonResourcePack.GetString("CurrentPath")}: {currentPath}")
                    .SetPageSize(pageSize)
                    .UseAutoComplete()
                    .AddOptions(options.ToArray())
                    .AddFlowCommonSelectionOptions()
            );

            if (FlowManager.TryHandleCommonSelectionResult(path))
            {
                result = string.Empty;
                return false;
            }

            if (path == "..")
            {
                currentPath = parent!;
                continue;
            }

            if (Directory.Exists(path))
            {
                currentPath = path;
                continue;
            }

            if (Path.GetExtension(path) == extension)
            {
                result = path;
                return true;
            }
        }
    }
}
