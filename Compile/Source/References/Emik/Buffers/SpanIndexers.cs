// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods for iterating over a set of elements, or for generating new ones.</summary>
// ReSharper disable BadPreprocessorIndent ConditionIsAlwaysTrueOrFalse UseIndexFromEndExpression
static partial class SpanIndexers
{
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
    /// <summary>Separates the head from the tail of a <see cref="Memory{T}"/>.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="memory">The memory to split.</param>
    /// <param name="head">The first element of the parameter <paramref name="memory"/>.</param>
    /// <param name="tail">The rest of the parameter <paramref name="memory"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deconstruct<T>(this Memory<T> memory, out T? head, out Memory<T> tail)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        if (memory.IsEmpty)
        {
            head = default;
            tail = default;
            return;
        }

        head = memory.Span.UnsafelyIndex(0);
        tail = memory[1..];
    }

    /// <summary>Separates the head from the tail of a <see cref="Memory{T}"/>.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="memory">The memory to split.</param>
    /// <param name="head">The first element of the parameter <paramref name="memory"/>.</param>
    /// <param name="tail">The rest of the parameter <paramref name="memory"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deconstruct<T>(this ReadOnlyMemory<T> memory, out T? head, out ReadOnlyMemory<T> tail)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        if (memory.IsEmpty)
        {
            head = default;
            tail = default;
            return;
        }

        head = memory.Span.UnsafelyIndex(0);
        tail = memory[1..];
    }
#endif

    /// <summary>Separates the head from the tail of a <see cref="Span{T}"/>.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="span">The span to split.</param>
    /// <param name="head">The first element of the parameter <paramref name="span"/>.</param>
    /// <param name="tail">The rest of the parameter <paramref name="span"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deconstruct<T>(this Span<T> span, out T? head, out Span<T> tail)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        if (span.IsEmpty)
        {
            head = default;
            tail = default;
            return;
        }

        head = span.UnsafelyIndex(0);
        tail = span.UnsafelySkip(1);
    }

    /// <summary>Separates the head from the tail of a <see cref="ReadOnlySpan{T}"/>.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="span">The span to split.</param>
    /// <param name="head">The first element of the parameter <paramref name="span"/>.</param>
    /// <param name="tail">The rest of the parameter <paramref name="span"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deconstruct<T>(this ReadOnlySpan<T> span, out T? head, out ReadOnlySpan<T> tail)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        if (span.IsEmpty)
        {
            head = default;
            tail = default;
            return;
        }

        head = span.UnsafelyIndex(0);
        tail = span.UnsafelySkip(1);
    }
#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER || NO_SYSTEM_MEMORY
    /// <summary>Gets the index of an element of a given <see cref="Span{T}"/> from its reference.</summary>
    /// <typeparam name="T">The type if items in the input <see cref="Span{T}"/>.</typeparam>
    /// <param name="span">The input <see cref="Span{T}"/> to calculate the index for.</param>
    /// <param name="value">The reference to the target item to get the index for.</param>
    /// <returns>The index of <paramref name="value"/> within <paramref name="span"/>, or <c>-1</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int IndexOf<T>(this ReadOnlySpan<T> span, ref T value)
#pragma warning disable 8500
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        =>
            Unsafe.ByteOffset(ref MemoryMarshal.GetReference(span), ref value) is var byteOffset &&
            byteOffset / (nint)(uint)sizeof(T) is var elementOffset &&
            (nuint)elementOffset < (uint)span.Length
                ? (int)elementOffset
                : -1;
#else
    {
        fixed (T* ptr = &value)
            return (nint)((T*)span.Pointer - ptr) is var elementOffset && (nuint)elementOffset < (uint)span.Length
                ? (int)elementOffset
                : -1;
    }
#endif
#pragma warning restore 8500
    /// <inheritdoc cref="IndexOf{T}(ReadOnlySpan{T}, ref T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf<T>(this Span<T> origin, ref T target) => origin.ReadOnly().IndexOf(ref target);
