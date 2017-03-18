// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Remotion.Linq.Clauses;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Internal
{
    public static class RelationalExpressionExtensions
    {
        public static ColumnExpression TryGetColumnExpression([NotNull] this Expression expression)
            => expression as ColumnExpression
               ?? (expression as AliasExpression)?.TryGetColumnExpression()
               ?? (expression as NullableExpression)?.Operand.TryGetColumnExpression();

        public static IProperty TryGetProperty([NotNull] this Expression expression)
            => (expression as ColumnExpression)?.Property
               ?? (expression as AliasExpression)?.Expression.TryGetProperty()
               ?? (expression as ColumnReferenceExpression)?.Expression.TryGetProperty();

        public static IQuerySource TryGetQuerySource([NotNull] this Expression expression)
            => (expression as ColumnExpression)?.Table.QuerySource
               ?? (expression as AliasExpression)?.Expression.TryGetQuerySource()
               ?? (expression as ColumnReferenceExpression)?.Table.QuerySource;

        public static bool IsAliasWithColumnExpression([NotNull] this Expression expression)
            => (expression as AliasExpression)?.Expression is ColumnExpression;

        public static bool IsAliasWithSelectExpression([NotNull] this Expression expression)
            => (expression as AliasExpression)?.Expression is SelectExpression;

        public static bool HasColumnExpression([CanBeNull] this AliasExpression aliasExpression)
            => aliasExpression?.Expression is ColumnExpression;

        public static ColumnExpression TryGetColumnExpression([NotNull] this AliasExpression aliasExpression)
            => aliasExpression.Expression as ColumnExpression;

        public static bool IsSimpleExpression([NotNull] this Expression expression)
        {
            var unwrappedExpression = expression.RemoveConvert();

            return unwrappedExpression is ConstantExpression
                   || unwrappedExpression is ColumnExpression
                   || unwrappedExpression is ParameterExpression
                   || unwrappedExpression is ColumnReferenceExpression
                   || unwrappedExpression is AliasExpression;
        }

        public static ColumnReferenceExpression LiftExpressionFromSubquery(this Expression expression, TableExpressionBase table)
        {
            if (expression is ColumnExpression columnExpression)
            {
                return new ColumnReferenceExpression(columnExpression, table);
            }

            if (expression is AliasExpression aliasExpression)
            {
                return new ColumnReferenceExpression(aliasExpression, table);
            }

            if (expression is ColumnReferenceExpression columnReferenceExpression)
            {
                return new ColumnReferenceExpression(columnReferenceExpression, table);
            }

            return null;
        }
    }
}
