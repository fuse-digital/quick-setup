using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace FuseDigital.QuickSetup.Logging;

public interface ILoggingService : ISingletonDependency
{
    LogLevel Verbosity { get; set; }

    string LogDirectory { get; }

    void CreateLogger();

    void CloseAndFlush();
}