using FuseDigital.QuickSetup.VersionControlSystems;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace FuseDigital.QuickSetup.UserFiles;

public class UserFileDomainService : DomainService, IUserFileDomainService
{
    private readonly IVersionControlDomainService _versionControl;
    private readonly QuickSetupOptions _options;

    public UserFileDomainService(IOptions<QuickSetupOptions> options, IVersionControlDomainService versionControl)
    {
        _options = options.Value;
        _versionControl = versionControl;
        _versionControl.WorkingDirectory = _options.UserProfile;
    }

    public bool Exists()
    {
        var path = Path.Combine(_options.UserProfile, _versionControl.RepositoryDirectory);
        return Directory.Exists(path);
    }

    public async Task InitialiseAsync(string sourceUrl, string defaultBranch)
    {
        Check.NotNull(sourceUrl, nameof(sourceUrl));
        Check.NotNull(defaultBranch, nameof(defaultBranch));

        if (Exists())
        {
            throw new RepositoryAlreadyExistsException(_options.UserProfile);
        }

        _versionControl.Init(_options.UserProfile);
        await CreateIgnoreFile();
        await CreateIncludeFile();
        _versionControl.Add(".");
        _versionControl.Commit();
        _versionControl.RenameBranch(defaultBranch);
        _versionControl.AddRemote(sourceUrl);
        _versionControl.PushSetUpstream(defaultBranch);
    }

    public Task CheckoutAsync(string sourceUrl, string branch)
    {
        Check.NotNull(sourceUrl, nameof(sourceUrl));
        Check.NotNull(branch, nameof(branch));

        if (Exists())
        {
            throw new RepositoryAlreadyExistsException(_options.UserProfile);
        }

        _versionControl.Init(_options.UserProfile);
        _versionControl.AddRemote(sourceUrl);
        _versionControl.Fetch();
        _versionControl.Checkout(branch);

        return Task.CompletedTask;
    }

    private async Task CreateIgnoreFile()
    {
        var ignore = new[]
        {
            "/*",
            "!.gitignore",
            "!.gitinclude",
            ".DS_Store"
        };
        await File.WriteAllLinesAsync(Path.Combine(_options.UserProfile, ".gitignore"), ignore);
    }

    private async Task CreateIncludeFile()
    {
        var include = new[]
        {
            $"{_options.BaseDirectory}/"
        };
        await File.WriteAllLinesAsync(Path.Combine(_options.UserProfile, ".gitinclude"), include);
    }
}