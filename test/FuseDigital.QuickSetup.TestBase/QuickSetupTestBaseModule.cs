using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
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

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Volo.Abp", LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.File($"../../../../../test-logs/qup-unit-test-logs.txt")
            .CreateLogger();
        
        context.Services.AddLogging(c => c.AddSerilog());
        
        base.ConfigureServices(context);

        Configure<QuickSetupOptions>(options =>
        {
            options.UserProfile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        });
    }
}