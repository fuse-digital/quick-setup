using Volo.Abp;

namespace FuseDigital.QuickSetup.VersionControlSystems;

public class RepositoryAlreadyExistsException : AbpException
{
    public RepositoryAlreadyExistsException(string message) : base(message)
    {
    }
}