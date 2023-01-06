using System.Threading.Tasks;
using FuseDigital.QuickSetup.PackageManagers.Dto;
using FuseDigital.QuickSetup.Platforms;
using Volo.Abp.DependencyInjection;

namespace FuseDigital.QuickSetup.PackageManagers;

public class UpdateCommand : QupCommandAsync, ITransientDependency
{
    private readonly IPackageManagerRepository _repository;
    private readonly IShellDomainService _shell;
    private readonly IConsoleService _console;
    private readonly PlatformOperatingSystem _currentOperatingSystem;

    public UpdateCommand(IPackageManagerRepository repository, IShellDomainService shell, IConsoleService console)
    {
        _repository = repository;
        _shell = shell;
        _console = console;
        _currentOperatingSystem = PlatformEnvironment.CurrentOperatingSystem();
    }

    public override async Task ExecuteAsync(IQupCommandOptions options)
    {
        await base.ExecuteAsync(options);

        var updateOptions = (UpdateOptions) options;
        if (!string.IsNullOrEmpty(updateOptions.PackageManager))
        {
            await UpdatePackageManagerAsync(updateOptions.PackageManager);
        }
        else
        {
            await UpdateAsync();
        }
    }

    private async Task UpdateAsync()
    {
        var packageManagers = await _repository
            .GetListAsync(x => x.RunsOn.Contains(_currentOperatingSystem) && !string.IsNullOrEmpty(x.Update));

        foreach (var packageManager in packageManagers)
        {
            await packageManager.UpdatePackagesAsync(_shell, _console);
        }
    }

    private async Task UpdatePackageManagerAsync(string name)
    {
        var packageManager = await _repository.GetAsync(name);
        await packageManager.UpdatePackagesAsync(_shell, _console);
    }
}