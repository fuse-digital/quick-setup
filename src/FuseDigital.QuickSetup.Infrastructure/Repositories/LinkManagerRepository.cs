using System;
using System.Threading.Tasks;
using FuseDigital.QuickSetup.LinkManagers;
using FuseDigital.QuickSetup.Platforms;
using FuseDigital.QuickSetup.Yaml;
using Volo.Abp.Domain.Entities;

namespace FuseDigital.QuickSetup.Repositories;

public class LinkManagerRepository : YamlRepository<LinkManager>, ILinkManagerRepository
{
    public override string FileName => "links.yml";

    public LinkManagerRepository(IYamlContext context) : base(context)
    {
    }

    public override Func<LinkManager, object> SortOrder<TKey>() => link => link.RunsOn;

    public async Task<LinkManager> GetAsync(PlatformOperatingSystem runsOn)
    {
        var linkManager = await FindAsync(x => x.RunsOn.Equals(runsOn));

        if (linkManager == null)
        {
            throw new EntityNotFoundException($"No links configured for {runsOn}");
        }

        return linkManager;
    }

    public async Task<LinkManager> FindAsync(PlatformOperatingSystem runsOn)
    {
        return await FindAsync(x => x.RunsOn.Equals(runsOn));
    }

    public async Task<LinkManager> InsertOrUpdateAsync(LinkManager linkManager)
    {
        var manager = await FindAsync(linkManager.RunsOn);

        return manager == null
            ? await InsertAsync(linkManager)
            : await UpdateAsync(linkManager);
    }
}