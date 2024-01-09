// SPDX-License-Identifier: MPL-2.0
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
// ReSharper disable once CheckNamespace EmptyNamespace RedundantUsingDirective
namespace Emik.Morsels;

using static OperatorCaching;

/// <inheritdoc cref="SpanQueries"/>
// ReSharper disable NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
#pragma warning disable MA0048
static partial class SpanQueries
#pragma warning restore MA0048
{
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
                    Fail<T>();

                InAscendingOrder<T>.UpTo(length).CopyTo(source);
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
            Fail<T>();

        InAscendingOrder<T>.UpTo(index + source.Length)[index..].CopyTo(source);
        return source;
    }

    static class InAscendingOrder<T>
    {
        // Vector512<T> is the largest vector type.
        const int InitialCapacity = 512;

        static T[] s_values = new T[InitialCapacity / Unsafe.SizeOf<T>()];

        static InAscendingOrder() => Populate(s_values);

        /// <summary>Gets the read-only span containing the set of values up to the specified parameter.</summary>
        /// <param name="length">The amount of items required.</param>
        /// <exception cref="MissingMethodException">The type <typeparamref name="T"/> is unsupported.</exception>
        /// <returns>
        /// The <see cref="ReadOnlySpan{T}"/> containing a range from 0 to <paramref name="length"/> - 1.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<T> UpTo(int length)
        {
            ReadOnlySpan<T> original = s_values;

            if (length <= original.Length)
                return original[..length];

            var replacement = new T[((uint)length).RoundUpToPowerOf2()];
            Span<T> span = replacement;
            original.CopyTo(span);
            Populate(span[(original.Length - 1)..]);
            s_values = replacement;
            return span[..length];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Populate(scoped Span<T> span)
        {
            for (var i = 1; i < span.Length; i++)
            {
                span[i] = span[i - 1];
                Increment(ref span[i]);
            }
        }
    }
}
#endif
