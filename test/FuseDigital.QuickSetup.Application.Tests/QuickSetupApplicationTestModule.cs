using Volo.Abp.Modularity;

namespace FuseDigital.QuickSetup;

[DependsOn(
    typeof(QuickSetupApplicationModule),
    typeof(QuickSetupDomainTestModule)
    )]
public class QuickSetupApplicationTestModule : AbpModule
{
}
