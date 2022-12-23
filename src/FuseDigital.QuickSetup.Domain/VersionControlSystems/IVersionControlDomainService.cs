using Volo.Abp.Domain.Services;

namespace FuseDigital.QuickSetup.VersionControlSystems;

public interface IVersionControlDomainService : IDomainService
{
    void Clone(string sourceUrl, string relativePath);
}