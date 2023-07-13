// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
/// <inheritdoc cref="SpanSimdQueries"/>
// ReSharper disable NullableWarningSuppressionIsUsed
#pragma warning disable MA0048
static partial class SpanSimdQueries
#pragma warning restore MA0048
{
    /// <inheritdoc cref="Enumerable.Max{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T>(this IMemoryOwner<T> enumerable)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, Maximum>(enumerable.Memory.Span);

    /// <inheritdoc cref="Enumerable.Max{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T>(this Memory<T> enumerable)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, Maximum>(enumerable.Span);

    /// <inheritdoc cref="Enumerable.Max{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T>(this scoped Span<T> enumerable)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, Maximum>(enumerable);

    /// <inheritdoc cref="Enumerable.Max{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T>(this ReadOnlyMemory<T> enumerable)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, Maximum>(enumerable.Span);

    /// <inheritdoc cref="Enumerable.Max{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T>(this scoped ReadOnlySpan<T> enumerable)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, Maximum>(enumerable);

    /// <inheritdoc cref="Enumerable.Min{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(this IMemoryOwner<T> enumerable)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, Minimum>(enumerable.Memory.Span);

    /// <inheritdoc cref="Enumerable.Min{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(this Memory<T> enumerable)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, Minimum>(enumerable.Span);

    /// <inheritdoc cref="Enumerable.Min{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(this scoped Span<T> enumerable)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, Minimum>(enumerable);

    /// <inheritdoc cref="Enumerable.Min{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(this ReadOnlyMemory<T> enumerable)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, Minimum>(enumerable.Span);

    /// <inheritdoc cref="Enumerable.Min{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(this scoped ReadOnlySpan<T> enumerable)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, Minimum>(enumerable);

    /// <inheritdoc cref="Enumerable.MaxBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T, TResult>(
        this IMemoryOwner<T> enumerable,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> keySelector
    )
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, TResult, Maximum>(enumerable.Memory.Span, keySelector);

    /// <inheritdoc cref="Enumerable.MaxBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T, TResult>(
        this Memory<T> enumerable,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> keySelector
    )
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, TResult, Maximum>(enumerable.Span, keySelector);

    /// <inheritdoc cref="Enumerable.MaxBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T, TResult>(
        this scoped Span<T> enumerable,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> keySelector
    )
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, TResult, Maximum>(enumerable, keySelector);

    /// <inheritdoc cref="Enumerable.MaxBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T, TResult>(
        this ReadOnlyMemory<T> enumerable,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> keySelector
    )
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, TResult, Maximum>(enumerable.Span, keySelector);

    /// <inheritdoc cref="Enumerable.MaxBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T, TResult>(
        this scoped ReadOnlySpan<T> enumerable,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> keySelector
    )
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, TResult, Maximum>(enumerable, keySelector);

    /// <inheritdoc cref="Enumerable.MinBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T, TResult>(
        this IMemoryOwner<T> enumerable,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> keySelector
    )
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, TResult, Minimum>(enumerable.Memory.Span, keySelector);

    /// <inheritdoc cref="Enumerable.MinBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T, TResult>(
        this Memory<T> enumerable,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> keySelector
    )
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, TResult, Minimum>(enumerable.Span, keySelector);

    /// <inheritdoc cref="Enumerable.MinBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T, TResult>(
        this scoped Span<T> enumerable,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> keySelector
    )
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, TResult, Minimum>(enumerable, keySelector);

    /// <inheritdoc cref="Enumerable.MinBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T, TResult>(
        this ReadOnlyMemory<T> enumerable,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> keySelector
    )
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, TResult, Minimum>(enumerable.Span, keySelector);

    /// <inheritdoc cref="Enumerable.MinBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T, TResult>(
        this scoped ReadOnlySpan<T> enumerable,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> keySelector
    )
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, TResult, Minimum>(enumerable, keySelector);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsNumericPrimitive<T>() =>
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable MA0051
    static T MinMax<T, TMinMax>(this ReadOnlySpan<T> span)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
