using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using FuseDigital.QuickSetup.Platforms;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace FuseDigital.QuickSetup.PackageManagers;

public class UpdatePackageManagerTests : QuickSetupDomainTestBase
{
    [Fact]
    public async Task Should_Execute_Update_When_Provided()
    {
        // Arrange
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();
        var manager = CreatePackageManager();

        // Act
        await manager.UpdatePackagesAsync(shell, console);

        // Assert
        A.CallTo(() => shell.RunProcessAsync(manager.Update))
            .MustHaveHappened();
    }

    private static PackageManager CreatePackageManager()
    {
        return new PackageManager
        {
            Name = "package-manager",
            RunsOn = new[]
            {
                PlatformOperatingSystem.Linux,
                PlatformOperatingSystem.macOS,
                PlatformOperatingSystem.Windows,
            },
            PreInstall = "package-manager update",
            Install = "package-manager install -y",
            Update = "package-manager update && package-manager upgrade",
            PostInstall = "package-manager clean",
            Packages = new List<string>()
            {
                "package-01",
                "package-02"
            }
        };
    }

    [Fact]
    public async Task Should_Throw_An_Exception_When_Package_Manager_Is_Not_Configured_To_Run_On_Operating_System()
    {
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();
        var manager = CreatePackageManager();
        manager.RunsOn = new List<PlatformOperatingSystem>();

        // Act
        var exception = await Should.ThrowAsync<BusinessException>(async () =>
        {
            await manager.UpdatePackagesAsync(shell, console);
        });
        
        // Assert
        exception.ShouldNotBeNull();
        exception.Code.ShouldBe("PM-001");
    }
    
    [Fact]
    public async Task Should_Throw_An_Exception_When_No_Update_Command_Specified()
    {
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();
        var manager = CreatePackageManager();
        manager.Update = "";

        // Act
        var exception = await Should.ThrowAsync<BusinessException>(async () =>
        {
            await manager.UpdatePackagesAsync(shell, console);
        });
        
        // Assert
        exception.ShouldNotBeNull();
        exception.Code.ShouldBe("PM-003");
    }
}