// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;

using static OperatorCaching;

/// <inheritdoc cref="SpanQueries"/>
// ReSharper disable NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
#pragma warning disable MA0048
static partial class SpanQueries
#pragma warning restore MA0048
{
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <inheritdoc cref="Range{T}(Span{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Range<T>(this IMemoryOwner<T> source) => Range(source.Memory.Span);

    /// <inheritdoc cref="Range{T}(Span{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> Range<T>(this Memory<T> source) => Range(source.Span);
#endif

    /// <summary>Creates the range.</summary>
    /// <typeparam name="T">The type of number.</typeparam>
    /// <param name="source">The <see cref="Span{T}"/> to mutate.</param>
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
                InAscendingOrder<T>.UpTo(length).CopyTo(source);
                return source;
        }
    }

    static class InAscendingOrder<T>
    {
        // Vector512<T> is the largest vector type.
        const int InitialCapacity = 512;

        static T[] s_values = new T[InitialCapacity];

        static InAscendingOrder() => Populate(s_values);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<T> UpTo(int length)
        {
            ReadOnlySpan<T> original = s_values;

            if (length <= original.Length)
                return original[..length];

            var replacement = new T[Math.Max(original.Length * 2, length)];
            Span<T> span = replacement;
            original.CopyTo(span);
            Populate(span[(original.Length - 1)..]);
            s_values = replacement;
            return span[..length];
        }

        [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
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
