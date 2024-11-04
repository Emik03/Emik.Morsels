// SPDX-License-Identifier: MPL-2.0

// ReSharper disable CheckNamespace EmptyNamespace WrongIndentSize
namespace System;
#pragma warning disable 8500
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
using Emik.Morsels;

/// <summary>Extension methods for <see cref="Span{T}"/>, <c>Memory&lt;T&gt;</c>, and friends.</summary>
static partial class MemoryExtensions
{
    public static nint StringAdjustment = MeasureStringAdjustment();
#pragma warning disable RCS1118 // ReSharper disable once ConvertToConstant.Local
    static unsafe nint MeasureStringAdjustment()
    {
        var text = "a";
#pragma warning restore RCS1118
        fixed (char* c = text)
        fixed (char* pinnable = &Span.Ret<Pinnable<char>>.From(text).Data)
            return (nint)(c - pinnable);
    }

    /// <summary>Determines whether this span ends with the specified value.</summary>
    /// <typeparam name="T">The type of span and value.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value">The value to search for.</param>
    /// <returns>The value determining whether this span ends with the specified value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EndsWith<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
    {
        var offset = span.Length - value.Length;

        if (offset < 0)
            return false;

        for (var i = offset; i < value.Length; i++)
            if (value[i] is { } next ? next.Equals(span[i]) : span[i] is not null)
                return false;

        return true;
    }

    /// <inheritdoc cref="StartsWith{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EndsWith<T>(this Span<T> span, ReadOnlySpan<T> value)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
        =>
            span.ReadOnly().StartsWith(value);

    /// <summary>
    /// Determines whether two sequences are equal by comparing the
    /// elements using <see cref="IEquatable{T}.Equals(T)"/>.
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
            span.ReadOnly().SequenceEqual(other);

    /// <summary>Determines whether this span starts with the specified value.</summary>
    /// <typeparam name="T">The type of span and value.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value">The value to search for.</param>
    /// <returns>The value determining whether this span starts with the specified value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWith<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
    {
        if (value.Length > span.Length)
            return false;

        for (var i = 0; i < value.Length; i++)
            if (value[i] is { } next ? next.Equals(span[i]) : span[i] is not null)
                return false;

        return true;
    }

    /// <inheritdoc cref="StartsWith{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWith<T>(this Span<T> span, ReadOnlySpan<T> value)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
        =>
            span.ReadOnly().StartsWith(value);

    /// <summary>
    /// Searches for the specified value and returns the index of its first occurrence.
    /// If not found, returns -1. Values are compared using <see cref="IEquatable{T}.Equals(T)"/>.
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
    /// If not found, returns -1. Values are compared using <see cref="IEquatable{T}.Equals(T)"/>.
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
            span.ReadOnly().IndexOf(value);

    /// <inheritdoc cref="IndexOf{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf<T>(this Span<T> span, ReadOnlySpan<T> value)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
        =>
            span.ReadOnly().IndexOf(value);

    /// <summary>
    /// Searches for the specified value and returns the index of its last occurrence.
    /// If not found, returns -1. Values are compared using <see cref="IEquatable{T}.Equals(T)"/>.
    /// </summary>
    /// <typeparam name="T">The type of span and value.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value">The value to search for.</param>
    /// <returns>The index.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LastIndexOf<T>(this ReadOnlySpan<T> span, T value)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
    {
        for (var i = span.Length - 1; i >= 0; i--)
            if (value?.Equals(span[i]) ?? span[i] is null)
                return i;

        return -1;
    }

    /// <inheritdoc cref="LastIndexOf{T}(ReadOnlySpan{T}, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LastIndexOf<T>(this Span<T> span, T value)
#if UNMANAGED_SPAN
        where T : unmanaged, IEquatable<T>?
#else
        where T : IEquatable<T>?
#endif
        =>
            span.ReadOnly().IndexOf(value);
}
#endif
