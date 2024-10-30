// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
using static Span;

/// <inheritdoc cref="SpanSimdQueries"/>
// ReSharper disable NullableWarningSuppressionIsUsed RedundantNameQualifier RedundantSuppressNullableWarningExpression UseSymbolAlias
static partial class SpanSimdQueries
{
    /// <inheritdoc cref="Enumerable.Max{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T>(this IMemoryOwner<T> enumerable)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, SMax>(enumerable.Memory.Span);

    /// <inheritdoc cref="Enumerable.Max{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T>(this Memory<T> enumerable)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, SMax>(enumerable.Span);

    /// <inheritdoc cref="Enumerable.Max{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T>(this scoped Span<T> enumerable)
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, SMax>(enumerable);

    /// <inheritdoc cref="Enumerable.Max{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T>(this ReadOnlyMemory<T> enumerable)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, SMax>(enumerable.Span);

    /// <inheritdoc cref="Enumerable.Max{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T>(this scoped ReadOnlySpan<T> enumerable)
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, SMax>(enumerable);

    /// <inheritdoc cref="Enumerable.Min{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(this IMemoryOwner<T> enumerable)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, SMin>(enumerable.Memory.Span);

    /// <inheritdoc cref="Enumerable.Min{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(this Memory<T> enumerable)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, SMin>(enumerable.Span);

    /// <inheritdoc cref="Enumerable.Min{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(this scoped Span<T> enumerable)
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, SMin>(enumerable);

    /// <inheritdoc cref="Enumerable.Min{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(this ReadOnlyMemory<T> enumerable)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, SMin>(enumerable.Span);

    /// <inheritdoc cref="Enumerable.Min{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(this scoped ReadOnlySpan<T> enumerable)
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, SMin>(enumerable);

#if NET6_0_OR_GREATER
    /// <inheritdoc cref="Enumerable.MaxBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
#else
    /// <inheritdoc cref="EnumerableMinMax.MaxBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T, TResult>(
        this IMemoryOwner<T> enumerable,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> keySelector
    )
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, TResult, SMax>(enumerable.Memory.Span, keySelector);
#if NET6_0_OR_GREATER
    /// <inheritdoc cref="Enumerable.MaxBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
#else
    /// <inheritdoc cref="EnumerableMinMax.MaxBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T, TResult>(
        this Memory<T> enumerable,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> keySelector
    )
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, TResult, SMax>(enumerable.Span, keySelector);

#if NET6_0_OR_GREATER
    /// <inheritdoc cref="Enumerable.MaxBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
#else
    /// <inheritdoc cref="EnumerableMinMax.MaxBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T, TResult>(
        this scoped Span<T> enumerable,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> keySelector
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, TResult, SMax>(enumerable, keySelector);
#if NET6_0_OR_GREATER
    /// <inheritdoc cref="Enumerable.MaxBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
#else
    /// <inheritdoc cref="EnumerableMinMax.MaxBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T, TResult>(
        this ReadOnlyMemory<T> enumerable,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> keySelector
    )
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, TResult, SMax>(enumerable.Span, keySelector);
#if NET6_0_OR_GREATER
    /// <inheritdoc cref="Enumerable.MaxBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
#else
    /// <inheritdoc cref="EnumerableMinMax.MaxBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T, TResult>(
        this scoped ReadOnlySpan<T> enumerable,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> keySelector
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, TResult, SMax>(enumerable, keySelector);
#if NET6_0_OR_GREATER
    /// <inheritdoc cref="Enumerable.MinBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
#else
    /// <inheritdoc cref="EnumerableMinMax.MinBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T, TResult>(
        this IMemoryOwner<T> enumerable,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> keySelector
    )
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, TResult, SMin>(enumerable.Memory.Span, keySelector);
#if NET6_0_OR_GREATER
    /// <inheritdoc cref="Enumerable.MinBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
#else
    /// <inheritdoc cref="EnumerableMinMax.MinBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T, TResult>(
        this Memory<T> enumerable,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> keySelector
    )
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, TResult, SMin>(enumerable.Span, keySelector);
#if NET6_0_OR_GREATER
    /// <inheritdoc cref="Enumerable.MinBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
