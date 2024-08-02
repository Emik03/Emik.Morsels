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
        this in Memory<T> source,
        [InstantHandle, RequireStaticDelegate] Func<T, T, T> func
    ) =>
        Aggregate((ReadOnlySpan<T>)source.Span, func);
#endif

    /// <inheritdoc cref="Enumerable.Aggregate{T}(IEnumerable{T}, Func{T, T, T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Aggregate<T>(
        this scoped in Span<T> source,
        [InstantHandle, RequireStaticDelegate] Func<T, T, T> func
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            Aggregate((ReadOnlySpan<T>)source, func);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Enumerable.Aggregate{T}(IEnumerable{T}, Func{T, T, T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Aggregate<T>(
        this in ReadOnlyMemory<T> source,
        [InstantHandle, RequireStaticDelegate] Func<T, T, T> func
    ) =>
        Aggregate(source.Span, func);
#endif

    /// <inheritdoc cref="Enumerable.Aggregate{T}(IEnumerable{T}, Func{T, T, T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Aggregate<T>(
        this scoped in ReadOnlySpan<T> source,
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
    ) =>
        Aggregate((ReadOnlySpan<T>)source.Memory.Span, seed, func);

    /// <inheritdoc cref="Enumerable.Aggregate{T, TAccumulate}(IEnumerable{T}, TAccumulate, Func{TAccumulate, T, TAccumulate})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TAccumulate Aggregate<T, TAccumulate>(
        this in Memory<T> source,
        TAccumulate seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, T, TAccumulate> func
    ) =>
        Aggregate((ReadOnlySpan<T>)source.Span, seed, func);
#endif

    /// <inheritdoc cref="Enumerable.Aggregate{T, TAccumulate}(IEnumerable{T}, TAccumulate, Func{TAccumulate, T, TAccumulate})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TAccumulate Aggregate<T, TAccumulate>(
        this scoped in Span<T> source,
        TAccumulate seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, T, TAccumulate> func
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            Aggregate((ReadOnlySpan<T>)source, seed, func);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Enumerable.Aggregate{T, TAccumulate}(IEnumerable{T}, TAccumulate, Func{TAccumulate, T, TAccumulate})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TAccumulate Aggregate<T, TAccumulate>(
        this in ReadOnlyMemory<T> source,
        TAccumulate seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, T, TAccumulate> func
    ) =>
        Aggregate(source.Span, seed, func);
#endif

    /// <inheritdoc cref="Enumerable.Aggregate{T, TAccumulate}(IEnumerable{T}, TAccumulate, Func{TAccumulate, T, TAccumulate})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TAccumulate Aggregate<T, TAccumulate>(
        this scoped in ReadOnlySpan<T> source,
        TAccumulate seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, T, TAccumulate> func
    )
#if UNMANAGED_SPAN
        where T : unmanaged
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
    ) =>
        Aggregate((ReadOnlySpan<T>)source.Memory.Span, seed, func, resultSelector);

    /// <inheritdoc cref="Enumerable.Aggregate{T, TAccumulate, TResult}(IEnumerable{T}, TAccumulate, Func{TAccumulate, T, TAccumulate}, Func{TAccumulate, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Aggregate<T, TAccumulate, TResult>(
        this in Memory<T> source,
        TAccumulate seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, T, TAccumulate> func,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, TResult> resultSelector
    ) =>
        Aggregate((ReadOnlySpan<T>)source.Span, seed, func, resultSelector);
#endif

    /// <inheritdoc cref="Enumerable.Aggregate{T, TAccumulate, TResult}(IEnumerable{T}, TAccumulate, Func{TAccumulate, T, TAccumulate}, Func{TAccumulate, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Aggregate<T, TAccumulate, TResult>(
        this scoped in Span<T> source,
        TAccumulate seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, T, TAccumulate> func,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, TResult> resultSelector
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            Aggregate((ReadOnlySpan<T>)source, seed, func, resultSelector);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Enumerable.Aggregate{T, TAccumulate, TResult}(IEnumerable{T}, TAccumulate, Func{TAccumulate, T, TAccumulate}, Func{TAccumulate, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Aggregate<T, TAccumulate, TResult>(
        this in ReadOnlyMemory<T> source,
        TAccumulate seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, T, TAccumulate> func,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, TResult> resultSelector
    ) =>
        Aggregate(source.Span, seed, func, resultSelector);
#endif

    /// <inheritdoc cref="Enumerable.Aggregate{T, TAccumulate, TResult}(IEnumerable{T}, TAccumulate, Func{TAccumulate, T, TAccumulate}, Func{TAccumulate, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Aggregate<T, TAccumulate, TResult>(
        this scoped in ReadOnlySpan<T> source,
        TAccumulate seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, T, TAccumulate> func,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, TResult> resultSelector
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        ref var start = ref MemoryMarshal.GetReference(source);
        ref var end = ref Unsafe.Add(ref start, source.Length);

        for (; Unsafe.IsAddressLessThan(ref start, ref end); start = ref Unsafe.Add(ref start, 1))
            seed = func(seed, start);

        return resultSelector(seed);
    }
}
