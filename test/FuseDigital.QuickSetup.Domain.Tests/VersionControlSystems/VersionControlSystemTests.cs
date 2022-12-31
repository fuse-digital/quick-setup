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
    private const string SecondRepoRelativePath = "~/projects/dot-files-second";
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
        SecondRepoWorkingDirectory = Settings.GetAbsolutePath(SecondRepoRelativePath);
        RemoteUrl = Settings.GetAbsolutePath($"~/server/dot-files.git");
    }

    [Fact]
    public void Clone_Should_Create_Repository_In_Working_Directory()
    {
        // Arrange
        _versionControlSystem.Init(RemoteUrl, new[] {"--bare", "-b", BranchName});

        // Act
        _versionControlSystem.Clone(RemoteUrl, RepoRelativePath);

        // Assert
        Directory.Exists(RepoWorkingDirectory).ShouldBeTrue();
    }

    [Fact]
    public async Task Fetch_Should_Retrieve_Metadata_From_Remote_Repository()
    {
        // Arrange
        await PushSetUpstream_Should_Set_The_Tracking_Branch_And_Push_Changes();
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
    public void Init_Should_Create_Repository_In_Working_Directory()
    {
        // Act
        _versionControlSystem.Init(RepoWorkingDirectory);

        // Assert
        Directory.Exists(Path.Combine(RepoWorkingDirectory, ".git")).ShouldBeTrue();
    }

    [Fact]
    public async Task Add_Should_Stage_Files_That_Changed_Are_Not_In_The_Ignore_File()
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
    public async Task Commit_Should_Save_All_Staged_Files()
    {
        // Arrange
        await Add_Should_Stage_Files_That_Changed_Are_Not_In_The_Ignore_File();
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
    public async Task RenameBranch_Should_Change_The_Name_Of_The_Current_Branch()
    {
        // Arrange
        await Commit_Should_Save_All_Staged_Files();

        // Act
        _versionControlSystem.RenameBranch(BranchName);

        // Assert
        _versionControlSystem
            .Status()
            .Count(x => x.Contains($"On branch {BranchName}", StringComparison.InvariantCultureIgnoreCase))
            .ShouldBe(1);
    }

    [Fact]
    public async Task PushSetUpstream_Should_Set_The_Tracking_Branch_And_Push_Changes()
    {
        // Arrange
        _versionControlSystem.Init(RemoteUrl, new[] {"--bare", "-b", BranchName});
        await RenameBranch_Should_Change_The_Name_Of_The_Current_Branch();

        // Act
        _versionControlSystem.AddRemote(RemoteUrl);
        var output = _versionControlSystem.PushSetUpstream(BranchName);

        // Assert
        output
            .Count(x => x.Contains($"branch '{BranchName}' set up to track", StringComparison.CurrentCultureIgnoreCase))
            .ShouldBe(1);
    }

    [Fact]
    public async Task AddAll_Should_Stage_All_The_Files_That_Changed_And_Are_Not_In_The_Ignore_File()
    {
        // Arrange
        _versionControlSystem.Init(RepoWorkingDirectory, changeWorkingDirectory: true);
        await CreateFileAsync(Filename);
        await CreateFileAsync($"Second-{Filename}");

        // Act
        _versionControlSystem.AddAll();

        // Assert
        _versionControlSystem
            .Status()
            .Count(x =>
                x.Contains("new file:", StringComparison.InvariantCultureIgnoreCase)
                && x.Contains(Filename, StringComparison.InvariantCultureIgnoreCase))
            .ShouldBe(2);
    }

    [Fact]
    public async Task GetStagedFiles_Should_Return_A_List_Of_All_Files_That_Have_Been_Staged()
    {
        // Arrange
        await AddAll_Should_Stage_All_The_Files_That_Changed_And_Are_Not_In_The_Ignore_File();

        // Act
        var output = _versionControlSystem.GetStagedFiles().ToList();

        // Assert
        output.ShouldNotBeEmpty();
        output.Count.ShouldBe(2);
    }

    [Fact]
    public async Task ForceAdd_Should_Add_Changed_Files_Even_If_They_Are_In_The_Ignore_Config()
    {
        // Arrange
        _versionControlSystem.Init(RepoWorkingDirectory, changeWorkingDirectory: true);
        await File.WriteAllTextAsync(Path.Combine(RepoWorkingDirectory, ".gitignore"), $"{Filename}");
        await CreateFileAsync(Filename);

        // Act
        _versionControlSystem.ForceAdd(Filename);

        // Assert
        var output = _versionControlSystem.GetStagedFiles().ToList();
        output.ShouldNotBeEmpty();
        output.Count.ShouldBe(1);
    }

    [Fact]
    public async Task Push_Should_Upload_The_Committed_Changes_To_The_Remote_Repository()
    {
        // Arrange
        _versionControlSystem.Init(RemoteUrl, new[] {"--bare", "-b", BranchName});
        _versionControlSystem.Clone(RemoteUrl, RepoRelativePath);
        _versionControlSystem.WorkingDirectory = RepoWorkingDirectory;
        _versionControlSystem.SetConfig("user.email", "john@doe.com");
        _versionControlSystem.SetConfig("user.name", "john@doe.com");

        await CreateFileAsync(Filename);
        _versionControlSystem.AddAll();
        _versionControlSystem.Commit();

        // Act
        _versionControlSystem.Push();

        // Assert
        var output = _versionControlSystem.Status().ToList();
        output.ShouldNotBeNull();
        output
            .Count(x => x.Contains("Your branch is up to date with", StringComparison.InvariantCultureIgnoreCase))
            .ShouldBe(1);
    }

    [Fact]
    public async Task PullAndRebase_Should_Pull_Remote_Changes_And_Rebase_Local_Repository()
    {
        // Arrange
        await Push_Should_Upload_The_Committed_Changes_To_The_Remote_Repository();

        _versionControlSystem.Clone(RemoteUrl, SecondRepoRelativePath);
        _versionControlSystem.WorkingDirectory = SecondRepoWorkingDirectory;
        _versionControlSystem.SetConfig("user.email", "john@doe.com");
        _versionControlSystem.SetConfig("user.name", "john@doe.com");

        await CreateFileAsync($"Second-{Filename}", SecondRepoWorkingDirectory);
        _versionControlSystem.AddAll();
        _versionControlSystem.Commit();
        _versionControlSystem.Push();

        _versionControlSystem.WorkingDirectory = RepoWorkingDirectory;
        await CreateFileAsync($"third-{Filename}", RepoWorkingDirectory);
        _versionControlSystem.AddAll();
        _versionControlSystem.Commit();

        // Act
        _versionControlSystem.PullAndRebase();

        // Assert
        var output = _versionControlSystem.Status().ToList();
        output.ShouldNotBeNull();
        output
            .Count(x => x.Contains("Your branch is ahead of", StringComparison.InvariantCultureIgnoreCase))
            .ShouldBe(1);
    }

    private async Task CreateFileAsync(string filename, string workingDirectory = default)
    {
        await File.WriteAllTextAsync(
            Path.Combine(workingDirectory ?? RepoWorkingDirectory, filename),
            "# qup-empty-dotfiles"
        );
    }
}