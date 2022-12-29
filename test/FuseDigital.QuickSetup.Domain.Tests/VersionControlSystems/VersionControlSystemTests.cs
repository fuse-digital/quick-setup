using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.DependencyInjection;
using Xunit;

namespace FuseDigital.QuickSetup.VersionControlSystems;

public sealed class VersionControlSystemTests : QuickSetupDomainTestBase
{
    private readonly VersionControlDomainService _versionControlSystem;
    private const string RepoRelativePath = "~/projects/dot-files";
    private const string Filename = "README.md";
    private const string BranchName = "trunk";
    private string RepoWorkingDirectory { get; }
    private string SecondRepoWorkingDirectory { get; }
    private string RemoteUrl { get; }

    public VersionControlSystemTests()
    {
        _versionControlSystem = new VersionControlDomainService(Options)
        {
            LazyServiceProvider = GetRequiredService<IAbpLazyServiceProvider>()
        };
        RepoWorkingDirectory = Settings.GetAbsolutePath(RepoRelativePath);
        SecondRepoWorkingDirectory = Settings.GetAbsolutePath("~/projects/dot-files-second");
        RemoteUrl = Settings.GetAbsolutePath($"~/server/dot-files.git");
    }

    [Fact]
    public void Clone_Should_Create_Repository_In_Working_Directory()
    {
        // Arrange
        _versionControlSystem.Init(RemoteUrl, new[] { "--bare", "-b", BranchName });

        // Act
        _versionControlSystem.Clone(RemoteUrl, RepoRelativePath);

        // Assert
        Directory.Exists(RepoWorkingDirectory).ShouldBeTrue();
    }

    [Fact]
    public async Task Fetch_Should_Retrieve_Metadata_From_Remote_Repository()
    {
        // Arrange
        await PushSetUpstream_Should_Execute_From_With_In_The_Working_Directory();
        _versionControlSystem.Init(SecondRepoWorkingDirectory, changeWorkingDirectory: true);
        _versionControlSystem.AddRemote(RemoteUrl);

        // Act
        _versionControlSystem.Fetch();

        // Assert
        Directory.Exists(SecondRepoWorkingDirectory).ShouldBeTrue();
    }

    [Fact]
    public async Task Checkout_Should_Retrieve_Files_From_Remote_Repository()
    {
        // Arrange
        await Fetch_Should_Retrieve_Metadata_From_Remote_Repository();

        // Act
        _versionControlSystem.Checkout(BranchName);

        // Assert
        File.Exists(Path.Combine(SecondRepoWorkingDirectory, Filename)).ShouldBeTrue();
    }

    [Fact]
    public void Init_Should_Create_Repository_In_Working_Direcotry()
    {
        // Act
        _versionControlSystem.Init(RepoWorkingDirectory);

        // Assert
        Directory.Exists(Path.Combine(RepoWorkingDirectory, ".git")).ShouldBeTrue();
    }

    [Fact]
    public async Task Add_Should_Stage_Files_From_With_In_The_Working_Directory()
    {
        // Arrange
        _versionControlSystem.Init(RepoWorkingDirectory, changeWorkingDirectory: true);
        await File.WriteAllTextAsync(Path.Combine(RepoWorkingDirectory, Filename), "# qup-empty-dotfiles");

        // Act
        _versionControlSystem.Add(Filename);

        // Assert
        _versionControlSystem
            .Status()
            .Count(x =>
                x.Contains("new file:", StringComparison.InvariantCultureIgnoreCase)
                && x.Contains(Filename, StringComparison.InvariantCultureIgnoreCase))
            .ShouldBe(1);
    }

    [Fact]
    public async Task Commit_Should_Execute_From_With_In_The_Working_Directory()
    {
        // Arrange
        await Add_Should_Stage_Files_From_With_In_The_Working_Directory();
        _versionControlSystem.SetConfig("user.email", "john@doe.com");
        _versionControlSystem.SetConfig("user.name", "john@doe.com");

        // Act
        var output = _versionControlSystem.Commit("First commit").ToList();

        // Assert
        output
            .Count(x => x.Contains(Filename, StringComparison.InvariantCultureIgnoreCase))
            .ShouldBe(1);
    }

    [Fact]
    public async Task RenameBranch_Should_Execute_From_With_In_The_Working_Directory()
    {
        // Arrange
        await Commit_Should_Execute_From_With_In_The_Working_Directory();

        // Act
        _versionControlSystem.RenameBranch(BranchName);

        // Assert
        _versionControlSystem
            .Status()
            .Count(x => x.Contains($"On branch {BranchName}", StringComparison.InvariantCultureIgnoreCase))
            .ShouldBe(1);
    }

    [Fact]
    public async Task PushSetUpstream_Should_Execute_From_With_In_The_Working_Directory()
    {
        // Arrange
        _versionControlSystem.Init(RemoteUrl, new[] { "--bare", "-b", BranchName });
        await RenameBranch_Should_Execute_From_With_In_The_Working_Directory();

        // Act
        _versionControlSystem.AddRemote(RemoteUrl);
        var output = _versionControlSystem.PushSetUpstream(BranchName);

        // Assert
        output
            .Count(x => x.Contains($"branch '{BranchName}' set up to track", StringComparison.CurrentCultureIgnoreCase))
            .ShouldBe(1);
    }
}