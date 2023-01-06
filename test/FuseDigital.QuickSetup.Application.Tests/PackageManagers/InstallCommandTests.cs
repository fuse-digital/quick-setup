using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FuseDigital.QuickSetup.PackageManagers.Dto;
using FuseDigital.QuickSetup.Platforms;
using FuseDigital.QuickSetup.Platforms.Dto;
using FuseDigital.QuickSetup.Repositories;
using FuseDigital.QuickSetup.Yaml;
using Shouldly;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Xunit;

namespace FuseDigital.QuickSetup.PackageManagers;

public class InstallCommandTests : QuickSetupApplicationTestBase
{
    [Fact]
    public async Task Should_Install_Packages_For_All_Package_Managers()
    {
        // Arrange
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();

        var context = GetRequiredService<IYamlContext>();
        var repository = new PackageManagerRepository(context);
        await CopyFileAsync("packages.yml", repository.FilePath);

        var options = new InstallOptions();
        var command = new InstallCommand(repository, shell, console)
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>(),
        };

        // Act
        await command.ExecuteAsync(options);

        // Assert
        A.CallTo(() => shell.RunProcessAsync("package-manager-1 update"))
            .MustHaveHappened();
        A.CallTo(() => shell.RunProcessAsync("package-manager1 install -y", "package01-01"))
            .MustHaveHappened();
        A.CallTo(() => shell.RunProcessAsync("package-manager1 install -y", "package01-02"))
            .MustHaveHappened();
        A.CallTo(() => shell.RunProcessAsync("package-manager1 --prune"))
            .MustHaveHappened();

        A.CallTo(() => shell.RunProcessAsync("package-manager-2 update"))
            .MustHaveHappened();
        A.CallTo(() => shell.RunProcessAsync("package-manager2 install -y", "package02-01"))
            .MustHaveHappened();
        A.CallTo(() => shell.RunProcessAsync("package-manager2 install -y", "package02-02"))
            .MustHaveHappened();
        A.CallTo(() => shell.RunProcessAsync("package-manager2 --prune"))
            .MustHaveHappened();
    }

    [Fact]
    public async Task Should_Install_Packages_Only_For_Specified_Package_Manager()
    {
        // Arrange
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();

        var context = GetRequiredService<IYamlContext>();
        var repository = new PackageManagerRepository(context);
        await CopyFileAsync("packages.yml", repository.FilePath);

        var options = new InstallOptions
        {
            PackageManager = "package-manager-1"
        };

        var command = new InstallCommand(repository, shell, console)
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>(),
        };

        // Act
        await command.ExecuteAsync(options);

        // Assert
        A.CallTo(() => shell.RunProcessAsync("package-manager-1 update"))
            .MustHaveHappened();
        A.CallTo(() => shell.RunProcessAsync("package-manager1 install -y", "package01-01"))
            .MustHaveHappened();
        A.CallTo(() => shell.RunProcessAsync("package-manager1 install -y", "package01-02"))
            .MustHaveHappened();
        A.CallTo(() => shell.RunProcessAsync("package-manager1 --prune"))
            .MustHaveHappened();

        A.CallTo(() => shell.RunProcessAsync("package-manager-2 update"))
            .MustNotHaveHappened();
        A.CallTo(() => shell.RunProcessAsync("package-manager2 install -y", "package02-01"))
            .MustNotHaveHappened();
        A.CallTo(() => shell.RunProcessAsync("package-manager2 install -y", "package02-02"))
            .MustNotHaveHappened();
        A.CallTo(() => shell.RunProcessAsync("package-manager2 --prune"))
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

        var options = new InstallOptions
        {
            PackageManager = "package-manager-3"
        };

        var command = new InstallCommand(repository, shell, console)
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
    
    [Fact]
    public async Task Should_Throw_An_Exception_When_The_Specified_Package_Exists_Already()
    {
        // Arrange
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();

        var context = GetRequiredService<IYamlContext>();
        var repository = new PackageManagerRepository(context);
        await CopyFileAsync("packages.yml", repository.FilePath);

        var options = new InstallOptions
        {
            PackageManager = "package-manager-1",
            Package = new []{"package01-01"}
        };

        var command = new InstallCommand(repository, shell, console)
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>(),
        };

        // Act
        var exception = await Should.ThrowAsync<BusinessException>(async () =>
        {
            await command.ExecuteAsync(options);
        });
        
        // Arrange
        exception.ShouldNotBeNull();
        exception.Code.ShouldBe("PM-003");
    }
    
    [Fact]
    public async Task Should_Add_Package_When_It_Does_Not_Exist()
    {
        // Arrange
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();

        var context = GetRequiredService<IYamlContext>();
        var repository = new PackageManagerRepository(context);
        await CopyFileAsync("packages.yml", repository.FilePath);

        var options = new InstallOptions
        {
            PackageManager = "package-manager-1",
            Package = new []{"package01-00"}
        };

        var command = new InstallCommand(repository, shell, console)
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>(),
        };

        // Act
        await command.ExecuteAsync(options);

        // Arrange
        A.CallTo(() => shell.RunProcessAsync("package-manager1 install -y", "package01-00"))
            .MustHaveHappened();

        var manager = await repository.GetAsync(options.PackageManager);
        manager.Packages.ShouldContain(options.Package.First());
    }

    [Fact]
    public async Task Should_Not_Add_The_Packages_When_The_Install_Failed()
    {
        // Arrange
        var console = A.Fake<IConsoleService>();
        var shell = A.Fake<IShellDomainService>();
        A.CallTo(() => shell.RunProcessAsync("package-manager1 install -y", "package01-00"))
            .Returns(999);

        var context = GetRequiredService<IYamlContext>();
        var repository = new PackageManagerRepository(context);
        await CopyFileAsync("packages.yml", repository.FilePath);

        var options = new InstallOptions
        {
            PackageManager = "package-manager-1",
            Package = new []{"package01-00"}
        };

        var command = new InstallCommand(repository, shell, console)
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>(),
        };

        // Act
        await command.ExecuteAsync(options);

        // Arrange
        A.CallTo(() => shell.RunProcessAsync("package-manager1 install -y", "package01-00"))
            .MustHaveHappened();

        var manager = await repository.GetAsync(options.PackageManager);
        manager.Packages.ShouldNotContain(options.Package.First());
    }
}