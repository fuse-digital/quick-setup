using System;
using System.Runtime.InteropServices;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace FuseDigital.QuickSetup.Cli;

[DependsOn(
    typeof(QuickSetupApplicationModule),
    typeof(AbpAutofacModule)
)]
public class QuickSetupCliModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
        Configure<QuickSetupOptions>(options =>
        {
            options.UserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        });
    }
}