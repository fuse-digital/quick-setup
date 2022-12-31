using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FuseDigital.QuickSetup.UserFiles;

public abstract class UserFilesCommandAsync : QupCommandAsync
{
    protected QuickSetupOptions Settings { get; }
    protected IUserFileDomainService UserFileService { get; }

    protected UserFilesCommandAsync(IOptions<QuickSetupOptions> options, IUserFileDomainService userFileServiceDomainService)
    {
        Settings = options.Value;
        UserFileService = userFileServiceDomainService;
    }

    protected void DisplayRepositoryExists()
    {
        const string message = "A local repository already exists in {0}";
        Logger.LogInformation(message, Settings.UserProfile);
        Console.WriteLine(message, Settings.UserProfile);
    }
    
    protected void DisplayRepositoryNotFound()
    {
        const string message = "The local repository does not exists. Please run the `qup init` to create or `qup checkout` to clone an existing remote repository";
        Logger.LogInformation(message);
        Console.WriteLine(message);
    }
}