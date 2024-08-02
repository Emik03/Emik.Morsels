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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Enumerable.All{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool All<T>(this IMemoryOwner<T> source, [InstantHandle, RequireStaticDelegate] Predicate<T> func) =>
        All((ReadOnlySpan<T>)source.Memory.Span, func);

    /// <inheritdoc cref="Enumerable.All{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool All<T>(this in Memory<T> source, [InstantHandle, RequireStaticDelegate] Predicate<T> func) =>
        All((ReadOnlySpan<T>)source.Span, func);
#endif

    /// <inheritdoc cref="Enumerable.All{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool All<T>(this scoped in Span<T> source, [InstantHandle, RequireStaticDelegate] Predicate<T> func)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            All((ReadOnlySpan<T>)source, func);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Enumerable.All{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool All<T>(
        this in ReadOnlyMemory<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> func
    ) =>
        All(source.Span, func);
#endif

    /// <inheritdoc cref="Enumerable.All{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool All<T>(
        this scoped in ReadOnlySpan<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> func
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        ref var start = ref MemoryMarshal.GetReference(source);

        for (ref var end = ref Unsafe.Add(ref start, source.Length);
            Unsafe.IsAddressLessThan(ref start, ref end);
            start = ref Unsafe.Add(ref start, 1))
            if (!func(start))
                return false;

        return true;
    }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Enumerable.Any{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Any<T>(this IMemoryOwner<T> source, [InstantHandle, RequireStaticDelegate] Predicate<T> func) =>
        Any((ReadOnlySpan<T>)source.Memory.Span, func);

    /// <inheritdoc cref="Enumerable.Any{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Any<T>(this in Memory<T> source, [InstantHandle, RequireStaticDelegate] Predicate<T> func) =>
        Any((ReadOnlySpan<T>)source.Span, func);
#endif

    /// <inheritdoc cref="Enumerable.Any{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Any<T>(this scoped in Span<T> source, [InstantHandle, RequireStaticDelegate] Predicate<T> func)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            Any((ReadOnlySpan<T>)source, func);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Enumerable.Any{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Any<T>(
        this in ReadOnlyMemory<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> func
    ) =>
        Any(source.Span, func);
