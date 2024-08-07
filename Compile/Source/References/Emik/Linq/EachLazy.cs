// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods for iterating over a set of elements, or for generating new ones.</summary>
static partial class EachLazy
{
    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    /// <param name="action">The action to do on each item in <paramref name="iterable"/>.</param>
    /// <returns>The parameter <paramref name="iterable"/>.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<T> Lazily<T>([NoEnumeration] this IEnumerable<T> iterable, Action<T> action) =>
        new Enumerable<T, object?>(iterable, null, action);

    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <typeparam name="TExternal">The type of external parameter to pass into the callback.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    /// <param name="external">Any external parameter to be passed repeatedly into the callback.</param>
    /// <param name="action">The action to do on each item in <paramref name="iterable"/>.</param>
    /// <returns>The parameter <paramref name="iterable"/>.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<T> Lazily<T, TExternal>(
        [NoEnumeration] this IEnumerable<T> iterable,
        TExternal external,
        Action<T, TExternal> action
    ) =>
        new Enumerable<T, TExternal>(iterable, external, action);

    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    /// <param name="action">The action to do on each item in <paramref name="iterable"/>.</param>
    /// <returns>The parameter <paramref name="iterable"/>.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<T> Lazily<T>([NoEnumeration] this IEnumerable<T> iterable, Action<T, int> action) =>
        new Enumerable<T, object?>(iterable, null, action);

    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <typeparam name="TExternal">The type of external parameter to pass into the callback.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    /// <param name="external">Any external parameter to be passed repeatedly into the callback.</param>
    /// <param name="action">The action to do on each item in <paramref name="iterable"/>.</param>
    /// <returns>The parameter <paramref name="iterable"/>.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<T> Lazily<T, TExternal>(
        [NoEnumeration] this IEnumerable<T> iterable,
        TExternal external,
        Action<T, int, TExternal> action
    ) =>
        new Enumerable<T, TExternal>(iterable, external, action);
}

/// <summary>
/// Defines an <see cref="IEnumerable{T}"/> with a <see cref="Delegate"/> that is invoked on iteration.
/// </summary>
/// <typeparam name="T">The type of item in the <see cref="IEnumerable{T}"/>.</typeparam>
/// <typeparam name="TExternal">The context element to pass into the <see cref="Delegate"/>.</typeparam>
#pragma warning disable MA0048
sealed partial class Enumerable<T, TExternal> : IEnumerable<T>
#pragma warning restore MA0048
{
    readonly Delegate _action;

    readonly IEnumerable<T> _enumerable;

    readonly TExternal _external;

    /// <inheritdoc />
    public Enumerable([ProvidesContext] IEnumerable<T> enumerable, TExternal external, Action<T> action)
        : this(enumerable, external, (Delegate)action) { }

    /// <inheritdoc />
    public Enumerable([ProvidesContext] IEnumerable<T> enumerable, TExternal external, Action<T, int> action)
        : this(enumerable, external, (Delegate)action) { }

    /// <inheritdoc />
    public Enumerable([ProvidesContext] IEnumerable<T> enumerable, TExternal external, Action<T, TExternal> action)
        : this(enumerable, external, (Delegate)action) { }

    /// <inheritdoc />
    public Enumerable([ProvidesContext] IEnumerable<T> enumerable, TExternal external, Action<T, int, TExternal> action)
        : this(enumerable, external, (Delegate)action) { }

    /// <summary>Initializes a new instance of the <see cref="Enumerable{T, TExternal}"/> class.</summary>
    /// <param name="enumerable">
    /// The <see cref="IEnumerable{T}"/> to create an <see cref="IEnumerator{T}"/> from.
    /// </param>
    /// <param name="external">The context element.</param>
    /// <param name="action">The <see cref="Delegate"/> to invoke on iteration.</param>
    Enumerable([ProvidesContext] IEnumerable<T> enumerable, TExternal external, Delegate action)
    {
        _enumerable = enumerable;
        _external = external;
        _action = action;
    }

    /// <inheritdoc />
    [CollectionAccess(CollectionAccessType.Read), Pure]
#pragma warning disable IDISP004
    public IEnumerator<T> GetEnumerator() => new Enumerator(_enumerable.GetEnumerator(), _external, _action);
#pragma warning restore IDISP004
    /// <inheritdoc />
    [CollectionAccess(CollectionAccessType.Read), Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    sealed class Enumerator(
        [ProvidesContext] IEnumerator<T> enumerator,
        TExternal external,
        Delegate action
    ) : IEnumerator<T>
    {
        int _index;

        /// <inheritdoc />
        // ReSharper disable once AssignNullToNotNullAttribute
        public T Current => enumerator.Current;

        /// <inheritdoc />
        object? IEnumerator.Current => ((IEnumerator)enumerator).Current;

        /// <inheritdoc />
        public void Reset()
        {
            enumerator.Reset();
            _index = 0;
        }

        /// <inheritdoc />
        public void Dispose() => enumerator.Dispose();

        /// <inheritdoc />
        public bool MoveNext()
        {
            if (!enumerator.MoveNext())
                return false;

            var current = Current;

            switch (action)
            {
                case Action<T> act:
                    act(current);
                    break;
                case Action<T, int> act:
                    act(current, _index);
                    break;
                case Action<T, TExternal> act:
                    act(current, external);
                    break;
                case Action<T, int, TExternal> act:
                    act(current, _index, external);
                    break;
            }

            _index++;
            return true;
        }
    }
}
