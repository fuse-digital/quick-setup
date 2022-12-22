using System.IO;
using System.Reflection;
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
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);

        Configure<QuickSetupOptions>(options =>
        {
            options.UserProfile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        });
    }
}