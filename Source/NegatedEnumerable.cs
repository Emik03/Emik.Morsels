#region Emik.MPL

// <copyright file="NegatedEnumerable.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

#endregion

#if !NET20 && !NET30
namespace Emik.Morsels;
#pragma warning disable CS1574, CS1580
/// <summary>Extension methods that negate functions from <see cref="Enumerable"/>.</summary>
static partial class NegatedEnumerable
{
    /// <summary>Negated <see cref="Enumerable.Distinct{T}(IEnumerable{T}, IEqualityComparer{T})"/>.</summary>
    /// <remarks><para>
    /// Filters out unique elements within an <see cref="Enumerable{T}"/>.
    /// Each duplicate appears exactly once within the returned value.
    /// </para></remarks>
    /// <typeparam name="T">The type of <see cref="IEnumerable{T}"/> and <see cref="IEqualityComparer{T}"/>.</typeparam>
    /// <param name="source">The source to filter.</param>
    /// <param name="comparer">The comparer to assess distinctiveness.</param>
    /// <returns>The parameter <paramref name="source"/>, filtering out all elements that only appear once.</returns>
    [LinqTunnel, Pure]
    internal static IEnumerable<T> DistinctDuplicates<T>(
        this IEnumerable<T> source,
        IEqualityComparer<T>? comparer = null
    ) =>
        source.GroupDuplicates(comparer).Select(x => x.Key);

    /// <summary>Negated <see cref="Enumerable.Distinct{T}(IEnumerable{T}, IEqualityComparer{T})"/>.</summary>
    /// <remarks><para>
    /// Filters out unique elements within an <see cref="Enumerable{T}"/>.
    /// Each duplicate appears two or more times within the returned value.
    /// </para></remarks>
    /// <typeparam name="T">The type of <see cref="IEnumerable{T}"/> and <see cref="IEqualityComparer{T}"/>.</typeparam>
    /// <param name="source">The source to filter.</param>
    /// <param name="comparer">The comparer to assess distinctiveness.</param>
    /// <returns>The parameter <paramref name="source"/>, filtering out all elements that only appear once.</returns>
    [LinqTunnel, Pure]
    internal static IEnumerable<T> Duplicates<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer = null) =>
        source.GroupDuplicates(comparer).SelectMany(x => x);

    /// <summary>Negated <see cref="Enumerable.Distinct{T}(IEnumerable{T}, IEqualityComparer{T})"/>.</summary>
    /// <remarks><para>Filters out unique elements within an <see cref="Enumerable{T}"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="IEnumerable{T}"/> and <see cref="IEqualityComparer{T}"/>.</typeparam>
    /// <param name="source">The source to filter.</param>
    /// <param name="comparer">The comparer to assess distinctiveness.</param>
    /// <returns>The parameter <paramref name="source"/>, filtering out all elements that only appear once.</returns>
    internal static IEnumerable<IGrouping<T, T>> GroupDuplicates<T>(
        this IEnumerable<T> source,
        IEqualityComparer<T>? comparer = null
    ) =>
        source.GroupBy(x => x, comparer).Where(x => x.Skip(1).Any());

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
    internal static IEnumerable<T> Omit<T>([NoEnumeration] this IEnumerable<T> source, Func<T, bool> predicate) =>
        source.Where(Not1(predicate));

    /// <summary>Negated <see cref="Enumerable.Where{T}(IEnumerable{T}, Func{T, int, bool})"/>.</summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}" /> that contains elements from
    /// the input sequence that do not satisfy the condition.
    /// </returns>
    /// <inheritdoc cref="Enumerable.Where{T}(IEnumerable{T}, Func{T, int, bool})"/>
    [LinqTunnel, Pure]
    internal static IEnumerable<T> Omit<T>(
        [NoEnumeration] this IEnumerable<T> source,
        Func<T, int, bool> predicate
    ) =>
        source.Where(Not2(predicate));
}
#endif
