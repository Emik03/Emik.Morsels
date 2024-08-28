// SPDX-License-Identifier: MPL-2.0
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
// ReSharper disable once CheckNamespace RedundantUsingDirective
namespace Emik.Morsels;

using static OperatorCaching;
using static Span;

/// <summary>Contains extension methods for fast SIMD operations.</summary>
// ReSharper disable NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
static partial class SpanSimdQueries
{
    /// <summary>Determines whether the type is a numeric primitive.</summary>
    /// <typeparam name="T">The type to test.</typeparam>
    /// <returns>Whether the type parameter <typeparamref name="T"/> is a primitive representing a number.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNumericPrimitive<T>()
#if !NO_ALLOWS_REF_STRUCT
        where T : allows ref struct
#endif
        =>
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
    /// <inheritdoc cref="Range{T}(Span{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Range<T>(this IMemoryOwner<T> source) => Range(source.Memory.Span);

    /// <inheritdoc cref="Range{T}(Span{T}, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Range<T>(this IMemoryOwner<T> source, int index) => Range(source.Memory.Span, index);

    /// <inheritdoc cref="Range{T}(Span{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Range<T>(this Memory<T> source) => Range(source.Span);

    /// <inheritdoc cref="Range{T}(Span{T}, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Range<T>(this Memory<T> source, int index) => Range(source.Span, index);
#endif
    /// <summary>Creates the range.</summary>
    /// <typeparam name="T">The type of number.</typeparam>
    /// <param name="source">The <see cref="Span{T}"/> to mutate.</param>
    /// <exception cref="MissingMethodException">The type <typeparamref name="T"/> is unsupported.</exception>
    /// <returns>The parameter <paramref name="source"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Range<T>(this Span<T> source)
    {
        switch (source.Length)
        {
            case 0: return source;
            case 1:
                MemoryMarshal.GetReference(source) = default!;
                return source;
            case var length:
                if (!IsNumericPrimitive<T>() && !IsSupported<T>())
                    _ = Fail<T>();

                SpanRange<T>(length).CopyTo(source);
                return source;
        }
    }

    /// <summary>Creates the range.</summary>
    /// <typeparam name="T">The type of number.</typeparam>
    /// <param name="source">The <see cref="Span{T}"/> to mutate.</param>
    /// <param name="index">The starting index.</param>
    /// <exception cref="MissingMethodException">The type <typeparamref name="T"/> is unsupported.</exception>
    /// <returns>The parameter <paramref name="source"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Range<T>(this Span<T> source, int index)
    {
        if (source.Length is 0)
            return source;

        if (!IsNumericPrimitive<T>() && !IsSupported<T>())
            _ = Fail<T>();

        SpanRange<T>(index + source.Length).UnsafelySkip(index).CopyTo(source);
        return source;
    }

    /// <inheritdoc cref="InAscendingOrder{T}.Memory"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlyMemory<byte> AsMemory(this byte length) => MemoryRange<byte>(length + 1)[length..];

    /// <inheritdoc cref="InAscendingOrder{T}.Memory"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlyMemory<char> AsMemory(this char length) => MemoryRange<char>(length + 1)[length..];

    /// <inheritdoc cref="InAscendingOrder{T}.Memory"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlyMemory<sbyte> AsMemory(this sbyte length) => MemoryRange<sbyte>(length + 1)[length..];

    /// <inheritdoc cref="InAscendingOrder{T}.Memory"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlyMemory<short> AsMemory(this short length) => MemoryRange<short>(length + 1)[length..];

    /// <inheritdoc cref="InAscendingOrder{T}.Memory"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlyMemory<ushort> AsMemory(this ushort length) => MemoryRange<ushort>(length + 1)[length..];

    /// <inheritdoc cref="InAscendingOrder{T}.Span"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<byte> AsSpan(this byte length) => SpanRange<byte>(length + 1).UnsafelySkip(length);

    /// <inheritdoc cref="InAscendingOrder{T}.Span"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<char> AsSpan(this char length) => SpanRange<char>(length + 1).UnsafelySkip(length);

    /// <inheritdoc cref="InAscendingOrder{T}.Span"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<sbyte> AsSpan(this sbyte length) => SpanRange<sbyte>(length + 1).UnsafelySkip(length);

    /// <inheritdoc cref="InAscendingOrder{T}.Span"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<short> AsSpan(this short length) => SpanRange<short>(length + 1).UnsafelySkip(length);

    /// <inheritdoc cref="InAscendingOrder{T}.Span"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<ushort> AsSpan(this ushort length) => SpanRange<ushort>(length + 1).UnsafelySkip(length);
#if NET5_0_OR_GREATER
    /// <inheritdoc cref="InAscendingOrder{T}.Span"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<Half> AsSpan(this Half length)
    {
        var i = (int)length;
        return SpanRange<Half>(i).UnsafelySkip(i);
    }
#endif
    /// <inheritdoc cref="InAscendingOrder{T}.Memory"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlyMemory<T> MemoryRange<T>(this int length) => InAscendingOrder<T>.Memory(length);

    /// <inheritdoc cref="InAscendingOrder{T}.Span"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<T> SpanRange<T>(this int length) => InAscendingOrder<T>.Span(length);

    static class InAscendingOrder<T>
    {
        // Vector512<T> is the largest vector type.
        const int InitialCapacity = 512;

        static T[] s_values = [];

        /// <summary>Gets the read-only span containing the set of values up to the specified parameter.</summary>
        /// <param name="length">The amount of items required.</param>
        /// <exception cref="MissingMethodException">The type <typeparamref name="T"/> is unsupported.</exception>
        /// <returns>
        /// The <see cref="ReadOnlySpan{T}"/> containing a range from 0 to <paramref name="length"/> - 1.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<T> Span(int length)
        {
            if (typeof(T) == typeof(char))
                return To<T>.From(length.SpanRange<ushort>());

            ReadOnlySpan<T> original = s_values;

            if (length <= original.Length)
                return original.UnsafelyTake(length);

            var replacement = new T[Math.Max(length.RoundUpToPowerOf2(), InitialCapacity / Unsafe.SizeOf<T>())];
            Span<T> span = replacement;
            original.CopyTo(span);
            Populate(span.UnsafelySkip(original.Length - (!original.IsEmpty).ToByte()));
            s_values = replacement;
            return span.UnsafelyTake(length);
        }

        /// <summary>Gets the read-only span containing the set of values up to the specified parameter.</summary>
        /// <param name="length">The amount of items required.</param>
        /// <exception cref="MissingMethodException">The type <typeparamref name="T"/> is unsupported.</exception>
        /// <returns>
        /// The <see cref="ReadOnlySpan{T}"/> containing a range from 0 to <paramref name="length"/> - 1.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlyMemory<T> Memory(int length)
        {
            if (typeof(T) == typeof(char))
                return Unsafe.As<ReadOnlyMemory<ushort>, ReadOnlyMemory<T>>(ref AsRef(length.MemoryRange<ushort>()));

            _ = Span(length);
            return new(s_values, 0, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Populate(scoped Span<T> span)
        {
            ref var start = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 1);
            ref var end = ref Unsafe.Add(ref start, span.Length);

            for (; Unsafe.IsAddressLessThan(ref start, ref end); start = ref Unsafe.Add(ref start, 1)!)
            {
                start = Unsafe.Subtract(ref start, 1);
                Increment(ref start);
            }
        }
    }
}
#endif
