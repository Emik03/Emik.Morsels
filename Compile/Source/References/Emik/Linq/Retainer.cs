﻿// SPDX-License-Identifier: MPL-2.0

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
}
