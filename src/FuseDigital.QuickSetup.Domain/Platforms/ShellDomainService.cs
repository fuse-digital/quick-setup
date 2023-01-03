using System.Diagnostics;
using FuseDigital.QuickSetup.Platforms.Dto;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Domain.Services;

namespace FuseDigital.QuickSetup.Platforms;

public class ShellDomainService : DomainService, IShellDomainService
{
    private QuickSetupOptions Settings { get; }
    private string ShellProgram { get; }
    private string ShellExitCommand { get; }

    public ShellDomainService(IOptions<QuickSetupOptions> options)
    {
        Settings = options.Value;
        var shellOptions = PlatformEnvironment.DefaultShell();
        ShellProgram = shellOptions.Program;
        ShellExitCommand = shellOptions.ExitCommand;
    }

    public async Task<RunProgramResult> RunProcessAsync(params string[] arguments)
    {
        var startInfo = CreateProcessStartInfo(ShellProgram);

        Logger.LogDebug("Execute {Program} from {WorkingDirectory}", ShellProgram, startInfo.WorkingDirectory);
        Logger.LogDebug("{Program} {Arguments}", ShellProgram, arguments);

        if (!string.IsNullOrEmpty(startInfo.WorkingDirectory) && !Directory.Exists(startInfo.WorkingDirectory))
        {
            Directory.CreateDirectory(startInfo.WorkingDirectory);
        }

        var process = Process.Start(startInfo);
        if (process == null)
        {
            return new RunProgramResult
            {
                ExitCode = 1,
            };
        }

        var args = arguments
            .JoinAsString(" ");

        await process.StandardInput.WriteLineAsync(args);
        await process.StandardInput.WriteLineAsync(ShellExitCommand);

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        Logger.LogDebug("program {Program} output {Output}", ShellProgram, output);

        return new RunProgramResult
        {
            ExitCode = process.ExitCode,
            Output = process.ExitCode == 0
                ? output.SplitToLines()
                : error?.SplitToLines(),
        };
    }

    private ProcessStartInfo CreateProcessStartInfo(string program)
    {
        return new ProcessStartInfo
        {
            FileName = program,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = Settings.UserProfile ?? ""
        };
    }
}