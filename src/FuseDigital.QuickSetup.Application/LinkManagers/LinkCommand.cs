using System.Threading.Tasks;
using FuseDigital.QuickSetup.LinkManagers.Dto;
using FuseDigital.QuickSetup.Platforms;
using Volo.Abp.DependencyInjection;

namespace FuseDigital.QuickSetup.LinkManagers;

public class LinkCommand : QupCommandAsync, ITransientDependency
{
    private readonly ILinkManagerRepository _repository;
    private readonly IShellDomainService _shell;
    private readonly IConsoleService _console;
    private readonly PlatformOperatingSystem _currentOperatingSystem;

    public LinkCommand(ILinkManagerRepository repository, IShellDomainService shell, IConsoleService console)
    {
        _repository = repository;
        _shell = shell;
        _console = console;
        _currentOperatingSystem = PlatformEnvironment.CurrentOperatingSystem();
    }

    public override async Task ExecuteAsync(IQupCommandOptions options)
    {
        await base.ExecuteAsync(options);

        var linkOptions = (LinkOptions) options;
        if (string.IsNullOrEmpty(linkOptions.Source) && string.IsNullOrEmpty(linkOptions.Target))
        {
            var linkManager = await _repository.GetAsync(_currentOperatingSystem);
            await linkManager.CreateLinksAsync(_shell, _console);
        }
        else
        {
            var link = new Link(linkOptions.Source, linkOptions.Target);
            var linkManager = await _repository.FindAsync(_currentOperatingSystem) ?? new LinkManager();
            await linkManager.AddLinkAsync(link, _shell, _console);
            await _repository.InsertOrUpdateAsync(linkManager);
        }
    }
}