#pragma warning restore MA0051
    {
        T value;

        if (span.IsEmpty)
            return default!;

        if (!IsNumericPrimitive<T>() || !Vector128.IsHardwareAccelerated || span.Length < Vector128<T>.Count)
        {
            value = span[0];

            for (var i = 1; i < span.Length; i++)
                if (typeof(TMinMax) switch
                {
                    var x when x == typeof(Maximum) => Comparer<T>.Default.Compare(span[i], value) > 0,
                    var x when x == typeof(Minimum) => Comparer<T>.Default.Compare(span[i], value) < 0,
                    _ => throw Unreachable,
                })
                    value = span[i];
        }
        else if (!Vector256.IsHardwareAccelerated || span.Length < Vector256<T>.Count)
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var lastVectorStart = ref Unsafe.Add(ref current, span.Length - Vector128<T>.Count);

            var best = Vector128.LoadUnsafe(ref current);
            current = ref Unsafe.Add(ref current, Vector128<T>.Count);

            while (Unsafe.IsAddressLessThan(ref current, ref lastVectorStart))
            {
                best = typeof(TMinMax) switch
                {
                    var x when x == typeof(Maximum) => Vector128.Max(best, Vector128.LoadUnsafe(ref current)),
                    var x when x == typeof(Minimum) => Vector128.Min(best, Vector128.LoadUnsafe(ref current)),
                    _ => throw Unreachable,
                };

                current = ref Unsafe.Add(ref current, Vector128<T>.Count);
            }

            best = typeof(TMinMax) switch
            {
                var x when x == typeof(Maximum) => Vector128.Max(best, Vector128.LoadUnsafe(ref lastVectorStart)),
                var x when x == typeof(Minimum) => Vector128.Min(best, Vector128.LoadUnsafe(ref lastVectorStart)),
                _ => throw Unreachable,
            };

            value = best[0];

            for (var i = 1; i < Vector128<T>.Count; i++)
                if (typeof(TMinMax) switch
                {
                    var x when x == typeof(Maximum) => Comparer<T>.Default.Compare(best[i], value) > 0,
                    var x when x == typeof(Minimum) => Comparer<T>.Default.Compare(best[i], value) < 0,
                    _ => throw Unreachable,
                })
                    value = best[i];
        }
        else
        {
            ref var current = ref MemoryMarshal.GetReference(span);
            ref var lastVectorStart = ref Unsafe.Add(ref current, span.Length - Vector256<T>.Count);

            var best = Vector256.LoadUnsafe(ref current);
            current = ref Unsafe.Add(ref current, Vector256<T>.Count);

            while (Unsafe.IsAddressLessThan(ref current, ref lastVectorStart))
            {
                best = typeof(TMinMax) switch
                {
                    var x when x == typeof(Maximum) => Vector256.Max(best, Vector256.LoadUnsafe(ref current)),
                    var x when x == typeof(Minimum) => Vector256.Min(best, Vector256.LoadUnsafe(ref current)),
                    _ => throw Unreachable,
                };

                current = ref Unsafe.Add(ref current, Vector256<T>.Count);
            }

            best = typeof(TMinMax) switch
            {
                var x when x == typeof(Maximum) => Vector256.Max(best, Vector256.LoadUnsafe(ref lastVectorStart)),
                var x when x == typeof(Minimum) => Vector256.Min(best, Vector256.LoadUnsafe(ref lastVectorStart)),
                _ => throw Unreachable,
            };

            value = best[0];

            for (var i = 1; i < Vector256<T>.Count; i++)
                if (typeof(TMinMax) switch
                {
                    var x when x == typeof(Maximum) => Comparer<T>.Default.Compare(best[i], value) > 0,
                    var x when x == typeof(Minimum) => Comparer<T>.Default.Compare(best[i], value) < 0,
                    _ => throw Unreachable,
                })
                    value = best[i];
        }

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T MinMax<T, TResult, TMinMax>(
        this scoped ReadOnlySpan<T> enumerable,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> converter
    )
    {
        if (enumerable.IsEmpty)
            return default!;

        var value = enumerable[0];
        var best = converter(value);

        for (var i = 1; i < enumerable.Length; i++)
            if (converter(enumerable[i]) is var next &&
                typeof(TMinMax) switch
                {
                    var x when x == typeof(Maximum) => Comparer<TResult>.Default.Compare(next, best) > 0,
                    var x when x == typeof(Minimum) => Comparer<TResult>.Default.Compare(next, best) < 0,
                    _ => throw Unreachable,
                })
                (value, best) = (enumerable[i], next);

        return value;
    }

    struct Minimum;

    struct Maximum;
}
#endif