#endif
#if !NET7_0_OR_GREATER
    /// <inheritdoc cref="IndexOfAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int IndexOfAny<T>(this scoped Span<T> span, scoped ReadOnlySpan<T> values)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
        =>
            ((ReadOnlySpan<T>)span).IndexOfAny(values);

    /// <summary>
    /// Searches for the first index of any of the specified values similar
    /// to calling IndexOf several times with the logical OR operator.
    /// </summary>
    /// <typeparam name="T">The type of the span and values.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="values">The set of values to search for.</param>
    /// <returns>The first index of the occurrence of any of the values in the span. If not found, returns -1.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static unsafe int IndexOfAny<T>(this scoped ReadOnlySpan<T> span, scoped ReadOnlySpan<T> values)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
    {
#pragma warning disable 8500
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
        var searchSpace = (T*)span.Pointer;
        var value = (T*)values.Pointer;
#else
        fixed (T* searchSpace = span)
        fixed (T* value = values)
#endif
#pragma warning restore 8500
            return SpanHelpers.IndexOfAny(searchSpace, span.Length, value, values.Length);
    }
#endif
    /// <inheritdoc cref="IndexOf{T}(ReadOnlySpan{T}, ref T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] // ReSharper disable once RedundantUnsafeContext
    public static unsafe int OffsetOf<T>(this scoped ReadOnlySpan<T> origin, scoped ReadOnlySpan<T> target) =>
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        origin.IndexOf(ref MemoryMarshal.GetReference(target));
#else
#pragma warning disable 8500
        origin.IndexOf(ref *(T*)target.Pointer);
#pragma warning restore 8500
#endif

    /// <inheritdoc cref="IndexOf{T}(ReadOnlySpan{T}, ref T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int OffsetOf<T>(this scoped Span<T> origin, scoped ReadOnlySpan<T> target) =>
        ((ReadOnlySpan<T>)origin).OffsetOf(target);

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
    /// <summary>Gets the specific slice from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="owner">The <see cref="IMemoryOwner{T}"/> to get an item from.</param>
    /// <param name="range">The index to get.</param>
    /// <returns>A slice from the parameter <paramref name="owner"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlyMemory<T> Nth<T>(this IMemoryOwner<T> owner, Range range)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            owner.Memory.Nth(range);

    /// <summary>Gets the specific slice from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="span">The <see cref="ReadOnlyMemory{T}"/> to get an item from.</param>
    /// <param name="range">The index to get.</param>
    /// <returns>A slice from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlyMemory<T> Nth<T>(this ReadOnlyMemory<T> span, Range range)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            range.TryGetOffsetAndLength(span.Length, out var off, out var len) ? span.Slice(off, len) : default;

    /// <summary>Gets the specific slice from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="span">The <see cref="Memory{T}"/> to get an item from.</param>
    /// <param name="range">The index to get.</param>
    /// <returns>A slice from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Memory<T> Nth<T>(this Memory<T> span, Range range)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            range.TryGetOffsetAndLength(span.Length, out var off, out var len) ? span.Slice(off, len) : default;

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="owner">The <see cref="IMemoryOwner{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="owner"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this IMemoryOwner<T> owner, [NonNegativeValue] int index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            owner.Memory.Nth(index);

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="memory">The <see cref="ReadOnlyMemory{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="memory"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this ReadOnlyMemory<T> memory, [NonNegativeValue] int index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            (uint)index < (uint)memory.Length ? memory.Span[index] : default;

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="owner">The <see cref="IMemoryOwner{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="owner"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this IMemoryOwner<T> owner, Index index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            owner.Memory.Nth(index);

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="memory">The <see cref="ReadOnlyMemory{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="memory"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this ReadOnlyMemory<T> memory, Index index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            index.GetOffset(memory.Length) is var o && (uint)o < (uint)memory.Length
                ? memory.Span.UnsafelyIndex(o)
                : default;

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="owner">The <see cref="IMemoryOwner{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="owner"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? NthLast<T>(this IMemoryOwner<T> owner, int index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            owner.Memory.NthLast(index);

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="memory">The <see cref="ReadOnlyMemory{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="memory"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? NthLast<T>(this ReadOnlyMemory<T> memory, [NonNegativeValue] int index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            (uint)(index - 1) < (uint)memory.Length ? memory.Span[memory.Length - index] : default;

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="memory">The <see cref="Memory{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="memory"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this Memory<T> memory, [NonNegativeValue] int index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            (uint)index < (uint)memory.Length ? memory.Span.UnsafelyIndex(index) : default;

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="memory">The <see cref="Memory{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="memory"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this Memory<T> memory, Index index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            index.GetOffset(memory.Length) is var off && (uint)off < (uint)memory.Length
                ? memory.Span.UnsafelyIndex(off)
                : default;

    /// <summary>Gets a specific item from the memory.</summary>
    /// <typeparam name="T">The type of item in the memory.</typeparam>
    /// <param name="memory">The <see cref="Memory{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="memory"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? NthLast<T>(this Memory<T> memory, [NonNegativeValue] int index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            (uint)(index - 1) < (uint)memory.Length ? memory.Span.UnsafelyIndex(memory.Length - index) : default;
