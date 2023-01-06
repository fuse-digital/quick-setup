using Volo.Abp;

namespace FuseDigital.QuickSetup.Platforms;

public class DefaultShellAttribute : Attribute
{
    public string Program { get; }

    public string CommandArgument { get; }

    public DefaultShellAttribute(string program, string commandArgument)
    {
        Check.NotNullOrEmpty(program, nameof(program));
        Check.NotNullOrEmpty(commandArgument, nameof(commandArgument));

        Program = program;
        CommandArgument = commandArgument;
    }
}