#else
    /// <inheritdoc cref="EnumerableMinMax.MinBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T, TResult>(
        this scoped Span<T> enumerable,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> keySelector
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, TResult, SMin>(enumerable, keySelector);
#if NET6_0_OR_GREATER
    /// <inheritdoc cref="Enumerable.MinBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
#else
    /// <inheritdoc cref="EnumerableMinMax.MinBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T, TResult>(
        this ReadOnlyMemory<T> enumerable,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> keySelector
    )
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, TResult, SMin>(enumerable.Span, keySelector);
#if NET6_0_OR_GREATER
    /// <inheritdoc cref="Enumerable.MinBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
#else
    /// <inheritdoc cref="EnumerableMinMax.MinBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T, TResult>(
        this scoped ReadOnlySpan<T> enumerable,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> keySelector
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, TResult, SMin>(enumerable, keySelector);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool Compare<T, TS>(T l, T r) =>
        0 switch
        {
            _ when typeof(TS) == typeof(SMax) && typeof(T) == typeof(byte) => (byte)(object)l! > (byte)(object)r!,
            _ when typeof(TS) == typeof(SMin) && typeof(T) == typeof(byte) => (byte)(object)l! < (byte)(object)r!,
            _ when typeof(TS) == typeof(SMax) && typeof(T) == typeof(double) => (double)(object)l! > (double)(object)r!,
            _ when typeof(TS) == typeof(SMin) && typeof(T) == typeof(double) => (double)(object)l! < (double)(object)r!,
            _ when typeof(TS) == typeof(SMax) && typeof(T) == typeof(float) => (float)(object)l! > (float)(object)r!,
            _ when typeof(TS) == typeof(SMin) && typeof(T) == typeof(float) => (float)(object)l! < (float)(object)r!,
            _ when typeof(TS) == typeof(SMax) && typeof(T) == typeof(int) => (int)(object)l! > (int)(object)r!,
            _ when typeof(TS) == typeof(SMin) && typeof(T) == typeof(int) => (int)(object)l! < (int)(object)r!,
            _ when typeof(TS) == typeof(SMax) && typeof(T) == typeof(nint) => (nint)(object)l! > (nint)(object)r!,
            _ when typeof(TS) == typeof(SMin) && typeof(T) == typeof(nint) => (nint)(object)l! < (nint)(object)r!,
            _ when typeof(TS) == typeof(SMax) && typeof(T) == typeof(nuint) => (nuint)(object)l! > (nuint)(object)r!,
            _ when typeof(TS) == typeof(SMin) && typeof(T) == typeof(nuint) => (nuint)(object)l! < (nuint)(object)r!,
            _ when typeof(TS) == typeof(SMax) && typeof(T) == typeof(sbyte) => (sbyte)(object)l! > (sbyte)(object)r!,
            _ when typeof(TS) == typeof(SMin) && typeof(T) == typeof(sbyte) => (sbyte)(object)l! < (sbyte)(object)r!,
            _ when typeof(TS) == typeof(SMax) && typeof(T) == typeof(short) => (short)(object)l! > (short)(object)r!,
            _ when typeof(TS) == typeof(SMin) && typeof(T) == typeof(short) => (short)(object)l! < (short)(object)r!,
            _ when typeof(TS) == typeof(SMax) && typeof(T) == typeof(uint) => (uint)(object)l! > (uint)(object)r!,
            _ when typeof(TS) == typeof(SMin) && typeof(T) == typeof(uint) => (uint)(object)l! < (uint)(object)r!,
            _ when typeof(TS) == typeof(SMax) && typeof(T) == typeof(ulong) => (ulong)(object)l! > (ulong)(object)r!,
            _ when typeof(TS) == typeof(SMin) && typeof(T) == typeof(ulong) => (ulong)(object)l! < (ulong)(object)r!,
            _ when typeof(TS) == typeof(SMax) && typeof(T) == typeof(ushort) => (ushort)(object)l! > (ushort)(object)r!,
            _ when typeof(TS) == typeof(SMin) && typeof(T) == typeof(ushort) => (ushort)(object)l! < (ushort)(object)r!,
            _ when typeof(TS) == typeof(SMax) => Comparer<T>.Default.Compare(l, r) > 0,
            _ when typeof(TS) == typeof(SMin) => Comparer<T>.Default.Compare(l, r) < 0,
            _ => throw Unreachable,
        };

    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static System.Numerics.Vector<T> LoadUnsafe<T>(scoped in T source)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
