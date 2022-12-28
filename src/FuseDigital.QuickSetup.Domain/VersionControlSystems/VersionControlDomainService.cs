using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Domain.Services;

namespace FuseDigital.QuickSetup.VersionControlSystems;

public class VersionControlDomainService : DomainService, IVersionControlDomainService
{
    private readonly QuickSetupOptions _options;

    public VersionControlDomainService(IOptions<QuickSetupOptions> options)
    {
        _options = options.Value;
    }

    public void Clone(string sourceUrl, string relativePath)
    {
        var workingDirectory = _options.GetAbsolutePath(relativePath);
        Logger.LogInformation("The absolute path is {AbsolutePath}", workingDirectory);
        Logger.LogInformation("Cloning {Repository} into {RelativePath}", sourceUrl, relativePath);
        RunGitCommand(new[] { "clone", sourceUrl, workingDirectory });
    }

    private static void RunGitCommand(IEnumerable<string> args)
    {
        var startInfo = GitCommand(args);
        var process = Process.Start(startInfo);
        process?.WaitForExit();
    }

    private static ProcessStartInfo GitCommand(IEnumerable<string> args)
    {
        var arguments = args
            .Select(x => x.Contains(" ") ? $"\"{x}\"" : x)
            .JoinAsString(" ");

        return new ProcessStartInfo
        {
            FileName = "git",
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
        };
    }
}