using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FuseDigital.QuickSetup.UserFiles;

public abstract class UserFilesCommandAsync : QupCommandAsync
{
    private QuickSetupOptions Settings { get; }
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
}