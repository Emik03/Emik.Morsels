// <copyright file="NegatedEnumerable.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>
namespace Emik.Morsels;

/// <summary>Extension methods that negate functions from <see cref="Enumerable"/>.</summary>
static class NegatedEnumerable
{
    /// <summary>Negated <see cref="Enumerable.TakeWhile{T}(IEnumerable{T}, Func{T, int, bool})"/>.</summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}" /> that contains elements from
    /// the input sequence that do not satisfy the condition.
    /// </returns>
    /// <inheritdoc cref="Enumerable.TakeWhile{T}(IEnumerable{T}, Func{T, int, bool})"/>
    [LinqTunnel, Pure]
    internal static IEnumerable<T> TakeUntil<T>([NoEnumeration] this IEnumerable<T> source, Func<T, bool> predicate) =>
        source.TakeWhile(Not1(predicate));

    /// <summary>Negated <see cref="Enumerable.TakeWhile{T}(IEnumerable{T}, Func{T, int, bool})"/>.</summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}" /> that contains elements from
    /// the input sequence that do not satisfy the condition.
    /// </returns>
    /// <inheritdoc cref="Enumerable.TakeWhile{T}(IEnumerable{T}, Func{T, int, bool})"/>
    [LinqTunnel, Pure]
    internal static IEnumerable<T> TakeUntil<T>(
        [NoEnumeration] this IEnumerable<T> source,
        Func<T, int, bool> predicate
    ) =>
        source.TakeWhile(Not2(predicate));

    /// <summary>
    /// Negated <see cref="IEnumerable.GetEnumerator"/>.
    /// Creates an <see cref="IEnumerable{T}"/> encapsulating an <see cref="IEnumerator{T}"/>.
    /// </summary>
    /// <param name="iterator">The <see cref="IEnumerator{T}"/> to encapsulate.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> encapsulating the parameter <paramref name="iterator"/>.</returns>
    [Pure]
    internal static IEnumerable<object?> ToEnumerable([InstantHandle] this IEnumerator? iterator)
    {
        if (iterator is null)
            yield break;

        while (iterator.MoveNext())
            yield return iterator.Current;
    }

    /// <summary>
    /// Negated <see cref="IEnumerable{T}.GetEnumerator"/>.
    /// Creates an <see cref="IEnumerable{T}"/> encapsulating an <see cref="IEnumerator{T}"/>.
    /// </summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterator">The <see cref="IEnumerator{T}"/> to encapsulate.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> encapsulating the parameter <paramref name="iterator"/>.</returns>
    [Pure]
    internal static IEnumerable<T> ToEnumerable<T>([InstantHandle] this IEnumerator<T>? iterator)
    {
        if (iterator is null)
            yield break;

        while (iterator.MoveNext())
            yield return iterator.Current;

        yield return iterator.Current;
    }

    /// <summary>Negated <see cref="Enumerable.Where{T}(IEnumerable{T}, Func{T, bool})"/>.</summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}" /> that contains elements from
    /// the input sequence that do not satisfy the condition.
    /// </returns>
    /// <inheritdoc cref="Enumerable.Where{T}(IEnumerable{T}, Func{T, bool})"/>
    [LinqTunnel, Pure]
    internal static IEnumerable<T> WhereNot<T>([NoEnumeration] this IEnumerable<T> source, Func<T, bool> predicate) =>
        source.Where(Not1(predicate));

    /// <summary>Negated <see cref="Enumerable.Where{T}(IEnumerable{T}, Func{T, int, bool})"/>.</summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}" /> that contains elements from
    /// the input sequence that do not satisfy the condition.
    /// </returns>
    /// <inheritdoc cref="Enumerable.Where{T}(IEnumerable{T}, Func{T, int, bool})"/>
    [LinqTunnel, Pure]
    internal static IEnumerable<T> WhereNot<T>(
        [NoEnumeration] this IEnumerable<T> source,
        Func<T, int, bool> predicate
    ) =>
        source.Where(Not2(predicate));
}
