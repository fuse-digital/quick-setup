using System;
using System.Threading.Tasks;
using FuseDigital.QuickSetup.UserFiles.Dto;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace FuseDigital.QuickSetup.UserFiles;

public class CheckoutCommand : UserFilesCommandAsync, ITransientDependency
{
    public CheckoutCommand(IOptions<QuickSetupOptions> options, IUserFileDomainService userFileServiceDomainService)
        : base(options, userFileServiceDomainService)
    {
    }

    public override async Task ExecuteAsync(IQupCommandOptions options)
    {
        await base.ExecuteAsync(options);

        var input = (CheckoutOptions) options;
        if (UserFileService.Exists())
        {
            DisplayRepositoryExists();
            return;
        }

        await UserFileService.CheckoutAsync(input.Repository, input.Branch);
    }
}