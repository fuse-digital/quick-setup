using System.ComponentModel.DataAnnotations;
using FuseDigital.QuickSetup.Platforms;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace FuseDigital.QuickSetup.PackageManagers;

public class PackageManager : Entity
{
    [Display(Description = "The name of the package manager")]
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9_-]+$")]
    public string Name { get; set; }

    [Display(Description = "The package manager supported operating systems.")]
    public IList<PlatformOperatingSystem> RunsOn { get; set; } = new List<PlatformOperatingSystem>();

    [Display(Description = "The description of the package manager")]
    public string Description { get; set; }

    [Display(Description = "The pre-install commands that needs to be executed for the package manager")]
    public string PreInstall { get; set; }

    [Display(Description = "The commands that needs to be executed for the package manager to install a packages")]
    [Required]
    public string Install { get; set; }

    [Display(Description = "Update all the packages that have been installed by the package manager")]
    public string Update { get; set; }

    [Display(Description = "The post install commands that needs to be executed for the package manager")]
    public string PostInstall { get; set; }

    [Display(Description = "The list of packages that are being maintained by the package manager")]
    public IList<string> Packages { get; set; } = new List<string>();

    public override object[] GetKeys()
    {
        return new object[] {Name};
    }

    public async Task InstallPackagesAsync(IShellDomainService shell)
    {
        var operatingSystem = PlatformEnvironment.CurrentOperatingSystem();
        if (!RunsOn.Contains(operatingSystem))
        {
            throw new BusinessException("PM-001",
                $"{Name} package manager is not configured to run on {operatingSystem}");
        }

        if (string.IsNullOrEmpty(Install))
        {
            throw new BusinessException("PM-002",
                $"No install settings specified for the {Name} package manager");
        }

        if (!string.IsNullOrEmpty(PreInstall))
        {
            await shell.RunProcessAsync(PreInstall);
        }

        foreach (var package in Packages ?? new List<string>())
        {
            await shell.RunProcessAsync(Install, package);
        }

        if (!string.IsNullOrEmpty(PostInstall))
        {
            await shell.RunProcessAsync(PostInstall);
        }
    }

    public async Task AddPackageAsync(string package, IShellDomainService shell)
    {
        if (Packages.Contains(package, StringComparer.InvariantCultureIgnoreCase))
        {
            throw new BusinessException("PM-003", $"Package {package} is already installed");
        }

        var result = await shell.RunProcessAsync(Install, package);

        if (result.ExitCode == 0)
        {
            Packages.Add(package);
            Packages = Packages.OrderBy(x => x).ToList();
        }
    }
}