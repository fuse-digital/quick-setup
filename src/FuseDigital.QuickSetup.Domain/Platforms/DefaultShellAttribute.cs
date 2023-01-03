namespace FuseDigital.QuickSetup.Platforms;

public class DefaultShellAttribute : Attribute
{
    public string Program { get; set; }

    public string ExitCommand { get; set; }

    public DefaultShellAttribute(string program, string exitCommand = "exit")
    {
        Program = program;
        ExitCommand = exitCommand;
    }
}