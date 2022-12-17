using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace FuseDigital.QuickSetup;

[DependsOn(
    typeof(AbpDddDomainModule)
)]
public class QuickSetupDomainModule : AbpModule
{
}
