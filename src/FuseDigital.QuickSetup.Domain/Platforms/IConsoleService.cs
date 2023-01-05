using System.Diagnostics.CodeAnalysis;
using Volo.Abp.DependencyInjection;

namespace FuseDigital.QuickSetup.Platforms;

public interface IConsoleService : ISingletonDependency
{
    Task WriteTitleAsync(
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, 
        params object[] arg);

    Task WriteHeadingAsync(
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, 
        params object[] arg);

    Task WriteLineAsync(
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string format = default,
        params object[] arg);
}