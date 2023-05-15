// SPDX-License-Identifier: MPL-2.0
#if NETFRAMEWORK && !NET40_OR_GREATER
namespace System.Linq.Expressions;

using static ExtendedExpression;

/// <summary>Exposes additional methods.</summary>
#pragma warning disable MA0048
static partial class ExtendedExpression
#pragma warning restore MA0048
{
    /// <summary>
    /// Creates a <see cref="CatchBlock"/> representing a catch statement.
    /// The <see cref="Type"/> of object to be caught can be specified but no reference to the object
    /// will be available for use in the <see cref="CatchBlock"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of <see cref="Exception"/> this <see cref="CatchBlock"/> will handle.</param>
    /// <param name="body">The body of the catch statement.</param>
    /// <returns>The created <see cref="CatchBlock"/>.</returns>
    public static CatchBlock Catch(Type type, Expression body) => MakeCatchBlock(type, null, body, null);

    /// <summary>
    /// Creates a <see cref="CatchBlock"/> representing a catch statement with
    /// a reference to the caught object for use in the handler body.
    /// </summary>
    /// <param name="variable">
    /// A <see cref="ParameterExpression"/> representing a reference to the
    /// <see cref="Exception"/> object caught by this handler.
    /// </param>
    /// <param name="body">The body of the catch statement.</param>
    /// <returns>The created <see cref="CatchBlock"/>.</returns>
    public static CatchBlock Catch(ParameterExpression variable, Expression body) =>
        MakeCatchBlock(variable.Type, variable, body, null);

    /// <summary>
    /// Creates a <see cref="CatchBlock"/> representing a catch statement with
    /// an <see cref="Exception"/> filter but no reference to the caught <see cref="Exception"/> object.
    /// </summary>
    /// <param name="type">
    /// The <see cref="Type"/> of <see cref="Exception"/> this <see cref="CatchBlock"/> will handle.
    /// </param>
    /// <param name="body">The body of the catch statement.</param>
    /// <param name="filter">The body of the <see cref="Exception"/> filter.</param>
    /// <returns>The created <see cref="CatchBlock"/>.</returns>
    public static CatchBlock Catch(Type type, Expression body, Expression? filter) =>
        MakeCatchBlock(type, null, body, filter);

    /// <summary>
    /// Creates a <see cref="CatchBlock"/> representing a catch statement with
    /// an <see cref="Exception"/> filter and a reference to the caught <see cref="Exception"/> object.
    /// </summary>
    /// <param name="variable">
    /// A <see cref="ParameterExpression"/> representing a reference to the
    /// <see cref="Exception"/> object caught by this handler.
    /// </param>
    /// <param name="body">The body of the catch statement.</param>
    /// <param name="filter">The body of the <see cref="Exception"/> filter.</param>
    /// <returns>The created <see cref="CatchBlock"/>.</returns>
    public static CatchBlock Catch(ParameterExpression variable, Expression body, Expression? filter) =>
        MakeCatchBlock(variable.Type, variable, body, filter);

    /// <summary>
    /// Creates a <see cref="CatchBlock"/> representing a catch statement with the specified elements.
    /// </summary>
    /// <remarks><para>
    /// <paramref name="type"/> must be non-null and match the type of <paramref name="variable"/> (if it is supplied).
    /// </para></remarks>
    /// <param name="type">
    /// The <see cref="Type"/> of <see cref="Exception"/> this <see cref="CatchBlock"/> will handle.
    /// </param>
    /// <param name="variable">
    /// A <see cref="ParameterExpression"/> representing a reference to the
    /// <see cref="Exception"/> object caught by this handler.
    /// </param>
    /// <param name="body">The body of the catch statement.</param>
    /// <param name="filter">The body of the <see cref="Exception"/> filter.</param>
    /// <returns>The created <see cref="CatchBlock"/>.</returns>
    public static CatchBlock MakeCatchBlock(
        Type type,
        ParameterExpression? variable,
        Expression body,
        Expression? filter
    )
    {
        // ContractUtils.Requires(variable == null || TypeUtils.AreEquivalent(variable.Type, type), nameof(variable));

        if (variable == null)
            ValidateType(type, nameof(type));

        ExpressionUtils.RequiresCanRead(body, nameof(body));

        if (filter is null)
            return new(type, variable, body, null);

        ExpressionUtils.RequiresCanRead(filter, nameof(filter));

        if (filter.Type != typeof(bool))
            throw new ArgumentException("Argument must be boolean", nameof(filter));

        return new(type, variable, body, filter);
    }

    static void ValidateType(Type type, string? paramName) => ValidateType(type, paramName, false, false);

    static void ValidateType(Type type, string? paramName, bool allowByRef, bool allowPointer)
    {
        if (!ValidateType(type, paramName, -1))
            return;

        if (!allowByRef && type.IsByRef)
            throw new ArgumentException("Type must not be by ref", paramName);

        if (!allowPointer && type.IsPointer)
            throw new ArgumentException("Type must not be pointer", paramName);
    }

    static bool ValidateType(Type type, string? paramName, int index)
    {
        if (type == typeof(void))
            return false; // Caller can skip further checks.

        if (type.ContainsGenericParameters)
            throw type.IsGenericTypeDefinition
                ? new ArgumentException($"Type {index} is generic.", paramName)
                : new($"Type {index} {type} contains generic parameters", paramName);

        return true;
    }
}

