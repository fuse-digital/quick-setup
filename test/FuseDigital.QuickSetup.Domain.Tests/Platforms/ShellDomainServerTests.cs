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
        
        // Act
        var result = await shell.RunProcessAsync("ls", "-a");
        
        // Assert
        result.ShouldNotBeNull();
        result.ExitCode.ShouldBe(0);
        result.Output.Count.ShouldBe(3);
    }

    [Fact]
    public async Task Should_Run_Program_And_Return_Error_Exit_Code_And_Error_Output()
    {
        // Arrange
        var shell = GetRequiredService<IShellDomainService>();
        
        // Act
        var result = await shell.RunProcessAsync("some-random-command", "--random-switch");
        
        // Assert
        result.ShouldNotBeNull();
        result.ExitCode.ShouldBeGreaterThan(0);
        result.Output.Count.ShouldBeGreaterThan(0);
    }
}