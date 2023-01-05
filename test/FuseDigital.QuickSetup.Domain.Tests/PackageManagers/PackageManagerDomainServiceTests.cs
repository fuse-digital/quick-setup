using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using FuseDigital.QuickSetup.Platforms;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace FuseDigital.QuickSetup.PackageManagers;

public class PackageManagerDomainServiceTests : QuickSetupDomainTestBase
{
    [Fact]
    public async Task Should_Execute_Pre_Install_And_Post_Install_Scripts_When_Provided()
    {
        // Arrange
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();
        var manager = CreatePackageManager();

        // Act
        await manager.InstallPackagesAsync(shell, console);

        // Assert
        A.CallTo(() => shell.RunProcessAsync(manager.PreInstall))
            .MustHaveHappened();
        A.CallTo(() => shell.RunProcessAsync(manager.Install, manager.Packages[0]))
            .MustHaveHappened();
        A.CallTo(() => shell.RunProcessAsync(manager.Install, manager.Packages[1]))
            .MustHaveHappened();
        A.CallTo(() => shell.RunProcessAsync(manager.PostInstall))
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
    public async Task Should_Not_Execute_Pre_Install_And_Post_Install_Scripts_When_Not_Provided()
    {
        // Arrange
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();
        var manager = CreatePackageManager();
        manager.PreInstall = "";
        manager.PostInstall = "";

        // Act
        await manager.InstallPackagesAsync(shell, console);

        // Assert
        A.CallTo(() => shell.RunProcessAsync(manager.PreInstall))
            .MustNotHaveHappened();
        A.CallTo(() => shell.RunProcessAsync(manager.Install, manager.Packages[0]))
            .MustHaveHappened();
        A.CallTo(() => shell.RunProcessAsync(manager.Install, manager.Packages[1]))
            .MustHaveHappened();
        A.CallTo(() => shell.RunProcessAsync(manager.PostInstall))
            .MustNotHaveHappened();
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
            await manager.InstallPackagesAsync(shell, console);
        });
        
        // Assert
        exception.ShouldNotBeNull();
        exception.Code.ShouldBe("PM-001");
    }
    
    [Fact]
    public async Task Should_Throw_An_Exception_When_No_Install_Command_Specified()
    {
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();
        var manager = CreatePackageManager();
        manager.Install = "";

        // Act
        var exception = await Should.ThrowAsync<BusinessException>(async () =>
        {
            await manager.InstallPackagesAsync(shell, console);
        });
        
        // Assert
        exception.ShouldNotBeNull();
        exception.Code.ShouldBe("PM-002");
    }
}