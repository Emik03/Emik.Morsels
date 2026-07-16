// SPDX-License-Identifier: MPL-2.0
// ReSharper disable CheckNamespace RedundantNameQualifier
namespace Emik.Morsels;

using Range = System.Range;

/// <summary>Extension methods for iterating over a set of elements, or for generating new ones.</summary>
static partial class Indexers
{
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
    /// <param name="str">The <see cref="IEnumerable{T}"/> to get an item from.</param>
    /// <param name="range">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="str"/>, or <see langword="default"/>.</returns>
    [Pure] // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public static string? Nth(this string str, Range range) =>
        range.TryGetOffsetAndLength(str.Length, out var offset, out var length) ? str.Substring(offset, length) : null;

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

    /// <summary>Gets an enumeration of an index.</summary>
    /// <param name="index">The index to count up or down to.</param>
    /// <returns>An enumeration from 0 to the index's value, or vice versa.</returns>
    [MustDisposeResource, Pure]
    public static IEnumerator<int> GetEnumerator(this Index index) => index.For().GetEnumerator();

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

        if (iterable.TryCount() is { } count)
            return Sub(iterable, RangeStart(range, count));

        var list = iterable.ToIList();
        return Sub(list, RangeStart(range, list.Count));
    }

    /// <summary>Gets a specific item from a collection.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="iterable"/>, or <see langword="default"/>.</returns>
    [MustUseReturnValue] // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public static T? Nth<T>([InstantHandle] this IEnumerable<T> iterable, Index index) =>
        index.IsFromEnd ? iterable.NthLast(index.Value - 1) : iterable.Nth(index.Value);
#if !NO_SYSTEM_MEMORY
    /// <summary>Gets the specific slice from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="owner">The <see cref="IMemoryOwner{T}"/> to get an item from.</param>
    /// <param name="range">The index to get.</param>
    /// <returns>A slice from the parameter <paramref name="owner"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlyMemory<T> Nth<T>(this IMemoryOwner<T> owner, Range range) => owner.Memory.Nth(range);

    /// <summary>Gets the specific slice from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="span">The <see cref="ReadOnlyMemory{T}"/> to get an item from.</param>
    /// <param name="range">The index to get.</param>
    /// <returns>A slice from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlyMemory<T> Nth<T>(this ReadOnlyMemory<T> span, Range range) =>
        range.TryGetOffsetAndLength(span.Length, out var off, out var len) ? span.Slice(off, len) : default;

    /// <summary>Gets the specific slice from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="span">The <see cref="Memory{T}"/> to get an item from.</param>
    /// <param name="range">The index to get.</param>
    /// <returns>A slice from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Memory<T> Nth<T>(this Memory<T> span, Range range) =>
        range.TryGetOffsetAndLength(span.Length, out var off, out var len) ? span.Slice(off, len) : default;

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="owner">The <see cref="IMemoryOwner{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="owner"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this IMemoryOwner<T> owner, [NonNegativeValue] int index) => owner.Memory.Nth(index);

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="memory">The <see cref="ReadOnlyMemory{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="memory"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this ReadOnlyMemory<T> memory, [NonNegativeValue] int index) =>
        (uint)index < (uint)memory.Length ? memory.Span[index] : default;

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="owner">The <see cref="IMemoryOwner{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="owner"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this IMemoryOwner<T> owner, Index index) => owner.Memory.Nth(index);

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="memory">The <see cref="ReadOnlyMemory{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="memory"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this ReadOnlyMemory<T> memory, Index index) =>
        index.GetOffset(memory.Length) is var o && (uint)o < (uint)memory.Length
            ? memory.Span.UnsafelyIndex(o)
            : default;

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="owner">The <see cref="IMemoryOwner{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="owner"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? NthLast<T>(this IMemoryOwner<T> owner, int index) => owner.Memory.NthLast(index);

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="memory">The <see cref="ReadOnlyMemory{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="memory"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? NthLast<T>(this ReadOnlyMemory<T> memory, [NonNegativeValue] int index) =>
        (uint)(index - 1) < (uint)memory.Length ? memory.Span[memory.Length - index] : default;

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="memory">The <see cref="Memory{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="memory"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this Memory<T> memory, [NonNegativeValue] int index) =>
        (uint)index < (uint)memory.Length ? memory.Span.UnsafelyIndex(index) : default;

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="memory">The <see cref="Memory{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="memory"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this Memory<T> memory, Index index) =>
        index.GetOffset(memory.Length) is var off && (uint)off < (uint)memory.Length
            ? memory.Span.UnsafelyIndex(off)
            : default;

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="memory">The <see cref="Memory{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="memory"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? NthLast<T>(this Memory<T> memory, [NonNegativeValue] int index) =>
        (uint)(index - 1) < (uint)memory.Length ? memory.Span.UnsafelyIndex(memory.Length - index) : default;

    /// <summary>Gets the specific slice from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to get an item from.</param>
    /// <param name="range">The index to get.</param>
    /// <returns>A slice from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<T> Nth<T>(this ReadOnlySpan<T> span, Range range) =>
        range.TryGetOffsetAndLength(span.Length, out var off, out var len) ? span.UnsafelySlice(off, len) : default;

    /// <summary>Gets the specific slice from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to get an item from.</param>
    /// <param name="range">The index to get.</param>
    /// <returns>A slice from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Span<T> Nth<T>(this Span<T> span, Range range) =>
        range.TryGetOffsetAndLength(span.Length, out var off, out var len) ? span.UnsafelySlice(off, len) : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this scoped ReadOnlySpan<T> span, [NonNegativeValue] int index) =>
        (uint)index < (uint)span.Length ? span.UnsafelyIndex(index) : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this scoped ReadOnlySpan<T> span, Index index) =>
        index.GetOffset(span.Length) is var o && (uint)o < (uint)span.Length ? span.UnsafelyIndex(o) : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? NthLast<T>(this scoped ReadOnlySpan<T> span, [NonNegativeValue] int index) =>
        (uint)(index - 1) < (uint)span.Length ? span.UnsafelyIndex(span.Length - index) : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this scoped Span<T> span, [NonNegativeValue] int index) =>
        (uint)index < (uint)span.Length ? span.UnsafelyIndex(index) : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this scoped Span<T> span, Index index) =>
        index.GetOffset(span.Length) is var o && (uint)o < (uint)span.Length ? span.UnsafelyIndex(o) : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? NthLast<T>(this scoped Span<T> span, [NonNegativeValue] int index) =>
        (uint)(index - 1) < (uint)span.Length ? span.UnsafelyIndex(span.Length - index) : default;
#endif
    [Pure]
    static Index IndexStart(Index index, int length) => index.IsFromEnd ? length - index.Value - 1 : index;

    [Pure]
    static Range RangeStart(Range range, int length) =>
        new(IndexStart(range.Start, length), IndexStart(range.End, length));
}
