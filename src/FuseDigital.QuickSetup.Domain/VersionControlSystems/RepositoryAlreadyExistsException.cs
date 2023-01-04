using Microsoft.Extensions.Logging;
using Volo.Abp;

namespace FuseDigital.QuickSetup.VersionControlSystems;

public class RepositoryAlreadyExistsException : BusinessException
{
    public RepositoryAlreadyExistsException(string message) : base("REP001", message)
    {
    }

    public new LogLevel LogLevel { get; set; } = LogLevel.Warning;
}