// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides methods for creating combinations of items.</summary>
static partial class Permuted
{
    /// <summary>Generates all combinations of the nested list.</summary>
    /// <typeparam name="T">The type of nested list.</typeparam>
    /// <param name="input">The input to generate combinations of.</param>
    /// <returns>Every combination of the items in <paramref name="input"/>.</returns>
#if NETFRAMEWORK && !NET45_OR_GREATER
    public static IEnumerable<IList<T>> Combinations<T>(this IList<IList<T>> input)
#else
    public static IEnumerable<IReadOnlyList<T>> Combinations<T>(this IReadOnlyList<IReadOnlyList<T>> input)
#endif
    {
        var indices = new int[input.Count];
        int pos = 0, index = 0;

        while (true)
        {
            var result = new T[input.Count];

            while (pos < result.Length)
            {
                indices[pos] = index;
                result[pos] = input[pos][index];
                index = 0;
                pos++;
            }

            yield return result;

            do
            {
                if (pos is 0)
                    yield break;

                index = indices[--pos] + 1;
            } while (index >= input[pos].Count);
        }
    }
}
