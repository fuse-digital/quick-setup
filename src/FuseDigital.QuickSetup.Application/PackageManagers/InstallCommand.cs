using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FuseDigital.QuickSetup.PackageManagers.Dto;
using FuseDigital.QuickSetup.Platforms;
using Volo.Abp.DependencyInjection;

namespace FuseDigital.QuickSetup.PackageManagers;

public class InstallCommand : QupCommandAsync, ITransientDependency
{
    private readonly IPackageManagerRepository _repository;
    private readonly IShellDomainService _shell;
    private readonly PlatformOperatingSystem _currentOperatingSystem;

    public InstallCommand(IPackageManagerRepository repository, IShellDomainService shell)
    {
        _repository = repository;
        _shell = shell;
        _currentOperatingSystem = PlatformEnvironment.CurrentOperatingSystem();
    }

    public override async Task ExecuteAsync(IQupCommandOptions options)
    {
        var installOptions = (InstallOptions) options;
        var packageManager = installOptions.PackageManager;
        var package = (installOptions.Package ?? Array.Empty<string>())
            .JoinAsString(" ");

        if (string.IsNullOrEmpty(packageManager))
        {
            await InstallAsync();
        }
        else if (string.IsNullOrEmpty(package))
        {
            await InstallPackageManagerAsync(packageManager);
        }
        else
        {
            await InstallPackage(packageManager, package);
        }
    }

    private async Task InstallAsync()
    {
        var packageManagers = await _repository
            .GetListAsync(x => x.RunsOn.Contains(_currentOperatingSystem));

        foreach (var packageManager in packageManagers)
        {
            await packageManager.InstallPackagesAsync(_shell);
        }
    }

    private async Task InstallPackageManagerAsync(string name)
    {
        var packageManager = await _repository.GetAsync(name);
        await packageManager.InstallPackagesAsync(_shell);
    }

    private async Task InstallPackage(string name, string package)
    {
        var packageManager = await _repository.GetAsync(name);
        await packageManager.AddPackageAsync(package, _shell);
        await _repository.UpdateAsync(packageManager);
    }
}