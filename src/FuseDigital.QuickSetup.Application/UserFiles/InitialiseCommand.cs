using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace FuseDigital.QuickSetup.UserFiles;

public class InitialiseCommand : UserFilesCommandAsync, ITransientDependency
{
    public InitialiseCommand(IOptions<QuickSetupOptions> options, IUserFileDomainService userFileServiceDomainService) 
        : base(options, userFileServiceDomainService)
    {
    }

    public override async Task ExecuteAsync(IQupCommandOptions options)
    {
        var init = (InitialiseOptions)options;
        if (UserFileService.Exists())
        {
            DisplayRepositoryExists();
            return;
        }

        await UserFileService.InitialiseAsync(init.Repository, init.DefaultBranchName);
    }
}