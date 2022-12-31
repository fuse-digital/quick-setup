using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FuseDigital.QuickSetup.Text;

public interface ITextRepository : IRepository
{
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    Task<string> FindAsync(Expression<Func<string, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<List<string>> GetListAsync(CancellationToken cancellationToken = default);

    Task<List<string>> GetListAsync(Expression<Func<string, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(Expression<Func<string, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<string> InsertAsync(string entity, CancellationToken cancellationToken = default);

    Task InsertManyAsync(IEnumerable<string> entities, CancellationToken cancellationToken = default);

    Task DeleteAsync(string entity, CancellationToken cancellationToken = default);

    Task DeleteManyAsync(IEnumerable<string> entities, CancellationToken cancellationToken = default);
}