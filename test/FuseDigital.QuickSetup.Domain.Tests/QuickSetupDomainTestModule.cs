using Volo.Abp.Modularity;

namespace FuseDigital.QuickSetup;

[DependsOn(
    typeof(QuickSetupTestBaseModule),
    typeof(QuickSetupInfrastructureTestModule)
)]
public class QuickSetupDomainTestModule : AbpModule
{
}
