// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;

/// <summary>Efficient LINQ-like methods for <see cref="ReadOnlySpan{T}"/> and siblings.</summary>
// ReSharper disable NullableWarningSuppressionIsUsed
#pragma warning disable MA0048
static partial class SpanQueries
#pragma warning restore MA0048
{
    /// <summary>Determines whether the type is a numeric primitive.</summary>
    /// <typeparam name="T">The type to test.</typeparam>
    /// <returns>Whether the type parameter <typeparamref name="T"/> is a primitive representing a number.</returns>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNumericPrimitive<T>() =>
        typeof(T) == typeof(byte) ||
        typeof(T) == typeof(double) ||
        typeof(T) == typeof(float) ||
        typeof(T) == typeof(int) ||
        typeof(T) == typeof(long) ||
        typeof(T) == typeof(nint) ||
        typeof(T) == typeof(nuint) ||
        typeof(T) == typeof(sbyte) ||
        typeof(T) == typeof(short) ||
        typeof(T) == typeof(uint) ||
        typeof(T) == typeof(ulong) ||
        typeof(T) == typeof(ushort);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Enumerable.All{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool All<T>(this IMemoryOwner<T> source, [InstantHandle, RequireStaticDelegate] Predicate<T> func) =>
        All((ReadOnlySpan<T>)source.Memory.Span, func);

    /// <inheritdoc cref="Enumerable.All{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool All<T>(this Memory<T> source, [InstantHandle, RequireStaticDelegate] Predicate<T> func) =>
        All((ReadOnlySpan<T>)source.Span, func);
#endif

    /// <inheritdoc cref="Enumerable.All{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool All<T>(this scoped Span<T> source, [InstantHandle, RequireStaticDelegate] Predicate<T> func)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            All((ReadOnlySpan<T>)source, func);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Enumerable.All{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool All<T>(
        this ReadOnlyMemory<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> func
    ) =>
        All(source.Span, func);
#endif

    /// <inheritdoc cref="Enumerable.All{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool All<T>(
        this scoped ReadOnlySpan<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> func
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        foreach (var next in source)
            if (!func(next))
                return false;

        return true;
    }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Enumerable.Any{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Any<T>(this IMemoryOwner<T> source, [InstantHandle, RequireStaticDelegate] Predicate<T> func) =>
        Any((ReadOnlySpan<T>)source.Memory.Span, func);

    /// <inheritdoc cref="Enumerable.Any{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Any<T>(this Memory<T> source, [InstantHandle, RequireStaticDelegate] Predicate<T> func) =>
        Any((ReadOnlySpan<T>)source.Span, func);
#endif

    /// <inheritdoc cref="Enumerable.Any{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Any<T>(this scoped Span<T> source, [InstantHandle, RequireStaticDelegate] Predicate<T> func)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            Any((ReadOnlySpan<T>)source, func);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Enumerable.Any{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Any<T>(
        this ReadOnlyMemory<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> func
    ) =>
        Any(source.Span, func);
#endif

    /// <inheritdoc cref="Enumerable.Any{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Any<T>(
        this scoped ReadOnlySpan<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> func
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        foreach (var next in source)
            if (func(next))
                return true;

        return false;
    }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Enumerable.Select{T, TResult}(IEnumerable{T}, Func{T, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IMemoryOwner<T> Select<T>(
        this IMemoryOwner<T> source,
        [InstantHandle, RequireStaticDelegate] Func<T, T> selector
    )
    {
        Select(source.Memory.Span, selector);
        return source;
    }

    /// <inheritdoc cref="Enumerable.Select{T, TResult}(IEnumerable{T}, Func{T, int, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IMemoryOwner<T> Select<T>(
        this IMemoryOwner<T> source,
        [InstantHandle, RequireStaticDelegate] Func<T, int, T> selector
    )
    {
        Select(source.Memory.Span, selector);
        return source;
    }

    /// <inheritdoc cref="Enumerable.Select{T, TResult}(IEnumerable{T}, Func{T, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> Select<T>(this Memory<T> source, [InstantHandle, RequireStaticDelegate] Func<T, T> selector)
    {
        Select(source.Span, selector);
        return source;
    }

    /// <inheritdoc cref="Enumerable.Select{T, TResult}(IEnumerable{T}, Func{T, int, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> Select<T>(
        this Memory<T> source,
        [InstantHandle, RequireStaticDelegate] Func<T, int, T> selector
    )
    {
        Select(source.Span, selector);
        return source;
    }
