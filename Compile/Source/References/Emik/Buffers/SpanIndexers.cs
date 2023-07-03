// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

#pragma warning disable IDE0056
/// <summary>Extension methods for iterating over a set of elements, or for generating new ones.</summary>
// ReSharper disable ConditionIsAlwaysTrueOrFalse UseIndexFromEndExpression
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

    /// <summary>Gets the specific slice from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to get an item from.</param>
    /// <param name="range">The index to get.</param>
    /// <returns>A slice from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ReadOnlySpan<T> Nth<T>(this ReadOnlySpan<T> span, Range range) =>
        range.TryGetOffsetAndLength(span.Length, out var offset, out var length) ? span.Slice(offset, length) : default;

    /// <summary>Gets the specific slice from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to get an item from.</param>
    /// <param name="range">The index to get.</param>
    /// <returns>A slice from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Span<T> Nth<T>(this Span<T> span, Range range) =>
        range.TryGetOffsetAndLength(span.Length, out var offset, out var length) ? span.Slice(offset, length) : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this scoped ReadOnlySpan<T> span, [NonNegativeValue] int index) =>
        index >= 0 && index < span.Length ? span[index] : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this scoped ReadOnlySpan<T> span, Index index) =>
        (index.IsFromEnd ? span.Length - index.Value : index.Value) is >= 0 and var offset && offset < span.Length
            ? span[offset]
            : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? NthLast<T>(this scoped ReadOnlySpan<T> span, [NonNegativeValue] int index) =>
        index > 0 && index <= span.Length ? span[span.Length - index] : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this scoped Span<T> span, [NonNegativeValue] int index) =>
        index >= 0 && index < span.Length ? span[index] : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? Nth<T>(this scoped Span<T> span, Index index) =>
        (index.IsFromEnd ? span.Length - index.Value : index.Value) is >= 0 and var offset && offset < span.Length
            ? span[offset]
            : default;

    /// <summary>Gets a specific item from the span.</summary>
    /// <typeparam name="T">The type of item in the span.</typeparam>
    /// <param name="span">The <see cref="Span{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="span"/>, or <see langword="default"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T? NthLast<T>(this scoped Span<T> span, [NonNegativeValue] int index) =>
        index > 0 && index <= span.Length ? span[span.Length - index] : default;
}
