using Volo.Abp.Modularity;

namespace FuseDigital.QuickSetup;

[DependsOn(
    typeof(QuickSetupTestBaseModule),
    typeof(QuickSetupInfrastructureModule))]
public class QuickSetupInfrastructureTestModule : AbpModule
{
}