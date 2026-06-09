using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using finsyncapi.Models; // Adjust to your models namespace

namespace finsyncapi.Helpers
{
    public static class QueryParser
    {
        // The main factory method that creates a fully populated QueryParameters object.
        public static QueryParameters Parse(IQueryCollection query)
        {
            return new QueryParameters
            {
                Page = int.TryParse(query["page"], out var p) ? Math.Max(p, 1) : 1,
                PageSize = int.TryParse(query["pageSize"], out var ps) ? Math.Max(ps, 1) : 10,
                SortItems = ParseSort(query["sort"]),
                FilterModel = ParseFilters(query)
            };
        }

        private static List<SortItem> ParseSort(string? sortString)
        {
            var sortItems = new List<SortItem>();
            if (string.IsNullOrWhiteSpace(sortString)) return sortItems;

            var parts = sortString.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var part in parts)
            {
                var field = part;
                var descending = field.StartsWith('-');
                if (descending) field = field[1..];

                if (!string.IsNullOrWhiteSpace(field))
                {
                    sortItems.Add(new SortItem { Field = field, Descending = descending });
                }
            }
            return sortItems;
        }

        private static FilterModel ParseFilters(IQueryCollection query)
        {
            var filterModel = new FilterModel();
            foreach (var kvp in query)
            {
                var match = Regex.Match(kvp.Key, @"filter\[([^\]]+)\](?:\[([^\]]+)\])?", RegexOptions.IgnoreCase);
                if (!match.Success) continue;

                var field = match.Groups[1].Value;
                var op = match.Groups.Count > 2 && match.Groups[2].Success ? match.Groups[2].Value.ToLower() : "eq";

                var matchMode = op switch
                {
                    "eq" => "equals",
                    "neq" => "notequals",
                    "gt" => "greaterthan",
                    "gte" => "greaterthanorequal",
                    "lt" => "lessthan",
                    "lte" => "lessthanorequal",
                    _ => op // pass through as-is (contains, notcontains, startswith, endswith, between, in, notin, isnull, isnotnull, isempty, isnotempty, equals, etc.)
                };

                if (!filterModel.Filters.ContainsKey(field))
                {
                    filterModel.Filters[field] = new List<Condition>();
                }
                foreach (var value in kvp.Value)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        filterModel.Filters[field].Add(new Condition { Value = value, MatchMode = matchMode, Operator = "and" });
                    }
                }
            }
            return filterModel;
        }
    }
}
