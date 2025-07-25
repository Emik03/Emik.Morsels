// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Methods for draining collections.</summary>
static partial class Retainer
{
    /// <summary>
    /// Removes all items from <paramref name="source"/> that do not satisfy <paramref name="predicate"/>.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="source">The collection to drain.</param>
    /// <param name="predicate">The predicate to apply.</param>
    public static void Retain<T>(this IList<T> source, [InstantHandle] Predicate<T> predicate)
    {
        int count = source.Count, i = 0;

        while (i < count)
        {
            if (predicate(source[i]))
            {
                i++;
                continue;
            }

            source.RemoveAt(i);
            count--;
        }
    }

    /// <summary>Removes all elements that match the conditions defined by the specified predicate.</summary>
    /// <typeparam name="T">The type of element in the collection to filter.</typeparam>
    /// <param name="collection">The collection to filter.</param>
    /// <param name="match">
    /// The <see cref="Predicate{T}"/> delegate that defines the conditions of the elements to remove.
    /// </param>
    /// <returns>The number of elements that were removed from the parameter <paramref name="collection"/>.</returns>
    public static int RemoveWhere<T>(this ICollection<T> collection, Func<T, bool> match) =>
        RemoveWhere<T, ICollection<T>>(collection, match);

    /// <summary>Removes all elements that match the conditions defined by the specified predicate.</summary>
    /// <typeparam name="TElement">The type of element in the collection to filter.</typeparam>
    /// <typeparam name="TCollection">The type of collection to filter.</typeparam>
    /// <param name="collection">The collection to filter.</param>
    /// <param name="match">
    /// The <see cref="Predicate{T}"/> delegate that defines the conditions of the elements to remove.
    /// </param>
    /// <returns>The number of elements that were removed from the parameter <paramref name="collection"/>.</returns>
    public static int RemoveWhere<TElement, TCollection>(this TCollection collection, Func<TElement, bool> match)
        where TCollection : ICollection<TElement>
    {
        List<TElement> matches = [..collection.Where(match)];
        var removed = 0;

        // Enumerate the results in reverse in an attempt to lower cost.
        for (var i = matches.Count - 1; i >= 0 && matches[i] is var current; i--)
            _ = collection.Remove(current) ? removed++ : 0;

        return removed;
    }
}
