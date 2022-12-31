using Volo.Abp.Domain.Services;

namespace FuseDigital.QuickSetup.UserFiles;

public interface IUserFileDomainService : IDomainService
{
    bool Exists();

    Task InitialiseAsync(string sourceUrl, string defaultBranch);

    Task CheckoutAsync(string sourceUrl, string branch);

    Task<bool> PatternExistsAsync(string pattern);

    Task AddAsync(string pattern);

    Task SynchroniseAsync();
}