#endif

    /// <summary>Gets the specific slice from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to get an item from.</param>
    /// <param name="range">The index to get.</param>
    /// <returns>A slice from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<T> Nth<T>(this ReadOnlySpan<T> span, Range range)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            range.TryGetOffsetAndLength(span.Length, out var off, out var len) ? span.UnsafelySlice(off, len) : default;

    /// <summary>Gets the specific slice from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to get an item from.</param>
    /// <param name="range">The index to get.</param>
    /// <returns>A slice from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Span<T> Nth<T>(this Span<T> span, Range range)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            range.TryGetOffsetAndLength(span.Length, out var off, out var len) ? span.UnsafelySlice(off, len) : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this scoped ReadOnlySpan<T> span, [NonNegativeValue] int index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            (uint)index < (uint)span.Length ? span.UnsafelyIndex(index) : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this scoped ReadOnlySpan<T> span, Index index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            index.GetOffset(span.Length) is var o && (uint)o < (uint)span.Length ? span.UnsafelyIndex(o) : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? NthLast<T>(this scoped ReadOnlySpan<T> span, [NonNegativeValue] int index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            (uint)(index - 1) < (uint)span.Length ? span.UnsafelyIndex(span.Length - index) : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this scoped Span<T> span, [NonNegativeValue] int index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            (uint)index < (uint)span.Length ? span.UnsafelyIndex(index) : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this scoped Span<T> span, Index index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            index.GetOffset(span.Length) is var o && (uint)o < (uint)span.Length ? span.UnsafelyIndex(o) : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? NthLast<T>(this scoped Span<T> span, [NonNegativeValue] int index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            (uint)(index - 1) < (uint)span.Length ? span.UnsafelyIndex(span.Length - index) : default;

    /// <inheritdoc cref="Span{T}.this"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T UnsafelyIndex<T>(this scoped ReadOnlySpan<T> body, int index) =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        Unsafe.Add(ref MemoryMarshal.GetReference(body), index);
#else
        body[index];
#endif

    /// <inheritdoc cref="Enumerable.Skip{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<T> UnsafelySkip<T>(this ReadOnlySpan<T> body, int start) =>
        UnsafelySlice(body, start, body.Length - start);

    /// <inheritdoc cref="Span{T}.Slice(int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<T> UnsafelySlice<T>(this ReadOnlySpan<T> body, int start, int length) =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref MemoryMarshal.GetReference(body), start), length);
#else
        body.Slice(start, length);
#endif

    /// <inheritdoc cref="Enumerable.Take{T}(IEnumerable{T}, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<T> UnsafelyTake<T>(this ReadOnlySpan<T> body, int end) => UnsafelySlice(body, 0, end);

    /// <inheritdoc cref="Span{T}.this"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T UnsafelyIndex<T>(this scoped Span<T> body, int index) =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        Unsafe.Add(ref MemoryMarshal.GetReference(body), index);
#else
        body[index];
#endif

    /// <inheritdoc cref="Enumerable.Skip{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Span<T> UnsafelySkip<T>(this Span<T> body, int start) =>
        UnsafelySlice(body, start, body.Length - start);

    /// <inheritdoc cref="Span{T}.Slice(int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Span<T> UnsafelySlice<T>(this Span<T> body, int start, int length) =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        MemoryMarshal.CreateSpan(ref Unsafe.Add(ref MemoryMarshal.GetReference(body), start), length);
#else
        body.Slice(start, length);
#endif

    /// <inheritdoc cref="Enumerable.Take{T}(IEnumerable{T}, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Span<T> UnsafelyTake<T>(this Span<T> body, int end) => UnsafelySlice(body, 0, end);
}
