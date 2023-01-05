using System;
using Serilog;
using System.Threading.Tasks;
using FuseDigital.QuickSetup.Logging;
using FuseDigital.QuickSetup.Platforms;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;

namespace FuseDigital.QuickSetup.Cli;

public abstract class Program
{
    private static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        using var application = await AbpApplicationFactory.CreateAsync<QuickSetupCliModule>(options =>
        {
            options.UseAutofac();
            options.Services.AddLogging(c => c.AddSerilog());
        });

        await application.InitializeAsync();

        var loggingService = application.ServiceProvider.GetRequiredService<ILoggingService>();
        loggingService.CreateLogger();

        await application.ServiceProvider
            .GetRequiredService<QuickSetupAppService>()
            .RunAsync(args);

        await application.ShutdownAsync();

        loggingService.CloseAndFlush();
    }
}