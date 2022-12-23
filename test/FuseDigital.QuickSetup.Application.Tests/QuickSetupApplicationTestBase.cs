using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FuseDigital.QuickSetup.Yaml;

namespace FuseDigital.QuickSetup;

public abstract class QuickSetupApplicationTestBase : QuickSetupTestBase<QuickSetupApplicationTestModule>
{
    protected IYamlContext GetContext(string basePath)
    {
        var context = GetRequiredService<IYamlContext>();
        var folder = GetTestMethodName() ?? Guid.NewGuid().ToString();
        context.Options.BaseDirectory = Path.Combine(basePath, folder);

        return context;
    }

    private string GetTestMethodName()
    {
        return new StackTrace()
            .GetFrames()
            .FirstOrDefault(x => x.GetMethod()!.Name.Contains("_"))
            ?.GetMethod()
            ?.Name;
    }

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