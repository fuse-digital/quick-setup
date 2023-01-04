using System.Threading.Tasks;
using FuseDigital.QuickSetup.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.DependencyInjection;

namespace FuseDigital.QuickSetup;

public abstract class QupCommandAsync : IQupCommandAsync
{
    public IAbpLazyServiceProvider LazyServiceProvider { get; set; }

    private ILoggerFactory LoggerFactory => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();

    private ILoggingService LoggingService => LazyServiceProvider.LazyGetRequiredService<ILoggingService>();

    protected ILogger Logger => LazyServiceProvider
        .LazyGetService<ILogger>(provider => LoggerFactory?
            .CreateLogger(GetType().FullName) ?? NullLogger.Instance);

    public virtual Task ExecuteAsync(IQupCommandOptions options)
    {
        if (options is QupCommandOptions commandOptions)
        {
            LoggingService.Verbosity = commandOptions.Verbosity;
        }

        return Task.CompletedTask;
    }
}