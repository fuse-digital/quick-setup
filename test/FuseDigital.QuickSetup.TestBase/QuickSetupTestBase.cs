using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;

namespace FuseDigital.QuickSetup;

public abstract class QuickSetupTestBase<TStartupModule> : AbpIntegratedTest<TStartupModule>
    where TStartupModule : IAbpModule
{
    protected IOptions<QuickSetupOptions> Options { get; private set; }

    protected QuickSetupOptions Settings => Options.Value;

    protected ILogger Logger { get; }

    protected QuickSetupTestBase()
    {
        var loggerFactory = GetRequiredService<ILoggerFactory>();
        Logger = loggerFactory?.CreateLogger(GetType()) ?? NullLogger.Instance;

        Options = GetRequiredService<IOptions<QuickSetupOptions>>();
        Options.Value.UserProfile = Path.Combine(Options.Value.UserProfile, Guid.NewGuid().ToString());
    }

    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
    {
        options.UseAutofac();
    }

    protected string GetUnitTestMethodName()
    {
        return (new StackTrace()).GetFrames()
            .Select(x => x.GetMethod()?.Name ?? string.Empty)
            .Where(x => !string.IsNullOrEmpty(x))
            .FirstOrDefault(x => x.Contains("_") && x.Contains("Should"));
    }

    protected void LogDebug(string message = default, params object[] args)
    {
        var methodName = GetUnitTestMethodName();
        Logger.LogDebug("Method name {TestMethodName}", methodName);

        if (!string.IsNullOrEmpty(message))
        {
            Logger.LogDebug(message, args);
        }
    }

    public override void Dispose()
    {
        if (Directory.Exists(Settings.UserProfile))
        {
            SetAttributes(new DirectoryInfo(Settings.UserProfile));
            Directory.Delete(Settings.UserProfile, true);
        }

        base.Dispose();
    }

    private void SetAttributes(DirectoryInfo directory)
    {
        foreach (var subDirectory in directory.GetDirectories())
        {
            SetAttributes(subDirectory);
        }

        foreach (var file in directory.GetFiles())
        {
            file.Attributes = FileAttributes.Normal;
        }
    }
}