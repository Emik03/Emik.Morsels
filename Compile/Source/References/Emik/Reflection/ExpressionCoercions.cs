// SPDX-License-Identifier: MPL-2.0
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Methods to provide coercions to <see cref="System.Linq.Expressions.Expression"/>.</summary>
// ReSharper disable RedundantNameQualifier
static partial class ExpressionCoercions
{
    /// <param name="x">The expression to get the <see langword="string"/> representation of.</param>
    extension(System.Linq.Expressions.Expression x)
    {
        /// <summary>Provides the verbose representation found in the debug view.</summary>
        /// <returns>The verbose representation of the parameter <paramref name="x"/>.</returns>
        public string? ToVerboseString() =>
            typeof(System.Linq.Expressions.Expression)
               .GetProperty("DebugView", BindingFlags.Instance | BindingFlags.NonPublic)
              ?.GetGetMethod(true)
              ?.Invoke(x, null) as string;

        /// <summary>Gets the field or property.</summary>
        /// <param name="member">The member to access.</param>
        /// <returns>The <see cref="BinaryExpression"/> representing <c>left.member = right</c>.</returns>
        public MemberExpression FieldOrProperty(
            MemberInfo member
        ) =>
            member switch
            {
                PropertyInfo p => System.Linq.Expressions.Expression.Property(x, p),
                FieldInfo f => System.Linq.Expressions.Expression.Field(x, f),
                _ => throw Unreachable,
            };
    }
}
