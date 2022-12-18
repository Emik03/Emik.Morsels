// <copyright file="Indexers.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>
namespace Emik.Morsels;

/// <summary>Extension methods for iterating over a set of elements, or for generating new ones.</summary>
static partial class Indexers
{
    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para>https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement.</para></remarks>
    /// <param name="index">The range of numbers to iterate over in the <see langword="for"/> loop.</param>
    /// <returns>An enumeration from a range's start to end.</returns>
    [Inline, LinqTunnel, Pure]
    internal static IEnumerable<int> For(this Index index) => (index.IsFromEnd ? ~index.Value : index.Value).For();

    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para>https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement.</para></remarks>
    /// <param name="range">The range of numbers to iterate over in the <see langword="for"/> loop.</param>
    /// <returns>An enumeration from a range's start to end.</returns>
    [Inline, LinqTunnel, Pure]
    internal static IEnumerable<int> For(this Range range) =>
        range.Start.Value is var start &&
        range.End.Value is var end &&
        start == end ? Enumerable.Empty<int>() :
        Math.Abs(start - end) is var len &&
        start < end ? Enumerable.Range(start, len) : Enumerable.Repeat(start, len).Select((x, i) => x - i - 1);

    /// <summary>Gets a specific item from a collection.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="iterable"/>, or <see langword="default"/>.</returns>
    [MustUseReturnValue] // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    internal static T? Nth<T>([InstantHandle] this IEnumerable<T> iterable, Index index) =>
        index.IsFromEnd ? iterable.NthLast(index.Value) : iterable.Nth(index.Value);

    /// <summary>Gets a range of items from a collection.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to get a range of items from.</param>
    /// <param name="range">The ranges to get.</param>
    /// <returns>A slice from the parameter <paramref name="iterable"/>.</returns>
    [LinqTunnel, Pure]
    internal static IEnumerable<T> Nth<T>([InstantHandle] this IEnumerable<T> iterable, Range range)
    {
        [LinqTunnel, Pure]
        static IEnumerable<TT> Sub<TT>([InstantHandle] IEnumerable<TT> iterable, Range range) =>
            iterable.Skip(range.Start.Value).Take(range.End.Value - range.Start.Value);

        if (!range.Start.IsFromEnd && !range.End.IsFromEnd)
            return Sub(iterable, range);

        if (iterable.TryCount() is { } count && RangeStart(range, count) is var startRange)
            return Sub(iterable, startRange);

        var arr = iterable.ToList();
        var arrRange = RangeStart(range, arr.Count);
        return Sub(arr, arrRange);
    }

    /// <summary>Gets an enumeration of an index.</summary>
    /// <param name="index">The index to count up or down to.</param>
    /// <returns>An enumeration from 0 to the index's value, or vice versa.</returns>
    [Inline, Pure]
    internal static IEnumerator<int> GetEnumerator(this Index index) => index.For().GetEnumerator();

    /// <summary>Gets an enumeration of a range.</summary>
    /// <param name="range">The range to iterate over.</param>
    /// <returns>An enumeration from the range's start to end.</returns>
    [Inline, Pure]
    internal static IEnumerator<int> GetEnumerator(this Range range) => range.For().GetEnumerator();

    [Pure]
    static Index IndexStart(Index index, int length) => index.IsFromEnd ? length - index.Value - 1 : index;

    [Pure]
    static Range RangeStart(Range range, int length) =>
        new(IndexStart(range.Start, length), IndexStart(range.End, length));
}
