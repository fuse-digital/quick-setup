using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FuseDigital.QuickSetup.Text;

public interface ITextContext
{
    QuickSetupOptions Options { get; }

    Task<IList<string>> LoadFromFileAsync(string path, CancellationToken cancellationToken = default);

    Task SaveToFileAsync(IEnumerable<string> items, string path, CancellationToken cancellationToken = default);
}