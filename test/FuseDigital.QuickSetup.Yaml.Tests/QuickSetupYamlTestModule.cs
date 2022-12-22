using Volo.Abp.Modularity;

namespace FuseDigital.QuickSetup;

[DependsOn(
    typeof(QuickSetupTestBaseModule),
    typeof(QuickSetupYamlModule))]
public class QuickSetupYamlTestModule : AbpModule
{
}