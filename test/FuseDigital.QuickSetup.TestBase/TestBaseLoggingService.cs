using FuseDigital.QuickSetup.Logging;
using Microsoft.Extensions.Logging;
using Serilog;

namespace FuseDigital.QuickSetup;

public class TestBaseLoggingService : ILoggingService
{
    public LogLevel Verbosity { get; set; }

    public string LogDirectory => "../../../../../test-logs/";

    public void CreateLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.File($"qup-unit-test-logs.txt")
            .CreateLogger();
    }

    public void CloseAndFlush()
    {
        Log.CloseAndFlush();
    }
}