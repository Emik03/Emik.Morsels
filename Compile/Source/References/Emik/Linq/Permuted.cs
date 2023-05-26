// SPDX-License-Identifier: MPL-2.0

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
        iterator.Select(x => x.ToReadOnly()).ToReadOnly().Combinations();

    /// <summary>Generates all combinations of the nested list.</summary>
    /// <typeparam name="T">The type of nested list.</typeparam>
    /// <param name="list">The input to generate combinations of.</param>
    /// <returns>Every combination of the items in <paramref name="list"/>.</returns>
    [Pure]
#if NETFRAMEWORK && !NET45_OR_GREATER
    public static IEnumerable<IList<T>> Combinations<T>(this IList<IList<T>> input)
#else
    public static IEnumerable<IReadOnlyList<T>> Combinations<T>(this IReadOnlyList<IReadOnlyList<T>> list)
#endif
    {
        if (list.Any(x => x is []))
            yield break;

        var indices = new int[list.Count];
        int pos = 0, index = 0;

        while (true)
        {
            var result = new T[list.Count];

            while (pos < result.Length)
            {
                indices[pos] = index;
                result[pos] = list[pos][index];
                index = 0;
                pos++;
            }

            yield return result;

            do
            {
                if (pos is 0)
                    yield break;

                index = indices[--pos] + 1;
            } while (index >= list[pos].Count);
        }
    }
}
