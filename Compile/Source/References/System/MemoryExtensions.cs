// SPDX-License-Identifier: MPL-2.0

// ReSharper disable CheckNamespace EmptyNamespace WrongIndentSize
namespace System;
#pragma warning disable 8500
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
/// <summary>Extension methods for <see cref="Span{T}"/>, Memory{T}, and friends.</summary>
static partial class MemoryExtensions
{
    /// <summary>
    /// Determines whether two sequences are equal by comparing the
    /// elements using <see cref="IEquatable{T}.Equals(T?)"/>.
    /// </summary>
    /// <typeparam name="T">The type of span.</typeparam>
    /// <param name="span">The first span.</param>
    /// <param name="other">The other span.</param>
    /// <returns>The value determining whether two sequences are equal.</returns>
    // Unrolled and vectorized for half-constant input
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool SequenceEqual<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
    {
        var length = span.Length;
        return length == other.Length && SpanHelpers.SequenceEqual((T*)span.Pointer, (T*)other.Pointer, length);
    }

    /// <inheritdoc cref="SequenceEqual{T}(ReadOnlySpan{T}, ReadOnlySpan{T})" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool SequenceEqual<T>(this Span<T> span, ReadOnlySpan<T> other)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
        =>
            ((ReadOnlySpan<T>)span).SequenceEqual(other);

    /// <summary>
    /// Searches for the specified value and returns the index of its first occurrence.
    /// If not found, returns -1. Values are compared using <see cref="IEquatable{T}.Equals(T?)"/>.
    /// </summary>
    /// <typeparam name="T">The type of span and value.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value">The value to search for.</param>
    /// <returns>The index.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int IndexOf<T>(this ReadOnlySpan<T> span, T value)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
        =>
            SpanHelpers.IndexOf((T*)span.Pointer, value, span.Length);

    /// <summary>
    /// Searches for the specified sequence and returns the index of its first occurrence.
    /// If not found, returns -1. Values are compared using <see cref="IEquatable{T}.Equals(T?)"/>.
    /// </summary>
    /// <typeparam name="T">The type of span and value.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value">The sequence to search for.</param>
    /// <returns>The index.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int IndexOf<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
        =>
            SpanHelpers.IndexOf((T*)span.Pointer, span.Length, (T*)value.Pointer, value.Length);

    /// <inheritdoc cref="IndexOf{T}(ReadOnlySpan{T}, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf<T>(this Span<T> span, T value)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
        =>
            ((ReadOnlySpan<T>)span).IndexOf(value);

    /// <inheritdoc cref="IndexOf{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf<T>(this Span<T> span, ReadOnlySpan<T> value)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
        =>
            ((ReadOnlySpan<T>)span).IndexOf(value);
}
#endif
