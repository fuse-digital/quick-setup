using System.ComponentModel.DataAnnotations;
using FuseDigital.QuickSetup.Entities;
using FuseDigital.QuickSetup.Extensions;
using FuseDigital.QuickSetup.Platforms;
using Volo.Abp;

namespace FuseDigital.QuickSetup.LinkManagers;

public partial class LinkManager : QupEntity
{
    public LinkManager()
    {
        RunsOn = PlatformEnvironment.CurrentOperatingSystem();
        CreateLinkFormat = RunsOn.GetAttribute<DefaultLinkCommandAttribute>().Format;
    }

    public async Task CreateLinksAsync(IShellDomainService shell, IConsoleService console)
    {
        CheckOperatingSystem();
        CheckCreateLinkFormat();
        await WriteDescriptionAsync(console);

        for (var index = 0; index < Links.Count; index++)
        {
            await console.WriteLineAsync();
            await console.WriteLineAsync($"[{index + 1}/{Links.Count}]");
            await RunCreateLinkCommandAsync(Links[index], shell, console);
        }
    }

    public string GenerateCreateLinkCommand(Link link)
    {
        return CreateLinkFormat
            .Replace("${source}", link.Source, StringComparison.InvariantCultureIgnoreCase)
            .Replace("${target}", link.Target, StringComparison.InvariantCultureIgnoreCase);
    }

    public async Task AddLinkAsync(Link link, IShellDomainService shell, IConsoleService console)
    {
        link.ValidateModel();
        CheckOperatingSystem();
        CheckCreateLinkFormat();

        var source = link.Source;
        var target = link.Target;

        var exists = Links.Any(x =>
            x.Source.Equals(source, StringComparison.InvariantCultureIgnoreCase)
            && x.Target.Equals(target, StringComparison.InvariantCultureIgnoreCase));

        if (exists)
        {
            throw new BusinessException("LM-001", $"Link from {source} to {target} already exists");
        }

        await WriteDescriptionAsync(console);
        var result = await RunCreateLinkCommandAsync(link, shell, console);
        if (result == 0)
        {
            Links.Add(link);
            Links = Links.OrderBy(x => x.Source).ToList();
        }
    }
}