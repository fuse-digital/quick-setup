using System.ComponentModel.DataAnnotations;
using FuseDigital.QuickSetup.Entities;
using FuseDigital.QuickSetup.VersionControlSystems;

namespace FuseDigital.QuickSetup.Projects;

public class Project : QupEntity
{
    [Key]
    public string RelativePath { get; set; }

    public string Repository { get; set; }

    public void Clone(IVersionControlDomainService versionControl)
    {
        versionControl.Clone(Repository, RelativePath);
    }
}