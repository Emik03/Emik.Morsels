// SPDX-License-Identifier: MPL-2.0
#if NETFRAMEWORK && !NET40_OR_GREATER
namespace System.Linq.Expressions;

using static ExtendedExpression;

/// <summary>Exposes additional methods.</summary>
#pragma warning disable MA0048
static partial class ExtendedExpression
#pragma warning restore MA0048
{
    static readonly InvalidOperationException
        s_argumentTypesMustMatch = new("Argument types must match"),
        s_mustHaveSameType = new("Body of catch must have same type as body of try.");

    static readonly ReadOnlyCollection<CatchBlock> s_empty = new(new CatchBlock[0]);

    /// <summary>
    /// Creates a <see cref="TryExpression"/> representing a try block with a fault block and no catch statements.
    /// </summary>
    /// <param name="body">The body of the try block.</param>
    /// <param name="fault">The body of the fault block.</param>
    /// <returns>The created <see cref="TryExpression"/>.</returns>
    public static TryExpression TryFault(Expression body, Expression? fault) => MakeTry(null, body, null, fault, null);

    /// <summary>
    /// Creates a <see cref="TryExpression"/> representing a try block with a finally block and no catch statements.
    /// </summary>
    /// <param name="body">The body of the try block.</param>
    /// <param name="finally">The body of the finally block.</param>
    /// <returns>The created <see cref="TryExpression"/>.</returns>
    public static TryExpression TryFinally(Expression body, Expression? @finally) =>
        MakeTry(null, body, @finally, null, null);

    /// <summary>
    /// Creates a <see cref="TryExpression"/> representing a try block with any
    /// number of catch statements and neither a fault nor finally block.
    /// </summary>
    /// <param name="body">The body of the try block.</param>
    /// <param name="handlers">
    /// The array of zero or more <see cref="CatchBlock"/>s representing
    /// the catch statements to be associated with the try block.
    /// </param>
    /// <returns>The created <see cref="TryExpression"/>.</returns>
    public static TryExpression TryCatch(Expression body, params CatchBlock[]? handlers) =>
        MakeTry(null, body, null, null, handlers);

    /// <summary>
    /// Creates a <see cref="TryExpression"/> representing a try block
    /// with any number of catch statements and a finally block.
    /// </summary>
    /// <param name="body">The body of the try block.</param>
    /// <param name="finally">The body of the finally block.</param>
    /// <param name="handlers">
    /// The array of zero or more <see cref="CatchBlock"/>s representing
    /// the catch statements to be associated with the try block.
    /// </param>
    /// <returns>The created <see cref="TryExpression"/>.</returns>
    public static TryExpression TryCatchFinally(Expression body, Expression? @finally, params CatchBlock[]? handlers) =>
        MakeTry(null, body, @finally, null, handlers);

    /// <summary>Creates a <see cref="TryExpression"/> representing a try block with the specified elements.</summary>
    /// <param name="type">
    /// The result type of the try expression. If null, body and all handlers must have identical type.
    /// </param>
    /// <param name="body">The body of the try block.</param>
    /// <param name="finally">
    /// The body of the finally block. Pass null if the try block has no finally block associated with it.
    /// </param>
    /// <param name="fault">
    /// The body of the t block. Pass null if the try block has no fault block associated with it
    /// .</param>
    /// <param name="handlers">
    /// A collection of <see cref="CatchBlock"/>s representing the catch statements to be associated with the try block.
    /// </param>
    /// <returns>The created <see cref="TryExpression"/>.</returns>
    public static TryExpression MakeTry(
        Type? type,
        Expression body,
        Expression? @finally,
        Expression? fault,
        IEnumerable<CatchBlock>? handlers
    )
    {
        ExpressionUtils.RequiresCanRead(body, nameof(body));

        var @catch = handlers is not null ? Array.AsReadOnly(handlers.ToArray()) : s_empty;

        if (@catch.Any(x => x is null))
            throw new ArgumentException("Contains null elements.", nameof(handlers));

        ValidateTryAndCatchHaveSameType(type, body, @catch);

        if (fault != null)
        {
            if (@finally != null || @catch.Count > 0)
                throw new ArgumentException("Fault cannot have catch or finally.", nameof(fault));

            ExpressionUtils.RequiresCanRead(fault, nameof(fault));
        }
        else if (@finally != null)
            ExpressionUtils.RequiresCanRead(@finally, nameof(@finally));
        else if (@catch.Count == 0)
            throw new InvalidOperationException("Try must have catch finally or fault.");

        return new(type ?? body.Type, body, @finally, fault, @catch);
    }

    // Validate that the body of the try expression must have the same type as the body of every try block.
    // ReSharper disable InvocationIsSkipped
    static void ValidateTryAndCatchHaveSameType(Type? type, Expression tryBody, ReadOnlyCollection<CatchBlock> handlers)
    {
        Debug.Assert(tryBody != null);

        // Type unification ... all parts must be reference assignable to "type"
        if (type != null)
        {
            if (type == typeof(void))
                return;

            if (type != tryBody.Type)
                throw s_argumentTypesMustMatch;

            if (handlers.Any(cb => type != cb.Body.Type))
                throw s_argumentTypesMustMatch;
        }
        else if (tryBody.Type == typeof(void)) // The body of every try block must be null or have void type.
            foreach (var cb in handlers)
            {
                Debug.Assert(cb.Body != null);

                if (cb.Body.Type != typeof(void))
                    throw s_mustHaveSameType;
            }
        else
        {
            // Body of every catch must have the same type of body of try.
            type = tryBody.Type;

            foreach (var cb in handlers)
            {
                Debug.Assert(cb.Body != null);

                if (cb.Body.Type != type)
                    throw s_mustHaveSameType;
            }
        }
    }
}

