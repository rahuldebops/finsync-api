using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Dapper;
using Microsoft.EntityFrameworkCore;
using finsyncapi.DTO;
using finsyncapi.Models;
using finsyncapi.Helper; // Adjust to your models namespace

namespace finsyncapi.Helpers
{
    public static class LinqQueryBuilder<T>
    {
        public static async Task<PagedResponse<T>> DynamicQueryResolver(IQueryable<T> query, QueryParameters queryParams)
        {
            query = FilterQueryResolver(query, queryParams.FilterModel);
            query = ApplySorting(query, queryParams.SortItems);

            int skip = (queryParams.Page - 1) * queryParams.PageSize;
            var pagedData = await query.Skip(skip).Take(queryParams.PageSize).ToListAsync();
            int totalRecords = await query.CountAsync();

            return new PagedResponse<T>(pagedData, queryParams.Page, queryParams.PageSize, totalRecords);
        }

        private static IQueryable<T> FilterQueryResolver(IQueryable<T> query, FilterModel filterModel)
        {
            if (filterModel.Filters.Any())
            {
                // NOTE: Assumes you have an ExpressionHelper to convert FilterModel to a LINQ expression.
                // This part is specific to your implementation.
                var filterExpression = ExpressionHelper.GetFilterExpression<T>(filterModel);
                query = query.Where(filterExpression);
            }
            return query;
        }

        private static IQueryable<T> ApplySorting(IQueryable<T> query, List<SortItem> sortItems)
        {
            if (!sortItems.Any()) return query;

            bool isFirst = true;
            foreach (var sort in sortItems)
            {
                var param = Expression.Parameter(typeof(T), "x");
                var prop = Expression.PropertyOrField(param, sort.Field);
                var lambda = Expression.Lambda(prop, param);

                string methodName = isFirst ?
                    (sort.Descending ? "OrderByDescending" : "OrderBy") :
                    (sort.Descending ? "ThenByDescending" : "ThenBy");

                var method = typeof(Queryable).GetMethods()
                    .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), prop.Type);

                query = (IQueryable<T>)method.Invoke(null, new object[] { query, lambda })!;
                isFirst = false;
            }
            return query;
        }

        internal static async Task<PagedResponse<T>> ExecutePaginationQuery(IDbConnection _connection, string query, PaginationQuery paginationQuery, object parameters)
        {
            string countQuery = $"SELECT COUNT(*) FROM ({query}) AS CountTable";
            int totalRecords = await _connection.ExecuteScalarAsync<int>(countQuery, parameters);

            query += $" LIMIT {paginationQuery.PageSize} OFFSET {(paginationQuery.PageSize * (paginationQuery.Page - 1))}";
            var data = await _connection.QueryAsync<T>(query, parameters);

            return new PagedResponse<T>(data, paginationQuery.Page, paginationQuery.PageSize, totalRecords);
        }        
    }
}
