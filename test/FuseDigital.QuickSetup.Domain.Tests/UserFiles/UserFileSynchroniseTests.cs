using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FakeItEasy;
using FuseDigital.QuickSetup.VersionControlSystems;
using Shouldly;
using Xunit;

namespace FuseDigital.QuickSetup.UserFiles;

public class UserFileSynchroniseTests : QuickSetupDomainTestBase
{
    [Fact]
    public async Task Should_Synchronise_When_Repository_Exist()
    {
        // Arrange
        const string repositoryDirectory = ".fake";
        Directory.CreateDirectory(Path.Combine(Settings.UserProfile, repositoryDirectory));

        const string filename = ".file";
        var repository = A.Fake<IUserFileRepository>();
        A.CallTo(() => repository.GetListAsync(default)).Returns(new List<string> {filename});

        var versionControl = A.Fake<IVersionControlDomainService>();
        A.CallTo(() => versionControl.RepositoryDirectory).Returns(repositoryDirectory);
        A.CallTo(() => versionControl.GetStagedFiles(default)).Returns(new[] {filename});

        var domainService = new UserFileDomainService(Options, repository, versionControl);

        // Act
        await domainService.SynchroniseAsync();

        // Assert
        A.CallTo(() => versionControl.ForceAdd(filename, default)).MustHaveHappened();
        A.CallTo(() => versionControl.AddAll(default)).MustHaveHappened();
        A.CallTo(() => versionControl.Commit(default, default)).MustHaveHappened();
        A.CallTo(() => versionControl.PullAndRebase(default)).MustHaveHappened();
        A.CallTo(() => versionControl.Push(default)).MustHaveHappened();
    }

    [Fact]
    public async Task Should_Not_Synchronise_When_Repository_Does_Not_Exist()
    {
        // Arrange
        const string repositoryDirectory = ".fake";
        var repository = A.Fake<IUserFileRepository>();

        var versionControl = A.Fake<IVersionControlDomainService>();
        A.CallTo(() => versionControl.RepositoryDirectory).Returns(repositoryDirectory);

        var domainService = new UserFileDomainService(Options, repository, versionControl);

        // Act
        var exception = await Should.ThrowAsync<RepositoryNotFoundException>(async () =>
        {
            await domainService.SynchroniseAsync();
        });

        // Assert
        exception.ShouldNotBeNull();
        exception.Message.ShouldContain(Settings.UserProfile);
        A.CallTo(() => versionControl.ForceAdd(A<string>._, default)).MustNotHaveHappened();
        A.CallTo(() => versionControl.AddAll(default)).MustNotHaveHappened();
        A.CallTo(() => versionControl.GetStagedFiles(default)).MustNotHaveHappened();
        A.CallTo(() => versionControl.Commit(default, default)).MustNotHaveHappened();
        A.CallTo(() => versionControl.PullAndRebase(default)).MustNotHaveHappened();
        A.CallTo(() => versionControl.Push(default)).MustNotHaveHappened();
    }
}