// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;

/// <inheritdoc cref="SpanSimdQueries"/>
// ReSharper disable NullableWarningSuppressionIsUsed
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
#elif !NET8_0_OR_GREATER
        where T : struct
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
#if !NET8_0_OR_GREATER
        where T : struct
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
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            OperatorCaching<TResult>._divider(span.Sum(converter), span.Length);

    /// <inheritdoc cref="Sum{T, TResult}(ReadOnlySpan{T}, Converter{T, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult? Sum<T, TResult>(
        this scoped Span<T> span,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> converter
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            Sum((ReadOnlySpan<T>)span, converter);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Sum{T, TResult}(ReadOnlySpan{T}, Converter{T, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult? Sum<T, TResult>(
        this ReadOnlyMemory<T> span,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> converter
    )
#if !NET8_0_OR_GREATER
        where T : struct
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
    public static TResult? Sum<T, TResult>(
        this scoped ReadOnlySpan<T> span,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> converter
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
    {
        TResult? sum = default;

        foreach (var a in span)
            sum = OperatorCaching<TResult>._adder(sum, converter(a));

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
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            Average((ReadOnlySpan<T>)span.Memory.Span, converter);

    /// <inheritdoc cref="Average{T, TResult}(ReadOnlySpan{T}, Converter{T, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Average<T, TResult>(
        this Memory<T> span,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> converter
    )
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            Average((ReadOnlySpan<T>)span.Span, converter);
#endif
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Sum{T, TResult}(ReadOnlySpan{T}, Converter{T, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult? Sum<T, TResult>(
        this IMemoryOwner<T> span,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> converter
    )
#if !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            Sum((ReadOnlySpan<T>)span.Memory.Span, converter);

    /// <inheritdoc cref="Sum{T, TResult}(ReadOnlySpan{T}, Converter{T, TResult})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult? Sum<T, TResult>(
        this Memory<T> span,
        [InstantHandle, RequireStaticDelegate] Converter<T, TResult> converter
    )
#if !NET8_0_OR_GREATER
        where T : struct
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

    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T Adder<T>(T l, T r)
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            typeof(T) switch
            {
                var x when x == typeof(byte) => (T)(object)((byte)(object)l! + (byte)(object)r!),
                var x when x == typeof(double) => (T)(object)((double)(object)l! + (double)(object)r!),
                var x when x == typeof(float) => (T)(object)((float)(object)l! + (float)(object)r!),
                var x when x == typeof(int) => (T)(object)((int)(object)l! + (int)(object)r!),
                var x when x == typeof(nint) => (T)(object)((nint)(object)l! + (nint)(object)r!),
                var x when x == typeof(nuint) => (T)(object)((nuint)(object)l! + (nuint)(object)r!),
                var x when x == typeof(sbyte) => (T)(object)((sbyte)(object)l! + (sbyte)(object)r!),
                var x when x == typeof(short) => (T)(object)((short)(object)l! + (short)(object)r!),
                var x when x == typeof(uint) => (T)(object)((uint)(object)l! + (uint)(object)r!),
                var x when x == typeof(ulong) => (T)(object)((ulong)(object)l! + (ulong)(object)r!),
                var x when x == typeof(ushort) => (T)(object)((ushort)(object)l! + (ushort)(object)r!),
                _ => OperatorCaching<T>._adder(l, r),
            };

    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T Divider<T>(T left, int right)
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            typeof(T) switch
            {
                var x when x == typeof(byte) => (T)(object)((byte)(object)left! + right),
                var x when x == typeof(double) => (T)(object)((double)(object)left! + right),
                var x when x == typeof(float) => (T)(object)((float)(object)left! + right),
                var x when x == typeof(int) => (T)(object)((int)(object)left! + right),
                var x when x == typeof(nint) => (T)(object)((nint)(object)left! + right),
                var x when x == typeof(nuint) => (T)(object)((nuint)(object)left! + (nuint)right),
                var x when x == typeof(sbyte) => (T)(object)((sbyte)(object)left! + right),
                var x when x == typeof(short) => (T)(object)((short)(object)left! + right),
                var x when x == typeof(uint) => (T)(object)((uint)(object)left! + right),
                var x when x == typeof(ulong) => (T)(object)((ulong)(object)left! + (ulong)right),
                var x when x == typeof(ushort) => (T)(object)((ushort)(object)left! + right),
                _ => OperatorCaching<T>._divider(left, right),
            };

    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T MinValue<T>()
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            typeof(T) switch
            {
                var x when x == typeof(byte) => (T)(object)byte.MinValue,
                var x when x == typeof(double) => (T)(object)double.MinValue,
                var x when x == typeof(float) => (T)(object)float.MinValue,
                var x when x == typeof(int) => (T)(object)int.MinValue,
#if NET5_0_OR_GREATER
                var x when x == typeof(nint) => (T)(object)nint.MinValue,
                var x when x == typeof(nuint) => (T)(object)nuint.MinValue,
#endif
                var x when x == typeof(sbyte) => (T)(object)sbyte.MinValue,
                var x when x == typeof(short) => (T)(object)short.MinValue,
                var x when x == typeof(uint) => (T)(object)uint.MinValue,
                var x when x == typeof(ulong) => (T)(object)ulong.MinValue,
                var x when x == typeof(ushort) => (T)(object)ushort.MinValue,
                _ => OperatorCaching<T>._minValue,
            };
#endif

    static class OperatorCaching<T>
    {
        const BindingFlags Flags = BindingFlags.Public | BindingFlags.Static;

        static readonly Type[] s_args = new[] { typeof(T), typeof(T) };

        internal static readonly T _minValue =
            (T?)typeof(T).GetField("MinValue", Flags)?.GetValue(null) ?? throw Unreachable;

        internal static readonly Func<T?, T?, T> _adder = Operator<T>("op_Addition", Expression.AddChecked);

        internal static readonly Func<T?, int, T> _divider =
            Operator<int>("op_Division", (x, y) => Expression.Divide(x, Expression.Convert(y, typeof(int))));

        static Func<T?, TRight?, T> Operator<TRight>(string name, Func<Expression, Expression, BinaryExpression> go) =>
            typeof(T).GetMethod(name, Flags, s_args) is not { } x &&
            Expression.Parameter(typeof(T), "left") is var left &&
            Expression.Parameter(typeof(TRight), "right") is var right
                ? Expression.Lambda<Func<T?, TRight?, T>>(go(left, right), left, right).Compile()
                : (Func<T?, TRight?, T>)Delegate.CreateDelegate(typeof(Func<T?, TRight?, T>), x);
    }
}
