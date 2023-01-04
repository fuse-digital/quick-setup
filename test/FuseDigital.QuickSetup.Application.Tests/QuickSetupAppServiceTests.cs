using System.Threading.Tasks;
using FakeItEasy;
using Shouldly;
using FuseDigital.QuickSetup.PackageManagers;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Xunit;

namespace FuseDigital.QuickSetup;

public class QuickSetupAppServiceTests : QuickSetupApplicationTestBase
{
    [Fact]
    public async Task Should_Wrap_And_Handle_Exceptions()
    {
        // Arrange
        var command = A.Fake<InstallCommand>();
        A.CallTo(() => command.ExecuteAsync(A<IQupCommandOptions>._))
            .Throws(new BusinessException("TEST-01", $"Test Exception"));

        var serviceProvide = A.Fake<IAbpLazyServiceProvider>();
        A.CallTo(() => serviceProvide.LazyGetRequiredService(typeof(InstallCommand)))
            .Returns(command);

        var appService = new QuickSetupAppService(Options)
        {
            LazyServiceProvider = serviceProvide
        };

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            await appService.RunAsync(new[] {"install"});
        });
    }
}