// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;

/// <summary>Efficient LINQ-like methods for <see cref="ReadOnlySpan{T}"/> and siblings.</summary>
// ReSharper disable NullableWarningSuppressionIsUsed
#pragma warning disable MA0048
static partial class SpanQueries
#pragma warning restore MA0048
{
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Enumerable.Aggregate{T}(IEnumerable{T}, Func{T, T, T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Aggregate<T>(
        this IMemoryOwner<T> source,
        [InstantHandle, RequireStaticDelegate] Func<T, T, T> func
    ) =>
        Aggregate((ReadOnlySpan<T>)source.Memory.Span, func);

    /// <inheritdoc cref="Enumerable.Aggregate{T}(IEnumerable{T}, Func{T, T, T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Aggregate<T>(
        this Memory<T> source,
        [InstantHandle, RequireStaticDelegate] Func<T, T, T> func
    ) =>
        Aggregate((ReadOnlySpan<T>)source.Span, func);
#endif

    /// <inheritdoc cref="Enumerable.Aggregate{T}(IEnumerable{T}, Func{T, T, T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Aggregate<T>(this scoped Span<T> source, [InstantHandle, RequireStaticDelegate] Func<T, T, T> func)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            Aggregate((ReadOnlySpan<T>)source, func);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Enumerable.Aggregate{T}(IEnumerable{T}, Func{T, T, T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Aggregate<T>(
        this ReadOnlyMemory<T> source,
        [InstantHandle, RequireStaticDelegate] Func<T, T, T> func
    ) =>
        Aggregate(source.Span, func);
#endif

    /// <inheritdoc cref="Enumerable.Aggregate{T}(IEnumerable{T}, Func{T, T, T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Aggregate<T>(
        this scoped ReadOnlySpan<T> source,
        [InstantHandle, RequireStaticDelegate] Func<T, T, T> func
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        if (source.IsEmpty)
            return default;

        var accumulator = MemoryMarshal.GetReference(source);
        ref var start = ref Unsafe.Add(ref MemoryMarshal.GetReference(source), 1);
        ref var end = ref Unsafe.Add(ref start, source.Length);

        for (; Unsafe.IsAddressLessThan(ref start, ref end); start = ref Unsafe.Add(ref start, 1))
            accumulator = func(accumulator, start);

        return accumulator;
    }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Enumerable.Aggregate{T, TAccumulate}(IEnumerable{T}, TAccumulate, Func{TAccumulate, T, TAccumulate})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TAccumulate Aggregate<T, TAccumulate>(
        this IMemoryOwner<T> source,
        TAccumulate seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, T, TAccumulate> func
    )
#if !NO_ALLOWS_REF_STRUCT
        where TAccumulate : allows ref struct
#endif
        =>
            Aggregate((ReadOnlySpan<T>)source.Memory.Span, seed, func);

    /// <inheritdoc cref="Enumerable.Aggregate{T, TAccumulate}(IEnumerable{T}, TAccumulate, Func{TAccumulate, T, TAccumulate})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TAccumulate Aggregate<T, TAccumulate>(
        this Memory<T> source,
        TAccumulate seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, T, TAccumulate> func
    )
#if !NO_ALLOWS_REF_STRUCT
        where TAccumulate : allows ref struct
#endif
        =>
            Aggregate((ReadOnlySpan<T>)source.Span, seed, func);
#endif

    /// <inheritdoc cref="Enumerable.Aggregate{T, TAccumulate}(IEnumerable{T}, TAccumulate, Func{TAccumulate, T, TAccumulate})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TAccumulate Aggregate<T, TAccumulate>(
        this scoped Span<T> source,
        TAccumulate seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, T, TAccumulate> func
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
#if !NO_ALLOWS_REF_STRUCT
        where TAccumulate : allows ref struct
#endif
        =>
            Aggregate((ReadOnlySpan<T>)source, seed, func);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Enumerable.Aggregate{T, TAccumulate}(IEnumerable{T}, TAccumulate, Func{TAccumulate, T, TAccumulate})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TAccumulate Aggregate<T, TAccumulate>(
        this ReadOnlyMemory<T> source,
        TAccumulate seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, T, TAccumulate> func
    )
