using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.DependencyInjection;

namespace FuseDigital.QuickSetup;

public abstract class QupCommandAsync : IQupCommandAsync
{
    public IAbpLazyServiceProvider LazyServiceProvider { get; set; }

    protected ILoggerFactory LoggerFactory => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();

    protected ILogger Logger => LazyServiceProvider
        .LazyGetService<ILogger>(provider => LoggerFactory?
            .CreateLogger(GetType().FullName) ?? NullLogger.Instance);

    public abstract Task ExecuteAsync(IQupCommandOptions options);
}