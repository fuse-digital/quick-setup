using System;
using System.Threading.Tasks;
using FuseDigital.QuickSetup.UserFiles.Dto;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace FuseDigital.QuickSetup.UserFiles;

public class SynchroniseCommand : UserFilesCommandAsync, ITransientDependency
{
    public SynchroniseCommand(IOptions<QuickSetupOptions> options, IUserFileDomainService userFileServiceDomainService)
        : base(options, userFileServiceDomainService)
    {
    }

    public override async Task ExecuteAsync(IQupCommandOptions options)
    {
        await base.ExecuteAsync(options);

        if (!UserFileService.Exists())
        {
            DisplayRepositoryNotFound();
            return;
        }

        await UserFileService.SynchroniseAsync();
    }
}