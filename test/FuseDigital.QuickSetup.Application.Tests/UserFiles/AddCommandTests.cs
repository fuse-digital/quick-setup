using System.Threading.Tasks;
using FakeItEasy;
using FuseDigital.QuickSetup.UserFiles.Dto;
using Volo.Abp.DependencyInjection;
using Xunit;

namespace FuseDigital.QuickSetup.UserFiles;

public class AddCommandTests : QuickSetupApplicationTestBase
{
    [Fact]
    public async Task Should_Not_Add_If_Pattern_Already_Exists()
    {
        // Arrange
        var domainService = A.Fake<IUserFileDomainService>();
        A.CallTo(() => domainService.PatternExistsAsync(A<string>._)).Returns(true);

        var command = GetAddCommand(domainService);
        var options = GetAddOptions();

        // Act
        await command.ExecuteAsync(options);

        // Assert
        A.CallTo(() => domainService.AddAsync(options.Pattern))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Should_Checkout_If_Repository_Does_Not_Exists()
    {
        // Arrange
        var domainService = A.Fake<IUserFileDomainService>();
        A.CallTo(() => domainService.Exists()).Returns(false);

        var command = GetAddCommand(domainService);
        var options = GetAddOptions();

        // Act
        await command.ExecuteAsync(options);

        // Assert
        A.CallTo(() => domainService.AddAsync(options.Pattern))
            .MustHaveHappened();
    }

    private AddOptions GetAddOptions()
    {
        return new AddOptions
        {
            Pattern = ".gitconfig"
        };
    }

    private AddCommand GetAddCommand(IUserFileDomainService domainService)
    {
        return new AddCommand(Options, domainService)
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>(),
        };
    }
}