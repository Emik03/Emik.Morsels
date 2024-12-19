// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace System.Linq;
#if !NET7_0_OR_GREATER
/// <summary>The backport of the Order and OrderDescending methods for <see cref="IEnumerable{T}"/>.</summary>
static partial class EnumerableOrder
{
    /// <summary>Sorts the elements of a sequence in ascending order.</summary>
    /// <typeparam name="T">The type of elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <returns>An <see cref="IOrderedEnumerable{TElement}"/> whose elements are sorted.</returns>
    [LinqTunnel, Pure]
    public static IOrderedEnumerable<T> Order<T>(this IEnumerable<T> source) => Order(source, null);

    /// <summary>Sorts the elements of a sequence in ascending order.</summary>
    /// <typeparam name="T">The type of elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
    /// <returns>An <see cref="IOrderedEnumerable{TElement}"/> whose elements are sorted.</returns>
    [LinqTunnel, Pure]
    public static IOrderedEnumerable<T> Order<T>(this IEnumerable<T> source, IComparer<T>? comparer) =>
        source.OrderBy(x => x, comparer);

    /// <summary>Sorts the elements of a sequence in descending order.</summary>
    /// <typeparam name="T">The type of elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <returns>An <see cref="IOrderedEnumerable{TElement}"/> whose elements are sorted.</returns>
    [LinqTunnel, Pure]
    public static IOrderedEnumerable<T> OrderDescending<T>(this IEnumerable<T> source) => OrderDescending(source, null);

    /// <summary>Sorts the elements of a sequence in descending order.</summary>
    /// <typeparam name="T">The type of elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
    /// <returns>An <see cref="IOrderedEnumerable{TElement}"/> whose elements are sorted.</returns>
    [LinqTunnel, Pure]
    public static IOrderedEnumerable<T> OrderDescending<T>(this IEnumerable<T> source, IComparer<T>? comparer) =>
        source.OrderByDescending(x => x, comparer);
}
#endif
