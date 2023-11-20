// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;
#pragma warning disable 1574, 1580, 1581, 1584, S1199 // ReSharper disable once RedundantUsingDirective
using static SpanQueries;

/// <inheritdoc cref="SpanSimdQueries"/>
// ReSharper disable NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
#pragma warning disable MA0048
static partial class SpanSimdQueries
#pragma warning restore MA0048
{
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
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
#endif

    /// <inheritdoc cref="Enumerable.Max{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T>(this scoped Span<T> enumerable)
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, Maximum>(enumerable);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Enumerable.Max{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T>(this ReadOnlyMemory<T> enumerable)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, Maximum>(enumerable.Span);
#endif

    /// <inheritdoc cref="Enumerable.Max{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T>(this scoped ReadOnlySpan<T> enumerable)
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, Maximum>(enumerable);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
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
#endif

    /// <inheritdoc cref="Enumerable.Min{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(this scoped Span<T> enumerable)
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, Minimum>(enumerable);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Enumerable.Min{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(this ReadOnlyMemory<T> enumerable)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, Minimum>(enumerable.Span);
#endif

    /// <inheritdoc cref="Enumerable.Min{T}(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(this scoped ReadOnlySpan<T> enumerable)
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            MinMax<T, Minimum>(enumerable);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
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
#endif

    /// <inheritdoc cref="Enumerable.MaxBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
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
            MinMax<T, TResult, Maximum>(enumerable, keySelector);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
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
#endif

    /// <inheritdoc cref="Enumerable.MaxBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
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
            MinMax<T, TResult, Maximum>(enumerable, keySelector);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
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
#endif

    /// <inheritdoc cref="Enumerable.MinBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
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
            MinMax<T, TResult, Minimum>(enumerable, keySelector);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
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
#endif

    /// <inheritdoc cref="Enumerable.MinBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
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
            MinMax<T, TResult, Minimum>(enumerable, keySelector);

    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool Compare<T, TMinMax>(T l, T r) =>
        typeof(TMinMax) switch
        {
            var x when x == typeof(Maximum) && typeof(T) == typeof(byte) => (byte)(object)l! > (byte)(object)r!,
            var x when x == typeof(Minimum) && typeof(T) == typeof(byte) => (byte)(object)l! < (byte)(object)r!,
            var x when x == typeof(Maximum) && typeof(T) == typeof(double) => (double)(object)l! > (double)(object)r!,
            var x when x == typeof(Minimum) && typeof(T) == typeof(double) => (double)(object)l! < (double)(object)r!,
            var x when x == typeof(Maximum) && typeof(T) == typeof(float) => (float)(object)l! > (float)(object)r!,
            var x when x == typeof(Minimum) && typeof(T) == typeof(float) => (float)(object)l! < (float)(object)r!,
            var x when x == typeof(Maximum) && typeof(T) == typeof(int) => (int)(object)l! > (int)(object)r!,
            var x when x == typeof(Minimum) && typeof(T) == typeof(int) => (int)(object)l! < (int)(object)r!,
            var x when x == typeof(Maximum) && typeof(T) == typeof(nint) => (nint)(object)l! > (nint)(object)r!,
            var x when x == typeof(Minimum) && typeof(T) == typeof(nint) => (nint)(object)l! < (nint)(object)r!,
            var x when x == typeof(Maximum) && typeof(T) == typeof(nuint) => (nuint)(object)l! > (nuint)(object)r!,
            var x when x == typeof(Minimum) && typeof(T) == typeof(nuint) => (nuint)(object)l! < (nuint)(object)r!,
            var x when x == typeof(Maximum) && typeof(T) == typeof(sbyte) => (sbyte)(object)l! > (sbyte)(object)r!,
            var x when x == typeof(Minimum) && typeof(T) == typeof(sbyte) => (sbyte)(object)l! < (sbyte)(object)r!,
            var x when x == typeof(Maximum) && typeof(T) == typeof(short) => (short)(object)l! > (short)(object)r!,
            var x when x == typeof(Minimum) && typeof(T) == typeof(short) => (short)(object)l! < (short)(object)r!,
            var x when x == typeof(Maximum) && typeof(T) == typeof(uint) => (uint)(object)l! > (uint)(object)r!,
            var x when x == typeof(Minimum) && typeof(T) == typeof(uint) => (uint)(object)l! < (uint)(object)r!,
            var x when x == typeof(Maximum) && typeof(T) == typeof(ulong) => (ulong)(object)l! > (ulong)(object)r!,
            var x when x == typeof(Minimum) && typeof(T) == typeof(ulong) => (ulong)(object)l! < (ulong)(object)r!,
            var x when x == typeof(Maximum) && typeof(T) == typeof(ushort) => (ushort)(object)l! > (ushort)(object)r!,
            var x when x == typeof(Minimum) && typeof(T) == typeof(ushort) => (ushort)(object)l! < (ushort)(object)r!,
            var x when x == typeof(Maximum) => Comparer<T>.Default.Compare(l, r) > 0,
            var x when x == typeof(Minimum) => Comparer<T>.Default.Compare(l, r) < 0,
            _ => throw Unreachable,
        };
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static Vector<T> LoadUnsafe<T>(in T source)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
#if CSHARPREPL
            Vector.LoadUnsafe(ref AsRef(source));
#elif NET8_0_OR_GREATER
            Vector.LoadUnsafe(source);
#else
            Unsafe.ReadUnaligned<Vector<T>>(ref Unsafe.As<T, byte>(ref Unsafe.AsRef(source)));
#endif
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
#pragma warning disable MA0051 // ReSharper disable once CognitiveComplexity
    static T MinMax<T, TMinMax>(this ReadOnlySpan<T> span)
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
#pragma warning restore MA0051
    {
        T value;

        if (span.IsEmpty)
            return default!;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
        if (!Vector<T>.IsSupported || !Vector.IsHardwareAccelerated || span.Length < Vector<T>.Count)
#endif
        {
            value = span[0];

            for (var i = 1; i < span.Length; i++)
                if (Compare<T, TMinMax>(span[i], value))
                    value = span[i];

            return value;
        }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
        ref var current = ref MemoryMarshal.GetReference(span);
        ref var lastVectorStart = ref Unsafe.Add(ref current, span.Length - Vector<T>.Count);
        var best = LoadUnsafe(current);
        current = ref Unsafe.Add(ref current, Vector<T>.Count)!;

        for (;
            Unsafe.IsAddressLessThan(ref current, ref lastVectorStart);
            current = ref Unsafe.Add(ref current, Vector<T>.Count)!)
            best = typeof(TMinMax) switch
            {
                var x when x == typeof(Maximum) => Vector.Max(best, LoadUnsafe(current)),
                var x when x == typeof(Minimum) => Vector.Min(best, LoadUnsafe(current)),
                _ => throw Unreachable,
            };

        best = typeof(TMinMax) switch
        {
            var x when x == typeof(Maximum) => Vector.Max(best, LoadUnsafe(lastVectorStart)),
            var x when x == typeof(Minimum) => Vector.Min(best, LoadUnsafe(lastVectorStart)),
            _ => throw Unreachable,
        };

        value = best[0];

        for (var i = 1; i < Vector<T>.Count; i++)
            if (typeof(TMinMax) switch
            {
                var x when x == typeof(Maximum) => Compare<T, TMinMax>(best[i], value),
                var x when x == typeof(Minimum) => Compare<T, TMinMax>(best[i], value),
                _ => throw Unreachable,
            })
                value = best[i];

        return value;
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    static T MinMax<T, TResult, TMinMax>(
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
        var bestKey = converter(current);

        for (; Unsafe.IsAddressLessThan(ref current, ref last); current = ref Unsafe.Add(ref current, 1)!)
            if (converter(current) is var next &&
                typeof(TMinMax) switch
                {
                    var x when x == typeof(Maximum) => Compare<TResult, TMinMax>(next, bestKey),
                    var x when x == typeof(Minimum) => Compare<TResult, TMinMax>(next, bestKey),
                    _ => throw Unreachable,
                })
            {
                bestKey = next;
                bestValue = ref current;
            }

        return bestValue;
    }

    struct Minimum;

    struct Maximum;
}
