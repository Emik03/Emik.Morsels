// SPDX-License-Identifier: MPL-2.0
#if !NET20 && !NET30
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides methods for creating combinations of items.</summary>
static partial class Permuted
{
    /// <summary>Generates all combinations of the nested enumerable.</summary>
    /// <typeparam name="T">The type of nested enumerable.</typeparam>
    /// <param name="iterator">The input to generate combinations of.</param>
    /// <returns>Every combination of the items in <paramref name="iterator"/>.</returns>
    [Pure]
#if NETFRAMEWORK && !NET45_OR_GREATER
    public static IEnumerable<IList<T>> Combinations<T>(
#else
    public static IEnumerable<IReadOnlyList<T>> Combinations<T>(
#endif
        [InstantHandle] this IEnumerable<IEnumerable<T>> iterator
    ) =>
#if NETFRAMEWORK && !NET45_OR_GREATER
        iterator.Select(x => x.ToIList()).ToIList().Combinations();
#else
        iterator.Select(x => x.ReadOnly()).ReadOnly().Combinations();
#endif
    /// <summary>Generates all combinations of the nested list.</summary>
    /// <typeparam name="T">The type of nested list.</typeparam>
    /// <param name="lists">The input to generate combinations of.</param>
    /// <returns>Every combination of the items in <paramref name="lists"/>.</returns>
    [Pure]
#if NETFRAMEWORK && !NET45_OR_GREATER
    public static IEnumerable<IList<T>> Combinations<T>(this IList<IList<T>> lists)
#else
    public static IEnumerable<IReadOnlyList<T>> Combinations<T>(this IReadOnlyList<IReadOnlyList<T>> lists)
#endif
    {
        if (lists.Any(x => x is []))
            yield break;

        int count = lists.Count, index = 0, pos = 0;
        var indices = new int[count];
        var accumulator = new T[count];

        while (true)
        {
            while (pos < accumulator.Length)
            {
                indices[pos] = index;
                accumulator[pos] = lists[pos][index];
                index = 0;
                pos++;
            }

            var result = new T[count];
            Array.Copy(accumulator, result, count);
            yield return result;

            do
            {
                if (pos is 0)
                    yield break;

                index = indices[--pos] + 1;
            } while (index >= lists[pos].Count);
        }
    }

    /// <summary>Generates all combinations of the nested list.</summary>
    /// <typeparam name="T">The type of nested list.</typeparam>
    /// <param name="lists">The input to generate combinations of.</param>
    /// <returns>Every combination of the items in <paramref name="lists"/>.</returns>
    [Pure]
    public static IEnumerable<SmallList<T>> Combinations<T>(this SmallList<SmallList<T>> lists)
    {
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var list in lists)
            if (list.IsEmpty)
                return [];

        return lists.CombinationsIterator();
    }

    /// <summary>Generates all combinations of the nested enumerable.</summary>
    /// <typeparam name="T">The type of nested enumerable.</typeparam>
    /// <param name="iterator">The input to generate combinations of.</param>
    /// <returns>Every combination of the items in <paramref name="iterator"/>.</returns>
    [Pure]
    public static IEnumerable<SmallList<T>> SmallListCombinations<T>(
        [InstantHandle] this IEnumerable<IEnumerable<T>> iterator
    ) =>
        iterator.Select(x => x.ToSmallList()).ToSmallList().Combinations();

    static IEnumerable<SmallList<T>> CombinationsIterator<T>(this SmallList<SmallList<T>> lists)
    {
        int count = lists.Count, index = 0, pos = 0;
        var indices = SmallList<int>.Uninit(count);
        var accumulator = SmallList<T>.Uninit(count);

        while (true)
        {
            while (pos < accumulator.Count)
            {
                indices[pos] = index;
                accumulator[pos] = lists[pos][index];
                index = 0;
                pos++;
            }

            yield return accumulator.Cloned;

            do
            {
                if (pos is 0)
                    yield break;

                index = indices[--pos] + 1;
            } while (index >= lists[pos].Count);
        }
    }
}
#endif
