using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FuseDigital.QuickSetup.LinkManagers.Dto;
using FuseDigital.QuickSetup.Platforms;
using FuseDigital.QuickSetup.Repositories;
using FuseDigital.QuickSetup.Yaml;
using Shouldly;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Xunit;

namespace FuseDigital.QuickSetup.LinkManagers;

public class LinkCommandTests : QuickSetupApplicationTestBase
{
    [Fact]
    public async Task Should_Only_Create_Links_For_Current_Platform()
    {
        // Arrange
        var operatingSystem = PlatformEnvironment.CurrentOperatingSystem();
        var unsupportedOperatingSystems = Enum.GetValues<PlatformOperatingSystem>()
            .Select(x => x != operatingSystem);

        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();

        var context = GetRequiredService<IYamlContext>();
        var repository = new LinkManagerRepository(context);
        await CopyFileAsync("links.yml", repository.FilePath);

        var options = new LinkOptions();
        var command = new LinkCommand(repository, shell, console)
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>(),
        };

        // Act
        await command.ExecuteAsync(options);

        // Assert
        A.CallTo(() => shell.RunProcessAsync($"{operatingSystem} sample-directory .qup/sample-directory"))
            .MustHaveHappened();
        A.CallTo(() => shell.RunProcessAsync($"{operatingSystem} sample-file .qup/sample-file"))
            .MustHaveHappened();

        foreach (var platform in unsupportedOperatingSystems)
        {
            A.CallTo(() => shell.RunProcessAsync($"{platform} sample-directory .qup/sample-directory"))
                .MustNotHaveHappened();
            A.CallTo(() => shell.RunProcessAsync($"{platform} sample-file .qup/sample-file"))
                .MustNotHaveHappened();
        }
    }

    [Fact]
    public async Task Should_Throw_An_Exception_When_The_Current_Platform_Link_Manager_Can_Not_Be_Found()
    {
        // Arrange
        var operatingSystem = PlatformEnvironment.CurrentOperatingSystem();
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();
    
        var context = GetRequiredService<IYamlContext>();
        var repository = new LinkManagerRepository(context);
        await CopyFileAsync("links.yml", repository.FilePath);
        await repository.DeleteAsync(x => x.RunsOn == operatingSystem);

        var options = new LinkOptions();
        var command = new LinkCommand(repository, shell, console)
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
        exception.Message.ShouldContain(operatingSystem.ToString());
    }
    
    
    [Fact]
    public async Task Should_Throw_An_Exception_When_The_Link_Exists_Already()
    {
        // Arrange
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();
    
        var context = GetRequiredService<IYamlContext>();
        var repository = new LinkManagerRepository(context);
        await CopyFileAsync("links.yml", repository.FilePath);
    
        var options = new LinkOptions
        {
            Target = "sample-file",
            Source = ".qup/sample-file",
        };
    
        var command = new LinkCommand(repository, shell, console)
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
        exception.Code.ShouldBe("LM-001");
    }
    
    [Fact]
    public async Task Should_Add_Link_When_It_Does_Not_Exist()
    {
        // Arrange
        var operatingSystem = PlatformEnvironment.CurrentOperatingSystem();
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();
    
        var context = GetRequiredService<IYamlContext>();
        var repository = new LinkManagerRepository(context);
        await CopyFileAsync("links.yml", repository.FilePath);
    
        var options = new LinkOptions
        {
            Target = "a-new-file",
            Source = ".a-folder/a-new-file"
        };
    
        var command = new LinkCommand(repository, shell, console)
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>(),
        };
    
        // Act
        await command.ExecuteAsync(options);
    
        // Arrange
        A.CallTo(() => shell.RunProcessAsync($"{operatingSystem} {options.Target} {options.Source}"))
            .MustHaveHappened();
    
        var manager = await repository.GetAsync(operatingSystem);
        manager.Links
            .Any(x => x.Target.Equals(options.Target) && x.Source.Equals(options.Source))
            .ShouldBeTrue();
    }
    
    [Fact]
    public async Task Should_Not_Add_The_Link_When_The_Create_Failed()
    {
        // Arrange
        var operatingSystem = PlatformEnvironment.CurrentOperatingSystem();
        var console = A.Fake<IConsoleService>();
        var shell = A.Fake<IShellDomainService>();
        A.CallTo(() => shell.RunProcessAsync(A<string[]>._))
            .Returns(999);
    
        var context = GetRequiredService<IYamlContext>();
        var repository = new LinkManagerRepository(context);
        await CopyFileAsync("links.yml", repository.FilePath);
    
        var options = new LinkOptions
        {
            Target = "a-new-file",
            Source = ".a-folder/a-new-file"
        };
    
        var command = new LinkCommand(repository, shell, console)
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>(),
        };
    
        // Act
        await command.ExecuteAsync(options);
    
        // Arrange
        var manager = await repository.GetAsync(operatingSystem);
        manager.Links
            .Any(x => x.Target.Equals(options.Target) && x.Source.Equals(options.Source))
            .ShouldBeFalse();
    }
    
    [Fact]
    public async Task Should_Add_Link_Using_Default_Command_Format_When_No_Link_Manager_Found_For_Current_Operating_System()
    {
        // Arrange
        var operatingSystem = PlatformEnvironment.CurrentOperatingSystem();
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();
    
        var context = GetRequiredService<IYamlContext>();
        var repository = new LinkManagerRepository(context);
    
        var options = new LinkOptions
        {
            Target = "a-new-file",
            Source = ".a-folder/a-new-file"
        };
    
        var command = new LinkCommand(repository, shell, console)
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>(),
        };
    
        // Act
        await command.ExecuteAsync(options);
    
        // Arrange
        A.CallTo(() => shell.RunProcessAsync(A<string[]>._))
            .MustHaveHappened();
    
        var manager = await repository.GetAsync(operatingSystem);
        manager.Links
            .Any(x => x.Target.Equals(options.Target) && x.Source.Equals(options.Source))
            .ShouldBeTrue();
    }
}