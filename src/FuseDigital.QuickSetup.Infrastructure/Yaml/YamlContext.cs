using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FuseDigital.QuickSetup.Yaml;

public class YamlContext : IYamlContext, ISingletonDependency
{
    private readonly ISerializer _serializer;

    private readonly IDeserializer _deserializer;
    public QuickSetupOptions Options { get; }

    public YamlContext(IOptions<QuickSetupOptions> options)
    {
        Options = options.Value;
        var namingConvention = HyphenatedNamingConvention.Instance;

        _serializer = new SerializerBuilder()
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .WithNamingConvention(namingConvention)
            .Build();

        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(namingConvention)
            .Build();
    }

    public string Serialize<TInput>(TInput input)
    {
        return _serializer.Serialize(input);
    }

    public TInput Deserialize<TInput>(string input)
    {
        return _deserializer.Deserialize<TInput>(input);
    }

    public async Task<TOutput> LoadFromFileAsync<TOutput>(string path, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(path))
        {
            return default;
        }

        var input = await File.ReadAllTextAsync(path, cancellationToken);
        return Deserialize<TOutput>(input);
    }

    public async Task SaveToFileAsync<TEntity>(TEntity input, string path,
        CancellationToken cancellationToken = default)
    {
        var content = Serialize(input);

        var directoryFullName = new FileInfo(path).Directory?.FullName;
        if (directoryFullName != null)
        {
            Directory.CreateDirectory(directoryFullName);
        }

        await File.WriteAllTextAsync(path, content, cancellationToken);
    }
}