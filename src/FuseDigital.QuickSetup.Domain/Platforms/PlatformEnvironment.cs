using FuseDigital.QuickSetup.Extensions;

namespace FuseDigital.QuickSetup.Platforms;

public static class PlatformEnvironment
{
    public static PlatformOperatingSystem CurrentOperatingSystem()
    {
        var item = Enum
            .GetNames(typeof(PlatformOperatingSystem))
            .FirstOrDefault(OperatingSystem.IsOSPlatform);

        if (item == null)
        {
            throw new NotSupportedException("Your current operating system is not supported");
        }

        return (PlatformOperatingSystem) Enum.Parse(typeof(PlatformOperatingSystem), item);
    }

    public static DefaultShellAttribute DefaultShell()
    {
        var operatingSystem = CurrentOperatingSystem();
        return operatingSystem.GetAttribute<DefaultShellAttribute>();
    }
}