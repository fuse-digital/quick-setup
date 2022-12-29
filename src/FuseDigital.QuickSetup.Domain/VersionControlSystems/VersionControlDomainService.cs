using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace FuseDigital.QuickSetup.VersionControlSystems;

public class VersionControlDomainService : DomainService, IVersionControlDomainService
{
    public string WorkingDirectory { get; set; }

    public string RepositoryDirectory => ".git";

    private readonly QuickSetupOptions _options;

    public VersionControlDomainService(IOptions<QuickSetupOptions> options)
    {
        _options = options.Value;
    }

    public IEnumerable<string> Clone(string sourceUrl, string relativePath)
    {
        Check.NotNullOrEmpty(sourceUrl, nameof(sourceUrl));
        Check.NotNullOrEmpty(relativePath, nameof(relativePath));

        var workingDirectory = _options.GetAbsolutePath(relativePath);
        return RunGitCommand(new[] { "clone", sourceUrl, workingDirectory });
    }

    public IEnumerable<string> Init(string workingDirectory, IEnumerable<string> args = default)
    {
        Check.NotNullOrEmpty(workingDirectory, nameof(workingDirectory));
        return RunGitCommand(new[] { "init", workingDirectory }, args);
    }

    public IEnumerable<string> SetConfig(string key, string value, bool global = false)
    {
        Check.NotNullOrEmpty(key, nameof(key));
        Check.NotNullOrEmpty(value, nameof(value));

        var command = new List<string>()
        {
            "config"
        };

        if (global)
        {
            command.Add("--global");
        }

        command.AddRange(new[] { key, value });

        return RunGitCommand(command);
    }

    public IEnumerable<string> Add(string pathSpec, IEnumerable<string> args = default)
    {
        Check.NotNullOrEmpty(pathSpec, nameof(pathSpec));
        return RunGitCommand(new[] { "add", pathSpec }, args);
    }

    public IEnumerable<string> Status(IEnumerable<string> args = default)
    {
        return RunGitCommand(new[] { "status" }, args);
    }

    public IEnumerable<string> Commit(string message = default, IEnumerable<string> args = default)
    {
        var commitMessage = string.IsNullOrEmpty(message)
            ? $"{DateTime.Now:yyyy.MM.dd_hh:mm:ss} from {Environment.UserName}@{Environment.MachineName}"
            : message;
        return RunGitCommand(new[] { "commit", "-m", commitMessage }, args);
    }

    public IEnumerable<string> RenameBranch(string name, IEnumerable<string> args = default)
    {
        Check.NotNullOrEmpty(name, nameof(name));
        return RunGitCommand(new[] { "branch", "-M", name }, args);
    }

    public IEnumerable<string> AddRemote(string remoteUrl, IEnumerable<string> args = default)
    {
        Check.NotNullOrEmpty(remoteUrl, nameof(remoteUrl));
        return RunGitCommand(new[] { "remote", "add", "origin", remoteUrl }, args);
    }

    public IEnumerable<string> PushSetUpstream(string branch, IEnumerable<string> args = default)
    {
        Check.NotNullOrEmpty(branch, nameof(branch));
        return RunGitCommand(new[] { "push", "-u", "origin", branch }, args);
    }

    private IEnumerable<string> RunGitCommand(IEnumerable<string> command, IEnumerable<string> args = default)
    {
        var arguments = command.Concat(args ?? new List<string>());
        Logger.LogInformation("Execute {Command} with {Arguments}", nameof(RunGitCommand), arguments);

        var startInfo = GitCommand(arguments);
        var process = Process.Start(startInfo);
        var output = process?.StandardOutput.ReadToEnd();
        process?.WaitForExit();

        return output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
    }

    private ProcessStartInfo GitCommand(IEnumerable<string> args)
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
            WorkingDirectory = WorkingDirectory ?? ""
        };
    }
}