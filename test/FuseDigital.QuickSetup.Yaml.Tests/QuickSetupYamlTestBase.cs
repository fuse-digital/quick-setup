using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FuseDigital.QuickSetup;

public abstract class QuickSetupYamlTestBase : QuickSetupTestBase<QuickSetupYamlTestModule>
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
}