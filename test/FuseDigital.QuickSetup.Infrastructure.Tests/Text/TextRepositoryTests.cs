using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FuseDigital.QuickSetup.Entities;
using Shouldly;
using Volo.Abp.Domain.Entities;
using Xunit;

namespace FuseDigital.QuickSetup.Text;

public class TextRepositoryTests : QuickSetupInfrastructureTestBase
{
    private const string RepositoryFileName = ".sample-file";
    private class SampleRepository : TextRepository
    {
        public SampleRepository(ITextContext context) : base(context)
        {
        }

        public override string FileName => RepositoryFileName;

        public override string FilePath => Path.Combine(Context.Options.UserProfile, FileName);
    }

    [Fact]
    public void Repository_Should_Resolve_FilePath()
    {
        // Arrange
        var context = GetRequiredService<ITextContext>();

        // Act
        var repository = new SampleRepository(context);

        // Assert
        var path = Path.Combine(context.Options.UserProfile, RepositoryFileName);
        repository.FilePath.ShouldBe(path);
    }
    
    [Fact]
    public async Task Repository_Should_Save_Collection_Sorted_By_Clustered_Index()
    {
        // Arrange
        var repository = GetSampleRepository();
        await CopyFileAsync(".sample-unordered-list", repository.FilePath);
        var input = "line-06";

        // Act
        var output = await repository.InsertAsync(input);
        var list = await repository.GetListAsync();

        // Assert
        output.ShouldNotBeNull();
        output.ShouldBe(input);
        list.First().ShouldBe("line-01");
        list.Last().ShouldBe("line-10");
    }
    
    [Fact]
    public async Task Should_Load_Empty_Collection_If_File_Does_Not_Exists()
    {
        // Arrange
        var repository = GetSampleRepository();

        // Act
        var list = await repository.GetDbSetAsync();

        // Assert
        list.ShouldNotBeNull();
        list.ShouldBeEmpty();
        list.Count.ShouldBe(0);
    }
    
    [Fact]
    public async Task InsertAsync_Should_Create_File_If_It_Does_Not_Exists()
    {
        // Arrange
        var repository = GetSampleRepository();
        var input = "line-01";

        // Act
        var output = await repository.InsertAsync(input);

        // Assert
        output.ShouldNotBeNull();
        output.ShouldBe(input);
        File.Exists(repository.FilePath).ShouldBeTrue();
    }
    
    [Fact]
    public async Task InsertAsync_Should_Throw_Exception_When_Entity_Already_Exists()
    {
        // Arrange
        var repository = GetSampleRepository();
        var input = "GetSampleEntity()";
        await repository.InsertAsync(input);

        // Act
        var exception = await Should.ThrowAsync<EntityAlreadyExistsException>(async () =>
        {
            var duplicate = "GetSampleEntity()";
            await repository.InsertAsync(duplicate);
        });

        // Assert
        exception.Message.ShouldContain("An Entity with the given id already exists. Entity type:");
        File.Exists(repository.FilePath).ShouldBeTrue();
    }
    
    [Fact]
    public async Task DeleteAsync_Should_Find_And_Remove_Existing_Entity()
    {
        var repository = GetSampleRepository();
        await CopyFileAsync(".sample-list", repository.FilePath);

        // Act
        await repository.DeleteAsync("line-08");

        // Assert
        (await repository.GetCountAsync()).ShouldBe(9);
    }

    [Fact]
    public async Task DeleteAsync_Should_Throw_Exception_When_Entity_Does_Not_Exists()
    {
        // Arrange
        var repository = GetSampleRepository();
        await CopyFileAsync(".sample-list", repository.FilePath);

        // Act
        var exception = await Should.ThrowAsync<EntityNotFoundException>(async () =>
        {
            await repository.DeleteAsync("line-11");
        });

        // Assert
        exception.ShouldNotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Entities_That_Match_Expression()
    {
        // Arrange
        var repository = GetSampleRepository();
        await CopyFileAsync(".sample-list", repository.FilePath);

        // Act
        await repository.DeleteAsync(x => x.StartsWith("line-0"));

        // Assert
        (await repository.GetCountAsync()).ShouldBe(1);
    }
    
    [Fact]
    public async Task FindAsync_Should_Return_First_Entity_That_Match_Expression_Function()
    {
        // Arrange
        var repository = GetSampleRepository();
        await CopyFileAsync(".sample-list", repository.FilePath);

        // Act
        var output = await repository.FindAsync(x => x.EndsWith("08"));

        // Assert
        output.ShouldNotBeNull();
        output.ShouldEndWith("08");
    }

    [Fact]
    public async Task FindAsync_Should_Return_Null_Empty_When_No_Entity_Match_Expression_Function()
    {
        // Arrange
        var repository = GetSampleRepository();
        await CopyFileAsync(".sample-list", repository.FilePath);

        // Act
        var output = await repository.FindAsync(x => x.EndsWith("69"));

        // Assert
        output.ShouldBeNull();
    }
    
    private SampleRepository GetSampleRepository()
    {
        var context = GetRequiredService<ITextContext>();
        context.Options.BaseDirectory = Settings.BaseDirectory;

        return new SampleRepository(context);
    }
}