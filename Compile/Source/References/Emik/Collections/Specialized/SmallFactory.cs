// SPDX-License-Identifier: MPL-2.0

// ReSharper disable NullableWarningSuppressionIsUsed RedundantExtendsListEntry RedundantUnsafeContext
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods that act as factories for <see cref="SmallList{T}"/>.</summary>
#pragma warning disable MA0048
static partial class SmallFactory
#pragma warning restore MA0048
{
#if NETCOREAPP3_1_OR_GREATER
    /// <inheritdoc cref="global::System.MemoryExtensions.Contains"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool Contains<T>(this scoped PooledSmallList<T> span, T item)
        where T : IEquatable<T>? =>
        span.View.Contains(item);
#endif
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
    /// <summary>Removes the first occurence of a specific object from the <see cref="PooledSmallList{T}"/>.</summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="span">The <see cref="PooledSmallList{T}"/> to remove an element from.</param>
    /// <param name="item">The item to remove from the <see cref="PooledSmallList{T}"/>.</param>
    /// <returns>
    /// Whether or not it found the parameter <paramref name="item"/> within the bounds of the
    /// parameter <paramref name="span"/>, and substantially removed it from the collection.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Remove<T>(this scoped PooledSmallList<T> span, T item)
        where T : IEquatable<T>?
    {
        var i = span.IndexOf(item);

        if (i is -1)
            return false;

        span.RemoveAt(i);
        return true;
    }

    /// <inheritdoc cref="global::System.MemoryExtensions.IndexOf"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int IndexOf<T>(this scoped PooledSmallList<T> span, T item)
        where T : IEquatable<T>? =>
        span.View.IndexOf(item);

    /// <summary>Creates a new instance of the <see cref="PooledSmallList{T}"/> struct.</summary>
    /// <typeparam name="T">The type of the collection.</typeparam>
    /// <param name="capacity">
    /// The initial allocation, which puts it on the heap immediately but can save future resizing.
    /// </param>
    /// <returns>The created instance of <see cref="PooledSmallList{T}"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static PooledSmallList<T> AsPooledSmallList<T>(this int capacity) => new(capacity);
#endif

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
    public static SmallList<T> ToSmallList<T>([InstantHandle] this IEnumerable<T>? iterable) => [..iterable];

    /// <summary>Mutates the enumerator; allocating the heaped list lazily.</summary>
    /// <typeparam name="T">The type of the <paramref name="iterator"/> and the <see langword="return"/>.</typeparam>
    /// <param name="iterator">The collection to turn into a <see cref="SmallList{T}"/>.</param>
    /// <returns>A <see cref="SmallList{T}"/> of <paramref name="iterator"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SmallList<T> ToSmallList<T>(this IEnumerator<T>? iterator) => new(iterator);
}
