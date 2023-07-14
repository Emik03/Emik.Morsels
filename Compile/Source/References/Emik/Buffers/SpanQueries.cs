// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;

/// <summary>Efficient LINQ-like methods for <see cref="ReadOnlySpan{T}"/> and siblings.</summary>
// ReSharper disable NullableWarningSuppressionIsUsed
#pragma warning disable MA0048
static partial class SpanQueries
#pragma warning restore MA0048
{
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

    /// <inheritdoc cref="Enumerable.Select{T, TResult}(IEnumerable{T}, Func{T, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> Select<T>(this Memory<T> source, [InstantHandle, RequireStaticDelegate] Func<T, T> selector)
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
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
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
#if NET7_0_OR_GREATER
    /// <inheritdoc cref="Range{T}(Span{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Range<T>(this IMemoryOwner<T> source)
        where T : INumberBase<T>
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            Range(source.Memory.Span, T.Zero);

    /// <inheritdoc cref="Range{T}(Span{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Range<T>(this Memory<T> source)
        where T : INumberBase<T>
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            Range(source.Span, T.Zero);

    /// <summary>Creates the range.</summary>
    /// <typeparam name="T">The type of number in the <see cref="Span{T}"/>.</typeparam>
    /// <param name="source">The <see cref="Span{T}"/> to mutate.</param>
    /// <returns>The parameter <paramref name="source"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Range<T>(this Span<T> source)
        where T : INumberBase<T>
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            Range(source, T.Zero);

    /// <inheritdoc cref="Range{T}(Span{T}, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Range<T>(this IMemoryOwner<T> source, T start)
        where T : IIncrementOperators<T>
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            Range(source.Memory.Span, start);

    /// <inheritdoc cref="Range{T}(Span{T}, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Range<T>(this Memory<T> source, T start)
        where T : IIncrementOperators<T>
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            Range(source.Span, start);

    /// <summary>Creates the range.</summary>
    /// <typeparam name="T">The type of number in the <see cref="Span{T}"/>.</typeparam>
    /// <param name="source">The <see cref="Span{T}"/> to mutate.</param>
    /// <param name="start">The number to start with.</param>
    /// <returns>The parameter <paramref name="source"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Range<T>(this Span<T> source, T start)
#if UNMANAGED_SPAN
        where T : IIncrementOperators<T>, unmanaged
#else
        where T : IIncrementOperators<T>
#endif
    {
        for (var i = 0; i < source.Length; i++, start++)
            source[i] = start;

        return source;
    }
#else
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Range(Span{int})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<int> Range(this IMemoryOwner<int> source) => Range(source.Memory.Span, 0);

    /// <inheritdoc cref="Range(Span{int})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<int> Range(this Memory<int> source) => Range(source.Span, 0);
#endif
    /// <summary>Creates the range.</summary>
    /// <param name="source">The <see cref="Span{T}"/> to mutate.</param>
    /// <returns>The parameter <paramref name="source"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<int> Range(this Span<int> source)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            Range(source, 0);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Range(Span{int}, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<int> Range(this IMemoryOwner<int> source, int start) => Range(source.Memory.Span, start);

    /// <inheritdoc cref="Range(Span{int}, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<int> Range(this Memory<int> source, int start) => Range(source.Span, start);
#endif
    /// <summary>Creates the range.</summary>
    /// <param name="source">The <see cref="Span{T}"/> to mutate.</param>
    /// <param name="start">The number to start with.</param>
    /// <returns>The parameter <paramref name="source"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<int> Range(this Span<int> source, int start)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        for (var i = 0; i < source.Length; i++)
            source[i] = i + start;

        return source;
    }
#endif
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Enumerable.Where{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> Where<T>(
        this IMemoryOwner<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    ) =>
        source.Memory[..^Filter(source.Memory.Span, predicate)];

    /// <inheritdoc cref="Enumerable.Where{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> Where<T>(
        this Memory<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    ) =>
        source[..^Filter(source.Span, predicate)];
