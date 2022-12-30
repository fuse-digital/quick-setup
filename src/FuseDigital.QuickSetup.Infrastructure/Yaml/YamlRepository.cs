using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FuseDigital.QuickSetup.Entities;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace FuseDigital.QuickSetup.Yaml;

public class YamlRepository<TEntity> : RepositoryBase<TEntity>, IYamlRepository<TEntity>
    where TEntity : class, IEntity
{
    public string FileName => GetType().Name
        .Replace("Repository", ".yml", StringComparison.InvariantCultureIgnoreCase)
        .ToLower();

    public string FilePath => Path.Combine(Context.Options.UserProfile, Context.Options.BaseDirectory, FileName);

    private IYamlContext Context { get; }

    protected YamlRepository(IYamlContext context)
    {
        Context = context;
    }

    public async Task<IList<TEntity>> GetDbSetAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = await Context.LoadFromFileAsync<List<TEntity>>(FilePath, cancellationToken);
        return dbSet ?? new List<TEntity>();
    }

    public virtual Func<TEntity, object> SortOrder<TKey>()
    {
        return null;
    }

    private async Task SaveChangesAsync(IList<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var sortSelector = SortOrder<object>();
        var index = sortSelector != null
            ? entities.OrderBy(sortSelector).ToList()
            : entities;
        
        await Context.SaveToFileAsync(index, FilePath, cancellationToken);
    }

    public override async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync(cancellationToken);
        if (EntityExists(dbSet, entity))
        {
            throw new EntityAlreadyExistsException(typeof(TEntity), entity.GetKeys());
        }

        dbSet.Add(entity);
        await SaveChangesAsync(dbSet, cancellationToken);
        return entity;
    }

    private bool EntityExists(IList<TEntity> dbSet, TEntity entity)
    {
        var keys = entity.GetUniqueIdentifier();
        return dbSet.Any(x => x.GetUniqueIdentifier().Equals(keys, StringComparison.InvariantCultureIgnoreCase));
    }

    public override async Task<TEntity> UpdateAsync(TEntity entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync(cancellationToken);
        await RemoveAsync(entity, dbSet, cancellationToken);
        dbSet.Add(entity);
        await SaveChangesAsync(dbSet, cancellationToken);

        return entity;
    }

    private async Task RemoveAsync(TEntity entity, IList<TEntity> dbSet, CancellationToken cancellationToken = default)
    {
        var item = await FindAsync(entity.GetKeys(), dbSet, cancellationToken);
        if (item == null)
        {
            throw new EntityNotFoundException(typeof(IEntity), entity.GetKeys());
        }

        dbSet.Remove(item);
    }

    public override async Task DeleteAsync(TEntity entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync(cancellationToken);
        await RemoveAsync(entity, dbSet, cancellationToken);
        await SaveChangesAsync(dbSet, cancellationToken);
    }

    public override async Task<List<TEntity>> GetListAsync(bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        return (await GetDbSetAsync(cancellationToken)).ToList();
    }

    public override async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return (await GetDbSetAsync(cancellationToken)).Count;
    }

    public override async Task<List<TEntity>> GetPagedListAsync(int skipCount,
        int maxResultCount,
        string sorting,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryableAsync(cancellationToken);
        return queryable
            .PageBy(skipCount, maxResultCount)
            .ToList();
    }

    public async Task<List<TEntity>> GetPagedListAsync(int skipCount,
        int maxResultCount,
        CancellationToken cancellationToken = default)
    {
        return await GetPagedListAsync(skipCount, maxResultCount, default, default, cancellationToken);
    }

    public override async Task<IQueryable<TEntity>> GetQueryableAsync()
    {
        return (await GetDbSetAsync()).AsQueryable();
    }

    private async Task<IQueryable<TEntity>> GetQueryableAsync(CancellationToken cancellationToken)
    {
        return (await GetDbSetAsync(cancellationToken)).AsQueryable();
    }

    public override async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate,
        bool includeDetails = true,
        CancellationToken cancellationToken = default)
    {
        return (await GetQueryableAsync(cancellationToken))
            .Where(predicate)
            .FirstOrDefault();
    }

    public override async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        var entities = (await GetQueryableAsync(cancellationToken))
            .Where(predicate)
            .ToList();

        await DeleteManyAsync(entities, autoSave, cancellationToken);
    }

    public override async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        return (await GetQueryableAsync(cancellationToken)).Where(predicate).ToList();
    }

    [Obsolete("Use GetQueryableAsync method.")]
    protected override IQueryable<TEntity> GetQueryable()
    {
        return GetQueryableAsync().GetAwaiter().GetResult();
    }

    protected virtual async Task<TEntity> FindAsync(object[] keys,
        IList<TEntity> dbSet = null,
        CancellationToken cancellationToken = default)
    {
        var id = keys.GetUniqueIdentifier();
        return (dbSet ?? await GetDbSetAsync(cancellationToken))
            .FirstOrDefault(x => x.GetUniqueIdentifier().Equals(id));
    }
}