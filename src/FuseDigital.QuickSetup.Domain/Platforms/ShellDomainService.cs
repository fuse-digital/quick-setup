using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Domain.Services;

namespace FuseDigital.QuickSetup.Platforms;

public class ShellDomainService : DomainService, IShellDomainService
{
    private QuickSetupOptions Settings { get; }
    private string ShellProgram { get; }
    private string ShellCommandArgument { get; }

    public ShellDomainService(IOptions<QuickSetupOptions> options)
    {
        Settings = options.Value;
        var shellOptions = PlatformEnvironment.DefaultShell();
        ShellProgram = shellOptions.Program;
        ShellCommandArgument = shellOptions.CommandArgument;
    }

    public async Task<int> RunProcessAsync(params string[] arguments)
    {
        var command = arguments
            .JoinAsString(" ")
            .Replace("\"", "\"\"");

        var startInfo = new ProcessStartInfo
        {
            FileName = ShellProgram,
            Arguments = $"{ShellCommandArgument} \"{command}\"",
            UseShellExecute = false,
            RedirectStandardInput = false,
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            WorkingDirectory = Settings.UserProfile ?? ""
        };

        Logger.LogDebug("Execute {Program} from {WorkingDirectory}", ShellProgram, startInfo.WorkingDirectory);
        Logger.LogDebug("{Program} {Arguments}", startInfo.FileName, startInfo.Arguments);

        if (!string.IsNullOrEmpty(startInfo.WorkingDirectory) && !Directory.Exists(startInfo.WorkingDirectory))
        {
            Directory.CreateDirectory(startInfo.WorkingDirectory);
        }

        var process = Process.Start(startInfo);
        if (process == null)
        {
            return 1;
        }

        await process.WaitForExitAsync();

        return process.ExitCode;
    }
}