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

    /// <summary>Creates the assignment of a field or property.</summary>
    /// <param name="left">The left-hand side to mutate.</param>
    /// <param name="member">The member to access and assign.</param>
    /// <param name="right">The right-hand side containing the value to insert.</param>
    /// <returns>The <see cref="BinaryExpression"/> representing <c>left.member = right</c>.</returns>
    public static BinaryExpression AssignFieldOrProperty(this Expression left, MemberInfo member, Expression right) =>
        System.Linq.Expressions.Expression.Assign(
            member switch
            {
                PropertyInfo p => System.Linq.Expressions.Expression.Property(left, p),
                FieldInfo f => System.Linq.Expressions.Expression.Field(left, f),
                _ => throw Unreachable,
            },
            right
        );
}
#endif
