using System.IO;
using Shouldly;
using Volo.Abp.DependencyInjection;
using Xunit;

namespace FuseDigital.QuickSetup.VersionControlSystems;

public class VersionControlSystemTests : QuickSetupDomainTestBase
{
    [Fact]
    public void Should_Clone_Project_In_Working_Directory()
    {
        // Arrange
        var versionControlSystem = new VersionControlDomainService(Options)
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>()
        };

        var relativePath = "~/projects/dot-files";
        var workingDirectory = Settings.GetAbsolutePath(relativePath);

        // Act
        versionControlSystem.Clone("https://github.com/jesperorb/dotfiles.git", relativePath);

        // Assert
        Directory.Exists(workingDirectory).ShouldBeTrue();
    }
}