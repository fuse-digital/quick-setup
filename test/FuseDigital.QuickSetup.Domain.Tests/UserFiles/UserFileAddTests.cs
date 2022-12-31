using System;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FakeItEasy;
using FuseDigital.QuickSetup.Entities;
using FuseDigital.QuickSetup.VersionControlSystems;
using Shouldly;
using Xunit;

namespace FuseDigital.QuickSetup.UserFiles;

public class UserFileAddTests : QuickSetupDomainTestBase
{
    [Fact]
    public async Task Should_Add_When_No_Pattern_Exits()
    {
        // Arrange
        const string pattern = ".gitconfig";
        var versionControl = A.Fake<IVersionControlDomainService>();
        var repository = A.Fake<IUserFileRepository>();
        
        A.CallTo(() => repository.FindAsync(A<Expression<Func<string, bool>>>._, default))
            .Returns(string.Empty);
        
        var domainService = new UserFileDomainService(Options, repository, versionControl);

        // Act
        await domainService.AddAsync(pattern);

        // Assert
        A.CallTo(() => repository.InsertAsync(pattern, default)).MustHaveHappened();
    }

    [Fact]
    public async Task Should_Not_Add_When_Pattern_Exits()
    {
        // Arrange
        const string pattern = ".gitconfig";
        var versionControl = A.Fake<IVersionControlDomainService>();
        var repository = A.Fake<IUserFileRepository>();
        
        A.CallTo(() => repository.FindAsync(A<Expression<Func<string, bool>>>._, default))
            .Returns(pattern);
        
        var domainService = new UserFileDomainService(Options, repository, versionControl);

        // Act
        var exception = await Should.ThrowAsync<EntityAlreadyExistsException>(async () =>
        {
            await domainService.AddAsync(pattern);
        });

        // Assert
        exception.ShouldNotBeNull();
        exception.Message.ShouldContain($"Entity type: System.String, id: {pattern}");
        A.CallTo(() => repository.InsertAsync(pattern, default)).MustNotHaveHappened();
    }
}