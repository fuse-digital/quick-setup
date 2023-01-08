using FuseDigital.QuickSetup.Platforms;
using Volo.Abp.Domain.Repositories;

namespace FuseDigital.QuickSetup.LinkManagers;

public interface ILinkManagerRepository : IRepository<LinkManager>
{
    Task<LinkManager> GetAsync(PlatformOperatingSystem runsOn);

    Task<LinkManager> FindAsync(PlatformOperatingSystem runsOn);

    Task<LinkManager> InsertOrUpdateAsync(LinkManager linkManager);
}