using Volo.Abp;

namespace FuseDigital.QuickSetup.VersionControlSystems;

public class RepositoryNotFoundException : AbpException
{
    public RepositoryNotFoundException(string message) : base(message)
    {
    }
}