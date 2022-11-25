// <copyright file="EachLazy.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>
namespace Emik.Morsels;

/// <summary>Extension methods for iterating over a set of elements, or for generating new ones.</summary>
static class EachLazy
{
    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable"/> or <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para>https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement.</para></remarks>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    /// <param name="action">The action to do on each item in <paramref name="iterable"/>.</param>
    /// <returns>The parameter <paramref name="iterable"/>.</returns>
    internal static IEnumerable<T> Lazily<T>([NoEnumeration] this IEnumerable<T> iterable, Action<T> action) =>
        new Enumerable<T, object?>(iterable, null, action);

    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable"/> or <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para>https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement.</para></remarks>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <typeparam name="TExternal">The type of external parameter to pass into the callback.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    /// <param name="external">Any external parameter to be passed repeatedly into the callback.</param>
    /// <param name="action">The action to do on each item in <paramref name="iterable"/>.</param>
    /// <returns>The parameter <paramref name="iterable"/>.</returns>
    internal static IEnumerable<T> Lazily<T, TExternal>(
        [NoEnumeration] this IEnumerable<T> iterable,
        TExternal external,
        Action<T, TExternal> action
    ) =>
        new Enumerable<T, TExternal>(iterable, external, action);

    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable"/> or <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para>https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement.</para></remarks>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    /// <param name="action">The action to do on each item in <paramref name="iterable"/>.</param>
    /// <returns>The parameter <paramref name="iterable"/>.</returns>
    internal static IEnumerable<T> Lazily<T>([NoEnumeration] this IEnumerable<T> iterable, Action<T, int> action) =>
        new Enumerable<T, object?>(iterable, null, action);

    /// <summary>
    /// The <see langword="foreach"/> statement executes a statement or a block of statements for each element in an
    /// instance of the type that implements the <see cref="IEnumerable"/> or <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    /// <remarks><para>https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement.</para></remarks>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <typeparam name="TExternal">The type of external parameter to pass into the callback.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    /// <param name="external">Any external parameter to be passed repeatedly into the callback.</param>
    /// <param name="action">The action to do on each item in <paramref name="iterable"/>.</param>
    /// <returns>The parameter <paramref name="iterable"/>.</returns>
    internal static IEnumerable<T> Lazily<T, TExternal>(
        [InstantHandle] this IEnumerable<T> iterable,
        TExternal external,
        [InstantHandle] Action<T, int, TExternal> action
    ) =>
        new Enumerable<T, TExternal>(iterable, external, action);
}

sealed class Enumerable<T, TExternal> : IEnumerable<T>
{
    readonly Delegate _action;

    readonly IEnumerable<T> _enumerable;

    readonly TExternal _external;

    internal Enumerable(IEnumerable<T> enumerable, TExternal external, Action<T> action)
        : this(enumerable, external, (Delegate)action) { }

    internal Enumerable(IEnumerable<T> enumerable, TExternal external, Action<T, int> action)
        : this(enumerable, external, (Delegate)action) { }

    internal Enumerable(IEnumerable<T> enumerable, TExternal external, Action<T, TExternal> action)
        : this(enumerable, external, (Delegate)action) { }

    internal Enumerable(IEnumerable<T> enumerable, TExternal external, Action<T, int, TExternal> action)
        : this(enumerable, external, (Delegate)action) { }

    Enumerable(IEnumerable<T> enumerable, TExternal external, Delegate action)
    {
        _enumerable = enumerable;
        _external = external;
        _action = action;
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => new Enumerator(_enumerable.GetEnumerator(), _external, _action);

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    sealed class Enumerator : IEnumerator<T>
    {
        readonly Delegate _action;

        readonly IEnumerator<T> _enumerator;

        readonly TExternal _external;

        int _index;

        internal Enumerator(IEnumerator<T> enumerator, TExternal external, Delegate action)
        {
            _enumerator = enumerator;
            _external = external;
            _action = action;
        }

        /// <inheritdoc />
        public T Current => _enumerator.Current;

        /// <inheritdoc />
        object? IEnumerator.Current => ((IEnumerator)_enumerator).Current;

        /// <inheritdoc />
        public void Reset()
        {
            _enumerator.Reset();
            _index = 0;
        }

        /// <inheritdoc />
        public void Dispose() => _enumerator.Dispose();

        /// <inheritdoc />
        public bool MoveNext()
        {
            var result = _enumerator.MoveNext();
            var current = Current;

            switch (_action)
            {
                case Action<T> action:
                    action(current);
                    break;
                case Action<T, int> action:
                    action(current, _index);
                    break;
                case Action<T, TExternal> action:
                    action(current, _external);
                    break;
                case Action<T, int, TExternal> action:
                    action(current, _index, _external);
                    break;
            }

            if (result)
                _index++;

            return result;
        }
    }
}
