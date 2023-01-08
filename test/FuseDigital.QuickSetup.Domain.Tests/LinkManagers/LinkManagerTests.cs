using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FuseDigital.QuickSetup.Platforms;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Validation;
using Xunit;

namespace FuseDigital.QuickSetup.LinkManagers;

public class LinkManagerTests : QuickSetupDomainTestBase
{
    [Fact]
    public async Task Should_Create_Links()
    {
        // Arrange
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();
        var manager = CreateLinkManager();

        // Act
        await manager.CreateLinksAsync(shell, console);

        // Assert
        A.CallTo(() => shell.RunProcessAsync(manager.GenerateCreateLinkCommand(manager.Links[0])))
            .MustHaveHappened();
        A.CallTo(() => shell.RunProcessAsync(manager.GenerateCreateLinkCommand(manager.Links[1])))
            .MustHaveHappened();
    }

    private static LinkManager CreateLinkManager()
    {
        return new LinkManager
        {
            RunsOn = PlatformEnvironment.CurrentOperatingSystem(),
            CreateLinkFormat = "sudo ln -sfv ${source} ${target}",
            Description = "Description for sample link manager",
            Links = new List<Link>
            {
                new("source/file", "target/file"),
                new("source/directory", "target/directory"),
            }
        };
    }

    [Fact]
    public async Task Should_Throw_An_Exception_When_Link_Manager_Is_Not_Configured_To_Run_On_Operating_System()
    {
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();
        var manager = new LinkManager();
        manager.RunsOn = Enum.GetValues<PlatformOperatingSystem>().First(x => x != manager.RunsOn);

        // Act
        var exception = await Should.ThrowAsync<BusinessException>(async () =>
        {
            await manager.CreateLinksAsync(shell, console);
        });

        // Assert
        exception.ShouldNotBeNull();
        exception.Code.ShouldBe("LM-001");
    }

    [Fact]
    public async Task Should_Throw_An_Exception_When_No_Create_Link_Format_Specified()
    {
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();
        var manager = CreateLinkManager();
        manager.CreateLinkFormat = null;

        // Act
        var exception = await Should.ThrowAsync<BusinessException>(async () =>
        {
            await manager.CreateLinksAsync(shell, console);
        });

        // Assert
        exception.ShouldNotBeNull();
        exception.Code.ShouldBe("LM-002");
    }

    [Fact]
    public async Task Should_Throw_An_Exception_When_Create_Link_Format_Does_Not_Contain_Source_Placeholder()
    {
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();
        var manager = CreateLinkManager();
        manager.CreateLinkFormat = "ln {source} ${target}";


        // Act
        var exception = await Should.ThrowAsync<BusinessException>(async () =>
        {
            await manager.CreateLinksAsync(shell, console);
        });

        // Assert
        exception.ShouldNotBeNull();
        exception.Code.ShouldBe("LM-003");
    }

    [Fact]
    public async Task Should_Throw_An_Exception_When_Create_Link_Format_Does_Not_Contain_Target_Placeholder()
    {
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();
        var manager = CreateLinkManager();
        manager.CreateLinkFormat = "ln ${source} {target}";

        // Act
        var exception = await Should.ThrowAsync<BusinessException>(async () =>
        {
            await manager.CreateLinksAsync(shell, console);
        });

        // Assert
        exception.ShouldNotBeNull();
        exception.Code.ShouldBe("LM-004");
    }

    [Fact]
    public async Task Should_Throw_An_Exception_When_No_Source_For_Link_Provided()
    {
        // Arrange
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();
        var manager = CreateLinkManager();
        var link = new Link(null, "target");

        // Act
        var exception = await Should.ThrowAsync<AbpValidationException>(async () =>
        {
            await manager.AddLinkAsync(link, shell, console);
        });

        // Assert
        exception.ShouldNotBeNull();
        exception.ValidationErrors.First().MemberNames.ShouldContain("Source");
    }

    [Fact]
    public async Task Should_Throw_An_Exception_When_No_Target_For_Link_Provided()
    {
        // Arrange
        var shell = A.Fake<IShellDomainService>();
        var console = A.Fake<IConsoleService>();
        var manager = CreateLinkManager();
        var link = new Link("source", "");

        // Act
        var exception = await Should.ThrowAsync<AbpValidationException>(async () =>
        {
            await manager.AddLinkAsync(link, shell, console);
        });

        // Assert
        exception.ShouldNotBeNull();
        exception.ValidationErrors.First().MemberNames.ShouldContain("Target");
    }
}