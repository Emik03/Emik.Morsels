// SPDX-License-Identifier: MPL-2.0
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;

// ReSharper disable once RedundantUsingDirective
using static OperatorCaching;
using static SpanQueries;

/// <inheritdoc cref="SpanSimdQueries"/>
// ReSharper disable NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
#pragma warning disable MA0048
static partial class SpanSimdQueries
#pragma warning restore MA0048
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
            Average((ReadOnlySpan<T>)span);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Average{T}(ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Average<T>(this ReadOnlyMemory<T> span)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            Average(span.Span);
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
            Sum((ReadOnlySpan<T>)span);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Sum{T}(ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Sum<T>(this ReadOnlyMemory<T> span)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            Sum(span.Span);
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
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
        if (IsNumericPrimitive<T>() &&
            Vector<T>.IsSupported &&
            Vector.IsHardwareAccelerated &&
            Vector<T>.Count > 2 &&
            span.Length >= Vector<T>.Count * 4)
            return SumVectorized(span);
#endif
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
            Average((ReadOnlySpan<T>)span, converter);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
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
            Sum((ReadOnlySpan<T>)span, converter);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
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
            Sum(span.Span, converter);
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

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Average{T}(ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Average<T>(this IMemoryOwner<T> span)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            Average((ReadOnlySpan<T>)span.Memory.Span);

    /// <inheritdoc cref="Average{T}(ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Average<T>(this Memory<T> span)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            Average((ReadOnlySpan<T>)span.Span);
#endif

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Sum{T}(ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Sum<T>(this IMemoryOwner<T> span)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            Sum((ReadOnlySpan<T>)span.Memory.Span);

    /// <inheritdoc cref="Sum{T}(ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Sum<T>(this Memory<T> span)
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            Sum((ReadOnlySpan<T>)span.Span);
#endif
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
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
            Average((ReadOnlySpan<T>)span.Memory.Span, converter);

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
            Average((ReadOnlySpan<T>)span.Span, converter);
#endif
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
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
            Sum((ReadOnlySpan<T>)span.Memory.Span, converter);

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
            Sum((ReadOnlySpan<T>)span.Span, converter);
#endif
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector<T> LoadUnsafe<T>(ref T source, nuint elementOffset)
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        var accumulator = Vector<T>.Zero;

        Vector<T> overflowTestVector = new(MinValue<T>());

        nuint index = 0;
        var limit = length - (nuint)Vector<T>.Count * 4;

        do
        {
            var data = LoadUnsafe(ref ptr, index);
            var accumulator2 = accumulator + data;
            var overflowTracking = (accumulator2 ^ accumulator) & (accumulator2 ^ data);

            data = LoadUnsafe(ref ptr, index + (nuint)Vector<T>.Count);
            accumulator = accumulator2 + data;
            overflowTracking |= (accumulator ^ accumulator2) & (accumulator ^ data);

            data = LoadUnsafe(ref ptr, index + (nuint)Vector<T>.Count * 2);
            accumulator2 = accumulator + data;
            overflowTracking |= (accumulator2 ^ accumulator) & (accumulator2 ^ data);

            data = LoadUnsafe(ref ptr, index + (nuint)Vector<T>.Count * 3);
            accumulator = accumulator2 + data;
            overflowTracking |= (accumulator ^ accumulator2) & (accumulator ^ data);

            if ((overflowTracking & overflowTestVector) != Vector<T>.Zero)
                throw new OverflowException();

            index += (nuint)Vector<T>.Count * 4;
        } while (index < limit);

        limit = length - (nuint)Vector<T>.Count;

        if (index < limit)
        {
            var overflowTracking = Vector<T>.Zero;

            do
            {
                var data = LoadUnsafe(ref ptr, index);
                var accumulator2 = accumulator + data;
                overflowTracking |= (accumulator2 ^ accumulator) & (accumulator2 ^ data);
                accumulator = accumulator2;

                index += (nuint)Vector<T>.Count;
            } while (index < limit);

            if ((overflowTracking & overflowTestVector) != Vector<T>.Zero)
                throw new OverflowException();
        }

        T result = default!;

        for (var i = 0; i < Vector<T>.Count; i++)
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
}
#endif
