using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;

namespace FuseDigital.QuickSetup;

public abstract class QuickSetupTestBase<TStartupModule> : AbpIntegratedTest<TStartupModule>
    where TStartupModule : IAbpModule
{
    protected IOptions<QuickSetupOptions> Options { get; private set; }

    protected QuickSetupOptions Settings => Options.Value;

    protected QuickSetupTestBase()
    {
        Options = GetRequiredService<IOptions<QuickSetupOptions>>();
        Options.Value.UserProfile = Path.Combine(Options.Value.UserProfile, Guid.NewGuid().ToString());
    }

    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
    {
        options.UseAutofac();
    }

    public override void Dispose()
    {
        if (Directory.Exists(Settings.UserProfile))
        {
            Directory.Delete(Settings.UserProfile, true);
        }

        base.Dispose();
    }
}