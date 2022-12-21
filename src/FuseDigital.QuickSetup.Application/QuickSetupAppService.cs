using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Application.Services;

namespace FuseDigital.QuickSetup;

public class QuickSetupAppService : ApplicationService
{
    private QuickSetupOptions _options;

    public QuickSetupAppService(IOptions<QuickSetupOptions> options)
    {
        _options = options.Value;
    }

    public Task RunAsync(string[] args)
    {
        Logger.LogInformation("Quick Setup CLI (https://github.com/fuse-digital/quick-setup)");
        Logger.LogInformation("User profile directory is set {0}", _options.UserProfile);
        Logger.LogInformation("The base directory for QUP is set {0}", _options.BaseDirectory);
        Logger.LogInformation("The system directory separator is set to {0}", _options.DirectorySeparatorChar);
        
        return Task.CompletedTask;
    }
}