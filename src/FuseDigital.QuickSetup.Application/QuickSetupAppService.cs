using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.Application.Services;

namespace FuseDigital.QuickSetup;

public class QuickSetupAppService : ApplicationService
{
    public Task RunAsync(string[] args)
    {
        Logger.LogInformation("Quick Setup CLI (https://qup.io)");
        return Task.CompletedTask;
    }
}