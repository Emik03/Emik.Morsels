// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent RedundantNameQualifier RedundantUnsafeContext UseSymbolAlias
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Defines methods for spans.</summary>
static partial class Span
{
    /// <summary>Gets the index of an element of a given <see cref="Span{T}"/> from its reference.</summary>
    /// <typeparam name="T">The type if items in the input <see cref="Span{T}"/>.</typeparam>
    /// <param name="span">The input <see cref="Span{T}"/> to calculate the index for.</param>
    /// <param name="value">The reference to the target item to get the index for.</param>
    /// <returns>The index of <paramref name="value"/> within <paramref name="span"/>, or <c>-1</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int IndexOf<T>(this scoped ReadOnlySpan<T> span, scoped ref T value)
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        =>
            Unsafe.ByteOffset(ref MemoryMarshal.GetReference(span), ref value) is var byteOffset &&
            byteOffset / (nint)(uint)Unsafe.SizeOf<T>() is var elementOffset &&
            (nuint)elementOffset < (uint)span.Length
                ? (int)elementOffset
                : -1;
#else
    {
        fixed (T* ptr = &value)
        fixed (T* s = span)
            return (nint)(span.Align(s) - ptr) is var elementOffset && (nuint)elementOffset < (uint)span.Length
                ? (int)elementOffset
                : -1;
    }
#endif
    /// <inheritdoc cref="IndexOf{T}(ReadOnlySpan{T}, ref T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int OffsetOf<T>(this scoped ReadOnlySpan<T> origin, scoped ReadOnlySpan<T> target)
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        =>
            origin.IndexOf(ref MemoryMarshal.GetReference(target));
#else
    {
        fixed (T* value = target)
            return origin.IndexOf(ref *target.Align(value));
    }
#endif
    /// <inheritdoc cref="IndexOf{T}(ReadOnlySpan{T}, ref T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int OffsetOf<T>(this scoped Span<T> origin, scoped ReadOnlySpan<T> target) =>
        origin.ReadOnly().OffsetOf(target);

    /// <summary>Converts the <see cref="Span{T}"/> to the <see cref="ReadOnlySpan{T}"/>.</summary>
    /// <typeparam name="T">The type of span.</typeparam>
    /// <param name="span">The span to convert.</param>
    /// <returns>The <see cref="ReadOnlySpan{T}"/> of the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<T> ReadOnly<T>(this Span<T> span) => span;

    /// <inheritdoc cref="Span{T}.this"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T UnsafelyIndex<T>(this scoped ReadOnlySpan<T> body, [NonNegativeValue] int index)
    {
        System.Diagnostics.Debug.Assert((uint)index < (uint)body.Length, "index is in range");
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        return Unsafe.Add(ref MemoryMarshal.GetReference(body), index);
#else
        return body[index];
#endif
    }

    /// <inheritdoc cref="Enumerable.Skip{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<T> UnsafelySkip<T>(this ReadOnlySpan<T> body, [NonNegativeValue] int start)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        System.Diagnostics.Debug.Assert((uint)start <= (uint)body.Length, "start is in range");
        return UnsafelySlice(body, start, body.Length - start);
    }

    /// <inheritdoc cref="Span{T}.Slice(int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<T> UnsafelySlice<T>(
        this ReadOnlySpan<T> body,
        [NonNegativeValue] int start,
        [NonNegativeValue] int length
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        System.Diagnostics.Debug.Assert((uint)(start + length) <= (uint)body.Length, "start and length is in range");
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref MemoryMarshal.GetReference(body), start), length);
#else
        return body.Slice(start, length);
#endif
    }

    /// <inheritdoc cref="Enumerable.Take{T}(IEnumerable{T}, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<T> UnsafelyTake<T>(this ReadOnlySpan<T> body, [NonNegativeValue] int end)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        System.Diagnostics.Debug.Assert((uint)end <= (uint)body.Length, "end is in range");
        return UnsafelySlice(body, 0, end);
    }

    /// <inheritdoc cref="Span{T}.this"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T UnsafelyIndex<T>(this scoped Span<T> body, [NonNegativeValue] int index)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        System.Diagnostics.Debug.Assert((uint)index < (uint)body.Length, "index is in range");
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
        return Unsafe.Add(ref MemoryMarshal.GetReference(body), index);
#else
        return body[index];
#endif
    }

    /// <inheritdoc cref="Enumerable.Skip{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Span<T> UnsafelySkip<T>(this Span<T> body, [NonNegativeValue] int start)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        System.Diagnostics.Debug.Assert((uint)start <= (uint)body.Length, "start is in range");
        return UnsafelySlice(body, start, body.Length - start);
    }

    /// <inheritdoc cref="Span{T}.Slice(int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Span<T> UnsafelySlice<T>(
        this Span<T> body,
        [NonNegativeValue] int start,
        [NonNegativeValue] int length
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        System.Diagnostics.Debug.Assert((uint)(start + length) <= (uint)body.Length, "start and length is in range");
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        return MemoryMarshal.CreateSpan(ref Unsafe.Add(ref MemoryMarshal.GetReference(body), start), length);
#else
        return body.Slice(start, length);
#endif
    }

    /// <inheritdoc cref="Enumerable.Take{T}(IEnumerable{T}, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Span<T> UnsafelyTake<T>(this Span<T> body, [NonNegativeValue] int end)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        System.Diagnostics.Debug.Assert((uint)end <= (uint)body.Length, "end is in range");
        return UnsafelySlice(body, 0, end);
    }
}
