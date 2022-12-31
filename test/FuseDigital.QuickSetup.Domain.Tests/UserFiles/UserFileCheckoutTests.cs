using System.IO;
using System.Threading.Tasks;
using FakeItEasy;
using FuseDigital.QuickSetup.VersionControlSystems;
using Shouldly;
using Xunit;

namespace FuseDigital.QuickSetup.UserFiles;

public class UserFileCheckoutTests : QuickSetupDomainTestBase
{
    [Fact]
    public async Task Should_Checkout_When_No_Git_Folder_Exits()
    {
        // Arrange
        const string branch = "trunk";
        var remoteUrl = Settings.GetAbsolutePath($"~/server/dot-files.git");
        Directory.CreateDirectory(Settings.UserProfile);
        var versionControl = A.Fake<IVersionControlDomainService>();
        A.CallTo(() => versionControl.RepositoryDirectory).Returns(".git");
        var repository = A.Fake<IUserFileRepository>();
        var domainService = new UserFileDomainService(Options, repository, versionControl);

        // Act
        await domainService.CheckoutAsync(remoteUrl, branch);

        // Assert
        A.CallTo(() => versionControl.Init(Settings.UserProfile, default, default)).MustHaveHappened();
        A.CallTo(() => versionControl.AddRemote(remoteUrl, default)).MustHaveHappened();
        A.CallTo(() => versionControl.Fetch(default)).MustHaveHappened();
        A.CallTo(() => versionControl.Checkout(branch, default)).MustHaveHappened();
    }

    [Fact]
    public async Task Should_Not_Checkout_When_Git_Folder_Exits()
    {
        // Arrange
        const string branch = "trunk";
        var remoteUrl = Settings.GetAbsolutePath($"~/server/dot-files.fake");
        Directory.CreateDirectory(Path.Combine(Settings.UserProfile, ".fake"));
        var versionControl = A.Fake<IVersionControlDomainService>();
        A.CallTo(() => versionControl.RepositoryDirectory).Returns(".fake");
        var repository = A.Fake<IUserFileRepository>();
        var domainService = new UserFileDomainService(Options, repository, versionControl);

        // Act
        var exception = await Should.ThrowAsync<RepositoryAlreadyExistsException>(async () =>
        {
            await domainService.CheckoutAsync(remoteUrl, branch);
        });

        // Assert
        exception.ShouldNotBeNull();
        exception.Message.ShouldBe(Settings.UserProfile);
        
        A.CallTo(() => versionControl.Init(Settings.UserProfile, default, default)).MustNotHaveHappened();
        A.CallTo(() => versionControl.AddRemote(remoteUrl, default)).MustNotHaveHappened();
        A.CallTo(() => versionControl.Fetch(default)).MustNotHaveHappened();
        A.CallTo(() => versionControl.Checkout(branch, default)).MustNotHaveHappened();
    }
}