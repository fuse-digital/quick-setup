using System;
using System.IO;
using System.Reflection;
using FuseDigital.QuickSetup.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace FuseDigital.QuickSetup;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpTestBaseModule),
    typeof(QuickSetupDomainModule)
)]
public class QuickSetupTestBaseModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        context.Services.AddLogging(c => c.AddSerilog());
        base.ConfigureServices(context);

        Configure<QuickSetupOptions>(options =>
        {
            options.UserProfile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        base.OnApplicationInitialization(context);
        context.ServiceProvider.GetRequiredService<ILoggingService>().CreateLogger();
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        base.OnApplicationShutdown(context);
        context.ServiceProvider.GetRequiredService<ILoggingService>().CloseAndFlush();
    }
}