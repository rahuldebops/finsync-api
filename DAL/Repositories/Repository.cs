using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using finsyncapi.DAL.IRepositories;
using finsyncapi.Helpers;
using finsyncapi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using finsyncapi.DAL.IRepositories;
using finsyncapi.Helpers;
using finsyncapi.Models;
using static Dapper.SqlMapper;
using finsyncapi.Helper;
using System.Data;
using Dapper;

namespace finsyncapi.DAL.Repositories
{
    public class Repository<TEntity, Tcontext> : IRepository<TEntity> where TEntity : class where Tcontext : DbContext
    {
        protected readonly Tcontext DbContext;
        internal DbSet<TEntity> dbSet;

        public Repository(Tcontext context)
        {
            DbContext = context;
            dbSet = context.Set<TEntity>();
        }

        private IQueryable<TEntity> Query(bool tracking) => tracking ? dbSet : dbSet.AsNoTracking();
        private IQueryable<TOtherEntity> Query<TOtherEntity>(bool tracking) where TOtherEntity : class
        {
            var set = DbContext.Set<TOtherEntity>();
            return tracking ? set : set.AsNoTracking();
        }

        public async Task<int> AddAsync(TEntity entity, bool? save = false)
        {
            await dbSet.AddAsync(entity);

            return save == true ? await SaveChangesAsync() : 0;
        }

        private DbSet<T> Set<T>() where T : class => DbContext.Set<T>();

        public async Task<int> AddAsync<T>(T entity, bool save = false) where T : class
        {
            await Set<T>().AddAsync(entity);

            return save ? await SaveChangesAsync() : 0;
        }

        public async Task<int> AddAllAsync(TEntity entity, bool? save = false)
        {
            await dbSet.AddAsync(entity);

            return save == true ? await SaveChangesAsync() : 0;
        }

        public async Task<int> AddAllAsync(IEnumerable<TEntity> entities, bool save = false)
        {
            if (entities == null || !entities.Any())
                return 0;

            await dbSet.AddRangeAsync(entities);

            return save ? await SaveChangesAsync() : 0;
        }

        public async Task<int> AddAllAsync<T>(IEnumerable<T> entities, bool save = false) where T : class
        {
            if (entities == null || !entities.Any())
                return 0;

            await DbContext.Set<T>().AddRangeAsync(entities);

            return save ? await SaveChangesAsync() : 0;
        }

        public async Task<IEnumerable<TResult>> GetAllSelectedColumnAsync<TResult>(Expression<Func<TEntity, TResult>> select, Expression<Func<TEntity, bool>>? filter = null, bool tracking = false)
        {
            var query = Query(tracking);
            if (filter != null) query = query.Where(filter);
            return await query.Select(select).ToListAsync();
        }

        public async Task<PagedResponse<TResult>> GetAllSelectedColumnAsync<TResult>(
        Expression<Func<TEntity, TResult>> select,
        QueryParameters queryParams,
        Expression<Func<TEntity, bool>>? filter = null)
        {
            var query = Query(true);  // Assuming this returns IQueryable<TEntity>

            if (filter != null)
                query = query.Where(filter);

            var projectedQuery = query.Select(select);  // This results in IQueryable<TResult>

            return await LinqQueryBuilder<TResult>.DynamicQueryResolver(projectedQuery, queryParams);
        }

        public async Task<ICollection<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter, bool tracking = false)
        {
            var query = Query(tracking);
            if (filter != null) query = query.Where(filter);
            return await query.ToListAsync();
        }

