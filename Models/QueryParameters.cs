using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using finsyncapi.Helpers;

namespace finsyncapi.Models
{
    [ModelBinder(BinderType = typeof(QueryParametersModelBinder))]
    public class QueryParameters : PaginationQuery
    {
        // These properties will be populated by the custom model binder.
        public List<SortItem> SortItems { get; set; } = new List<SortItem>();
        public FilterModel FilterModel { get; set; } = new FilterModel();

        // A public parameterless constructor is still good practice.
        public QueryParameters() { }
    }

    public class PaginationQuery
    {
        [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be at least 1")]
        public int Page { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "PageSize must be at least 1")]
        public int PageSize { get; set; } = 10;
    }

    public class SortItem
    {
        public string Field { get; set; } = string.Empty;
        public bool Descending { get; set; }
    }

    public class FilterModel
    {
        public Dictionary<string, List<Condition>> Filters { get; set; } = new();
        public string Operator { get; set; } = "and"; // 'and'/'or' for combining different fields
    }

    public class Condition
    {
        public object? Value { get; set; }
        public string MatchMode { get; set; } = string.Empty;
        public string Operator { get; set; } = "or"; // 'and'/'or' for combining multiple values for the same field
    }

    internal sealed class QueryColumnMetadata
    {
        public required string PropertyName { get; init; }

        public required string OuterColumnName { get; init; }

        public required Type PropertyType { get; init; }

        public Type? DataType { get; init; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class QueryColumnAttribute : Attribute
    {
        /// <summary>
        /// Override the type used for filter parameter conversion.
        /// Use when the DTO property type differs from the DB column type
        /// (e.g., property is string but DB column is timestamp).
        /// </summary>
        public Type? DataType { get; set; }
    }
}
