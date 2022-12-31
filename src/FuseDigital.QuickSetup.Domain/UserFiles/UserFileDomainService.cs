using FuseDigital.QuickSetup.Entities;
using FuseDigital.QuickSetup.VersionControlSystems;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace FuseDigital.QuickSetup.UserFiles;

public class UserFileDomainService : DomainService, IUserFileDomainService
{
    private readonly IUserFileRepository _repository;
    private readonly IVersionControlDomainService _versionControl;
    private readonly QuickSetupOptions _options;

    public UserFileDomainService(IOptions<QuickSetupOptions> options,
        IUserFileRepository repository,
        IVersionControlDomainService versionControl)
    {
        _options = options.Value;
        _repository = repository;
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
        Check.NotNullOrEmpty(sourceUrl, nameof(sourceUrl));
        Check.NotNullOrEmpty(defaultBranch, nameof(defaultBranch));

        if (Exists())
        {
            throw new RepositoryAlreadyExistsException(_options.UserProfile);
        }

        _versionControl.Init(_options.UserProfile);
        await CreateIgnoreFile();
        await CreateIncludeFile();
        _versionControl.AddAll();
        _versionControl.Commit();
        _versionControl.RenameBranch(defaultBranch);
        _versionControl.AddRemote(sourceUrl);
        _versionControl.PushSetUpstream(defaultBranch);
    }

    public Task CheckoutAsync(string sourceUrl, string branch)
    {
        Check.NotNullOrEmpty(sourceUrl, nameof(sourceUrl));
        Check.NotNullOrEmpty(branch, nameof(branch));

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

    public async Task<bool> PatternExistsAsync(string pattern)
    {
        Check.NotNullOrEmpty(pattern, nameof(pattern));

        var entry = await _repository.FindAsync(x => x.Equals(pattern));
        return !string.IsNullOrEmpty(entry);
    }

    public async Task AddAsync(string pattern)
    {
        Check.NotNullOrEmpty(pattern, nameof(pattern));

        if (await PatternExistsAsync(pattern))
        {
            throw new EntityAlreadyExistsException(typeof(string), pattern);
        }

        await _repository.InsertAsync(pattern);
    }

    public async Task SynchroniseAsync()
    {
        if (!Exists())
        {
            throw new RepositoryNotFoundException(_options.UserProfile);
        }

        await StageUserFiles();
        var files = _versionControl.GetStagedFiles();
        if (files.Any())
        {
            _versionControl.Commit();
        }

        _versionControl.PullAndRebase();
        _versionControl.Push();
    }

    private async Task StageUserFiles()
    {
        var items = await _repository.GetListAsync();
        foreach (var item in items)
        {
            _versionControl.ForceAdd(item);
        }
        _versionControl.AddAll();
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