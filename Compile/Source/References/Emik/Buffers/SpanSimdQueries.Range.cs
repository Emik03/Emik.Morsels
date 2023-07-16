// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;

using static OperatorCaching;

/// <inheritdoc cref="SpanSimdQueries"/>
// ReSharper disable NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
#pragma warning disable MA0048
static partial class SpanSimdQueries
#pragma warning restore MA0048
{
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Range{T}(Span{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Range<T>(this IMemoryOwner<T> source)
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            Range(source.Memory.Span);

    /// <inheritdoc cref="Range{T}(Span{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Range<T>(this Memory<T> source)
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
            Range(source.Span);
#endif

    /// <summary>Creates the range.</summary>
    /// <typeparam name="T">The type of number.</typeparam>
    /// <param name="source">The <see cref="Span{T}"/> to mutate.</param>
    /// <returns>The parameter <paramref name="source"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable MA0051
    public static Span<T> Range<T>(this Span<T> source)
#pragma warning restore MA0051
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
    {
        if (source.IsEmpty)
            return source;

        ref var start = ref MemoryMarshal.GetReference(source);
        ref var end = ref Unsafe.Add(ref start, source.Length);

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
#if NET8_0_OR_GREATER
        if (Vector512<T>.IsSupported && Vector512.IsHardwareAccelerated && source.Length >= Vector512<T>.Count)
        {
            ref var last = ref Unsafe.Add(ref start, source.Length - Vector512<T>.Count);
            Unsafe.As<T, Vector512<T>>(ref start) = Vectorized<T>.Vec512;
            start = ref Unsafe.Add(ref start, Vector512<T>.Count);

            for (;
                Unsafe.IsAddressLessThan(ref start, ref last);
                start = ref Unsafe.Add(ref start, Vector512<T>.Count))
                Unsafe.As<T, Vector512<T>>(ref start) =
                    Unsafe.Add(ref Unsafe.As<T, Vector512<T>>(ref start), -1) +
                    Vector512.Create(Vectorized<T>.Step);
        }
        else
#endif
        if (Vector256<T>.IsSupported && Vector256.IsHardwareAccelerated && source.Length >= Vector256<T>.Count)
        {
            ref var last = ref Unsafe.Add(ref start, source.Length - Vector256<T>.Count);
            Unsafe.As<T, Vector256<T>>(ref start) = Vectorized<T>.Vec256;
            start = ref Unsafe.Add(ref start, Vector256<T>.Count);

            for (;
                Unsafe.IsAddressLessThan(ref start, ref last);
                start = ref Unsafe.Add(ref start, Vector256<T>.Count))
                Unsafe.As<T, Vector256<T>>(ref start) =
                    Unsafe.Add(ref Unsafe.As<T, Vector256<T>>(ref start), -1) +
                    Vector256.Create(Vectorized<T>.Step);
        }
        else if (Vector128<T>.IsSupported && Vector128.IsHardwareAccelerated && source.Length >= Vector128<T>.Count)
        {
            ref var last = ref Unsafe.Add(ref start, source.Length - Vector128<T>.Count);
            Unsafe.As<T, Vector128<T>>(ref start) = Vectorized<T>.Vec128;
            start = ref Unsafe.Add(ref start, Vector128<T>.Count);

            for (;
                Unsafe.IsAddressLessThan(ref start, ref last);
                start = ref Unsafe.Add(ref start, Vector128<T>.Count))
                Unsafe.As<T, Vector128<T>>(ref start) =
                    Unsafe.Add(ref Unsafe.As<T, Vector128<T>>(ref start), -1) +
                    Vector128.Create(Vectorized<T>.Step);
        }
        else if (Vector64<T>.IsSupported && Vector64.IsHardwareAccelerated && source.Length >= Vector64<T>.Count)
        {
            ref var last = ref Unsafe.Add(ref start, source.Length - Vector64<T>.Count);
            Unsafe.As<T, Vector64<T>>(ref start) = Vectorized<T>.Vec64;
            start = ref Unsafe.Add(ref start, Vector64<T>.Count);

            for (;
                Unsafe.IsAddressLessThan(ref start, ref last);
                start = ref Unsafe.Add(ref start, Vector64<T>.Count))
                Unsafe.As<T, Vector64<T>>(ref start) =
                    Unsafe.Add(ref Unsafe.As<T, Vector64<T>>(ref start), -1) +
                    Vector64.Create(Vectorized<T>.Step);
        }
        else
#endif
        {
            start = default!;
            start = ref Unsafe.Add(ref start, 1);
        }

        for (;
            Unsafe.IsAddressLessThan(ref start, ref end);
            start = ref Unsafe.Add(ref start, 1))
        {
            var t = Unsafe.Add(ref start, -1);
            Increment(ref t);
            start = t;
        }

        return source;
    }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    static class Vectorized<T>
#if !NET8_0_OR_GREATER
        where T : struct
#endif
    {
        static readonly int s_size =
#if NET8_0_OR_GREATER
            Vector512<T>.Count + 1;
#else
            Vector256<T>.Count + 1;
#endif

        static readonly T[] s_values = new T[s_size];

        static Vectorized()
        {
            T current = default!;

            for (var i = 0; i < s_values.Length; i++)
            {
                s_values[i] = current;
                Increment(ref current);
            }
        }

        public static Vector64<T> Vec64 => Vector64.Create(s_values);

        public static Vector128<T> Vec128 => Vector128.Create(s_values);

        public static Vector256<T> Vec256 => Vector256.Create(s_values);
#if NET8_0_OR_GREATER
        public static Vector512<T> Vec512 => Vector512.Create(s_values);
#endif
        public static T Step => s_values[^1];
    }
#endif
}
