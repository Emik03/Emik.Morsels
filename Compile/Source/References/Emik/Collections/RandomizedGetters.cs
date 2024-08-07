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
        var list = iterable.ToIListLazily();

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

    /// <inheritdoc cref="Shuffle{T}(IEnumerable{T}, Func{int, int, int})" />
    [MustUseReturnValue] // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public static Span<T> Shuffle<T>(this Span<T> iterable, [InstantHandle] Func<int, int, int>? selector = null)
    {
        selector ??= Rand();

        for (var j = iterable.Length; j >= 1; j--)
        {
            var item = selector(0, j);

            if (item >= j - 1)
                continue;

            // Tuples might not necessarily be imported.
#pragma warning disable IDE0180 // ReSharper disable once SwapViaDeconstruction
            var t = iterable[item];
            iterable[item] = iterable[j - 1];
            iterable[j - 1] = t;
#pragma warning restore IDE0180
        }

        return iterable;
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

    /// <inheritdoc cref="PickRandom{T}(IEnumerable{T}, Func{int, int, int})" />
    [MustUseReturnValue]
    public static T PickRandom<T>([InstantHandle] this scoped Span<T> iterable, Func<int, int, int>? selector = null) =>
        PickRandom((ReadOnlySpan<T>)iterable, selector);

    /// <inheritdoc cref="PickRandom{T}(IEnumerable{T}, Func{int, int, int})" />
    [MustUseReturnValue]
    public static T PickRandom<T>(
        [InstantHandle] this scoped ReadOnlySpan<T> iterable,
        Func<int, int, int>? selector = null
    )
    {
        selector ??= Rand();
        return iterable[selector(0, iterable.Length)];
    }

    [Pure]
    static Func<int, int, int> Rand() =>
#if KTANE
        UnityEngine.Random.Range;
#elif NET6_0_OR_GREATER
        Random.Shared.Next;
#else
        // ReSharper disable once RedundantNameQualifier
        new Random().Next;
#endif
}
#endif
