// SPDX-License-Identifier: MPL-2.0
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods for randomized getters.</summary>
static partial class RandomizedGetters
{
    // ReSharper disable once RedundantNameQualifier
    static readonly Func<int, int, int> s_rng =
#if NET6_0_OR_GREATER
        System.Random.Shared.Next;
#else
        new System.Random().Next;
#endif
    /// <summary>Shuffles a collection.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to shuffle.</param>
    /// <param name="selector">The indices to swap with.</param>
    /// <returns>A randomized list of items in the parameter <paramref name="selector"/>.</returns>
    public static IList<T> Shuffle<T>(
        this IEnumerable<T> iterable,
        [InstantHandle] Func<int, int, int>? selector = null
    )
    {
        selector ??= s_rng;
        var list = iterable.ToIList();

        for (var j = list.Count; j >= 1; j--)
        {
            var item = selector(0, j);

            if (item >= j - 1)
                continue;

            // Tuples might not necessarily be imported.
#pragma warning disable IDE0180 // ReSharper disable once SwapViaDeconstruction
            var t = list[item];
#pragma warning restore IDE0180
            list[item] = list[j - 1];
            list[j - 1] = t;
        }

        return list;
    }
#if !NO_SYSTEM_MEMORY
    /// <inheritdoc cref="Shuffle{T}(IEnumerable{T}, Func{int, int, int})" />
    public static Span<T> Shuffle<T>(this Span<T> iterable, [InstantHandle] Func<int, int, int>? selector = null)
    {
        selector ??= s_rng;

        for (var j = iterable.Length; j >= 1; j--)
        {
            var item = selector(0, j);

            if (item >= j - 1)
                continue;

            // Tuples might not necessarily be imported.
#pragma warning disable IDE0180 // ReSharper disable once SwapViaDeconstruction
            var t = iterable[item];
#pragma warning restore IDE0180
            iterable[item] = iterable[j - 1];
            iterable[j - 1] = t;
        }

        return iterable;
    }
#endif
    /// <summary>Shuffles a collection.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to shuffle.</param>
    /// <param name="selector">The indices to swap with.</param>
    /// <returns>A randomized list of items in the parameter <paramref name="selector"/>.</returns>
    [MustUseReturnValue] // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public static T PickRandom<T>(
        [InstantHandle] this IEnumerable<T> iterable,
        [InstantHandle] Func<int, int, int>? selector = null
    )
    {
        static T Fallback(IEnumerable<T> iterable, Func<int, int, int> selector)
        {
            var list = iterable.ReadOnly();
            return list[selector(0, list.Count)];
        }

        selector ??= s_rng;

        return iterable switch
        {
            IList<T> list => list[selector(0, list.Count)],
            IReadOnlyList<T> list => list[selector(0, list.Count)],
            _ when iterable.TryCount() is { } count => iterable.ElementAt(selector(0, count)),
            _ => Fallback(iterable, selector),
        };
    }
#if !NO_SYSTEM_MEMORY
    /// <inheritdoc cref="PickRandom{T}(IEnumerable{T}, Func{int, int, int})" />
    [MustUseReturnValue]
    public static T PickRandom<T>(
        [InstantHandle] this scoped Span<T> iterable,
        [InstantHandle] Func<int, int, int>? selector = null
    ) =>
        iterable.ReadOnly().PickRandom(selector);

    /// <inheritdoc cref="PickRandom{T}(IEnumerable{T}, Func{int, int, int})" />
    [MustUseReturnValue]
    public static T PickRandom<T>(
        [InstantHandle] this scoped ReadOnlySpan<T> iterable,
        [InstantHandle] Func<int, int, int>? selector = null
    ) =>
        iterable[(selector ?? s_rng)(0, iterable.Length)];
#endif
}
