// SPDX-License-Identifier: MPL-2.0

// ReSharper disable NullableWarningSuppressionIsUsed RedundantUnsafeContext
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods that act as factories for <see cref="SmallList{T}"/>.</summary>
static partial class SmallFactory
{
    /// <summary>Collects the enumerable; allocating the heaped list lazily.</summary>
    /// <typeparam name="T">The type of the <paramref name="iterable"/> and the <see langword="return"/>.</typeparam>
    /// <param name="iterable">The collection to turn into a <see cref="SmallList{T}"/>.</param>
    /// <returns>A <see cref="SmallList{T}"/> of <paramref name="iterable"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SmallList<T> ToSmallList<T>([InstantHandle] this IEnumerable<T>? iterable) =>
        iterable is null ? default : [..iterable];

    /// <summary>Mutates the enumerator; allocating the heaped list lazily.</summary>
    /// <typeparam name="T">The type of the <paramref name="iterator"/> and the <see langword="return"/>.</typeparam>
    /// <param name="iterator">The collection to turn into a <see cref="SmallList{T}"/>.</param>
    /// <returns>A <see cref="SmallList{T}"/> of <paramref name="iterator"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SmallList<T> ToSmallList<T>(this IEnumerator<T>? iterator) => new(iterator);
}
