using System.Threading.Tasks;
using FuseDigital.QuickSetup.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using Shouldly;
using Volo.Abp;
using Volo.Abp.ObjectMapping;
using Xunit;

namespace FuseDigital.QuickSetup.Cli;

public class LogLevelMappingTests
{
    [Fact]
    public async Task Verbosity_round_trips_every_log_level()
    {
        using var application = await AbpApplicationFactory.CreateAsync<QuickSetupCliModule>(options =>
        {
            options.UseAutofac();
        });
        await application.InitializeAsync();

        var logging = application.ServiceProvider.GetRequiredService<ILoggingService>();
        var objectMapper = application.ServiceProvider.GetRequiredService<IObjectMapper>();
        var pairs = new (LogLevel Level, LogEventLevel EventLevel)[]
        {
            (LogLevel.Trace, LogEventLevel.Verbose),
            (LogLevel.Debug, LogEventLevel.Debug),
            (LogLevel.Information, LogEventLevel.Information),
            (LogLevel.Warning, LogEventLevel.Warning),
            (LogLevel.Error, LogEventLevel.Error),
            (LogLevel.Critical, LogEventLevel.Fatal),
            (LogLevel.None, (LogEventLevel)((int)LogEventLevel.Fatal + 1)),
        };

        foreach (var (level, eventLevel) in pairs)
        {
            objectMapper.Map<LogLevel, LogEventLevel>(level).ShouldBe(eventLevel);
            objectMapper.Map<LogEventLevel, LogLevel>(eventLevel).ShouldBe(level);
            logging.Verbosity = level;
            logging.Verbosity.ShouldBe(level);
        }

        await application.ShutdownAsync();
    }
}
