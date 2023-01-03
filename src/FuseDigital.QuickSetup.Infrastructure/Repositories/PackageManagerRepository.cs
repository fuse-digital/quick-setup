using System;
using System.Threading.Tasks;
using FuseDigital.QuickSetup.PackageManagers;
using FuseDigital.QuickSetup.Yaml;
using Volo.Abp.Domain.Entities;

namespace FuseDigital.QuickSetup.Repositories;

public class PackageManagerRepository : YamlRepository<PackageManager>, IPackageManagerRepository
{
    public override string FileName => "packages.yml";

    public PackageManagerRepository(IYamlContext context) : base(context)
    {
    }

    public async Task<PackageManager> GetAsync(string name)
    {
        var packageManager = await FindAsync(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (packageManager == null)
        {
            throw new EntityNotFoundException($"No {name} package manager configured");
        }

        return packageManager;
    }
}