#endif
    /// <inheritdoc cref="Enumerable.Where{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Where<T>(
        this Span<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            source[..^Filter(source, predicate)];
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Enumerable.Aggregate{T}(IEnumerable{T}, Func{T, T, T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Aggregate<T>(
        this IMemoryOwner<T> source,
        [InstantHandle, RequireStaticDelegate] Func<T, T, T> func
    ) =>
        Aggregate((ReadOnlySpan<T>)source.Memory.Span, func);

    /// <inheritdoc cref="Enumerable.Aggregate{T}(IEnumerable{T}, Func{T, T, T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Aggregate<T>(this Memory<T> source, [InstantHandle, RequireStaticDelegate] Func<T, T, T> func) =>
        Aggregate((ReadOnlySpan<T>)source.Span, func);
#endif
    /// <inheritdoc cref="Enumerable.Aggregate{T}(IEnumerable{T}, Func{T, T, T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Aggregate<T>(
        this scoped Span<T> source,
        [InstantHandle, RequireStaticDelegate] Func<T, T, T> func
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            Aggregate((ReadOnlySpan<T>)source, func);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
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
        var e = source.GetEnumerator();

        if (!e.MoveNext())
            return default;

        var accumulator = e.Current;

        while (e.MoveNext())
            accumulator = func(accumulator, e.Current);

        return accumulator;
    }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
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
        this Memory<T> source,
        TAccumulate seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, T, TAccumulate> func
    ) =>
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
        =>
            Aggregate((ReadOnlySpan<T>)source, seed, func);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Enumerable.Aggregate{T, TAccumulate}(IEnumerable{T}, TAccumulate, Func{TAccumulate, T, TAccumulate})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TAccumulate Aggregate<T, TAccumulate>(
        this ReadOnlyMemory<T> source,
        TAccumulate seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, T, TAccumulate> func
    ) =>
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
    {
        foreach (var next in source)
            seed = func(seed, next);

        return seed;
    }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
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
        this Memory<T> source,
        TAccumulate seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, T, TAccumulate> func,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, TResult> resultSelector
    ) =>
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
        =>
            Aggregate((ReadOnlySpan<T>)source, seed, func, resultSelector);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Enumerable.Aggregate{T, TAccumulate, TResult}(IEnumerable{T}, TAccumulate, Func{TAccumulate, T, TAccumulate}, Func{TAccumulate, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Aggregate<T, TAccumulate, TResult>(
        this ReadOnlyMemory<T> source,
        TAccumulate seed,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, T, TAccumulate> func,
        [InstantHandle, RequireStaticDelegate] Func<TAccumulate, TResult> resultSelector
    ) =>
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
    {
        foreach (var next in source)
            seed = func(seed, next);

        return resultSelector(seed);
    }

    // Surprisingly, direct indexing is more efficient than .CopyTo despite latter guaranteeing SIMD.
    // Benchmarked with various sizes and on function "static x => BitOperations.PopCount(x) % 2 is 0".
    // This function was chosen as the baseline due to it being cheap to compute, evenly distributed,
    // and ensuring all patterns of [[false, false], [false, true], [true, false], [true, true]] would appear.
    // ReSharper disable CommentTypo
    // https://sharplab.io/#v2:D4AQTAjAsAULIQGwAICWA7ALsg6gCwFMAnAgSXXWIB4AVAPgApZkXkBlABwEN1a7kAzgHsArkQDGBADTNWABRIATVOK6YCfZByUq1BWAEpYAb1ksAZkKLIGANy7XUyALzIADAG40yKoNESCADoAGQJ0AHNMPC9UAGpYoxhWZFMk5NZUcxttAmVVdQZhMUkAbVQAXQNE9JrkcSEsDBECD1gzdPtrAUwHbFdUVrg0msUhdtq0LIZ4pzpXIoCQsMi8aonakAB2QR6iTEGJgHc8VAAbAhsAQhy8vUL/UoqqwfHWTuRlAQ5TrkkAWzCfW8AFodr0DrUFo9AoFyoEAMJCDgATxoQnuxQIJW6vRhlQhNShF3mDyxMIAep9vr8CACsOUCeknMDXFSfv9AYyAL5tYbILZ+TFLCJRQZcoA
    // https://cdn.discordapp.com/attachments/445375602648940544/1129045669148098610/image.png
    // ReSharper restore CommentTypo

    [NonNegativeValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int Filter<T>(Span<T> source, [InstantHandle, RequireStaticDelegate] Predicate<T> predicate)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        var end = 0;

        for (var i = 0; i < source.Length; i++)
        {
            if (predicate(source[i]))
            {
                if (end > 0)
                    source[i - end] = source[i];

                continue;
            }

            var start = i;

            do
                if (++i >= source.Length)
                    return end + i - start;
            while (!predicate(source[i]));

            end += i - start;
            source[i - end] = source[i];
        }

        return end;
    }
}
