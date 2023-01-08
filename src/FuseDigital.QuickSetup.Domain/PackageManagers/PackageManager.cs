using System.ComponentModel.DataAnnotations;
using FuseDigital.QuickSetup.Entities;
using FuseDigital.QuickSetup.Platforms;
using Volo.Abp;

namespace FuseDigital.QuickSetup.PackageManagers;

public class PackageManager : QupEntity
{
    [Key]
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

    public async Task InstallPackagesAsync(IShellDomainService shell, IConsoleService console)
    {
        CheckOperatingSystem();
        CheckInstallCommand();

        await WritePackageManagerInfoAsync(console);

        if (!string.IsNullOrEmpty(PreInstall))
        {
            await shell.RunProcessAsync(PreInstall);
        }

        var packages = Packages ?? new List<string>();
        for (var index = 0; index < packages.Count; index++)
        {
            await console.WriteLineAsync();
            await console.WriteLineAsync("[{0}/{1}] - Installing {2}", index + 1, packages.Count, packages[index]);
            await shell.RunProcessAsync(Install, packages[index]);
        }

        if (!string.IsNullOrEmpty(PostInstall))
        {
            await shell.RunProcessAsync(PostInstall);
        }
    }

    private void CheckInstallCommand()
    {
        if (string.IsNullOrEmpty(Install))
        {
            throw new BusinessException("PM-002",
                $"No install settings specified for the {Name} package manager");
        }
    }

    private void CheckOperatingSystem()
    {
        var operatingSystem = PlatformEnvironment.CurrentOperatingSystem();
        if (!RunsOn.Contains(operatingSystem))
        {
            throw new BusinessException("PM-001",
                $"{Name} package manager is not configured to run on {operatingSystem}");
        }
    }

    private async Task WritePackageManagerInfoAsync(IConsoleService console)
    {
        await console.WriteTitleAsync(Name);
        await console.WriteHeadingAsync(Description);
    }

    public async Task AddPackageAsync(string package, IShellDomainService shell, IConsoleService console)
    {
        if (Packages.Contains(package, StringComparer.InvariantCultureIgnoreCase))
        {
            throw new BusinessException("PM-003", $"Package {package} is already installed");
        }

        await WritePackageManagerInfoAsync(console);
        await console.WriteLineAsync("Installing {0}", package);

        var result = await shell.RunProcessAsync(Install, package);
        if (result == 0)
        {
            Packages.Add(package);
            Packages = Packages.OrderBy(x => x).ToList();
        }
    }

    public async Task UpdatePackagesAsync(IShellDomainService shell, IConsoleService console)
    {
        CheckOperatingSystem();
        CheckInstallCommand();

        if (string.IsNullOrEmpty(Update))
        {
            throw new BusinessException("PM-003",
                $"No update settings specified for the {Name} package manager");
        }

        await WritePackageManagerInfoAsync(console);
        await shell.RunProcessAsync(Update);
    }
}