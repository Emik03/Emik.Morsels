// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;
#if !NET20 && !NET30 && !NET471_OR_GREATER && !NETSTANDARD1_6_OR_GREATER && !NETCOREAPP
/// <summary>Adds support for Append and Prepend in lower frameworks.</summary>
static partial class Attachments
{
    /// <summary>Appends a value to the end of the sequence.</summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values.</param>
    /// <param name="element">The value to append to <paramref name="source"/>.</param>
    /// <returns>A new sequence that ends with <paramref name="element"/>.</returns>
    public static IEnumerable<TSource> Append<TSource>(this IEnumerable<TSource> source, TSource element) =>
        source.Concat(element.Yield());

    /// <summary>Prepends a value to the end of the sequence.</summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values.</param>
    /// <param name="element">The value to prepend to <paramref name="source"/>.</param>
    /// <returns>A new sequence that starts with <paramref name="element"/>.</returns>
    public static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource element) =>
        element.Yield().Concat(source);
}
#endif
