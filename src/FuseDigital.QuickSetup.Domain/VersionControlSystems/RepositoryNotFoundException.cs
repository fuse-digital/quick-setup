using Microsoft.Extensions.Logging;
using Volo.Abp;

namespace FuseDigital.QuickSetup.VersionControlSystems;

public class RepositoryNotFoundException : BusinessException
{
    public RepositoryNotFoundException(string message) : base("REP002", message)
    {
    }

    public new LogLevel LogLevel { get; set; } = LogLevel.Warning;
}