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
        iterator.Select(x => x.ToListLazily()).ToListLazily().Combinations();
#else
        iterator.Select(x => x.ToReadOnly()).ToReadOnly().Combinations();
#endif

    /// <summary>Generates all combinations of the nested list.</summary>
    /// <typeparam name="T">The type of nested list.</typeparam>
    /// <param name="list">The input to generate combinations of.</param>
    /// <returns>Every combination of the items in <paramref name="list"/>.</returns>
    [Pure]
#if NETFRAMEWORK && !NET45_OR_GREATER
    public static IEnumerable<IList<T>> Combinations<T>(this IList<IList<T>> list)
#else
    public static IEnumerable<IReadOnlyList<T>> Combinations<T>(this IReadOnlyList<IReadOnlyList<T>> list)
#endif
    {
        if (list.Any(x => x is []))
            yield break;

        int count = list.Count, index = 0, pos = 0;
        var indices = new int[count];
        var accumulator = new T[count];

        while (true)
        {
            while (pos < accumulator.Length)
            {
                indices[pos] = index;
                accumulator[pos] = list[pos][index];
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
            } while (index >= list[pos].Count);
        }
    }
}
#endif
