namespace FuseDigital.QuickSetup.Platforms.Dto;

public class RunProgramResult
{
    public int ExitCode { get; set; }
    public IList<string> Output { get; set; }
}