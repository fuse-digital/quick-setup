using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace FuseDigital.QuickSetup.Text;

public class TextContext : ITextContext, ISingletonDependency
{
    public QuickSetupOptions Options { get; }

    public TextContext(IOptions<QuickSetupOptions> options)
    {
        Options = options.Value;
    }

    public async Task<IList<string>> LoadFromFileAsync(string path, CancellationToken cancellationToken = default)
    {
        Check.NotNullOrEmpty(path, nameof(path));

        return File.Exists(path)
            ? (await File.ReadAllLinesAsync(path, cancellationToken)).ToList()
            : new List<string>();
    }

    public async Task SaveToFileAsync(IEnumerable<string> items,
        string path,
        CancellationToken cancellationToken = default)
    {
        Check.NotNullOrEmpty(path, nameof(path));

        var file = new FileInfo(path);
        if (file.Directory is {Exists: false})
        {
            Directory.CreateDirectory(file.Directory.FullName);
        }

        await File.WriteAllLinesAsync(path, items, cancellationToken);
    }
}