#region Emik.MPL

// <copyright file="TryTake.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

#endregion

namespace Emik.Morsels;

/// <summary>Extension methods to attempt to grab values from enumerables.</summary>
static partial class TryTake
{
    /// <summary>Attempts to determine the number of elements in a sequence without forcing an enumeration.</summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <param name="source">A sequence that contains elements to be counted.</param>
    /// <remarks><para>
    /// The method performs a series of type tests, identifying common subtypes whose
    /// count can be determined without enumerating; this includes <see cref="ICollection{T}"/>,
    /// <see cref="ICollection"/>, and <see cref="IReadOnlyCollection{T}"/>.
    /// </para><para>
    /// The method is typically a constant-time operation, but ultimately this depends on the complexity
    /// characteristics of the underlying collection implementation.
    /// </para></remarks>
    /// <returns>The length of the collection if pre-computed, or <see langword="null"/>.</returns>
    [Pure]
    internal static int? TryCount<TSource>([NoEnumeration] this IEnumerable<TSource> source) =>
        source switch
        {
            IReadOnlyCollection<TSource> col => col.Count,
            ICollection<TSource> col => col.Count,
            ICollection col => col.Count,
            _ => null,
        };

    /// <summary>Takes the last item lazily, or a fallback value.</summary>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    /// <param name="fallback">The fallback item.</param>
    /// <returns>The last item, or the parameter <paramref name="fallback"/>.</returns>
    [Pure]
    internal static T EnumerateOr<T>([InstantHandle] this IEnumerable<T> iterable, T fallback)
    {
        using var iterator = iterable.GetEnumerator();

        if (!iterator.MoveNext())
            return fallback;

        var last = iterator.Current;

        while (iterator.MoveNext())
            last = iterator.Current;

        return last;
    }

    /// <summary>Takes the first item, or a fallback value.</summary>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    /// <param name="fallback">The fallback item.</param>
    /// <returns>The first item, or the parameter <paramref name="fallback"/>.</returns>
    [MustUseReturnValue]
    internal static T FirstOr<T>([InstantHandle] this IEnumerable<T> iterable, T fallback)
    {
        if (iterable is IList<T> list)
            return list.Count is 0 ? fallback : list[0];

        using var iterator = iterable.GetEnumerator();
        return iterator.MoveNext() ? iterator.Current : fallback;
    }

    /// <summary>Takes the last item, or a fallback value.</summary>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    /// <param name="fallback">The fallback item.</param>
    /// <returns>The last item, or the parameter <paramref name="fallback"/>.</returns>
    [MustUseReturnValue]
    internal static T LastOr<T>([InstantHandle] this IEnumerable<T> iterable, T fallback) =>
        iterable is IList<T> list // ReSharper disable once UseIndexFromEndExpression
            ? list.Count is 0 ? fallback : list[list.Count - 1]
            : iterable.EnumerateOr(fallback);

    /// <summary>Gets a specific item from a collection.</summary>
    /// <typeparam name="TKey">The key item in the collection.</typeparam>
    /// <typeparam name="TValue">The value item in the collection.</typeparam>
    /// <param name="dictionary">The <see cref="IEnumerable{T}"/> to get an item from.</param>
    /// <param name="key">The key to use to get the value.</param>
    /// <returns>An element from the parameter <paramref name="dictionary"/>, or <see langword="default"/>.</returns>
    [MustUseReturnValue]
    internal static TValue? Nth<TKey, TValue>([InstantHandle] this IDictionary<TKey, TValue> dictionary, TKey key)
        where TKey : notnull =>
        dictionary.TryGetValue(key, out var value) ? value : default;

#if !NET20 && !NET30
    /// <summary>Returns the enumeration, or an empty enumeration.</summary>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <param name="iterable">The enumeration to potentially return.</param>
    /// <returns>The parameter <paramref name="iterable"/>, or <see cref="Enumerable.Empty{T}"/>.</returns>
    [LinqTunnel, Pure]
    internal static IEnumerable<T> OrEmpty<T>([NoEnumeration] this IEnumerable<T>? iterable) =>
        iterable ?? Enumerable.Empty<T>();

    /// <summary>Gets a specific item from a collection.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="iterable"/>, or <see langword="default"/>.</returns>
    [MustUseReturnValue] // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    internal static T? Nth<T>([InstantHandle] this IEnumerable<T> iterable, [NonNegativeValue] int index)
    {
        // Runtime check.
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (index < 0)
            return default;

        return iterable switch
        {
            IList<T> list => index < list.Count ? list[index] : default,
            IReadOnlyList<T> re => index < re.Count ? re[index] : default,
            _ => iterable.Skip(index).FirstOrDefault(),
        };
    }

    /// <summary>Gets a specific item from a collection.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="iterable"/>, or <see langword="default"/>.</returns>
    [MustUseReturnValue] // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    internal static T? NthLast<T>([InstantHandle] this IEnumerable<T> iterable, [NonNegativeValue] int index)
    {
        // Runtime check.
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (index < 0)
            return default;

        return iterable switch
        {
            IList<T> list => index < list.Count ? list[list.Count - index - 1] : default,
            IReadOnlyList<T> list => index < list.Count ? list[list.Count - index - 1] : default,
            _ when iterable.ToList() is var list => list[list.Count - index - 1],
            _ => throw Unreachable,
        };
    }
#endif
}
