// SPDX-License-Identifier: MPL-2.0
#if NETFRAMEWORK && !NET40_OR_GREATER
namespace System.Linq.Expressions;

/// <summary>Represents a visitor or rewriter for expression trees.</summary>
/// <remarks><para>
/// This class is designed to be inherited to create more specialized classes whose
/// functionality requires traversing, examining or copying an expression tree.
/// </para></remarks>
// ReSharper disable VirtualMemberNeverOverridden.Global
abstract partial class ExpressionVisitor
{
    /// <summary>Initializes a new instance of the <see cref="ExpressionVisitor"/> class.</summary>
    protected ExpressionVisitor() { }

    /// <summary>Visits all nodes in the collection using a specified element visitor.</summary>
    /// <typeparam name="T">The type of the nodes.</typeparam>
    /// <param name="nodes">The nodes to visit.</param>
    /// <param name="elementVisitor">A delegate that visits a single element,
    /// optionally replacing it with a new element.</param>
    /// <returns>The modified node list, if any of the elements were modified;
    /// otherwise, returns the original node list.</returns>
    public static ReadOnlyCollection<T> Visit<T>(ReadOnlyCollection<T> nodes, Func<T, T> elementVisitor)
    {
        T[]? newNodes = null;

        for (int i = 0, n = nodes.Count; i < n; i++)
        {
            var node = elementVisitor(nodes[i]);

            if (newNodes != null)
                newNodes[i] = node;
            else if (!ReferenceEquals(node, nodes[i]))
            {
                newNodes = new T[n];

                for (var j = 0; j < i; j++)
                    newNodes[j] = nodes[j];

                newNodes[i] = node;
            }
        }

        return newNodes is null ? nodes : new(newNodes);
    }

    /// <summary>Dispatches the expression to one of the more specialized visit methods in this class.</summary>
    /// <param name="node">The expression to visit.</param>
    /// <returns>The modified expression, if it or any subexpression was modified;
    /// otherwise, returns the original expression.</returns>
    [return: NotNullIfNotNull(nameof(node))]
    public virtual Expression? Visit(Expression? node) => node;

    /// <summary>Visits an expression, casting the result back to the original expression type.</summary>
    /// <typeparam name="T">The type of the expression.</typeparam>
    /// <param name="node">The expression to visit.</param>
    /// <param name="callerName">The name of the calling method; used to report a better error message.</param>
    /// <returns>The modified expression, if it or any subexpression was modified;
    /// otherwise, returns the original expression.</returns>
    /// <exception cref="InvalidOperationException">
    /// The visit method for this node returned a different type.
    /// </exception>
    [return: NotNullIfNotNull(nameof(node))]
    public T? VisitAndConvert<T>(T? node, string? callerName)
        where T : Expression =>
        node is null
            ? null
            : Visit(node) as T ?? throw new ArgumentException($"Must rewrite to same node: {typeof(T)}", callerName);

    /// <summary>Visits an expression, casting the result back to the original expression type.</summary>
    /// <typeparam name="T">The type of the expression.</typeparam>
    /// <param name="nodes">The expression to visit.</param>
    /// <param name="callerName">The name of the calling method; used to report a better error message.</param>
    /// <returns>The modified expression, if it or any subexpression was modified;
    /// otherwise, returns the original expression.</returns>
    /// <exception cref="InvalidOperationException">
    /// The visit method for this node returned a different type.
    /// </exception>
    public ReadOnlyCollection<T> VisitAndConvert<T>(ReadOnlyCollection<T> nodes, string? callerName)
        where T : Expression
    {
        T[]? newNodes = null;

        for (int i = 0, n = nodes.Count; i < n; i++)
        {
            var node = Visit(nodes[i]) as T ??
                throw new ArgumentException($"Must rewrite to same node: {typeof(T)}", callerName);

            if (newNodes is not null)
                newNodes[i] = node;
            else if (!ReferenceEquals(node, nodes[i]))
            {
                newNodes = new T[n];

                for (var j = 0; j < i; j++)
                    newNodes[j] = nodes[j];

                newNodes[i] = node;
            }
        }

        return newNodes is null ? nodes : new(newNodes);
    }

    /// <summary>Visits the children of the <see cref="CatchBlock"/>.</summary>
    /// <param name="node">The expression to visit.</param>
    /// <returns>The modified expression, if it or any subexpression was modified;
    /// otherwise, returns the original expression.</returns>
    internal virtual CatchBlock VisitCatchBlock(CatchBlock node) =>
        node.Update(VisitAndConvert(node.Variable, nameof(VisitCatchBlock)), Visit(node.Filter), Visit(node.Body));

    /// <summary>Visits the children of the <see cref="TryExpression"/>.</summary>
    /// <param name="node">The expression to visit.</param>
    /// <returns>The modified expression, if it or any subexpression was modified;
    /// otherwise, returns the original expression.</returns>
    internal virtual Expression VisitTry(TryExpression node) =>
        node.Update(Visit(node.Body), Visit(node.Handlers, VisitCatchBlock), Visit(node.Finally), Visit(node.Fault));
}
#endif
