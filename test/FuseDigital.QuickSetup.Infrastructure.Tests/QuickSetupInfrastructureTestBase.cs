using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FuseDigital.QuickSetup;

public abstract class QuickSetupInfrastructureTestBase : QuickSetupTestBase<QuickSetupInfrastructureTestModule>
{
    protected string GetFileContents(string fileName)
    {
        var fullyQualifiedName = $"{GetType().Namespace}.{fileName}";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fullyQualifiedName);
        if (stream == null)
        {
            return null;
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
    
    protected async Task CopyFileAsync(string source, string destination)
    {
        var content = GetFileContents(source);
        if (content != null)
        {
            var directoryFullName = new FileInfo(destination).Directory?.FullName;
            if (directoryFullName != null)
            {
                Directory.CreateDirectory(directoryFullName);
            }

            await File.WriteAllTextAsync(destination, content);
        }
    }
}