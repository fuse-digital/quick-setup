using System.Threading.Tasks;

namespace FuseDigital.QuickSetup;

public interface IQupCommandAsync
{
    Task ExecuteAsync(IQupCommandOptions options);
}