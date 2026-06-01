using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Dapper;
using Microsoft.EntityFrameworkCore;
using finsyncapi.DTO;
using finsyncapi.Models;
using finsyncapi.Helper;

namespace finsyncapi.Helpers
{
    public static class RawSqlQueryBuilder<T>
    {
        private static readonly Dictionary<string, QueryColumnMetadata> QueryColumns =
            typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(x => new
                {
                    Property = x,
                    Attribute = x.GetCustomAttribute<QueryColumnAttribute>()
                })
                .Where(x => x.Attribute != null)
                .ToDictionary(
                    x => x.Property.Name,
                    x => new QueryColumnMetadata
                    {
                        PropertyName = x.Property.Name,
                        ColumnName = x.Attribute!.ColumnName,
                        AllowFilter = x.Attribute.AllowFilter,
                        AllowSort = x.Attribute.AllowSort,
                        PropertyType = Nullable.GetUnderlyingType(x.Property.PropertyType) ?? x.Property.PropertyType
                    },
                    StringComparer.OrdinalIgnoreCase);

        public static async Task<PagedResponse<T>> ExecuteAsync(IDbConnection connection,string baseQuery,QueryParameters queryParameters,object? parameters = null,IDbTransaction? transaction = null, int? commandTimeout = null)
        {
            if (string.IsNullOrWhiteSpace(baseQuery))
                throw new ArgumentException("Base query cannot be empty.");

            var dynamicParameters = new DynamicParameters(parameters);

            var sqlBuilder = new StringBuilder(baseQuery.Trim());

            AppendFilters(sqlBuilder,dynamicParameters,queryParameters.FilterModel);

            AppendSorting(sqlBuilder,queryParameters.SortItems);

            AppendPagination(sqlBuilder,dynamicParameters,queryParameters.Page,queryParameters.PageSize);

            string finalQuery = sqlBuilder.ToString();

            string countQuery = BuildCountQuery(baseQuery,queryParameters.FilterModel,dynamicParameters);

            int totalRecords = await connection.ExecuteScalarAsync<int>(countQuery,dynamicParameters,transaction,commandTimeout);

            var data = await connection.QueryAsync<T>(finalQuery,dynamicParameters,transaction,commandTimeout);

            return new PagedResponse<T>(data,queryParameters.Page,queryParameters.PageSize,totalRecords);
        }

        private static void AppendFilters(StringBuilder sql,DynamicParameters parameters,FilterModel filterModel)
        {
            if (filterModel?.Filters == null || filterModel.Filters.Count == 0)
            {
                return;
            }

            int parameterIndex = 0;

            var globalConditions = new List<string>();

            foreach (var filter in filterModel.Filters)
            {
                if (!QueryColumns.TryGetValue(filter.Key, out var metadata))
                    continue;

                if (!metadata.AllowFilter)
                    continue;

                var fieldConditions = new List<string>();

                foreach (var condition in filter.Value)
                {
                    string parameterName = $"p_{parameterIndex++}";

                    string? sqlCondition = BuildCondition(metadata,condition,parameterName,parameters);

                    if (!string.IsNullOrWhiteSpace(sqlCondition))
                    {
                        fieldConditions.Add(sqlCondition);
                    }
                }

                if (fieldConditions.Count == 0)
                    continue;

                string fieldOperator =
                    filter.Value.FirstOrDefault()?.Operator?.Equals("or",StringComparison.OrdinalIgnoreCase) == true? " OR ": " AND ";

                globalConditions.Add($"({string.Join(fieldOperator, fieldConditions)})");
            }

            if (globalConditions.Count == 0)return;

            string globalOperator =
                filterModel.Operator.Equals("or",StringComparison.OrdinalIgnoreCase)? " OR ": " AND ";

            sql.AppendLine();
            sql.Append(" AND ");
            sql.Append(string.Join(globalOperator, globalConditions));
        }

