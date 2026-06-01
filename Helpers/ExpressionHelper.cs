using finsyncapi.Models;
using System.Linq.Expressions;
using System.Reflection;

namespace finsyncapi.Helpers
{
    public class ExpressionHelper
    {
        public static Expression<Func<T, bool>> GetFilterExpression<T>(FilterModel filterModel)
        {
            var parameter = Expression.Parameter(typeof(T), "e");
            Expression? filterExpression = null;

            foreach (var field in filterModel.Filters)
            {
                string fieldName = field.Key;
                Expression? individualFieldFilterExpression = null;

                foreach (var condition in field.Value)
                {
                    if (condition.Value == null || condition.MatchMode == null) continue;
                    // Access property dynamically
                    var property = Expression.Property(parameter, fieldName);
                    var propertyType = ((PropertyInfo)property.Member).PropertyType;

                    // Convert value to the property's type
                    var value = Expression.Constant(Convert.ChangeType(condition.Value, propertyType), propertyType);

                    // Build condition based on match mode
                    Expression conditionExpression = condition.MatchMode switch
                    {
                        "gt" => Expression.GreaterThan(property, value),
                        "lt" => Expression.LessThan(property, value),
                        "equals" => Expression.Equal(property, value),
                        "notEquals" => Expression.NotEqual(property, value),
                        "startsWith" => propertyType == typeof(string)
                            ? Expression.Call(
                                property,
                                typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!,
                                value
                            )
                            : throw new NotSupportedException($"'startswith' not supported for type {propertyType}"),
                        "endsWith" => propertyType == typeof(string)
                            ? Expression.Call(
                                property,
                                typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!,
                                value
                            )
                            : throw new NotSupportedException($"'endswith' not supported for type {propertyType}"),
                        "contains" => propertyType == typeof(string)
                            ? Expression.Call(
                                property,
                                typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                                value
                            )
                            : throw new NotSupportedException($"'contains' not supported for type {propertyType}"),
                        "notContains" => propertyType == typeof(string)
                            ? Expression.Not(
                                Expression.Call(
                                    property,
                                    typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                                    value
                                )
                            )
                            : throw new NotSupportedException($"'notcontains' not supported for type {propertyType}"),
                        _ => throw new NotSupportedException($"Unsupported match mode: {condition.MatchMode}.")
                    };
                    if (conditionExpression == null) continue;
                    individualFieldFilterExpression = CombineExpression(individualFieldFilterExpression, conditionExpression, condition.Operator);
                }

                if (individualFieldFilterExpression == null) continue;
                filterExpression = CombineExpression(filterExpression, individualFieldFilterExpression, filterModel.Operator);
            }

            // Return a valid expression even if no filters were provided
            return Expression.Lambda<Func<T, bool>>(filterExpression ?? Expression.Constant(true), parameter);
        }

        private static Expression CombineExpression(Expression? existing, Expression toAdd, string @operator)
        {
            if (existing == null)
            {
                return toAdd;
            }

            return @operator == "and"
                ? Expression.AndAlso(existing, toAdd)
                : @operator == "or"
                    ? Expression.OrElse(existing, toAdd)
                    : throw new NotSupportedException($"Unsupported operator type: {@operator}.");
        }

        public static Expression<Func<T, bool>> AndAlso<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }


        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                    return _newValue;
                return base.Visit(node);
            }
        }
    }
}
