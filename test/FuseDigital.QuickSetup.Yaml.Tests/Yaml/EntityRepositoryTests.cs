using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FuseDigital.QuickSetup.Entities;
using Shouldly;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace FuseDigital.QuickSetup.Yaml;

public class EntityRepositoryTests : QuickSetupYamlTestBase
{
    private class SampleEntity : Entity
    {
        [Key] public string SampleKey { get; set; }

        public string SampleValue { get; set; }

        public IList<string> SampleList { get; set; }

        public override object[] GetKeys()
        {
            return new object[] {SampleKey};
        }
    }

    private class SampleRepository : YamlRepository<SampleEntity>
    {
        public SampleRepository(IYamlContext context) : base(context)
        {
        }

        public override Func<SampleEntity, object> SortOrder<TKey>() => entity => entity.SampleKey;
    }

    [Fact]
    public void Repository_Should_Resolve_FileName()
    {
        // Arrange
        var context = GetRequiredService<IYamlContext>();

        // Act
        var repository = new SampleRepository(context);

        // Assert
        repository.FileName.ShouldBe("sample.yml");
    }

    [Fact]
    public void Repository_Should_Resolve_FilePath()
    {
        // Arrange
        var context = GetRequiredService<IYamlContext>();

        // Act
        var repository = new SampleRepository(context);

        // Assert
        var path = Path.Combine(context.Options.UserProfile, context.Options.BaseDirectory, "sample.yml");
        repository.FilePath.ShouldBe(path);
    }

    [Fact]
    public async Task Repository_Should_Save_Collection_Sorted_By_Clustered_Index()
    {
        // Arrange
        var repository = GetSampleRepository();
        await CopyFileAsync("sample-unordered-list.yml", repository.FilePath);
        var input = GetSampleEntity();
        input.SampleKey = "SampleKey06";

        // Act
        var output = await repository.UpdateAsync(input);
        var list = await repository.GetListAsync();

        // Assert
        output.ShouldNotBeNull();
        output.SampleKey.ShouldBe(input.SampleKey);
        list.First().SampleKey.ShouldBe("SampleKey01");
        list.Last().SampleKey.ShouldBe("SampleKey10");
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
        var input = GetSampleEntity();

        // Act
        var output = await repository.InsertAsync(input);

        // Assert
        output.ShouldNotBeNull();
        output.SampleKey.ShouldBe(input.SampleKey);
        output.SampleValue.ShouldBe(input.SampleValue);
        File.Exists(repository.FilePath).ShouldBeTrue();
    }

    [Fact]
    public async Task InsertAsync_Should_Throw_Exception_When_Entity_Already_Exists()
    {
        // Arrange
        var repository = GetSampleRepository();
        var input = GetSampleEntity();
        await repository.InsertAsync(input);

        // Act
        var exception = await Should.ThrowAsync<EntityAlreadyExistsException>(async () =>
        {
            var duplicate = GetSampleEntity();
            await repository.InsertAsync(duplicate);
        });

        // Assert
        exception.Message.ShouldContain("An Entity with the given id already exists. Entity type:");
        File.Exists(repository.FilePath).ShouldBeTrue();
    }

    [Fact]
    public async Task UpdateAsync_Should_Find_And_Update_Existing_Entity()
    {
        // Arrange
        var repository = GetSampleRepository();
        await CopyFileAsync("sample-list.yml", repository.FilePath);
        var input = GetSampleEntity();
        input.SampleKey = "SampleKey08";

        // Act
        var output = await repository.UpdateAsync(input);

        // Assert
        output.ShouldNotBeNull();
        output.SampleKey.ShouldBe(input.SampleKey);
    }

    [Fact]
    public async Task UpdateAsync_Should_Throw_Exception_When_Entity_Does_Not_Exists()
    {
        // Arrange
        var repository = GetSampleRepository();
        await CopyFileAsync("sample-list.yml", repository.FilePath);


        // Act
        var exception = await Should.ThrowAsync<EntityNotFoundException>(async () =>
        {
            var input = GetSampleEntity();
            await repository.UpdateAsync(input);
        });

        // Assert
        exception.ShouldNotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_Should_Find_And_Remove_Existing_Entity()
    {
        var repository = GetSampleRepository();
        await CopyFileAsync("sample-list.yml", repository.FilePath);
        var input = GetSampleEntity();
        input.SampleKey = "SampleKey08";

        // Act
        await repository.DeleteAsync(input);

        // Assert
        (await repository.GetCountAsync()).ShouldBe(9);
    }

    [Fact]
    public async Task DeleteAsync_Should_Throw_Exception_When_Entity_Does_Not_Exists()
    {
        // Arrange
        var repository = GetSampleRepository();
        await CopyFileAsync("sample-list.yml", repository.FilePath);

        // Act
        var exception = await Should.ThrowAsync<EntityNotFoundException>(async () =>
        {
            var input = GetSampleEntity();
            await repository.DeleteAsync(input);
        });

        // Assert
        exception.ShouldNotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Entities_That_Match_Expression()
    {
        // Arrange
        var repository = GetSampleRepository();
        await CopyFileAsync("sample-list.yml", repository.FilePath);

        // Act
        await repository.DeleteAsync(x => x.SampleKey.StartsWith("SampleKey0"));

        // Assert
        (await repository.CountAsync()).ShouldBe(1);
    }

    [Fact]
    public async Task FindAsync_Should_Return_First_Entity_That_Match_Expression_Function()
    {
        // Arrange
        var repository = GetSampleRepository();
        await CopyFileAsync("sample-list.yml", repository.FilePath);

        // Act
        var output = await repository.FindAsync(x => x.SampleValue.EndsWith("08"));

        // Assert
        output.ShouldNotBeNull();
        output.SampleValue.ShouldEndWith("08");
    }

    [Fact]
    public async Task FindAsync_Should_Return_Null_Empty_When_No_Entity_Match_Expression_Function()
    {
        // Arrange
        var repository = GetSampleRepository();
        await CopyFileAsync("sample-list.yml", repository.FilePath);

        // Act
        var output = await repository.FindAsync(x => x.SampleValue.EndsWith("69"));

        // Assert
        output.ShouldBeNull();
    }

    [Fact]
    public async Task GetPagedListAsync_Should_Return_Second_Page_Of_Results()
    {
        // Arrange
        var repository = GetSampleRepository();
        await CopyFileAsync("sample-list.yml", repository.FilePath);

        // Act
        var output = await repository.GetPagedListAsync(3, 3);

        // Assert
        output.Count.ShouldBe(3);
    }

    private async Task CopyFileAsync(string source, string destination)
    {
        var content = GetFileContents(source);
        if (content != null)
        {
            var directoryFullName = new FileInfo(destination).Directory?.FullName;
            if (directoryFullName != null)
            {
                Directory.CreateDirectory(directoryFullName);
            }

            await File.WriteAllTextAsync(destination, content);
        }
    }

    private SampleEntity GetSampleEntity()
    {
        return new SampleEntity
        {
            SampleKey = nameof(SampleEntity.SampleKey),
            SampleValue = nameof(SampleEntity.SampleValue),
            SampleList = new List<string> {"James", "John", "Mary"},
        };
    }

    private SampleRepository GetSampleRepository()
    {
        var context = GetRequiredService<IYamlContext>();
        context.Options.BaseDirectory = Settings.BaseDirectory;

        return new SampleRepository(context)
        {
            LazyServiceProvider = GetService<IAbpLazyServiceProvider>()
        };
    }
}