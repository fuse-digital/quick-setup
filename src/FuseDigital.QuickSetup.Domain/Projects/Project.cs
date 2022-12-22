using System.ComponentModel.DataAnnotations;
using FuseDigital.QuickSetup.VersionControlSystems;
using Volo.Abp.Domain.Entities;

namespace FuseDigital.QuickSetup.Projects;

public class Project : Entity
{
    [Key]
    public string RelativePath { get; set; }

    public string Repository { get; set; }

    public override object[] GetKeys()
    {
        return new object[] {RelativePath};
    }

    public void Clone(IVersionControlDomainService versionControl)
    {
        versionControl.Clone(Repository, RelativePath);
    }
}