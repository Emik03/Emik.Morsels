// SPDX-License-Identifier: MPL-2.0
#if NETFRAMEWORK && !NET40_OR_GREATER
namespace System.Linq.Expressions;

/// <summary>Creates a string out of a try-catch.</summary>
sealed partial class ExpressionStringBuilder : ExpressionVisitor
{
    readonly StringBuilder _out;

    ExpressionStringBuilder() => _out = new();

    /// <inheritdoc/>
    public override string ToString() => _out.ToString();

    /// <summary>Serializes the node.</summary>
    /// <param name="node">The node to serialize.</param>
    /// <returns>The parameter <paramref name="node"/> serialized.</returns>
    internal static string CatchBlockToString(CatchBlock node)
    {
        ExpressionStringBuilder esb = new();
        esb.VisitCatchBlock(node);
        return esb.ToString();
    }

    /// <inheritdoc/>
    internal override Expression VisitTry(TryExpression node)
    {
        _out.Append("try { ... }");
        return node;
    }

    /// <inheritdoc/>
    internal override CatchBlock VisitCatchBlock(CatchBlock node)
    {
        _out.Append("catch (").Append(node.Test.Name);

        if (!string.IsNullOrEmpty(node.Variable?.Name))
            _out.Append(' ').Append(node.Variable.Name);

        _out.Append(") { ... }");
        return node;
    }
}
#endif