#endif

    /// <inheritdoc cref="Enumerable.Any{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Any<T>(
        this scoped in ReadOnlySpan<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> func
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        ref var start = ref MemoryMarshal.GetReference(source);

        for (ref var end = ref Unsafe.Add(ref start, source.Length);
            Unsafe.IsAddressLessThan(ref start, ref end);
            start = ref Unsafe.Add(ref start, 1))
            if (func(start))
                return true;

        return false;
    }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="System.MemoryExtensions.Equals(ReadOnlySpan{char}, ReadOnlySpan{char}, StringComparison)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool EqualsIgnoreCase(this string left, scoped in ReadOnlySpan<char> right) =>
        ((ReadOnlySpan<char>)left).Equals(right, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc cref="System.MemoryExtensions.Equals(ReadOnlySpan{char}, ReadOnlySpan{char}, StringComparison)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool EqualsIgnoreCase(this IMemoryOwner<char> left, scoped in ReadOnlySpan<char> right) =>
        ((ReadOnlySpan<char>)left.Memory.Span).Equals(right, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc cref="System.MemoryExtensions.Equals(ReadOnlySpan{char}, ReadOnlySpan{char}, StringComparison)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool EqualsIgnoreCase(this in Memory<char> left, scoped in ReadOnlySpan<char> right) =>
        ((ReadOnlySpan<char>)left.Span).Equals(right, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc cref="System.MemoryExtensions.Equals(ReadOnlySpan{char}, ReadOnlySpan{char}, StringComparison)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool EqualsIgnoreCase(this scoped in Span<char> left, scoped in ReadOnlySpan<char> right) =>
        ((ReadOnlySpan<char>)left).Equals(right, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc cref="System.MemoryExtensions.Equals(ReadOnlySpan{char}, ReadOnlySpan{char}, StringComparison)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool EqualsIgnoreCase(this in ReadOnlyMemory<char> left, scoped in ReadOnlySpan<char> right) =>
        left.Span.Equals(right, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc cref="System.MemoryExtensions.Equals(ReadOnlySpan{char}, ReadOnlySpan{char}, StringComparison)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool EqualsIgnoreCase(this scoped in ReadOnlySpan<char> left, scoped in ReadOnlySpan<char> right) =>
        left.Equals(right, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc cref="Enumerable.Select{T, TResult}(IEnumerable{T}, Func{T, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IMemoryOwner<T> Select<T>(
        this IMemoryOwner<T> source,
        [InstantHandle, RequireStaticDelegate] Func<T, T> selector
    )
    {
        _ = Select(source.Memory.Span, selector);
        return source;
    }

    /// <inheritdoc cref="Enumerable.Select{T, TResult}(IEnumerable{T}, Func{T, int, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IMemoryOwner<T> Select<T>(
        this IMemoryOwner<T> source,
        [InstantHandle, RequireStaticDelegate] Func<T, int, T> selector
    )
    {
        _ = Select(source.Memory.Span, selector);
        return source;
    }

    /// <inheritdoc cref="Enumerable.Select{T, TResult}(IEnumerable{T}, Func{T, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> Select<T>(
        this in Memory<T> source,
        [InstantHandle, RequireStaticDelegate] Func<T, T> selector
    )
    {
        _ = Select(source.Span, selector);
        return source;
    }

    /// <inheritdoc cref="Enumerable.Select{T, TResult}(IEnumerable{T}, Func{T, int, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> Select<T>(
        this in Memory<T> source,
        [InstantHandle, RequireStaticDelegate] Func<T, int, T> selector
    )
    {
        _ = Select(source.Span, selector);
        return source;
    }
#endif

    /// <inheritdoc cref="Enumerable.Select{T, TResult}(IEnumerable{T}, Func{T, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Select<T>(
        this scoped in Span<T> source,
        [InstantHandle, RequireStaticDelegate] Func<T, T> selector
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        ref var start = ref MemoryMarshal.GetReference(source);

        for (ref var end = ref Unsafe.Add(ref start, source.Length);
            Unsafe.IsAddressLessThan(ref start, ref end);
            start = Unsafe.Add(ref start, 1))
            start = selector(start);

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
        ref var start = ref MemoryMarshal.GetReference(source);
        ref var end = ref Unsafe.Add(ref start, source.Length);

        for (var i = 0; Unsafe.IsAddressLessThan(ref start, ref end); i++, start = ref Unsafe.Add(ref start, 1))
            start = selector(start, i);

        return source;
    }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
#if NET6_0_OR_GREATER
    /// <inheritdoc cref="System.MemoryExtensions.SequenceEqual{T}(Span{T}, ReadOnlySpan{T}, IEqualityComparer{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool SequenceEqual<T>(
        this in Memory<T> span,
        in ReadOnlyMemory<T> other,
        IEqualityComparer<T>? comparer = null
    ) =>
        span.Span.SequenceEqual(other.Span, comparer);

    /// <inheritdoc cref="System.MemoryExtensions.SequenceEqual{T}(Span{T}, ReadOnlySpan{T}, IEqualityComparer{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool SequenceEqual<T>(
        this in ReadOnlyMemory<T> span,
        in ReadOnlyMemory<T> other,
        IEqualityComparer<T>? comparer = null
    ) =>
        span.Span.SequenceEqual(other.Span, comparer);
#else
    /// <inheritdoc cref="System.MemoryExtensions.SequenceEqual{T}(Span{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool SequenceEqual<T>(this in Memory<T> span, in ReadOnlyMemory<T> other)
        where T : IEquatable<T>? =>
        span.Span.SequenceEqual(other.Span);

    /// <inheritdoc cref="System.MemoryExtensions.SequenceEqual{T}(Span{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool SequenceEqual<T>(this in ReadOnlyMemory<T> span, in ReadOnlyMemory<T> other)
        where T : IEquatable<T>? =>
        span.Span.SequenceEqual(other.Span);
#endif

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
        this in Memory<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    )
    {
        var span = source.Span;
        ref var start = ref MemoryMarshal.GetReference(span);

        for (ref var end = ref Unsafe.Add(ref start, span.Length);
            Unsafe.IsAddressLessThan(ref start, ref end);
            start = ref Unsafe.Add(ref start, 1))
            if (!predicate(start))
                return source[span.Distance(start)..];

        return source;
    }
