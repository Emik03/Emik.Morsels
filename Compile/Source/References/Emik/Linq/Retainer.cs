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
        for (var i = 0; i < source.Count; i++)
            if (!predicate(source[i]))
                source.RemoveAt(i--);
    }
}
