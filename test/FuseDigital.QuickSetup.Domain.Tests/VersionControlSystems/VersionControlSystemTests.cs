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

    public VersionControlSystemTests()
    {
        _versionControlSystem = new VersionControlDomainService(Options)
        {
            LazyServiceProvider = GetRequiredService<IAbpLazyServiceProvider>()
        };
    }

    [Fact]
    public void Clone_Should_Create_Repository_In_Working_Directory()
    {
        // Arrange
        var relativePath = "~/projects/dot-files";
        var sourceUrl = Settings.GetAbsolutePath($"{relativePath}.git");
        var workingDirectory = Settings.GetAbsolutePath(relativePath);
        _versionControlSystem.Init(sourceUrl, new[] {"--bare", "-b", "trunk"});

        // Act
        _versionControlSystem.Clone(sourceUrl, relativePath);

        // Assert
        Directory.Exists(workingDirectory).ShouldBeTrue();
    }

    [Fact]
    public void Init_Should_Create_Repository_In_Working_Direcotry()
    {
        // Arrange
        var relativePath = "~/projects/dot-files.git";
        var workingDirectory = Settings.GetAbsolutePath(relativePath);
        
        // Act
        _versionControlSystem.Init(workingDirectory);
        
        // Assert
        Directory.Exists(Path.Combine(workingDirectory, ".git")).ShouldBeTrue();
    }

    [Fact]
    public async Task Add_Should_Stage_Files_From_With_In_The_Working_Directory()
    {
        // Arrange
        var relativePath = "~/projects/dot-files";
        var workingDirectory = Settings.GetAbsolutePath(relativePath);
        _versionControlSystem.Init(workingDirectory);

        const string filename = "README.md";
        await File.WriteAllTextAsync(Path.Combine(workingDirectory, filename), "# qup-empty-dotfiles");
        
        // Act
        _versionControlSystem.WorkingDirectory = workingDirectory;
        _versionControlSystem.Add(filename);
        
        // Assert
        _versionControlSystem
            .Status()
            .Count(x => x.Contains("new file:", StringComparison.InvariantCultureIgnoreCase) && x.Contains(filename, StringComparison.InvariantCultureIgnoreCase))
            .ShouldBe(1);
    }
    
    [Fact]
    public async Task Commit_Should_Execute_From_With_In_The_Working_Directory()
    {
        // Arrange
        const string filename = "README.md";
        await Add_Should_Stage_Files_From_With_In_The_Working_Directory();
        
        // Act
        var output = _versionControlSystem.Commit("First commit");
        
        // Assert
        output
            .Count(x => x.Contains(filename, StringComparison.InvariantCultureIgnoreCase))
            .ShouldBe(1);
    }
    
    [Fact]
    public async Task RenameBranch_Should_Execute_From_With_In_The_Working_Directory()
    {
        // Arrange
        const string name = "trunk";
        await Commit_Should_Execute_From_With_In_The_Working_Directory();
        
        // Act
        _versionControlSystem.RenameBranch(name);
        
        // Assert
        _versionControlSystem
            .Status()
            .Count(x => x.Contains($"On branch {name}", StringComparison.InvariantCultureIgnoreCase))
            .ShouldBe(1);
    }

    [Fact]
    public async Task PushSetUpstream_Should_Execute_From_With_In_The_Working_Directory()
    {
        // Arrange
        const string name = "trunk";
        var remoteUrl = Settings.GetAbsolutePath($"~/server/dot-files.git");
        _versionControlSystem.Init(remoteUrl, new[] {"--bare", "-b", name});
        await RenameBranch_Should_Execute_From_With_In_The_Working_Directory();

        // Act
        _versionControlSystem.AddRemote(remoteUrl);
        var output = _versionControlSystem.PushSetUpstream(name);

        // Assert
        output
            .Count(x => x.Contains($"branch '{name}' set up to track", StringComparison.CurrentCultureIgnoreCase))
            .ShouldBe(1);
    }
}