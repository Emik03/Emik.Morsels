// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace System.Linq;
#if !NET9_0_OR_GREATER
/// <summary>The backport of the AggregateBy method for <see cref="IEnumerable{T}"/>.</summary>
static partial class EnumerableAggregate
{
    /// <summary>Performs a specified accumulator function over a sequence.</summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
    /// <param name="source">The sequence to accumulate over.</param>
    /// <param name="keySelector">The key selector function.</param>
    /// <param name="seed">The initial accumulator value.</param>
    /// <param name="func">The accumulator function.</param>
    /// <param name="keyComparer">The key comparer.</param>
    /// <returns>The accumulated value.</returns>
    public static IEnumerable<KeyValuePair<TKey, TAccumulate>> AggregateBy<TSource, TKey, TAccumulate>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        TAccumulate seed,
        Func<TAccumulate, TSource, TAccumulate> func,
        IEqualityComparer<TKey>? keyComparer = null
    )
        where TKey : notnull =>
        source.TryCount() is 0 ? [] : AggregateByIterator(source, keySelector, seed, func, keyComparer);

    /// <summary>Performs a specified accumulator function over a sequence.</summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
    /// <param name="source">The sequence to accumulate over.</param>
    /// <param name="keySelector">The key selector function.</param>
    /// <param name="seedSelector">The seed selector function.</param>
    /// <param name="func">The accumulator function.</param>
    /// <param name="keyComparer">The key comparer.</param>
    /// <returns>The accumulated value.</returns>
    public static IEnumerable<KeyValuePair<TKey, TAccumulate>> AggregateBy<TSource, TKey, TAccumulate>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TKey, TAccumulate> seedSelector,
        Func<TAccumulate, TSource, TAccumulate> func,
        IEqualityComparer<TKey>? keyComparer = null
    )
        where TKey : notnull =>
        source.TryCount() is 0 ? [] : AggregateByIterator(source, keySelector, seedSelector, func, keyComparer);

    static IEnumerable<KeyValuePair<TKey, TAccumulate>> AggregateByIterator<TSource, TKey, TAccumulate>(
        IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        TAccumulate seed,
        Func<TAccumulate, TSource, TAccumulate> func,
        IEqualityComparer<TKey>? keyComparer
    )
        where TKey : notnull
    {
        static Dictionary<TKey, TAccumulate> PopulateDictionary(
            IEnumerator<TSource> enumerator,
            Func<TSource, TKey> keySelector,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func,
            IEqualityComparer<TKey>? keyComparer
        )
        {
            Dictionary<TKey, TAccumulate> dict = new(keyComparer);

            do
            {
                var value = enumerator.Current;
                var key = keySelector(value);
                dict[key] = func(dict.GetValueOrDefault(key, seed), value);
            } while (enumerator.MoveNext());

            return dict;
        }

        using var enumerator = source.GetEnumerator();

        if (!enumerator.MoveNext())
            yield break;

        foreach (var countBy in PopulateDictionary(enumerator, keySelector, seed, func, keyComparer))
            yield return countBy;
    }

    static IEnumerable<KeyValuePair<TKey, TAccumulate>> AggregateByIterator<TSource, TKey, TAccumulate>(
        IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TKey, TAccumulate> seedSelector,
        Func<TAccumulate, TSource, TAccumulate> func,
        IEqualityComparer<TKey>? keyComparer
    )
        where TKey : notnull
    {
        static Dictionary<TKey, TAccumulate> PopulateDictionary(
            IEnumerator<TSource> enumerator,
            Func<TSource, TKey> keySelector,
            Func<TKey, TAccumulate> seedSelector,
            Func<TAccumulate, TSource, TAccumulate> func,
            IEqualityComparer<TKey>? keyComparer
        )
        {
            Dictionary<TKey, TAccumulate> dict = new(keyComparer);

            do
            {
                var value = enumerator.Current;
                var key = keySelector(value);
                dict[key] = func(dict.GetValueOrDefault(key, seedSelector(key)), value);
            } while (enumerator.MoveNext());

            return dict;
        }

        using var enumerator = source.GetEnumerator();

        if (!enumerator.MoveNext())
            yield break;

        foreach (var countBy in PopulateDictionary(enumerator, keySelector, seedSelector, func, keyComparer))
            yield return countBy;
    }
}
#endif
