using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace FuseDigital.QuickSetup.UserFiles;

public class InitialiseCommand : QupCommandAsync, ITransientDependency
{
    private readonly IUserFileDomainService _userFileDomainService;
    private readonly QuickSetupOptions _options;

    public InitialiseCommand(IUserFileDomainService userFileDomainService, IOptions<QuickSetupOptions> options)
    {
        _userFileDomainService = userFileDomainService;
        _options = options.Value;
    }

    public override async Task ExecuteAsync(IQupCommandOptions options)
    {
        var init = (InitialiseOptions) options;

        if (_userFileDomainService.Exists())
        {
            const string message = "A source repository already exists in {0}";
            Logger.LogInformation(message, _options.UserProfile);
            Console.WriteLine(message, _options.UserProfile);
            return;
        }

        await _userFileDomainService.Initialise(init.Repository, init.DefaultBranchName);
    }
}