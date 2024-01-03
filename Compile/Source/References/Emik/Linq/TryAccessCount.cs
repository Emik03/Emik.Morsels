// SPDX-License-Identifier: MPL-2.0

// ReSharper disable CheckNamespace RedundantUsingDirective
namespace Emik.Morsels;

/// <summary>Extension methods to attempt to grab the length from enumerables.</summary>
static partial class TryAccessCount
{
    /// <summary>Tries to count the elements in the enumerable.</summary>
    /// <typeparam name="T">The type of element in the <see cref="IEnumerable{T}"/>.</typeparam>
    /// <param name="enumerable">The enumerable to count.</param>
    /// <returns>
    /// If relatively cheap to compute, the number of elements in the parameter
    /// <paramref name="enumerable"/>; otherwise, <see langword="null"/>.</returns>
    [NonNegativeValue]
    public static int? TryCount<T>([NoEnumeration] this IEnumerable<T>? enumerable) =>
        enumerable switch
        {
            null => null,
            string { Length: var length } => length,
            IReadOnlyCollection<T> { Count: var count } => count,
            _ => enumerable.TryGetNonEnumeratedCount(out var count) ? count : null,
        };
}
