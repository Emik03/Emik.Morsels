// SPDX-License-Identifier: MPL-2.0
#if !NET20 && !NET30
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods for randomized getters.</summary>
static partial class RandomizedGetters
{
    /// <summary>Shuffles a collection.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to shuffle.</param>
    /// <param name="selector">The indices to swap with, when left unspecified, uses <see cref="Rand"/>.</param>
    /// <returns>A randomized list of items in the parameter <paramref name="selector"/>.</returns>
    [MustUseReturnValue] // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public static IList<T> Shuffle<T>(
        [InstantHandle] this IEnumerable<T> iterable,
        [InstantHandle] Func<int, int, int>? selector = null
    )
    {
        selector ??= Rand();

        var list = iterable.ToListLazily();

        for (var j = list.Count; j >= 1; j--)
        {
            var item = selector(0, j);

            if (item >= j - 1)
                continue;

            // Tuples might not necessarily be imported.
#pragma warning disable IDE0180 // ReSharper disable once SwapViaDeconstruction
            var t = list[item];
            list[item] = list[j - 1];
            list[j - 1] = t;
#pragma warning restore IDE0180
        }

        return list;
    }

    /// <summary>Shuffles a collection.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to shuffle.</param>
    /// <param name="selector">The indices to swap with, when left unspecified, uses <see cref="Rand"/>.</param>
    /// <returns>A randomized list of items in the parameter <paramref name="selector"/>.</returns>
    [MustUseReturnValue] // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public static T PickRandom<T>(
        [InstantHandle] this IEnumerable<T> iterable,
        [InstantHandle] Func<int, int, int>? selector = null
    )
    {
        selector ??= Rand();

        return iterable switch
        {
            IList<T> list => list[selector(0, list.Count)],
            IReadOnlyList<T> list => list[selector(0, list.Count)],
            _ when iterable.ToList() is var list => list[selector(0, list.Count)],
            _ => throw Unreachable,
        };
    }

    [Pure]
    static Func<int, int, int> Rand() =>
#if NET35
        UnityEngine.Random.Range;
#elif NET6_0_OR_GREATER
        Random.Shared.Next;
#else
        // ReSharper disable once RedundantNameQualifier
        new Random().Next;
#endif
}
#endif
