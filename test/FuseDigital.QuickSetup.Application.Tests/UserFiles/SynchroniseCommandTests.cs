using System.Threading.Tasks;
using FakeItEasy;
using Volo.Abp.DependencyInjection;
using Xunit;

namespace FuseDigital.QuickSetup.UserFiles;

public class SynchroniseCommandTests  : QuickSetupApplicationTestBase
{
    [Fact]
    public async Task Should_Not_Sync_If_Repository_Does_Not_Exists()
    {
        // Arrange
        var domainService = A.Fake<IUserFileDomainService>();
        A.CallTo(() => domainService.Exists()).Returns(false);
        var command = GetSyncCommand(domainService);

        // Act
        await command.ExecuteAsync(default);

        // Assert
        A.CallTo(() => domainService.SynchroniseAsync())
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Should_Sync_If_Repository_Exists()
    {
        // Arrange
        var domainService = A.Fake<IUserFileDomainService>();
        A.CallTo(() => domainService.Exists()).Returns(true);
        var command = GetSyncCommand(domainService);

        // Act
        await command.ExecuteAsync(default);

        // Assert
        A.CallTo(() => domainService.SynchroniseAsync())
            .MustHaveHappened();
    }

    private SynchroniseCommand GetSyncCommand(IUserFileDomainService domainService)
    {
        return new SynchroniseCommand(Options, domainService)
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>(),
        };
    }
}