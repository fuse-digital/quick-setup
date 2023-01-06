namespace FuseDigital.QuickSetup.Platforms;

public enum PlatformOperatingSystem
{
    [DefaultShell("bash", "-c")]
    Linux,

    [DefaultShellAttribute("bash", "-c")]
    macOS,

    [DefaultShell("cmd", "/C")]
    Windows,
}