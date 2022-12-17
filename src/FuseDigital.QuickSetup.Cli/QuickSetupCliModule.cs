using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace FuseDigital.QuickSetup.Cli;

[DependsOn(
    typeof(QuickSetupApplicationModule),
    typeof(AbpAutofacModule)
)]
public class QuickSetupCliModule : AbpModule
{
}