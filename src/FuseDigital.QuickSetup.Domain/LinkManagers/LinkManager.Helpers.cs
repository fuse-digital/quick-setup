using FuseDigital.QuickSetup.Platforms;
using Volo.Abp;

namespace FuseDigital.QuickSetup.LinkManagers;

public partial class LinkManager
{
    private void CheckOperatingSystem()
    {
        var operatingSystem = PlatformEnvironment.CurrentOperatingSystem();
        if (!RunsOn.Equals(operatingSystem))
        {
            throw new BusinessException("LM-001",
                $"Link manager is not configured to run on {operatingSystem}");
        }
    }
    
    private void CheckCreateLinkFormat()
    {
        if (string.IsNullOrEmpty(CreateLinkFormat))
        {
            throw new BusinessException("LM-002",
                $"No create link format specified for the {RunsOn} link manager");
        }

        if (!CreateLinkFormat.Contains("${source}", StringComparison.InvariantCultureIgnoreCase))
        {
            throw new BusinessException("LM-003",
                "The create link format does not contain a ${source} placeholder");
        }
        
        if (!CreateLinkFormat.Contains("${target}", StringComparison.InvariantCultureIgnoreCase))
        {
            throw new BusinessException("LM-004",
                "The create link format does not contain a ${target} placeholder");
        }
    }

    private async Task<int> RunCreateLinkCommandAsync(Link link, IShellDomainService shell, IConsoleService console)
    {
        await console.WriteLineAsync($"Create Link from {link.Source} to {link.Target}");
        return await shell.RunProcessAsync(GenerateCreateLinkCommand(link));
    }

    private async Task WriteDescriptionAsync(IConsoleService console)
    {
        await console.WriteHeadingAsync(RunsOn.ToString());
        await console.WriteLineAsync(Description);
    }
}