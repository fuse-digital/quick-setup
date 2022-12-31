using System.Threading.Tasks;
using FakeItEasy;
using FuseDigital.QuickSetup.UserFiles.Dto;
using Volo.Abp.DependencyInjection;
using Xunit;

namespace FuseDigital.QuickSetup.UserFiles;

public class CheckoutCommandTests : QuickSetupApplicationTestBase
{
    [Fact]
    public async Task Should_Not_Checkout_If_Repository_Already_Exists()
    {
        // Arrange
        var domainService = A.Fake<IUserFileDomainService>();
        A.CallTo(() => domainService.Exists()).Returns(true);

        var command = GetCheckoutCommand(domainService);
        var options = GetCheckoutOptions();

        // Act
        await command.ExecuteAsync(options);

        // Assert
        A.CallTo(() => domainService.CheckoutAsync(options.Repository, options.Branch))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Should_Checkout_If_Repository_Does_Not_Exists()
    {
        // Arrange
        var domainService = A.Fake<IUserFileDomainService>();
        A.CallTo(() => domainService.Exists()).Returns(false);

        var command = GetCheckoutCommand(domainService);
        var options = GetCheckoutOptions();

        // Act
        await command.ExecuteAsync(options);

        // Assert
        A.CallTo(() => domainService.CheckoutAsync(options.Repository, options.Branch))
            .MustHaveHappened();
    }

    private CheckoutOptions GetCheckoutOptions()
    {
        return new CheckoutOptions
        {
            Repository = "git@github.com:user/project.git",
            Branch = "master"
        };
    }

    private CheckoutCommand GetCheckoutCommand(IUserFileDomainService domainService)
    {
        return new CheckoutCommand(Options, domainService)
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>(),
        };
    }
}