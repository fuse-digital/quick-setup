using Volo.Abp.Modularity;

namespace FuseDigital.QuickSetup;

[DependsOn(
    typeof(QuickSetupTestBaseModule)
    )]
public class QuickSetupDomainTestModule : AbpModule
{
}
