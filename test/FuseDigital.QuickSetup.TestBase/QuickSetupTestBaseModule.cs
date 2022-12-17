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
}