        public async Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>>? filter, bool tracking = false)
        {
            var query = Query(tracking);
            if (filter != null) query = query.Where(filter);
            return await query.FirstAsync();
        }

        public async Task<TEntity> GetLastAsync(Expression<Func<TEntity, bool>>? filter, bool tracking = false)
        {
            var query = Query(tracking);
            if (filter != null) query = query.Where(filter);
            return await query.OrderBy(e => true).LastAsync(); // dummy order
        }

        public async Task<TResult?> GetSingleOrDefaultWithSelectedColoumnAysnc<TResult>(
            Expression<Func<TEntity, TResult>> select, Expression<Func<TEntity, bool>> filter, bool tracking = false)
        {
            return await Query(tracking).Where(filter).Select(select).SingleOrDefaultAsync();
        }

        public async Task<TResult> GetSingleWithSelectedColoumnAysnc<TResult>(
            Expression<Func<TEntity, TResult>> select, Expression<Func<TEntity, bool>> filter, bool tracking = false)
        {
            return await Query(tracking).Where(filter).Select(select).SingleAsync();
        }

        // Cross-entity implementations
        public async Task<TResult?> GetSingleOrDefaultWithSelectedColoumnAysnc<TOtherEntity, TResult>(Expression<Func<TOtherEntity, TResult>> select, Expression<Func<TOtherEntity, bool>> filter, bool tracking = false) where TOtherEntity : class
        {
            return await Query<TOtherEntity>(tracking).Where(filter).Select(select).SingleOrDefaultAsync();
        }

        public async Task<TResult> GetSingleWithSelectedColoumnAysnc<TOtherEntity, TResult>(Expression<Func<TOtherEntity, TResult>> select, Expression<Func<TOtherEntity, bool>> filter, bool tracking = false) where TOtherEntity : class
        {
            return await Query<TOtherEntity>(tracking).Where(filter).Select(select).SingleAsync();
        }

        public async Task<IEnumerable<TResult>> GetAllSelectedColumnAsync<TOtherEntity, TResult>(Expression<Func<TOtherEntity, TResult>> select, Expression<Func<TOtherEntity, bool>>? filter = null, bool tracking = false) where TOtherEntity : class
        {
            var query = Query<TOtherEntity>(tracking);
            if (filter != null) query = query.Where(filter);
            return await query.Select(select).ToListAsync();
        }

        public async Task<PagedResponse<TResult>> GetAllSelectedColumnAsync<TOtherEntity, TResult>(Expression<Func<TOtherEntity, TResult>> select, QueryParameters queryParams, Expression<Func<TOtherEntity, bool>>? filter = null) where TOtherEntity : class
        {
            var query = Query<TOtherEntity>(true);
            if (filter != null) query = query.Where(filter);
            var projectedQuery = query.Select(select);
            return await LinqQueryBuilder<TResult>.DynamicQueryResolver(projectedQuery, queryParams);
        }



        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter)
        {
            return await (filter != null ? dbSet.CountAsync(filter) : dbSet.CountAsync());
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? filter)
        {
            return filter != null ? await dbSet.AnyAsync(filter): await dbSet.AnyAsync();
        }

        public async Task<bool> ExistsAsync<TOtherEntity>(Expression<Func<TOtherEntity, bool>>? filter = null) where TOtherEntity : class
        {
            var query = Query<TOtherEntity>(true);

            return filter != null ? await query.AnyAsync(filter): await query.AnyAsync();
        }

        public async Task<long> LongCountAsync(Expression<Func<TEntity, bool>>? filter)
        {
            return await (filter != null ? dbSet.LongCountAsync(filter) : dbSet.LongCountAsync());
        }

        public bool UpdatePartial(TEntity entity, params Expression<Func<TEntity, object>>[] properties)
        {
            var entry = DbContext.Entry(entity);
            foreach (var prop in properties)
            {
                var name = (prop.Body as MemberExpression ?? ((UnaryExpression)prop.Body).Operand as MemberExpression)?.Member.Name;
                if (!string.IsNullOrEmpty(name)) entry.Property(name).IsModified = true;
            }
            return true;
        }

        public Task<int> SaveChangesAsync() => DbContext.SaveChangesAsync();
        public Task<IDbContextTransaction> BeginTransactionAsync() => DbContext.Database.BeginTransactionAsync();

        public async Task<PagedResponse<T>> ExecutePagedQueryAsync<T>(IDbConnection connection, string baseQuery, PaginationQuery pagination, object? parameters = null, string? totalCountQuery = null, CancellationToken cancellationToken = default)
        {
            baseQuery = AppHelper.SanitizeQuery(baseQuery);

            string countQuery = totalCountQuery ?? $"SELECT COUNT(1) FROM ({baseQuery}) AS CountTable";

            countQuery = AppHelper.SanitizeQuery(countQuery);

            string paginatedQuery = $@" {baseQuery} OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var pagingParams = new
            {
                Offset = (pagination.Page - 1) * pagination.PageSize,
                PageSize = pagination.PageSize
            };

            var combinedParams = AppHelper.MergeParameters(parameters, pagingParams);

            string combinedQuery = $@"{countQuery};{paginatedQuery};";

            using var multi = await connection.QueryMultipleAsync(
                new CommandDefinition(
                    combinedQuery,
                    combinedParams,
                    cancellationToken: cancellationToken
                )
            );

            int totalRecords = await multi.ReadSingleAsync<int>();
            var data = await multi.ReadAsync<T>();

            return new PagedResponse<T>(data, pagination.Page, pagination.PageSize, totalRecords);
        }
    }

}