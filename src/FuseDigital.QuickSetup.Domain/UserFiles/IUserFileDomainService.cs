using Volo.Abp.Domain.Services;

namespace FuseDigital.QuickSetup.UserFiles;

public interface IUserFileDomainService : IDomainService
{
    bool Exists();

    Task Initialise(string sourceUrl, string defaultBranch);
}