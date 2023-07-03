// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods for iterating over a set of elements, or for generating new ones.</summary>
static partial class SpanIndexers
{
    /// <summary>Separates the head from the tail of a <see cref="Span{T}"/>.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="span">The span to split.</param>
    /// <param name="head">The first element of the parameter <paramref name="span"/>.</param>
    /// <param name="tail">The rest of the parameter <paramref name="span"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deconstruct<T>(this Span<T> span, out T? head, out Span<T> tail)
    {
        if (span.IsEmpty)
        {
            head = default;
            tail = default;
            return;
        }

        head = span[0];
        tail = span[1..];
    }

    /// <summary>Separates the head from the tail of a <see cref="ReadOnlySpan{T}"/>.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="span">The span to split.</param>
    /// <param name="head">The first element of the parameter <paramref name="span"/>.</param>
    /// <param name="tail">The rest of the parameter <paramref name="span"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deconstruct<T>(this ReadOnlySpan<T> span, out T? head, out ReadOnlySpan<T> tail)
    {
        if (span.IsEmpty)
        {
            head = default;
            tail = default;
            return;
        }

        head = span[0];
        tail = span[1..];
    }
}
