using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace FuseDigital.QuickSetup;

[DependsOn(
    typeof(QuickSetupDomainModule),
    typeof(AbpDddApplicationModule)
)]
public class QuickSetupApplicationModule : AbpModule
{
}