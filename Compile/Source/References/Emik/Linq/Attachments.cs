// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;
#if NETFRAMEWORK && !NET471_OR_GREATER
/// <summary>Adds support for Append and Prepend in lower frameworks.</summary>
static partial class Attachments
{
    /// <param name="source">A sequence of values.</param>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    extension<TSource>(IEnumerable<TSource> source)
    {
        /// <summary>Appends a value to the end of the sequence.</summary>
        /// <param name="element">The value to append to <paramref name="source"/>.</param>
        /// <returns>A new sequence that ends with <paramref name="element"/>.</returns>
        public IEnumerable<TSource> Append(TSource element) =>
            source.Concat(element.Yield());

        /// <summary>Prepends a value to the end of the sequence.</summary>
        /// <param name="element">The value to prepend to <paramref name="source"/>.</param>
        /// <returns>A new sequence that starts with <paramref name="element"/>.</returns>
        public IEnumerable<TSource> Prepend(TSource element) =>
            element.Yield().Concat(source);
    }
}
#endif
