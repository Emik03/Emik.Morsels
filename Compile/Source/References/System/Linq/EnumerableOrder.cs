// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace System.Linq;
#if !NET7_0_OR_GREATER
/// <summary>The backport of the Order and OrderDescending methods for <see cref="IEnumerable{T}"/>.</summary>
static partial class EnumerableOrder
{
    /// <param name="source">A sequence of values to order.</param>
    /// <typeparam name="T">The type of elements of <paramref name="source"/>.</typeparam>
    extension<T>(IEnumerable<T> source)
    {
        /// <summary>Sorts the elements of a sequence in ascending order.</summary>
        /// <returns>An <see cref="IOrderedEnumerable{TElement}"/> whose elements are sorted.</returns>
        [LinqTunnel, Pure]
        public IOrderedEnumerable<T> Order() => Order(source, null);

        /// <summary>Sorts the elements of a sequence in ascending order.</summary>
        /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
        /// <returns>An <see cref="IOrderedEnumerable{TElement}"/> whose elements are sorted.</returns>
        [LinqTunnel, Pure]
        public IOrderedEnumerable<T> Order(IComparer<T>? comparer) =>
            source.OrderBy(x => x, comparer);

        /// <summary>Sorts the elements of a sequence in descending order.</summary>
        /// <returns>An <see cref="IOrderedEnumerable{TElement}"/> whose elements are sorted.</returns>
        [LinqTunnel, Pure]
        public IOrderedEnumerable<T> OrderDescending() => OrderDescending(source, null);

        /// <summary>Sorts the elements of a sequence in descending order.</summary>
        /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
        /// <returns>An <see cref="IOrderedEnumerable{TElement}"/> whose elements are sorted.</returns>
        [LinqTunnel, Pure]
        public IOrderedEnumerable<T> OrderDescending(IComparer<T>? comparer) =>
            source.OrderByDescending(x => x, comparer);
    }
}
#endif
