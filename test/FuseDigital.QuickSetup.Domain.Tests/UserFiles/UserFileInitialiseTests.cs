using System.IO;
using System.Threading.Tasks;
using FakeItEasy;
using FuseDigital.QuickSetup.VersionControlSystems;
using Shouldly;
using Xunit;

namespace FuseDigital.QuickSetup.UserFiles;

public class UserFileInitialiseTests : QuickSetupDomainTestBase
{
    [Fact]
    public async Task Should_Initialise_When_No_Git_Folder_Exits()
    {
        // Arrange
        const string defaultBranch = "trunk";
        var remoteUrl = Settings.GetAbsolutePath($"~/server/dot-files.git");
        Directory.CreateDirectory(Settings.UserProfile);
        var versionControl = A.Fake<IVersionControlDomainService>();
        A.CallTo(() => versionControl.RepositoryDirectory).Returns(".git");
        var domainService = new UserFileDomainService(Options, versionControl);

        // Act
        await domainService.Initialise(remoteUrl, defaultBranch);

        // Assert
        A.CallTo(() => versionControl.Init(Settings.UserProfile, default)).MustHaveHappened();
        A.CallTo(() => versionControl.Add(".", default)).MustHaveHappened();
        A.CallTo(() => versionControl.Commit(default, default)).MustHaveHappened();
        A.CallTo(() => versionControl.RenameBranch(defaultBranch, default)).MustHaveHappened();
        A.CallTo(() => versionControl.AddRemote(remoteUrl, default)).MustHaveHappened();
        A.CallTo(() => versionControl.PushSetUpstream(defaultBranch, default)).MustHaveHappened();
    }

    [Fact]
    public async Task Should_Not_Initialise_When_Git_Folder_Exits()
    {
        // Arrange
        const string defaultBranch = "trunk";
        var remoteUrl = Settings.GetAbsolutePath($"~/server/dot-files.fake");
        Directory.CreateDirectory(Path.Combine(Settings.UserProfile, ".fake"));
        var versionControl = A.Fake<IVersionControlDomainService>();
        A.CallTo(() => versionControl.RepositoryDirectory).Returns(".fake");
        var domainService = new UserFileDomainService(Options, versionControl);

        // Act
        var exception = await Should.ThrowAsync<RepositoryAlreadyExistsException>(async () =>
        {
            await domainService.Initialise(remoteUrl, defaultBranch);
        });

        // Assert
        exception.ShouldNotBeNull();
        exception.Message.ShouldBe(Settings.UserProfile);
        
        A.CallTo(() => versionControl.Init(Settings.UserProfile, default)).MustNotHaveHappened();
        A.CallTo(() => versionControl.Add(".", default)).MustNotHaveHappened();
        A.CallTo(() => versionControl.Commit(default, default)).MustNotHaveHappened();
        A.CallTo(() => versionControl.RenameBranch(defaultBranch, default)).MustNotHaveHappened();
        A.CallTo(() => versionControl.AddRemote(remoteUrl, default)).MustNotHaveHappened();
        A.CallTo(() => versionControl.PushSetUpstream(defaultBranch, default)).MustNotHaveHappened();
    }
}