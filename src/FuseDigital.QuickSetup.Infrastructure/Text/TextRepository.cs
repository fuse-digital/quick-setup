using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FuseDigital.QuickSetup.Entities;
using Volo.Abp.Domain.Entities;

namespace FuseDigital.QuickSetup.Text;

public abstract class TextRepository : ITextRepository
{
    public abstract string FileName { get; }

    public virtual string FilePath => Path.Combine(Context.Options.UserProfile, Context.Options.BaseDirectory, FileName);

    protected ITextContext Context { get; }

    public TextRepository(ITextContext context)
    {
        Context = context;
    }

    public async Task<IList<string>> GetDbSetAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = await Context.LoadFromFileAsync(FilePath, cancellationToken);
        return dbSet ?? new List<string>();
    }
    
    private async Task SaveChangesAsync(IList<string> entities, CancellationToken cancellationToken = default)
    {
        var dbSet = entities.Order().ToList();
        await Context.SaveToFileAsync(dbSet, FilePath, cancellationToken);
    }
    
    private async Task<IQueryable<string>> GetQueryableAsync(CancellationToken cancellationToken)
    {
        return (await GetDbSetAsync(cancellationToken)).AsQueryable();
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryableAsync(cancellationToken);
        return queryable.Count();
    }

    public async Task<string> FindAsync(Expression<Func<string, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return (await GetQueryableAsync(cancellationToken))
            .Where(predicate)
            .FirstOrDefault();
    }

    public async Task<List<string>> GetListAsync(CancellationToken cancellationToken = default)
    {
        return (await GetDbSetAsync(cancellationToken)).ToList();
    }

    public async Task<List<string>> GetListAsync(Expression<Func<string, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return (await GetQueryableAsync(cancellationToken)).Where(predicate).ToList();
    }

    public async Task DeleteAsync(Expression<Func<string, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entities = (await GetQueryableAsync(cancellationToken))
            .Where(predicate)
            .ToList();

        await DeleteManyAsync(entities, cancellationToken);
    }

    public async Task<string> InsertAsync(string entity, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync(cancellationToken);
        if (EntityExists(dbSet, entity))
        {
            throw new EntityAlreadyExistsException(typeof(string), entity);
        }
        dbSet.Add(entity);
        await SaveChangesAsync(dbSet, cancellationToken);
        return entity;
    }
    
    private bool EntityExists(IList<string> dbSet, string entity)
    {
        return dbSet.Any(x => x.Equals(entity, StringComparison.InvariantCultureIgnoreCase));
    }

    public async Task InsertManyAsync(IEnumerable<string> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await InsertAsync(entity, cancellationToken: cancellationToken);
        }
    }
    
    protected virtual async Task<string> FindAsync(string entity,
        IList<string> dbSet = null,
        CancellationToken cancellationToken = default)
    {
        return (dbSet ?? await GetDbSetAsync(cancellationToken))
            .FirstOrDefault(x => x.Equals(entity, StringComparison.InvariantCultureIgnoreCase));
    }
    
    private async Task RemoveAsync(string entity, IList<string> dbSet, CancellationToken cancellationToken = default)
    {
        var item = await FindAsync(entity, dbSet, cancellationToken);
        if (item == null)
        {
            throw new EntityNotFoundException(typeof(string), entity);
        }

        dbSet.Remove(item);
    }

    public async Task DeleteAsync(string entity, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync(cancellationToken);
        await RemoveAsync(entity, dbSet, cancellationToken);
        await SaveChangesAsync(dbSet, cancellationToken);
    }

    public async Task DeleteManyAsync(IEnumerable<string> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await DeleteAsync(entity, cancellationToken: cancellationToken);
        }
    }
}