namespace FuseDigital.QuickSetup.Platforms;

public enum PlatformOperatingSystem
{
    [DefaultShell("bash")]
    Linux,

    [DefaultShellAttribute("bash")]
    macOS,

    [DefaultShell("cmd.exe")]
    Windows,
}