/// <summary>
/// Represents a catch statement in a try block.
/// This must have the same return type (i.e., the type of <see cref="Body"/>) as the try block it is associated with.
/// </summary>
[DebuggerTypeProxy(typeof(CatchBlockProxy))]
sealed partial class CatchBlock
{
    /// <summary>Initializes a new instance of the <see cref="CatchBlock"/> class.</summary>
    /// <param name="test">The reference to the <see cref="Exception"/> object caught by this handler.</param>
    /// <param name="variable">The type of <see cref="Exception"/> this handler catches.</param>
    /// <param name="body">The body of the catch block.</param>
    /// <param name="filter">The body of the <see cref="CatchBlock"/>'s filter.</param>
    internal CatchBlock(Type test, ParameterExpression? variable, Expression body, Expression? filter)
    {
        Test = test;
        Variable = variable;
        Body = body;
        Filter = filter;
    }

    /// <summary>Gets a reference to the <see cref="Exception"/> object caught by this handler.</summary>
    public ParameterExpression? Variable { get; }

    /// <summary>Gets the type of <see cref="Exception"/> this handler catches.</summary>
    public Type Test { get; }

    /// <summary>Gets the body of the catch block.</summary>
    public Expression Body { get; }

    /// <summary>Gets the body of the <see cref="CatchBlock"/>'s filter.</summary>
    public Expression? Filter { get; }

    /// <summary>Returns a <see cref="string"/> that represents the current <see cref="object"/>.</summary>
    /// <returns>A <see cref="string"/> that represents the current <see cref="object"/>.</returns>
    public override string ToString() => ExpressionStringBuilder.CatchBlockToString(this);

    /// <summary>
    /// Creates a new expression that is like this one, but using the supplied children.
    /// If all of the children are the same, it will return this expression.
    /// </summary>
    /// <param name="variable">The <see cref="Variable"/> property of the result.</param>
    /// <param name="filter">The <see cref="Filter"/> property of the result.</param>
    /// <param name="body">The <see cref="Body"/> property of the result.</param>
    /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
    public CatchBlock Update(ParameterExpression? variable, Expression? filter, Expression body) =>
        variable == Variable && filter == Filter && body == Body ? this : MakeCatchBlock(Test, variable, body, filter);

    sealed class CatchBlockProxy
    {
        readonly CatchBlock _node;

        public CatchBlockProxy(CatchBlock node) => _node = node; // ReSharper disable UnusedMember.Local

        public Expression Body => _node.Body;

        public Expression? Filter => _node.Filter;

        public Type Test => _node.Test;

        public ParameterExpression? Variable => _node.Variable; // ReSharper restore UnusedMember.Local
    }
}
#endif
