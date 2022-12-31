using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FuseDigital.QuickSetup;

[DependsOn(
    typeof(QuickSetupDomainModule)
)]
public class QuickSetupInfrastructureModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);

        context.Services.AddSingleton(
            new SerializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .Build());

        context.Services.AddSingleton(
            new DeserializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .Build());
    }
}