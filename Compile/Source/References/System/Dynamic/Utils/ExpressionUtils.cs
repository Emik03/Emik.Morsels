// SPDX-License-Identifier: MPL-2.0
#if NETFRAMEWORK && !NET40_OR_GREATER
namespace System.Dynamic.Utils;

/// <summary>Methods needed for expression polyfills.</summary>
static partial class ExpressionUtils
{
    public static void RequiresCanRead(Expression expression, string paramName) =>
        RequiresCanRead(expression, paramName, -1);

    public static void RequiresCanRead(Expression expression, string paramName, int idx)
    {
        // Validate that we can read the node.
        if (expression.NodeType != ExpressionType.MemberAccess)
            return;

        var member = (MemberExpression)expression;

        if (member.Member is not PropertyInfo prop)
            return;

        if (!prop.CanRead)
            throw new ArgumentException($"Expression {idx} must be readable.", paramName);
    }

    internal static bool SameElements<T>(ICollection<T>? replacement, ReadOnlyCollection<T> current)
        where T : class
    {
        // Relatively common case, so particularly useful to take the short-circuit.
        if (Equals(replacement, current))
            return true;

        // Treat null as empty.
        if (replacement == null)
            return current.Count == 0;

        return SameElementsInCollection(replacement, current);
    }

    internal static bool SameElements<T>(ref IEnumerable<T>? replacement, ReadOnlyCollection<T> current)
        where T : class
    {
        // Relatively common case, so particularly useful to take the short-circuit.
        if (Equals(replacement, current))
            return true;

        // Treat null as empty.
        if (replacement == null)
            return current.Count == 0;

        // Ensure arguments is safe to enumerate twice.
        // If we have to build a collection, build a TrueReadOnlyCollection<T>
        // so it won't be built a second time if used.
        if (replacement is not ICollection<T> replacementCol)
            replacement = replacementCol = Array.AsReadOnly(replacement.ToArray());

        return SameElementsInCollection(replacementCol, current);
    }

    static bool SameElementsInCollection<T>(ICollection<T> replacement, ReadOnlyCollection<T> current)
        where T : class
    {
        var count = current.Count;

        if (replacement.Count != count)
            return false;

        if (count == 0)
            return true;

        var index = 0;

        foreach (var replacementObject in replacement)
        {
            if (replacementObject != current[index])
                return false;

            index++;
        }

        return true;
    }
}
#endif
