using System.Threading.Tasks;
using FakeItEasy;
using Volo.Abp.DependencyInjection;
using Xunit;

namespace FuseDigital.QuickSetup.UserFiles;

public class InitialiseCommandTests : QuickSetupApplicationTestBase
{
    [Fact]
    public async Task Should_Not_Initialise_If_Repository_Already_Exists()
    {
        // Arrange
        var domainService = A.Fake<IUserFileDomainService>();
        A.CallTo(() => domainService.Exists()).Returns(true);

        var command = GetInitialiseCommand(domainService);
        var options = GetInitialiseOptions();

        // Act
        await command.ExecuteAsync(options);

        // Assert
        A.CallTo(() => domainService.InitialiseAsync(options.Repository, options.DefaultBranchName))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Should_Initialise_If_Repository_Does_Not_Exists()
    {
        // Arrange
        var domainService = A.Fake<IUserFileDomainService>();
        A.CallTo(() => domainService.Exists()).Returns(false);

        var command = GetInitialiseCommand(domainService);
        var options = GetInitialiseOptions();

        // Act
        await command.ExecuteAsync(options);

        // Assert
        A.CallTo(() => domainService.InitialiseAsync(options.Repository, options.DefaultBranchName))
            .MustHaveHappened();
    }

    private InitialiseOptions GetInitialiseOptions()
    {
        return new InitialiseOptions
        {
            Repository = "git@github.com:user/project.git",
            DefaultBranchName = "master",
        };
    }

    private InitialiseCommand GetInitialiseCommand(IUserFileDomainService domainService)
    {
        return new InitialiseCommand(Options, domainService)
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>(),
        };
    }
}