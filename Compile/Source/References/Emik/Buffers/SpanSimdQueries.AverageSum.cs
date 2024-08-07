// SPDX-License-Identifier: MPL-2.0
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

// ReSharper disable once RedundantUsingDirective
using static OperatorCaching;
using static Span;

/// <inheritdoc cref="SpanSimdQueries"/>
// ReSharper disable InvocationIsSkipped NullableWarningSuppressionIsUsed RedundantNameQualifier RedundantSuppressNullableWarningExpression UseSymbolAlias
static partial class SpanSimdQueries
{
    /// <inheritdoc cref="Average{T}(ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Average<T>(this scoped Span<T> span)
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            span.ReadOnly().Average();
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Average{T}(ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Average<T>(this ReadOnlyMemory<T> span)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            span.Span.Average();
#endif

    /// <summary>Gets the average.</summary>
    /// <typeparam name="T">The type of <see cref="Span{T}"/>.</typeparam>
    /// <param name="span">The span to get the average of.</param>
    /// <returns>The average of <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Average<T>(this scoped ReadOnlySpan<T> span)
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            Divider(span.Sum(), span.Length);

    /// <inheritdoc cref="Sum{T}(ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Sum<T>(this scoped Span<T> span)
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            span.ReadOnly().Sum();
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Sum{T}(ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Sum<T>(this ReadOnlyMemory<T> span)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            span.Span.Sum();
#endif

    /// <summary>Gets the sum.</summary>
    /// <typeparam name="T">The type of <see cref="Span{T}"/>.</typeparam>
    /// <param name="span">The span to get the sum of.</param>
    /// <returns>The sum of <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Sum<T>(this scoped ReadOnlySpan<T> span)
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP_3_0_OR_GREATER || NET5_0_OR_GREATER
        if (IsNumericPrimitive<T>() &&
#if NET7_0_OR_GREATER
            System.Numerics.Vector<T>.IsSupported &&
#endif
            Vector.IsHardwareAccelerated &&
            System.Numerics.Vector<T>.Count > 2 &&
            span.Length >= System.Numerics.Vector<T>.Count * 4)
            return SumVectorized(span);
#endif
        if (typeof(T).IsEnum)
            return UnderlyingSum(span);

        T sum = default!;

        foreach (var value in span)
            checked
            {
                sum = Adder(sum, value);
            }

        return sum;
    }

    /// <inheritdoc cref="Average{T, TResult}(ReadOnlySpan{T}, Converter{T, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Average<T, TResult>(
        this scoped Span<T> span,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> converter
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
#if !NET8_0_OR_GREATER
        where TResult : struct
#endif
        =>
            Average(span.ReadOnly(), converter);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Average{T, TResult}(ReadOnlySpan{T}, Converter{T, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Average<T, TResult>(
        this ReadOnlyMemory<T> span,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> converter
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
#if !NET8_0_OR_GREATER
        where TResult : struct
#endif
        =>
            Average(span.Span, converter);
#endif

    /// <summary>Gets the average.</summary>
    /// <typeparam name="T">The type of <see cref="Span{T}"/>.</typeparam>
    /// <typeparam name="TResult">The type of return.</typeparam>
    /// <param name="span">The span to get the average of.</param>
    /// <param name="converter">The mapping of each element.</param>
    /// <returns>The average of each mapping of <paramref name="span"/> by <paramref name="converter"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Average<T, TResult>(
        this scoped ReadOnlySpan<T> span,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> converter
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
#if !NET8_0_OR_GREATER
        where TResult : struct
#endif
        =>
            Divider(span.Sum(converter), span.Length);

    /// <inheritdoc cref="Sum{T, TResult}(ReadOnlySpan{T}, Converter{T, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Sum<T, TResult>(
        this scoped Span<T> span,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> converter
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
#if !NET8_0_OR_GREATER
        where TResult : struct
#endif
        =>
            span.ReadOnly().Sum(converter);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Sum{T, TResult}(ReadOnlySpan{T}, Converter{T, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Sum<T, TResult>(
        this ReadOnlyMemory<T> span,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> converter
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
#if !NET8_0_OR_GREATER
        where TResult : struct
#endif
        =>
            span.Span.Sum(converter);
#endif

    /// <summary>Gets the sum.</summary>
    /// <typeparam name="T">The type of <see cref="Span{T}"/>.</typeparam>
    /// <typeparam name="TResult">The type of return.</typeparam>
    /// <param name="span">The span to get the sum of.</param>
    /// <param name="converter">The mapping of each element.</param>
    /// <returns>The sum of each mapping of <paramref name="span"/> by <paramref name="converter"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Sum<T, TResult>(
        this scoped ReadOnlySpan<T> span,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> converter
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
#if !NET8_0_OR_GREATER
        where TResult : struct
#endif
    {
        TResult sum = default!;

        foreach (var x in span)
            sum = Adder(sum, converter(x));

        return sum;
    }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Average{T}(ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Average<T>(this IMemoryOwner<T> span)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            span.Memory.Span.ReadOnly().Average();

    /// <inheritdoc cref="Average{T}(ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Average<T>(this Memory<T> span)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            span.Span.ReadOnly().Average();
#endif

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Sum{T}(ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Sum<T>(this IMemoryOwner<T> span)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            span.Memory.Span.ReadOnly().Sum();

    /// <inheritdoc cref="Sum{T}(ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Sum<T>(this Memory<T> span)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            span.Span.ReadOnly().Sum();
#endif
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Average{T, TResult}(ReadOnlySpan{T}, Converter{T, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Average<T, TResult>(
        this IMemoryOwner<T> span,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> converter
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
#if !NET8_0_OR_GREATER
        where TResult : struct
#endif
        =>
            span.Memory.Span.ReadOnly().Average(converter);

    /// <inheritdoc cref="Average{T, TResult}(ReadOnlySpan{T}, Converter{T, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Average<T, TResult>(
        this Memory<T> span,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> converter
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
#if !NET8_0_OR_GREATER
        where TResult : struct
#endif
        =>
            span.Span.ReadOnly().Average(converter);
#endif
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || ROSLYN
    /// <inheritdoc cref="Sum{T, TResult}(ReadOnlySpan{T}, Converter{T, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Sum<T, TResult>(
        this IMemoryOwner<T> span,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> converter
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
#if !NET8_0_OR_GREATER
        where TResult : struct
#endif
        =>
            span.Memory.Span.Sum(converter);

    /// <inheritdoc cref="Sum{T, TResult}(ReadOnlySpan{T}, Converter{T, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Sum<T, TResult>(
        this Memory<T> span,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> converter
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
#if !NET8_0_OR_GREATER
        where TResult : struct
#endif
        =>
            span.Span.ReadOnly().Sum(converter);
#endif
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP_3_0_OR_GREATER || NET5_0_OR_GREATER
    [CLSCompliant(false), Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    static System.Numerics.Vector<T> LoadUnsafe<T>(ref T source, nuint elementOffset)
#if NET8_0_OR_GREATER
        =>
            Vector.LoadUnsafe(ref source, elementOffset);
#else
        where T : struct
    {
        source = ref Unsafe.Add(ref source, (nint)elementOffset);
        return Unsafe.ReadUnaligned<Vector<T>>(ref Unsafe.As<T, byte>(ref source));
    }
#endif
#endif
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP_3_0_OR_GREATER || NET5_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
#pragma warning disable MA0051
    static T SumVectorized<T>(scoped ReadOnlySpan<T> span)
#pragma warning restore MA0051
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
    {
        ref var ptr = ref MemoryMarshal.GetReference(span);
        var length = (nuint)span.Length;
        var accumulator = System.Numerics.Vector<T>.Zero;

        System.Numerics.Vector<T> overflowTestVector = new(MinValue<T>());

        nuint index = 0;
        var limit = length - (nuint)System.Numerics.Vector<T>.Count * 4;

        do
        {
            var data = LoadUnsafe(ref ptr, index);
            var accumulator2 = accumulator + data;
            var overflowTracking = (accumulator2 ^ accumulator) & (accumulator2 ^ data);

            data = LoadUnsafe(ref ptr, index + (nuint)System.Numerics.Vector<T>.Count);
            accumulator = accumulator2 + data;
            overflowTracking |= (accumulator ^ accumulator2) & (accumulator ^ data);

            data = LoadUnsafe(ref ptr, index + (nuint)System.Numerics.Vector<T>.Count * 2);
            accumulator2 = accumulator + data;
            overflowTracking |= (accumulator2 ^ accumulator) & (accumulator2 ^ data);

            data = LoadUnsafe(ref ptr, index + (nuint)System.Numerics.Vector<T>.Count * 3);
            accumulator = accumulator2 + data;
            overflowTracking |= (accumulator ^ accumulator2) & (accumulator ^ data);

            if ((overflowTracking & overflowTestVector) != System.Numerics.Vector<T>.Zero)
                throw new OverflowException();

            index += (nuint)System.Numerics.Vector<T>.Count * 4;
        } while (index < limit);

        limit = length - (nuint)System.Numerics.Vector<T>.Count;

        if (index < limit)
        {
            var overflowTracking = System.Numerics.Vector<T>.Zero;

            do
            {
                var data = LoadUnsafe(ref ptr, index);
                var accumulator2 = accumulator + data;
                overflowTracking |= (accumulator2 ^ accumulator) & (accumulator2 ^ data);
                accumulator = accumulator2;

                index += (nuint)System.Numerics.Vector<T>.Count;
            } while (index < limit);

            if ((overflowTracking & overflowTestVector) != System.Numerics.Vector<T>.Zero)
                throw new OverflowException();
        }

        T result = default!;

        for (var i = 0; i < System.Numerics.Vector<T>.Count; i++)
            checked
            {
                result = Adder(result, accumulator[i]);
            }

        while (index < length)
        {
            checked
            {
                result = Adder(result, Unsafe.Add(ref ptr, index));
            }

            index++;
        }

        return result;
    }
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static T UnderlyingSum<T>(scoped ReadOnlySpan<T> span) =>
        typeof(T).GetEnumUnderlyingType() switch
        {
            var x when x == typeof(sbyte) => (T)(object)To<sbyte>.From(span).Sum(),
            var x when x == typeof(byte) => (T)(object)To<byte>.From(span).Sum(),
            var x when x == typeof(short) => (T)(object)To<short>.From(span).Sum(),
            var x when x == typeof(ushort) => (T)(object)To<ushort>.From(span).Sum(),
            var x when x == typeof(int) => (T)(object)To<int>.From(span).Sum(),
            var x when x == typeof(uint) => (T)(object)To<uint>.From(span).Sum(),
            var x when x == typeof(long) => (T)(object)To<long>.From(span).Sum(),
            var x when x == typeof(ulong) => (T)(object)To<ulong>.From(span).Sum(),
            var x when x == typeof(nint) => (T)(object)To<nint>.From(span).Sum(),
            var x when x == typeof(nuint) => (T)(object)To<nuint>.From(span).Sum(),
            _ => throw Unreachable,
        };
}
#endif
