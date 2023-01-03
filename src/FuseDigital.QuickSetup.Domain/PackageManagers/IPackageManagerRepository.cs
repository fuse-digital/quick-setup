using Volo.Abp.Domain.Repositories;

namespace FuseDigital.QuickSetup.PackageManagers;

public interface IPackageManagerRepository : IRepository<PackageManager>
{
    Task<PackageManager> GetAsync(string name);
}