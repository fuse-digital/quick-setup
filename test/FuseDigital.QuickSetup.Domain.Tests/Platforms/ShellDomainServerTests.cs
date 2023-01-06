using System.IO;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace FuseDigital.QuickSetup.Platforms;

public class ShellDomainServerTests : QuickSetupDomainTestBase
{
    [Fact]
    public async Task Should_Run_Program_And_Return_Success_Results()
    {
        // Arrange
        var shell = GetRequiredService<IShellDomainService>();

        var filename = "README.md";
        Directory.CreateDirectory(Settings.UserProfile);
        await File.WriteAllTextAsync(Path.Combine(Settings.UserProfile, filename), "# qup-empty-dotfiles");

        // Act
        var result = await shell.RunProcessAsync("ls");

        // Assert
        result.ShouldBe(0);
    }

    [Fact]
    public async Task Should_Run_Program_And_Return_Error_Exit_Code_And_Error_Output()
    {
        // Arrange
        var shell = GetRequiredService<IShellDomainService>();

        // Act
        var result = await shell.RunProcessAsync("some-random-command", "--random-switch");

        // Assert
        result.ShouldBeGreaterThan(0);
    }
}