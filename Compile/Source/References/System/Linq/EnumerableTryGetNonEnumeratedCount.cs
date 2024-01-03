// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace System.Linq;

#if !NET6_0_OR_GREATER
/// <summary>The backport of the TryGetNonEnumeratedCount method for <see cref="IEnumerable{T}"/>.</summary>
static partial class EnumerableTryGetNonEnumeratedCount
{
    /// <summary>Attempts to determine the number of elements in a sequence without forcing an enumeration.</summary>
    /// <remarks><para>
    /// The method performs a series of type tests, identifying common subtypes whose count
    /// can be determined without enumerating; this includes <see cref="ICollection{T}"/>,
    /// <see cref="ICollection"/> as well as internal types used in the LINQ implementation.
    /// </para><para>
    /// The method is typically a constant-time operation, but ultimately this depends on the complexity
    /// characteristics of the underlying collection implementation.
    /// </para></remarks>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence that contains elements to be counted.</param>
    /// <param name="count">
    /// When this method returns, contains the number of elements in <paramref name="source"/>,
    /// or 0 if the count couldn't be determined without enumeration.
    /// </param>
    /// <returns>A sequence of tuples with elements taken from the first and second sequence, in that order.</returns>
    [Pure]
    public static bool TryGetNonEnumeratedCount<TSource>(
        [NoEnumeration] this IEnumerable<TSource> source,
        [NonNegativeValue] out int count
    ) =>
        source switch
        {
            ICollection c => (count = c.Count) is var _,
            ICollection<TSource> c => (count = c.Count) is var _,
            _ => !((count = 0) is var _),
        };
}
#endif
