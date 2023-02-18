using System;
using System.IO;
using System.Threading.Tasks;
using FakeItEasy;
using FuseDigital.QuickSetup.Platforms;
using FuseDigital.QuickSetup.Projects.Dto;
using FuseDigital.QuickSetup.Repositories;
using FuseDigital.QuickSetup.VersionControlSystems;
using FuseDigital.QuickSetup.Yaml;
using Volo.Abp.DependencyInjection;
using Xunit;

namespace FuseDigital.QuickSetup.Projects;

public class FetchCommandTests : QuickSetupApplicationTestBase
{
    private readonly string _basePath;

    public FetchCommandTests()
    {
        _basePath = Guid.NewGuid().ToString();
    }

    [Fact]
    public async Task Should_Clone_Project_If_It_Does_Not_Exist()
    {
        // Arrange
        var repository = GetRepository();
        await CopyFileAsync("project-exists.yml", repository.FilePath);

        var versionControl = A.Fake<IVersionControlDomainService>();
        A.CallTo(() => versionControl.Exists(A<string>._, A<bool>._))
            .Returns(false);

        var command = new FetchCommand(repository, versionControl, A.Fake<IConsoleService>())
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>(),
        };

        // Act
        await command.ExecuteAsync(new FetchOptions());

        // Assert
        A.CallTo(() => versionControl.Clone(A<string>._, A<string>._))
            .MustHaveHappened();
    }

    [Fact]
    public async Task Should_Fetch_Project_If_It_Does_Exist()
    {
        // Arrange
        var repository = GetRepository();
        await CopyFileAsync("project-exists.yml", repository.FilePath);

        var versionControl = A.Fake<IVersionControlDomainService>();
        A.CallTo(() => versionControl.Exists(A<string>._, A<bool>._))
            .Returns(true);

        var command = new FetchCommand(repository, versionControl, A.Fake<IConsoleService>())
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>(),
        };

        // Act
        await command.ExecuteAsync(new FetchOptions());

        // Assert
        A.CallTo(() => versionControl.Fetch(null))
            .MustHaveHappened();
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