#endif

    /// <inheritdoc cref="Enumerable.Select{T, TResult}(IEnumerable{T}, Func{T, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Select<T>(this Span<T> source, [InstantHandle, RequireStaticDelegate] Func<T, T> selector)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        for (var i = 0; i < source.Length; i++)
            source[i] = selector(source[i]);

        return source;
    }

    /// <inheritdoc cref="Enumerable.Select{T, TResult}(IEnumerable{T}, Func{T, int, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Select<T>(
        this Span<T> source,
        [InstantHandle, RequireStaticDelegate] Func<T, int, T> selector
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        for (var i = 0; i < source.Length; i++)
            source[i] = selector(source[i], i);

        return source;
    }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="System.MemoryExtensions.SequenceEqual{T}(Span{T}, ReadOnlySpan{T}, IEqualityComparer{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool SequenceEqual<T>(
        this Memory<T> span,
        ReadOnlyMemory<T> other,
        IEqualityComparer<T>? comparer = null
    ) =>
        span.Span.SequenceEqual(other.Span, comparer);

    /// <inheritdoc cref="System.MemoryExtensions.SequenceEqual{T}(Span{T}, ReadOnlySpan{T}, IEqualityComparer{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool SequenceEqual<T>(
        this ReadOnlyMemory<T> span,
        ReadOnlyMemory<T> other,
        IEqualityComparer<T>? comparer = null
    ) =>
        span.Span.SequenceEqual(other.Span, comparer);

    /// <inheritdoc cref="Enumerable.SkipWhile{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> SkipWhile<T>(
        this IMemoryOwner<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    ) =>
        SkipWhile(source.Memory, predicate);

    /// <inheritdoc cref="Enumerable.SkipWhile{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> SkipWhile<T>(
        this Memory<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    )
    {
        var span = source.Span;

        for (var i = 0; i < source.Length; i++)
            if (!predicate(span[i]))
                return source[i..];

        return source;
    }
#endif

    /// <inheritdoc cref="Enumerable.SkipWhile{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> SkipWhile<T>(
        this Span<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        for (var i = 0; i < source.Length; i++)
            if (!predicate(source[i]))
                return source[i..];

        return source;
    }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Enumerable.SkipWhile{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyMemory<T> SkipWhile<T>(
        this ReadOnlyMemory<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    )
    {
        var span = source.Span;

        for (var i = 0; i < source.Length; i++)
            if (!predicate(span[i]))
                return source[i..];

        return source;
    }
#endif

    /// <inheritdoc cref="Enumerable.SkipWhile{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> SkipWhile<T>(
        this ReadOnlySpan<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        for (var i = 0; i < source.Length; i++)
            if (!predicate(source[i]))
                return source[i..];

        return source;
    }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Enumerable.TakeWhile{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> TakeWhile<T>(
        this IMemoryOwner<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    ) =>
        TakeWhile(source.Memory, predicate);

    /// <inheritdoc cref="Enumerable.TakeWhile{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> TakeWhile<T>(
        this Memory<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    )
    {
        var span = source.Span;

        for (var i = 0; i < source.Length; i++)
            if (predicate(span[i]))
                return source[..i];

        return source;
    }
#endif

    /// <inheritdoc cref="Enumerable.TakeWhile{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> TakeWhile<T>(
        this Span<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        for (var i = 0; i < source.Length; i++)
            if (predicate(source[i]))
                return source[..i];

        return source;
    }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Enumerable.TakeWhile{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyMemory<T> TakeWhile<T>(
        this ReadOnlyMemory<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    )
    {
        var span = source.Span;

        for (var i = 0; i < source.Length; i++)
            if (predicate(span[i]))
                return source[..i];

        return source;
    }
#endif

    /// <inheritdoc cref="Enumerable.TakeWhile{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> TakeWhile<T>(
        this ReadOnlySpan<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        for (var i = 0; i < source.Length; i++)
            if (predicate(source[i]))
                return source[..i];

        return source;
    }
}
