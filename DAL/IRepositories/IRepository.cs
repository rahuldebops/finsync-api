using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using finsyncapi.Models;
using Microsoft.EntityFrameworkCore.Storage;
using static Dapper.SqlMapper;

namespace finsyncapi.DAL.IRepositories
{
    public interface IRepository<TEntity>
    {
        Task<int> AddAsync(TEntity entity, bool? save = false);
        Task<int> AddAsync<T>(T entity, bool save = false) where T : class;
        Task<int> AddAllAsync(IEnumerable<TEntity> entities, bool save = false);
        Task<int> AddAllAsync<T>(IEnumerable<T> entities, bool save = false) where T : class;
        Task<TResult?> GetSingleOrDefaultWithSelectedColoumnAysnc<TResult>(Expression<Func<TEntity, TResult>> select, Expression<Func<TEntity, bool>> filter, bool tracking = false);
        Task<TResult> GetSingleWithSelectedColoumnAysnc<TResult>(Expression<Func<TEntity, TResult>> select, Expression<Func<TEntity, bool>> filter, bool tracking = false);
        Task<IEnumerable<TResult>> GetAllSelectedColumnAsync<TResult>(Expression<Func<TEntity, TResult>> select, Expression<Func<TEntity, bool>>? filter = null, bool tracking = false);
        Task<PagedResponse<TResult>> GetAllSelectedColumnAsync<TResult>(Expression<Func<TEntity, TResult>> select, QueryParameters queryParams, Expression<Func<TEntity, bool>>? filter = null);

        // Cross-entity methods
        Task<TResult?> GetSingleOrDefaultWithSelectedColoumnAysnc<TOtherEntity, TResult>(Expression<Func<TOtherEntity, TResult>> select, Expression<Func<TOtherEntity, bool>> filter, bool tracking = false) where TOtherEntity : class;
        Task<TResult> GetSingleWithSelectedColoumnAysnc<TOtherEntity, TResult>(Expression<Func<TOtherEntity, TResult>> select, Expression<Func<TOtherEntity, bool>> filter, bool tracking = false) where TOtherEntity : class;
        Task<IEnumerable<TResult>> GetAllSelectedColumnAsync<TOtherEntity, TResult>(Expression<Func<TOtherEntity, TResult>> select, Expression<Func<TOtherEntity, bool>>? filter = null, bool tracking = false) where TOtherEntity : class;
        Task<PagedResponse<TResult>> GetAllSelectedColumnAsync<TOtherEntity, TResult>(Expression<Func<TOtherEntity, TResult>> select, QueryParameters queryParams, Expression<Func<TOtherEntity, bool>>? filter = null) where TOtherEntity : class;

        Task<ICollection<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter, bool tracking = false);
        Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>>? filter, bool tracking = false);
        Task<TEntity> GetLastAsync(Expression<Func<TEntity, bool>>? filter, bool tracking = false);
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? filter);
        Task<bool> ExistsAsync<TOtherEntity>(Expression<Func<TOtherEntity, bool>>? filter = null) where TOtherEntity : class;
        Task<long> LongCountAsync(Expression<Func<TEntity, bool>>? filter);
        bool UpdatePartial(TEntity entity, params Expression<Func<TEntity, object>>[] properties);
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<PagedResponse<T>> ExecutePagedQueryAsync<T>(IDbConnection connection, string baseQuery, PaginationQuery pagination, object? parameters = null, string? totalCountQuery = null, CancellationToken cancellationToken = default);
    }


}