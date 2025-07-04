// SPDX-License-Identifier: MPL-2.0
#if !NET20 && !NET30
// ReSharper disable CheckNamespace RedundantNameQualifier
namespace Emik.Morsels;

using Range = System.Range;

/// <summary>Extension methods for iterating over a set of elements, or for generating new ones.</summary>
static partial class Indexers
{
    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <param name="index">The range of numbers to iterate over in the <see langword="for"/> loop.</param>
    /// <returns>An enumeration from a range's start to end.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<int> For(this Index index) => (index.IsFromEnd ? -index.Value : index.Value).For();

    /// <summary>
    /// The <see langword="for"/> statement executes a statement or a block of statements while a specified
    /// Boolean expression evaluates to <see langword="true"/>.
    /// </summary>
    /// <remarks><para><a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements#the-for-statement">
    /// See here for more information.
    /// </a></para></remarks>
    /// <param name="range">The range of numbers to iterate over in the <see langword="for"/> loop.</param>
    /// <returns>An enumeration from a range's start to end.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<int> For(this Range range) =>
        (range.Start.IsFromEnd ? -range.Start.Value : range.Start.Value) is var start &&
        (range.End.IsFromEnd ? -range.End.Value : range.End.Value) is var end &&
        start == end ? [] :
        start < end ? Enumerable.Range(start - (!range.Start.IsFromEnd && range.End.IsFromEnd ? 1 : 0), end - start) :
        Enumerable.Repeat(start, start - end).Select((x, i) => x - i - 1);

    /// <summary>Separates the head from the tail of an <see cref="IEnumerable{T}"/>.</summary>
    /// <remarks><para>
    /// The tail is not guaranteed to be able to be enumerated over multiple times.
    /// As such, use a method like <see cref="Collected.ToICollection{T}"/> if multiple enumerations are needed.
    /// </para></remarks>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="enumerable">The enumerable to split.</param>
    /// <param name="head">The first element of the parameter <paramref name="enumerable"/>.</param>
    /// <param name="tail">The rest of the parameter <paramref name="enumerable"/>.</param>
    public static void Deconstruct<T>(
        this IEnumerable<T>? enumerable,
        out T? head,
        [MustDisposeResource] out IEnumerable<T> tail
    )
    {
        using var e = enumerable?.GetEnumerator();

        if (e is null)
        {
            head = default;
            tail = [];
            return;
        }

        head = e.MoveNext() ? e.Current : default;
        tail = e.AsEnumerable();
    }

    /// <summary>Gets a specific item from a collection.</summary>
    /// <param name="str">The <see cref="IEnumerable{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="str"/>, or <see langword="default"/>.</returns>
    [Pure] // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public static char? Nth(this string str, Index index) =>
        index.IsFromEnd ? str.NthLast(index.Value - 1) : str.Nth(index.Value);

    /// <summary>Gets a specific item from a collection.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="iterable"/>, or <see langword="default"/>.</returns>
    [MustUseReturnValue] // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public static T? Nth<T>([InstantHandle] this IEnumerable<T> iterable, Index index) =>
        index.IsFromEnd ? iterable.NthLast(index.Value - 1) : iterable.Nth(index.Value);

    /// <summary>Gets a specific item from a collection.</summary>
    /// <param name="str">The <see cref="IEnumerable{T}"/> to get an item from.</param>
    /// <param name="range">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="str"/>, or <see langword="default"/>.</returns>
    [Pure] // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public static string? Nth(this string str, Range range) =>
        range.TryGetOffsetAndLength(str.Length, out var offset, out var length) ? str.Substring(offset, length) : null;

    /// <summary>Gets a range of items from a collection.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to get a range of items from.</param>
    /// <param name="range">The ranges to get.</param>
    /// <returns>A slice from the parameter <paramref name="iterable"/>.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<T> Nth<T>([InstantHandle] this IEnumerable<T> iterable, Range range)
    {
        [LinqTunnel, Pure]
        static IEnumerable<TT> Sub<TT>([InstantHandle] IEnumerable<TT> iterable, Range range) =>
            iterable.Skip(range.Start.Value).Take(range.End.Value - range.Start.Value);

        if (!range.Start.IsFromEnd && !range.End.IsFromEnd)
            return Sub(iterable, range);

        if (iterable.TryGetNonEnumeratedCount(out var count) && RangeStart(range, count) is var startRange)
            return Sub(iterable, startRange);

        var arr = iterable.ToIList();
        var arrRange = RangeStart(range, arr.Count);
        return Sub(arr, arrRange);
    }

    /// <summary>Gets an enumeration of an index.</summary>
    /// <param name="index">The index to count up or down to.</param>
    /// <returns>An enumeration from 0 to the index's value, or vice versa.</returns>
    [MustDisposeResource, Pure]
    public static IEnumerator<int> GetEnumerator(this Index index) => index.For().GetEnumerator();

    /// <summary>Gets an enumeration of a range.</summary>
    /// <param name="range">The range to iterate over.</param>
    /// <returns>An enumeration from the range's start to end.</returns>
    [MustDisposeResource, Pure]
    public static IEnumerator<int> GetEnumerator(this Range range) => range.For().GetEnumerator();

    [Pure]
    static Index IndexStart(Index index, int length) => index.IsFromEnd ? length - index.Value - 1 : index;

    [Pure]
    static Range RangeStart(Range range, int length) =>
        new(IndexStart(range.Start, length), IndexStart(range.End, length));
}
#endif
