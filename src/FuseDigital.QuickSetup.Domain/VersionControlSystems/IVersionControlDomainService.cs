using Volo.Abp.Domain.Services;

namespace FuseDigital.QuickSetup.VersionControlSystems;

public interface IVersionControlDomainService : IDomainService
{
    string WorkingDirectory { get; set; }
    
    string RepositoryDirectory { get; }

    IEnumerable<string> Clone(string sourceUrl, string relativePath);

    IEnumerable<string> Init(string workingDirectory, IEnumerable<string> args = default);

    IEnumerable<string> Add(string pathSpec, IEnumerable<string> args = default);

    IEnumerable<string> Status(IEnumerable<string> args = default);

    IEnumerable<string> Commit(string message = default, IEnumerable<string> args = default);

    IEnumerable<string> RenameBranch(string name, IEnumerable<string> args = default);

    IEnumerable<string> AddRemote(string remoteUrl, IEnumerable<string> args = default);

    IEnumerable<string> PushSetUpstream(string branch, IEnumerable<string> args = default);
}