using Volo.Abp.Domain.Services;

namespace FuseDigital.QuickSetup.UserFiles;

public interface IUserFileDomainService : IDomainService
{
    bool Exists();

    Task InitialiseAsync(string sourceUrl, string defaultBranch);
}