namespace FuseDigital.QuickSetup.Platforms;

public enum PlatformOperatingSystem
{
    [DefaultShell("bash", "-c")]
    [DefaultLinkCommand("sudo ln -sfv ${source} ${target}")]
    Linux,

    [DefaultShell("bash", "-c")]
    [DefaultLinkCommand("sudo ln -sfv ${source} ${target}")]
    macOS,

    [DefaultShell("cmd", "/C")]
    [DefaultLinkCommand("mklink /j ${target} ${source}")]
    Windows,
}