using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace FuseDigital.QuickSetup.Yaml;

public interface IYamlRepository<TEntity> : IRepository<TEntity>
    where TEntity : class, IEntity
{
    string FileName { get; }
    string FilePath { get; }
}

public interface IYamlRepository<TEntity, TKey> : IYamlRepository<TEntity>, IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
}