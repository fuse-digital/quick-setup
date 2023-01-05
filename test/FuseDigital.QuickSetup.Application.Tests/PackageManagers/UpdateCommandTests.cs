using System.Threading.Tasks;
using FakeItEasy;
using FuseDigital.QuickSetup.PackageManagers.Dto;
using FuseDigital.QuickSetup.Platforms;
using FuseDigital.QuickSetup.Repositories;
using FuseDigital.QuickSetup.Yaml;
using Shouldly;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Xunit;

namespace FuseDigital.QuickSetup.PackageManagers;

public class UpdateCommandTests : QuickSetupApplicationTestBase
{
    [Fact]
    public async Task Should_Update_Packages_For_All_Package_Managers()
    {
        // Arrange
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();

        var context = GetRequiredService<IYamlContext>();
        var repository = new PackageManagerRepository(context);
        await CopyFileAsync("packages.yml", repository.FilePath);

        var options = new UpdateOptions();
        var command = new UpdateCommand(repository, shell, console)
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>(),
        };

        // Act
        await command.ExecuteAsync(options);

        // Assert
        A.CallTo(() => shell.RunProcessAsync("package-manager1 upgrade"))
            .MustHaveHappened();

        A.CallTo(() => shell.RunProcessAsync("package-manager2 upgrade"))
            .MustHaveHappened();
    }

    [Fact]
    public async Task Should_Update_Packages_Only_For_Specified_Package_Manager()
    {
        // Arrange
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();

        var context = GetRequiredService<IYamlContext>();
        var repository = new PackageManagerRepository(context);
        await CopyFileAsync("packages.yml", repository.FilePath);

        var options = new UpdateOptions
        {
            PackageManager = "package-manager-1"
        };

        var command = new UpdateCommand(repository, shell, console)
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>(),
        };

        // Act
        await command.ExecuteAsync(options);

        // Assert
        A.CallTo(() => shell.RunProcessAsync("package-manager1 upgrade"))
            .MustHaveHappened();
        
        A.CallTo(() => shell.RunProcessAsync("package-manager2 upgrade"))
            .MustNotHaveHappened();
    }
    
    [Fact]
    public async Task Should_Throw_An_Exception_When_The_Specified_Package_Manager_Can_Not_Be_Found()
    {
        // Arrange
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();

        var context = GetRequiredService<IYamlContext>();
        var repository = new PackageManagerRepository(context);
        await CopyFileAsync("packages.yml", repository.FilePath);

        var options = new UpdateOptions
        {
            PackageManager = "package-manager-3"
        };

        var command = new UpdateCommand(repository, shell, console)
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>(),
        };

        // Act
        var exception = await Should.ThrowAsync<EntityNotFoundException>(async () =>
        {
            await command.ExecuteAsync(options);
        });
        
        // Arrange
        exception.ShouldNotBeNull();
        exception.Message.ShouldContain(options.PackageManager);
    }
}