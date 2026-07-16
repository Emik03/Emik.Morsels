// SPDX-License-Identifier: MPL-2.0
// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;

/// <summary>Provides methods for creating combinations of items.</summary>
static partial class Permuted
{
    /// <summary>Generates all combinations of the nested enumerable.</summary>
    /// <typeparam name="T">The type of nested enumerable.</typeparam>
    /// <param name="iterator">The input to generate combinations of.</param>
    /// <returns>Every combination of the items in <paramref name="iterator"/>.</returns>
    [Pure]
    public static IEnumerable<IList<T>> Combinations<T>([InstantHandle] this IEnumerable<IEnumerable<T>> iterator) =>
        iterator.Select(x => x.ToIList()).ToIList().Combinations();

    /// <summary>Generates all combinations of the nested list.</summary>
    /// <typeparam name="T">The type of nested list.</typeparam>
    /// <param name="lists">The input to generate combinations of.</param>
    /// <returns>Every combination of the items in <paramref name="lists"/>.</returns>
    [Pure]
    public static IEnumerable<IList<T>> Combinations<T>(this IList<IList<T>> lists)
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
}
