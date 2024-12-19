// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace System.Linq;
#if !NET6_0_OR_GREATER
/// <summary>
/// The backport of the DistinctBy, ExceptBy, UnionBy, and IntersectBy methods for <see cref="IEnumerable{T}"/>.
/// </summary>
static partial class EnumerableUnion
{
#if NET472_OR_GREATER || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
    const int DefaultInternalSetCapacity = 7;
#endif
    /// <summary>Returns distinct elements from a sequence according to a specified key selector function.</summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <typeparam name="TKey">The type of key to distinguish elements by.</typeparam>
    /// <param name="source">The sequence to remove duplicate elements from.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <returns>An <see cref="IEnumerable{T}" /> that contains distinct elements from the source sequence.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source" /> is <see langword="null" />.</exception>
    /// <remarks>
    /// <para>This method is implemented by using deferred execution. The immediate return value is an object that stores all the information that is required to perform the action. The query represented by this method is not executed until the object is enumerated either by calling its `GetEnumerator` method directly or by using `foreach` in Visual C# or `For Each` in Visual Basic.</para>
    /// <para>The <see cref="DistinctBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})" /> method returns an unordered sequence that contains no duplicate values. The default equality comparer, <see cref="EqualityComparer{T}.Default" />, is used to compare values.</para>
    /// </remarks>
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector
    ) =>
        DistinctBy(source, keySelector, null);

    /// <summary>Returns distinct elements from a sequence according to a specified key selector function.</summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <typeparam name="TKey">The type of key to distinguish elements by.</typeparam>
    /// <param name="source">The sequence to remove duplicate elements from.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{TKey}" /> to compare keys.</param>
    /// <returns>An <see cref="IEnumerable{T}" /> that contains distinct elements from the source sequence.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source" /> is <see langword="null" />.</exception>
    /// <remarks>
    /// <para>This method is implemented by using deferred execution. The immediate return value is an object that stores all the information that is required to perform the action. The query represented by this method is not executed until the object is enumerated either by calling its `GetEnumerator` method directly or by using `foreach` in Visual C# or `For Each` in Visual Basic.</para>
    /// <para>The <see cref="DistinctBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey}, IEqualityComparer{TKey}?)" /> method returns an unordered sequence that contains no duplicate values. If <paramref name="comparer" /> is <see langword="null" />, the default equality comparer, <see cref="EqualityComparer{T}.Default" />, is used to compare values.</para>
    /// </remarks>
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer
    )
    {
#if NET472_OR_GREATER || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        HashSet<TKey> set = new(DefaultInternalSetCapacity, comparer);
#else
        HashSet<TKey> set = new(comparer);
#endif
        return source.Where(element => set.Add(keySelector(element)));
    }

    /// <summary>Produces the set difference of two sequences according to a specified key selector function.</summary>
    /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
    /// <typeparam name="TKey">The type of key to identify elements by.</typeparam>
    /// <param name="first">
    /// An <see cref="IEnumerable{TSource}"/> whose keys that are
    /// not also in <paramref name="second"/> will be returned.
    /// </param>
    /// <param name="second">
    /// An <see cref="IEnumerable{TKey}"/> whose keys that also occur in the first
    /// sequence will cause those elements to be removed from the returned sequence.
    /// </param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <returns>A sequence that contains the set difference of the elements of two sequences.</returns>
    public static IEnumerable<TSource> ExceptBy<TSource, TKey>(
        this IEnumerable<TSource> first,
        IEnumerable<TKey> second,
        Func<TSource, TKey> keySelector
    ) =>
        ExceptBy(first, second, keySelector, null);

    /// <summary>Produces the set difference of two sequences according to a specified key selector function.</summary>
    /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
    /// <typeparam name="TKey">The type of key to identify elements by.</typeparam>
    /// <param name="first">
    /// An <see cref="IEnumerable{TSource}" /> whose keys that are
    /// not also in <paramref name="second"/> will be returned.
    /// </param>
    /// <param name="second">
    /// An <see cref="IEnumerable{TKey}"/> whose keys that also occur in the first
    /// sequence will cause those elements to be removed from the returned sequence.
    /// </param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{TKey}"/> to compare values.</param>
    /// <returns>A sequence that contains the set difference of the elements of two sequences.</returns>
    public static IEnumerable<TSource> ExceptBy<TSource, TKey>(
        this IEnumerable<TSource> first,
        IEnumerable<TKey> second,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer
    )
    {
        HashSet<TKey> set = new(second, comparer);
        return first.Where(element => set.Add(keySelector(element)));
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
