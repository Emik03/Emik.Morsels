// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace System.Linq;

#if !NET6_0_OR_GREATER
/// <summary>The backport of the UnionBy and IntersectBy methods for <see cref="IEnumerable{T}"/>.</summary>
static partial class EnumerableUnion
{
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

    /// <summary>
    /// Produces the set intersection of two sequences according to a specified key selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
    /// <typeparam name="TKey">The type of key to identify elements by.</typeparam>
    /// <param name="first">
    /// An <see cref="IEnumerable{T}"/> whose distinct elements that
    /// also appear in <paramref name="second"/> will be returned.
    /// </param>
    /// <param name="second">
    /// An <see cref="IEnumerable{T}"/> whose distinct elements that also appear in the first sequence will be returned.
    /// </param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <returns>A sequence that contains the elements that form the set intersection of two sequences.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<TSource> IntersectBy<TSource, TKey>(
        this IEnumerable<TSource> first,
        IEnumerable<TKey> second,
        Func<TSource, TKey> keySelector
    ) =>
        IntersectBy(first, second, keySelector, null);

    /// <summary>
    /// Produces the set intersection of two sequences according to a specified key selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
    /// <typeparam name="TKey">The type of key to identify elements by.</typeparam>
    /// <param name="first">
    /// An <see cref="IEnumerable{T}"/> whose distinct elements that
    /// also appear in <paramref name="second"/> will be returned.
    /// </param>
    /// <param name="second">
    /// An <see cref="IEnumerable{T}"/> whose distinct elements that also appear in the first sequence will be returned.
    /// </param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to compare values.</param>
    /// <returns>A sequence that contains the elements that form the set intersection of two sequences.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<TSource> IntersectBy<TSource, TKey>(
        this IEnumerable<TSource> first,
        IEnumerable<TKey> second,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer
    )
    {
        HashSet<TKey> set = new(second, comparer);
        return first.Where(x => !set.Add(keySelector(x)));
    }
}
#endif