#endif

    /// <inheritdoc cref="Enumerable.SkipWhile{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> SkipWhile<T>(
        this scoped in Span<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        ref var start = ref MemoryMarshal.GetReference(source);

        for (ref var end = ref Unsafe.Add(ref start, source.Length);
            Unsafe.IsAddressLessThan(ref start, ref end);
            start = ref Unsafe.Add(ref start, 1))
            if (!predicate(start))
                return MemoryMarshal.CreateSpan(ref start, source.Distance(start));

        return source;
    }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Enumerable.SkipWhile{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyMemory<T> SkipWhile<T>(
        this in ReadOnlyMemory<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    )
    {
        var span = source.Span;
        ref var start = ref MemoryMarshal.GetReference(span);

        for (ref var end = ref Unsafe.Add(ref start, span.Length);
            Unsafe.IsAddressLessThan(ref start, ref end);
            start = ref Unsafe.Add(ref start, 1))
            if (!predicate(start))
                return source[span.Distance(start)..];

        return source;
    }
#endif

    /// <inheritdoc cref="Enumerable.SkipWhile{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> SkipWhile<T>(
        this scoped in ReadOnlySpan<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        ref var start = ref MemoryMarshal.GetReference(source);

        for (ref var end = ref Unsafe.Add(ref start, source.Length);
            Unsafe.IsAddressLessThan(ref start, ref end);
            start = ref Unsafe.Add(ref start, 1))
            if (!predicate(start))
                return MemoryMarshal.CreateReadOnlySpan(ref start, source.Distance(start));

        return source;
    }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
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
        this in Memory<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    )
    {
        var span = source.Span;
        ref var start = ref MemoryMarshal.GetReference(span);

        for (ref var end = ref Unsafe.Add(ref start, span.Length);
            Unsafe.IsAddressLessThan(ref start, ref end);
            start = ref Unsafe.Add(ref start, 1))
            if (!predicate(start))
                return source[..span.Distance(start)];

        return source;
    }
#endif

    /// <inheritdoc cref="Enumerable.TakeWhile{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> TakeWhile<T>(
        this scoped in Span<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        ref var start = ref MemoryMarshal.GetReference(source);

        for (ref var end = ref Unsafe.Add(ref start, source.Length);
            Unsafe.IsAddressLessThan(ref start, ref end);
            start = ref Unsafe.Add(ref start, 1))
            if (!predicate(start))
                return MemoryMarshal.CreateSpan(ref MemoryMarshal.GetReference(source), source.Distance(start));

        return source;
    }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Enumerable.TakeWhile{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyMemory<T> TakeWhile<T>(
        this in ReadOnlyMemory<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    )
    {
        var span = source.Span;
        ref var start = ref MemoryMarshal.GetReference(span);

        for (ref var end = ref Unsafe.Add(ref start, span.Length);
            Unsafe.IsAddressLessThan(ref start, ref end);
            start = ref Unsafe.Add(ref start, 1))
            if (!predicate(start))
                return source[..span.Distance(start)];

        return source;
    }
#endif

    /// <inheritdoc cref="Enumerable.TakeWhile{T}(IEnumerable{T}, Func{T, bool})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> TakeWhile<T>(
        this scoped in ReadOnlySpan<T> source,
        [InstantHandle, RequireStaticDelegate] Predicate<T> predicate
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        ref var start = ref MemoryMarshal.GetReference(source);

        for (ref var end = ref Unsafe.Add(ref start, source.Length);
            Unsafe.IsAddressLessThan(ref start, ref end);
            start = ref Unsafe.Add(ref start, 1))
            if (!predicate(start))
                return MemoryMarshal.CreateReadOnlySpan(ref MemoryMarshal.GetReference(source), source.Distance(start));

        return source;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static int Distance<T>(this scoped in ReadOnlySpan<T> source, in T start) =>
        source.Length - (int)Unsafe.ByteOffset(ref MemoryMarshal.GetReference(source), start) / Unsafe.SizeOf<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static int Distance<T>(this scoped in Span<T> source, in T start) =>
        source.Length - (int)Unsafe.ByteOffset(ref MemoryMarshal.GetReference(source), start) / Unsafe.SizeOf<T>();
}
