using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;

namespace FuseDigital.QuickSetup;

public abstract class QuickSetupTestBase<TStartupModule> : AbpIntegratedTest<TStartupModule>
    where TStartupModule : IAbpModule
{
    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
    {
        options.UseAutofac();
    }
}