#if !NO_ALLOWS_REF_STRUCT
        where TAccumulate : allows ref struct
#endif
        =>
            Aggregate(source.Span, seed, func);
#endif

    /// <inheritdoc cref="Enumerable.Aggregate{T, TAccumulate}(IEnumerable{T}, TAccumulate, Func{TAccumulate, T, TAccumulate})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TAccumulate Aggregate<T, TAccumulate>(
        this scoped ReadOnlySpan<T> source,
        TAccumulate seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, T, TAccumulate> func
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
#if !NO_ALLOWS_REF_STRUCT
        where TAccumulate : allows ref struct
#endif
    {
        ref var start = ref MemoryMarshal.GetReference(source);
        ref var end = ref Unsafe.Add(ref start, source.Length);

        for (; Unsafe.IsAddressLessThan(ref start, ref end); start = ref Unsafe.Add(ref start, 1))
            seed = func(seed, start);

        return seed;
    }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Enumerable.Aggregate{T, TAccumulate, TResult}(IEnumerable{T}, TAccumulate, Func{TAccumulate, T, TAccumulate}, Func{TAccumulate, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Aggregate<T, TAccumulate, TResult>(
        this IMemoryOwner<T> source,
        TAccumulate seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, T, TAccumulate> func,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, TResult> resultSelector
    )
#if !NO_ALLOWS_REF_STRUCT
        where TAccumulate : allows ref struct
#endif
        =>
            Aggregate((ReadOnlySpan<T>)source.Memory.Span, seed, func, resultSelector);

    /// <inheritdoc cref="Enumerable.Aggregate{T, TAccumulate, TResult}(IEnumerable{T}, TAccumulate, Func{TAccumulate, T, TAccumulate}, Func{TAccumulate, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Aggregate<T, TAccumulate, TResult>(
        this Memory<T> source,
        TAccumulate seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, T, TAccumulate> func,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, TResult> resultSelector
    )
#if !NO_ALLOWS_REF_STRUCT
        where TAccumulate : allows ref struct
#endif
        =>
            Aggregate((ReadOnlySpan<T>)source.Span, seed, func, resultSelector);
#endif

    /// <inheritdoc cref="Enumerable.Aggregate{T, TAccumulate, TResult}(IEnumerable{T}, TAccumulate, Func{TAccumulate, T, TAccumulate}, Func{TAccumulate, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Aggregate<T, TAccumulate, TResult>(
        this scoped Span<T> source,
        TAccumulate seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, T, TAccumulate> func,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, TResult> resultSelector
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
#if !NO_ALLOWS_REF_STRUCT
        where TAccumulate : allows ref struct
#endif
        =>
            Aggregate((ReadOnlySpan<T>)source, seed, func, resultSelector);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Enumerable.Aggregate{T, TAccumulate, TResult}(IEnumerable{T}, TAccumulate, Func{TAccumulate, T, TAccumulate}, Func{TAccumulate, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Aggregate<T, TAccumulate, TResult>(
        this ReadOnlyMemory<T> source,
        TAccumulate seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, T, TAccumulate> func,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, TResult> resultSelector
    )
#if !NO_ALLOWS_REF_STRUCT
        where TAccumulate : allows ref struct
#endif
        =>
            Aggregate(source.Span, seed, func, resultSelector);
#endif

    /// <inheritdoc cref="Enumerable.Aggregate{T, TAccumulate, TResult}(IEnumerable{T}, TAccumulate, Func{TAccumulate, T, TAccumulate}, Func{TAccumulate, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Aggregate<T, TAccumulate, TResult>(
        this scoped ReadOnlySpan<T> source,
        TAccumulate seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, T, TAccumulate> func,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, TResult> resultSelector
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
#if !NO_ALLOWS_REF_STRUCT
        where TAccumulate : allows ref struct
#endif
    {
        ref var start = ref MemoryMarshal.GetReference(source);
        ref var end = ref Unsafe.Add(ref start, source.Length);

        for (; Unsafe.IsAddressLessThan(ref start, ref end); start = ref Unsafe.Add(ref start, 1))
            seed = func(seed, start);

        return resultSelector(seed);
    }
}
