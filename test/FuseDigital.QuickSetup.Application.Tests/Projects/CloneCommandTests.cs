using System;
using System.IO;
using System.Threading.Tasks;
using FakeItEasy;
using FuseDigital.QuickSetup.Repositories;
using FuseDigital.QuickSetup.VersionControlSystems;
using FuseDigital.QuickSetup.Yaml;
using Microsoft.Extensions.Options;
using Shouldly;
using Volo.Abp.DependencyInjection;
using Xunit;

namespace FuseDigital.QuickSetup.Projects;

public class CloneCommandTests : QuickSetupApplicationTestBase
{
    private readonly string _basePath;

    public CloneCommandTests()
    {
        _basePath = Guid.NewGuid().ToString();
    }

    [Fact]
    public async Task Should_Not_Clone_If_Project_Already_Exists()
    {
        // Arrange
        var repository = GetRepository();
        var options = GetRequiredService<IOptions<QuickSetupOptions>>();
        var versionControl = A.Fake<IVersionControlDomainService>();
        var command = new CloneCommand(options, repository, versionControl)
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>(),
        };

        var commandOptions = new CloneOptions
        {
            LocalFolder = "~/organization/project/repository",
            Repository = "git@ssh.dev.azure.com:v3/organization/project/repository",
        };
        await CopyFileAsync("project-exists.yml", repository.FilePath);

        // Act
        await command.ExecuteAsync(commandOptions);

        // Assert
        A.CallTo(() => versionControl
                .Clone(commandOptions.Repository, commandOptions.LocalFolder))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Should_Add_Project_If_It_Does_Not_Exists()
    {
        // Arrange
        var repository = GetRepository();
        var options = GetRequiredService<IOptions<QuickSetupOptions>>();
        var versionControl = A.Fake<IVersionControlDomainService>();
        var command = new CloneCommand(options, repository, versionControl)
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>(),
        };
        var commandOptions = new CloneOptions
        {
            LocalFolder = "~/organization/project/repository",
            Repository = "git@ssh.dev.azure.com:v3/organization/project/repository",
        };

        // Act
        await command.ExecuteAsync(commandOptions);

        // Assert
        A.CallTo(() => versionControl.Clone(commandOptions.Repository, commandOptions.LocalFolder))
            .MustHaveHappened();

        var entity = await repository.FindAsync(x => x.RelativePath.Equals(commandOptions.LocalFolder));
        entity.ShouldNotBeNull();
    }

    private ProjectRepository GetRepository(IYamlContext context = default)
    {
        return new ProjectRepository(context ?? GetContext(_basePath))
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>()
        };
    }

    public override void Dispose()
    {
        var context = GetRequiredService<IYamlContext>();
        var path = Path.Combine(context.Options.UserProfile, _basePath);
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }

        base.Dispose();
    }
}