/// <summary><para>Represents a try/catch/finally/fault block.</para>
/// <para>
/// The body is protected by the try block.
/// The handlers consist of a set of <see cref="CatchBlock"/>s that can either be catch or filters.
/// The fault runs if an exception is thrown.
/// The finally runs regardless of how control exits the body.
/// Only one of fault or finally can be supplied.
/// The return type of the try block must match the return type of any associated catch statements.
/// </para>
/// </summary>
[DebuggerTypeProxy(typeof(TryExpressionProxy))]
sealed partial class TryExpression : Expression
{
    const ExpressionType Node = (ExpressionType)61;

    /// <summary>Initializes a new instance of the <see cref="TryExpression"/> class.</summary>
    /// <param name="type">
    /// The <see cref="Type"/> to set as the type of the expression that this <see cref="Expression"/> represents.
    /// </param>
    /// <param name="body">The <see cref="Expression"/> representing the body of the try block.</param>
    /// <param name="finally">The <see cref="Expression"/> representing the finally block.</param>
    /// <param name="fault">The <see cref="Expression"/> representing the fault block.</param>
    /// <param name="handlers">The collection of <see cref="CatchBlock"/>s associated with the try block.</param>
    internal TryExpression(
        Type type,
        Expression body,
        Expression? @finally,
        Expression? fault,
        ReadOnlyCollection<CatchBlock> handlers
    )
        : base(Node, type)
    {
        Body = body;
        Handlers = handlers;
        Finally = @finally;
        Fault = fault;
    }

    /// <summary>Gets the <see cref="Expression"/> representing the body of the try block.</summary>
    public Expression Body { get; }

    /// <summary>Gets the collection of <see cref="CatchBlock"/>s associated with the try block.</summary>
    public ReadOnlyCollection<CatchBlock> Handlers { get; }

    /// <summary>Gets the <see cref="Expression"/> representing the finally block.</summary>
    public Expression? Finally { get; }

    /// <summary>Gets the <see cref="Expression"/> representing the fault block.</summary>
    public Expression? Fault { get; }

    /// <summary>Dispatches to the specific visit method for this node type.</summary>
    /// <param name="visitor">The visitor.</param>
    /// <returns>The modified expression, if it or any subexpression was modified;
    /// otherwise, returns the original expression.</returns>
    internal Expression Accept(ExpressionVisitor visitor) => visitor.VisitTry(this);

    /// <summary>
    /// Creates a new expression that is like this one, but using the supplied children.
    /// If all of the children are the same, it will return this expression.
    /// </summary>
    /// <param name="body">The <see cref="Body"/> property of the result.</param>
    /// <param name="handlers">The <see cref="Handlers"/> property of the result.</param>
    /// <param name="finally">The <see cref="Finally"/> property of the result.</param>
    /// <param name="fault">The <see cref="Fault"/> property of the result.</param>
    /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
    public TryExpression Update(
        Expression body,
        IEnumerable<CatchBlock>? handlers,
        Expression? @finally,
        Expression? fault
    )
    {
        if (!(body == Body & @finally == Finally & fault == Fault))
            return MakeTry(Type, body, @finally, fault, handlers);

        return ExpressionUtils.SameElements(ref handlers, Handlers)
            ? this
            : MakeTry(Type, body, @finally, fault, handlers);
    }

    sealed class TryExpressionProxy
    {
        readonly TryExpression _node;

        public TryExpressionProxy(TryExpression node) => _node = node; // ReSharper disable UnusedMember.Local

        public Expression Body => _node.Body;

        // public bool CanReduce => _node.CanReduce;

        // public string DebugView => _node.DebugView;

        public Expression? Fault => _node.Fault;

        public Expression? Finally => _node.Finally;

        public ReadOnlyCollection<CatchBlock> Handlers => _node.Handlers;

        public ExpressionType NodeType => _node.NodeType;

        public Type Type => _node.Type; // ReSharper restore UnusedMember.Local
    }
}
#endif
