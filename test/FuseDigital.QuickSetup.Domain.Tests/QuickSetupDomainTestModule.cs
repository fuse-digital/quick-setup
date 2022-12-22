using Volo.Abp.Modularity;

namespace FuseDigital.QuickSetup;

[DependsOn(
    typeof(QuickSetupTestBaseModule),
    typeof(QuickSetupYamlTestModule)
)]
public class QuickSetupDomainTestModule : AbpModule
{
}
