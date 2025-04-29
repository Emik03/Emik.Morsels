// SPDX-License-Identifier: MPL-2.0
#if !NETSTANDARD || NETSTANDARD2_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Methods to provide coercions to <see cref="Expression"/>.</summary>
// ReSharper disable RedundantNameQualifier
static partial class ExpressionCoercions
{
    /// <summary>Provides the verbose representation found in the debug view.</summary>
    /// <param name="x">The expression to get the <see langword="string"/> representation of.</param>
    /// <returns>The verbose representation of the parameter <paramref name="x"/>.</returns>
    public static string? ToVerboseString(this Expression x) =>
        typeof(System.Linq.Expressions.Expression)
           .GetProperty("DebugView", BindingFlags.Instance | BindingFlags.NonPublic)
          ?.GetGetMethod(true)
          ?.Invoke(x, null) as string;

    /// <summary>Gets the field or property.</summary>
    /// <param name="expression">The expression to retrieve from.</param>
    /// <param name="member">The member to access.</param>
    /// <returns>The <see cref="BinaryExpression"/> representing <c>left.member = right</c>.</returns>
    public static MemberExpression FieldOrProperty(this Expression expression, MemberInfo member) =>
        member switch
        {
            PropertyInfo p => System.Linq.Expressions.Expression.Property(expression, p),
            FieldInfo f => System.Linq.Expressions.Expression.Field(expression, f),
            _ => throw Unreachable,
        };
}
#endif