        private static string? BuildCondition(QueryColumnMetadata metadata,Condition condition,string parameterName,DynamicParameters parameters)
        {
            string column = metadata.ColumnName;

            string matchMode =condition.MatchMode?.Trim()?.ToLowerInvariant()?? "equals";

            object? value = ConvertValue(condition.Value,metadata.PropertyType);

            switch (matchMode)
            {
                case "equals":
                    parameters.Add(parameterName, value);
                    return $"{column} = @{parameterName}";

                case "notequals":
                    parameters.Add(parameterName, value);
                    return $"{column} <> @{parameterName}";

                case "contains":
                    parameters.Add(parameterName, $"%{value}%");
                    return $"{column} ILIKE @{parameterName}";

                case "notcontains":
                    parameters.Add(parameterName, $"%{value}%");
                    return $"{column} NOT ILIKE @{parameterName}";

                case "startswith":
                    parameters.Add(parameterName, $"{value}%");
                    return $"{column} ILIKE @{parameterName}";

                case "endswith":
                    parameters.Add(parameterName, $"%{value}");
                    return $"{column} ILIKE @{parameterName}";

                case "greaterthan":
                    parameters.Add(parameterName, value);
                    return $"{column} > @{parameterName}";

                case "greaterthanorequal":
                    parameters.Add(parameterName, value);
                    return $"{column} >= @{parameterName}";

                case "lessthan":
                    parameters.Add(parameterName, value);
                    return $"{column} < @{parameterName}";

                case "lessthanorequal":
                    parameters.Add(parameterName, value);
                    return $"{column} <= @{parameterName}";

                case "between":
                    {
                        if (condition.Value is not IEnumerable<object> values)
                            return null;

                        var items = values.ToList();

                        if (items.Count != 2)
                            return null;

                        string startParam = $"{parameterName}_start";
                        string endParam = $"{parameterName}_end";

                        parameters.Add(startParam,
                            ConvertValue(items[0], metadata.PropertyType));

                        parameters.Add(endParam,
                            ConvertValue(items[1], metadata.PropertyType));

                        return $"{column} BETWEEN @{startParam} AND @{endParam}";
                    }

                case "in":
                    {
                        if (condition.Value is not IEnumerable<object> values)
                            return null;

                        var list = values.ToList();

                        if (list.Count == 0)
                            return null;

                        parameters.Add(parameterName, list);

                        return $"{column} = ANY(@{parameterName})";
                    }

                case "notin":
                    {
                        if (condition.Value is not IEnumerable<object> values)
                            return null;

                        var list = values.ToList();

                        if (list.Count == 0)
                            return null;

                        parameters.Add(parameterName, list);

                        return $"NOT ({column} = ANY(@{parameterName}))";
                    }

                case "isnull":
                    return $"{column} IS NULL";

                case "isnotnull":
                    return $"{column} IS NOT NULL";

                case "isempty":
                    return $"({column} IS NULL OR {column} = '')";

                case "isnotempty":
                    return $"({column} IS NOT NULL AND {column} <> '')";

                default:
                    return null;
            }
        }

        private static void AppendSorting(StringBuilder sql,List<SortItem> sortItems)
        {
            if (sortItems == null || sortItems.Count == 0)
                return;

            var orderByClauses = new List<string>();

            foreach (var item in sortItems)
            {
                if (!QueryColumns.TryGetValue(item.Field, out var metadata))
                    continue;

                if (!metadata.AllowSort)
                    continue;

                orderByClauses.Add(
                    $"{metadata.ColumnName} {(item.Descending ? "DESC" : "ASC")}");
            }

            if (orderByClauses.Count == 0)
                return;

            sql.AppendLine();
            sql.Append(" ORDER BY ");
            sql.Append(string.Join(", ", orderByClauses));
        }

        private static void AppendPagination(StringBuilder sql,DynamicParameters parameters,int page,int pageSize)
        {
            int offset = (page - 1) * pageSize;

            parameters.Add("PageSize", pageSize);
            parameters.Add("Offset", offset);

            sql.AppendLine();
            sql.Append(" LIMIT @PageSize OFFSET @Offset ");
        }

        private static string BuildCountQuery(string baseQuery,FilterModel filterModel,DynamicParameters parameters)
        {
            var sql = new StringBuilder(baseQuery.Trim());

            AppendFilters(sql, parameters, filterModel);

            return $"SELECT COUNT(*) FROM ({sql}) count_query";
        }

        private static object? ConvertValue(object? value,Type targetType)
        {
            if (value == null)
                return null;

            if (targetType.IsEnum)
            {
                return Enum.Parse(targetType,value.ToString()!,true);
            }

            if (targetType == typeof(Guid))
            {
                return Guid.Parse(value.ToString()!);
            }

            return Convert.ChangeType(value, targetType);
        }
    }
}
