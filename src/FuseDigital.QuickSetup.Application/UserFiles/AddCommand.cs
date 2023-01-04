using System;
using System.Threading.Tasks;
using FuseDigital.QuickSetup.UserFiles.Dto;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace FuseDigital.QuickSetup.UserFiles;

public class AddCommand : UserFilesCommandAsync, ITransientDependency
{
    public AddCommand(IOptions<QuickSetupOptions> options, IUserFileDomainService userFileServiceDomainService)
        : base(options, userFileServiceDomainService)
    {
    }

    public override async Task ExecuteAsync(IQupCommandOptions options)
    {
        await base.ExecuteAsync(options);

        var input = (AddOptions) options;
        if (await UserFileService.PatternExistsAsync(input.Pattern))
        {
            DisplayPatternExists(input.Pattern);
            return;
        }

        await UserFileService.AddAsync(input.Pattern);
    }

    private void DisplayPatternExists(string pattern)
    {
        const string message = "An entry for {0} already exists";
        Logger.LogInformation(message, pattern);
        Console.WriteLine(message, pattern);
    }
}