#if CSHARPREPL
            System.Numerics.Vector.LoadUnsafe(ref AsRef(source));
#elif NET8_0_OR_GREATER
            System.Numerics.Vector.LoadUnsafe(source);
#else
            Unsafe.ReadUnaligned<System.Numerics.Vector<T>>(ref Unsafe.As<T, byte>(ref AsRef(source)));
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] // ReSharper disable once CognitiveComplexity
    static T MinMax<T, TS>(this scoped ReadOnlySpan<T> span)
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
    {
        // ReSharper disable once TooWideLocalVariableScope
        T value;

        if (span.IsEmpty)
            return default!;

        if (!IsNumericPrimitive<T>() ||
#if NET7_0_OR_GREATER
            !System.Numerics.Vector<T>.IsSupported ||
#endif
            !System.Numerics.Vector.IsHardwareAccelerated ||
            span.Length < System.Numerics.Vector<T>.Count
        )
        {
            value = span.UnsafelyIndex(0);

            for (var i = 1; i < span.Length; i++)
                if (span.UnsafelyIndex(i) is var next && Compare<T, TS>(next, value))
                    value = next;

            return value;
        }

        ref var current = ref MemoryMarshal.GetReference(span);
        ref var lastVectorStart = ref Unsafe.Add(ref current, span.Length - System.Numerics.Vector<T>.Count);
        var best = LoadUnsafe(current);
        current = ref Unsafe.Add(ref current, System.Numerics.Vector<T>.Count)!;

        for (;
            Unsafe.IsAddressLessThan(ref current, ref lastVectorStart);
            current = ref Unsafe.Add(ref current, System.Numerics.Vector<T>.Count)!)
            best = 0 switch
            {
                _ when typeof(TS) == typeof(SMax) => System.Numerics.Vector.Max(best, LoadUnsafe(current)),
                _ when typeof(TS) == typeof(SMin) => System.Numerics.Vector.Min(best, LoadUnsafe(current)),
                _ => throw Unreachable,
            };

        best = 0 switch
        {
            _ when typeof(TS) == typeof(SMax) => System.Numerics.Vector.Max(best, LoadUnsafe(lastVectorStart)),
            _ when typeof(TS) == typeof(SMin) => System.Numerics.Vector.Min(best, LoadUnsafe(lastVectorStart)),
            _ => throw Unreachable,
        };

        value = best[0];

        for (var i = 1; i < System.Numerics.Vector<T>.Count; i++)
            if (0 switch
            {
                _ when typeof(TS) == typeof(SMax) => Compare<T, TS>(best[i], value),
                _ when typeof(TS) == typeof(SMin) => Compare<T, TS>(best[i], value),
                _ => throw Unreachable,
            })
                value = best[i];

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    static T MinMax<T, TResult, TS>(
        this scoped ReadOnlySpan<T> enumerable,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> converter
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
    {
        if (enumerable.IsEmpty)
            return default!;

        ref var bestValue = ref MemoryMarshal.GetReference(enumerable);
        ref var current = ref Unsafe.Add(ref bestValue, 1);
        ref var last = ref Unsafe.Add(ref bestValue, enumerable.Length);
        var bestKey = converter(bestValue);

        for (; Unsafe.IsAddressLessThan(ref current, ref last); current = ref Unsafe.Add(ref current, 1)!)
            if (converter(current) is var next &&
                0 switch
                {
                    _ when typeof(TS) == typeof(SMax) => Compare<TResult, TS>(next, bestKey),
                    _ when typeof(TS) == typeof(SMin) => Compare<TResult, TS>(next, bestKey),
                    _ => throw Unreachable,
                })
            {
                bestKey = next;
                bestValue = ref current;
            }

        return bestValue;
    }

    struct SMin;

    struct SMax;
}
#endif
