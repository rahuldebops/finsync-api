using System.Text.RegularExpressions;
using Dapper;
using finsyncapi.Models;

namespace finsyncapi.Helpers
{
    public static class SqlClauseBuilder
    {
        // A simple whitelist to prevent invalid characters in column names from the mapping.
        private static readonly Regex UnsafeColumnNameRegex = new Regex(@"[^a-zA-Z0-9_.]");

        /// <summary>
        /// Builds a SQL ORDER BY clause from a list of SortItem objects.
        /// </summary>
        /// <param name="sortItems">The list of fields to sort by and their direction.</param>
        /// <param name="columnMapping">A dictionary mapping API sort names to database column names.</param>
        /// <returns>A SQL ORDER BY clause string (e.g., "ORDER BY g.name DESC, g.created_at ASC").</returns>
        public static string BuildOrderByClause(
            List<SortItem> sortItems,
            Dictionary<string, string> columnMapping)
        {
            if (sortItems == null || !sortItems.Any())
            {
                return string.Empty; // Or a default sort order
            }

            var sortClauses = new List<string>();
            foreach (var item in sortItems)
            {
                if (string.IsNullOrWhiteSpace(item.Field) || !columnMapping.TryGetValue(item.Field, out var dbColumn))
                {
                    // Ignore invalid or unmapped sort fields
                    continue;
                }

                // Sanitize the mapped column name to be extra safe
                if (UnsafeColumnNameRegex.IsMatch(dbColumn))
                {
                    // This indicates a configuration error in the mapping, not user input.
                    // Log this error or throw an exception.
                    continue;
                }

                string direction = item.Descending ? "DESC" : "ASC";
                sortClauses.Add($"{dbColumn} {direction}");
            }

            if (!sortClauses.Any())
            {
                return string.Empty;
            }

            return $" ORDER BY {string.Join(", ", sortClauses)}";
        }

        /// <summary>
        /// Builds a SQL WHERE clause from a complex FilterModel.
        /// Supports nested conditions and multiple operators.
        /// </summary>
        /// <param name="filterModel">The filter model containing fields, conditions, and operators.</param>
        /// <param name="columnMapping">A dictionary mapping API filter names to database column names.</param>
        /// <param name="dapperParams">An out parameter for the Dapper parameters to prevent SQL injection.</param>
        /// <returns>A SQL WHERE clause string (e.g., "AND ((g.currency_code = @p0) OR (g.name LIKE @p1))").</returns>
        public static string BuildWhereClause(
            FilterModel filterModel,
            Dictionary<string, string> columnMapping,
            out DynamicParameters dapperParams)
        {
            dapperParams = new DynamicParameters();
            if (filterModel?.Filters == null || !filterModel.Filters.Any())
            {
                return string.Empty;
            }

            var fieldClauses = new List<string>();
            int paramIndex = 0;

            foreach (var fieldFilter in filterModel.Filters)
            {
                string apiField = fieldFilter.Key;
                List<Condition> conditions = fieldFilter.Value;

                if (!columnMapping.TryGetValue(apiField, out var dbColumn) || !conditions.Any())
                {
                    continue; // Skip unmapped or empty filters
                }

                if (UnsafeColumnNameRegex.IsMatch(dbColumn)) continue;

                var conditionClauses = new List<string>();
                foreach (var condition in conditions)
                {
                    if (condition.Value == null) continue;

                    string paramName = $"@p{paramIndex++}";
                    string? clause = GenerateClauseForCondition(dbColumn, condition, paramName, dapperParams);

                    if (!string.IsNullOrEmpty(clause))
                    {
                        conditionClauses.Add(clause);
                    }
                }

                if (conditionClauses.Any())
                {
                    // Use the operator from the first condition to join all conditions for this field.
                    // E.g., for filter[name][0][op]=or&filter[name][1]..., all "name" filters are joined by OR.
                    string innerOperator = GetSqlOperator(conditions.First().Operator);
                    fieldClauses.Add($"({string.Join($" {innerOperator} ", conditionClauses)})");
                }
            }

            if (!fieldClauses.Any())
            {
                return string.Empty;
            }

            // Combine clauses from different fields (e.g., name and currency) using the main operator.
            string outerOperator = GetSqlOperator(filterModel.Operator);
            return $" AND {string.Join($" {outerOperator} ", fieldClauses)}";
        }

        private static string? GenerateClauseForCondition(string dbColumn, Condition condition, string paramName, DynamicParameters dapperParams)
        {
            string op;
            object paramValue = condition.Value;

            switch (condition.MatchMode.ToLowerInvariant())
            {
                case "equals": op = "="; break;
                case "notequals": op = "!="; break;
                case "contains":
                    op = "LIKE";
                    paramValue = $"%{condition.Value}%";
                    break;
                case "startswith":
                    op = "LIKE";
                    paramValue = $"{condition.Value}%";
                    break;
                case "endswith":
                    op = "LIKE";
                    paramValue = $"%{condition.Value}%";
                    break;
                case "gt":
                case "greaterthan": op = ">"; break;
                case "lt":
                case "lessthan": op = "<"; break;
                case "gte":
                case "greaterthanorequal": op = ">="; break;
                case "lte":
                case "lessthanorequal": op = "<="; break;
                case "in":
                    // For 'IN' clauses, Dapper handles collections automatically.
                    // The value should be a list/array from the JSON payload.
                    op = "IN";
                    break;
                case "notin":
                    op = "NOT IN";
                    break;
                default: return null; // Ignore unsupported match modes
            }

            dapperParams.Add(paramName, paramValue);
            return $"{dbColumn} {op} {paramName}";
        }

        private static string GetSqlOperator(string op) => op.ToLowerInvariant() == "and" ? "AND" : "OR";
    }
}
