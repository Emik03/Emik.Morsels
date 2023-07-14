// SPDX-License-Identifier: MPL-2.0

// ReSharper disable NullableWarningSuppressionIsUsed RedundantExtendsListEntry RedundantUnsafeContext
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods that act as factories for <see cref="SmallList{T}"/>.</summary>
#pragma warning disable MA0048
static partial class SmallFactory
#pragma warning restore MA0048
{
    /// <inheritdoc cref="System.MemoryExtensions.Contains"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains<T, TTwo>(this SmallList<T, TTwo> span, T item)
        where T : IEquatable<T>? =>
        span.View.Contains(item);

    /// <summary>Removes the first occurence of a specific object from the <see cref="SmallList{T, TRef}"/>.</summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <typeparam name="TRef">The type of reference value for the <see cref="SmallList{T, TRef}"/>.</typeparam>
    /// <param name="span">The <see cref="SmallList{T, TRef}"/> to remove an element from.</param>
    /// <param name="item">The item to remove from the <see cref="SmallList{T, TRef}"/>.</param>
    /// <returns>
    /// Whether or not it found the parameter <paramref name="item"/> within the bounds of the
    /// parameter <paramref name="span"/>, and substantially removed it from the collection.
    /// </returns>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Remove<T, TRef>(this SmallList<T, TRef> span, T item)
        where T : IEquatable<T>?
    {
        var i = span.IndexOf(item);

        if (i is -1)
            return false;

        span.RemoveAt(i);
        return true;
    }

    /// <inheritdoc cref="System.MemoryExtensions.IndexOf"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf<T, TTwo>(this SmallList<T, TTwo> span, T item)
        where T : IEquatable<T>? =>
        span.View.IndexOf(item);

    /// <inheritdoc cref="SmallList{T}.op_Implicit(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SmallList<T> AsSmallList<T>(this T value) => value;

    /// <inheritdoc cref="SmallList{T}.op_Implicit(ValueTuple{T, T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SmallList<T2> AsSmallList<T1, T2>(this (T1 First, T2 Second) tuple)
        where T1 : T2 =>
        tuple;

    /// <inheritdoc cref="SmallList{T}.op_Implicit(ValueTuple{T, T, T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SmallList<T3> AsSmallList<T1, T2, T3>(this (T1 First, T2 Second, T3 Third) tuple)
        where T1 : T3
        where T2 : T3 =>
        tuple;

    /// <inheritdoc cref="SmallList{T}.Uninit"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SmallList<T> AsUninitSmallList<T>(this int length) => SmallList<T>.Uninit(length);

    /// <inheritdoc cref="SmallList{T}.Zeroed"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SmallList<T> AsZeroedSmallList<T>(this int length) => SmallList<T>.Zeroed(length);

    /// <summary>Collects the enumerable; allocating the heaped list lazily.</summary>
    /// <typeparam name="T">The type of the <paramref name="iterable"/> and the <see langword="return"/>.</typeparam>
    /// <param name="iterable">The collection to turn into a <see cref="SmallList{T}"/>.</param>
    /// <returns>A <see cref="SmallList{T}"/> of <paramref name="iterable"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SmallList<T> ToSmallList<T>(this IEnumerable<T>? iterable) => new(iterable);

    /// <summary>Mutates the enumerator; allocating the heaped list lazily.</summary>
    /// <typeparam name="T">The type of the <paramref name="iterator"/> and the <see langword="return"/>.</typeparam>
    /// <param name="iterator">The collection to turn into a <see cref="SmallList{T}"/>.</param>
    /// <returns>A <see cref="SmallList{T}"/> of <paramref name="iterator"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SmallList<T> ToSmallList<T>(this IEnumerator<T>? iterator) => new(iterator);
}
