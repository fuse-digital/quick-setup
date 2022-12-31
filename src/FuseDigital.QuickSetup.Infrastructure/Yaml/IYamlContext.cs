using System.Threading;
using System.Threading.Tasks;

namespace FuseDigital.QuickSetup.Yaml;

public interface IYamlContext
{
    QuickSetupOptions Options { get; }

    string Serialize<TInput>(TInput input);

    TOutput Deserialize<TOutput>(string input);

    Task<TEntity> LoadFromFileAsync<TEntity>(string path, CancellationToken cancellationToken = default);

    Task SaveToFileAsync<TEntity>(TEntity input, string path, CancellationToken cancellationToken = default);
}