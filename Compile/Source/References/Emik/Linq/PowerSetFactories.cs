// SPDX-License-Identifier: MPL-2.0
#if !NET20 && !NET30
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods to create power sets.</summary>
static partial class PowerSetFactories
{
    /// <inheritdoc cref="PowerSet{T}(ICollection{T})"/>
    [LinqTunnel, Pure]
    public static IEnumerable<IEnumerable<object>> PowerSet(this ICollection collection) =>
        collection.Cast<object>().PowerSetInner(collection.Count);

    /// <summary>Creates a power set from a collection.</summary>
    /// <remarks><para>
    /// The power set is defined as the set of all subsets, including the empty set and the set itself.
    /// </para></remarks>
    /// <typeparam name="T">The type of item in the set.</typeparam>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The argument <paramref name="collection"/> has 32 or more elements.
    /// </exception>
    /// <param name="collection">The set to create a power set.</param>
    /// <returns>The power set of the parameter <paramref name="collection"/>.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<IEnumerable<T>> PowerSet<T>(this ICollection<T> collection) =>
        collection.PowerSetInner(collection.Count);

    /// <inheritdoc cref="PowerSet{T}(ICollection{T})"/>
    [LinqTunnel, Pure]
    public static IEnumerable<IEnumerable<T>> PowerSet<T>(this IReadOnlyCollection<T> collection) =>
        collection.PowerSetInner(collection.Count);

    /// <inheritdoc cref="PowerSet{T}(ICollection{T})"/>
    [LinqTunnel, Pure]
    public static IEnumerable<IEnumerable<T>> PowerSet<T>(this T[] collection) =>
        ((ICollection<T>)collection).PowerSet();

    // ReSharper disable ConditionIsAlwaysTrueOrFalse
    [LinqTunnel, Pure]
    static IEnumerable<IEnumerable<T>> PowerSetInner<T>(this IEnumerable<T> iterable, [ValueRange(0, 31)] int count) =>
        count < 32
            ? Enumerable.Range(0, 1 << count).Select(mask => iterable.Where((_, j) => (1 << j & mask) is not 0))
            : throw new ArgumentOutOfRangeException(nameof(count), count, $"Cannot exceed bits in {nameof(Int32)}.");
}
#endif
