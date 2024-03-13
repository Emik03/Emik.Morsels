// SPDX-License-Identifier: MPL-2.0

// NOTE: This file should be moved to ./Source/References/System/Linq/EnumerableCount.cs when .NET 9 is released
// and CSharpRepl is updated to use it, as anything in ./Compile/Source/References/System/ is not included in REPL.csx.
#if !CSHARPREPL
// ReSharper disable once CheckNamespace EmptyNamespace
namespace System.Linq;
#endif
#if !NET9_0_OR_GREATER
/// <summary>The backport of the CountBy method for <see cref="IEnumerable{T}"/>.</summary>
static partial class EnumerableCount
{
    /// <summary>Performs a count by operation.</summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">The sequence to count by.</param>
    /// <param name="keySelector">The key selector function.</param>
    /// <param name="keyComparer">The key comparer.</param>
    /// <returns>The count by operation.</returns>
    public static IEnumerable<KeyValuePair<TKey, int>> CountBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? keyComparer = null
    )
        where TKey : notnull =>
        source.TryCount() is 0 ? [] : CountByIterator(source, keySelector, keyComparer);

    static IEnumerable<KeyValuePair<TKey, int>> CountByIterator<TSource, TKey>(
        IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? keyComparer
    )
        where TKey : notnull
    {
        using var enumerator = source.GetEnumerator();

        if (!enumerator.MoveNext())
            yield break;

        foreach (var countBy in BuildCountDictionary(enumerator, keySelector, keyComparer))
            yield return countBy;

        static Dictionary<TKey, int> BuildCountDictionary(
            IEnumerator<TSource> enumerator,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey>? keyComparer
        )
        {
            Dictionary<TKey, int> countsBy = new(keyComparer);

            do
            {
                var key = keySelector(enumerator.Current);
                countsBy[key] = checked(countsBy.GetValueOrDefault(key) + 1);
            } while (enumerator.MoveNext());

            return countsBy;
        }
    }
}
#endif
