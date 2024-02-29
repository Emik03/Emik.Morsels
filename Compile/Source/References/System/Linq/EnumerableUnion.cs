// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace System.Linq;

#if !NET6_0_OR_GREATER
/// <summary>The backport of the Union and UnionBy methods for <see cref="IEnumerable{T}"/>.</summary>
static partial class EnumerableUnion
{
    /// <summary>Produces the set union of two sequences by using the default equality comparer.</summary>
    /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
    /// <param name="first">
    /// An <see cref="IEnumerable{T}"/> whose distinct elements form the first set for the union.
    /// </param>
    /// <param name="second">
    /// An <see cref="IEnumerable{T}"/> whose distinct elements form the second set for the union.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> that contains the elements from both input sequences, excluding duplicates.
    /// </returns>
    [LinqTunnel, Pure]
    public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second) =>
        Union(first, second, null);

    /// <summary>
    /// Produces the set union of two sequences by using a specified <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
    /// <param name="first">
    /// An <see cref="IEnumerable{T}"/> whose distinct elements form the first set for the union.
    /// </param>
    /// <param name="second">
    /// An <see cref="IEnumerable{T}"/> whose distinct elements form the second set for the union.
    /// </param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to compare values.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> that contains the elements from both input sequences, excluding duplicates.
    /// </returns>
    [LinqTunnel, Pure]
    public static IEnumerable<TSource> Union<TSource>(
        this IEnumerable<TSource> first,
        IEnumerable<TSource> second,
        IEqualityComparer<TSource>? comparer
    )
    {
        HashSet<TSource> set = new(comparer);
        return first.Where(set.Add).Concat(second.Where(set.Add));
    }

    /// <summary>Produces the set union of two sequences according to a specified key selector function.</summary>
    /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
    /// <typeparam name="TKey">The type of key to identify elements by.</typeparam>
    /// <param name="first">
    /// An <see cref="IEnumerable{T}"/> whose distinct elements form the first set for the union.
    /// </param>
    /// <param name="second">
    /// An <see cref="IEnumerable{T}"/> whose distinct elements form the second set for the union.
    /// </param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> that contains the elements from both input sequences, excluding duplicates.
    /// </returns>
    [LinqTunnel, Pure]
    public static IEnumerable<TSource> UnionBy<TSource, TKey>(
        this IEnumerable<TSource> first,
        IEnumerable<TSource> second,
        Func<TSource, TKey> keySelector
    ) =>
        UnionBy(first, second, keySelector, null);

    /// <summary>Produces the set union of two sequences according to a specified key selector function.</summary>
    /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
    /// <typeparam name="TKey">The type of key to identify elements by.</typeparam>
    /// <param name="first">
    /// An <see cref="IEnumerable{T}"/> whose distinct elements form the first set for the union.
    /// </param>
    /// <param name="second">
    /// An <see cref="IEnumerable{T}"/> whose distinct elements form the second set for the union.
    /// </param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to compare values.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> that contains the elements from both input sequences, excluding duplicates.
    /// </returns>
    [LinqTunnel, Pure]
    public static IEnumerable<TSource> UnionBy<TSource, TKey>(
        this IEnumerable<TSource> first,
        IEnumerable<TSource> second,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer
    )
    {
        HashSet<TKey> set = new(comparer);
        return first.Where(x => set.Add(keySelector(x))).Concat(second.Where(x => set.Add(keySelector(x))));
    }
}
#endif
