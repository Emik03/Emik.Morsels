// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace System.Linq;
#if !NET9_0_OR_GREATER
/// <summary>The backport of the Index method for <see cref="IEnumerable{T}"/>.</summary>
static partial class EnumerableIndex
{
    /// <summary>Returns an enumerable that incorporates the element's index into a tuple.</summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <param name="source">The source enumerable providing the elements.</param>
    /// <returns>The enumerable that incorporates the element's index into a tuple.</returns>
    public static IEnumerable<(int Index, TSource Item)> Index<TSource>(this IEnumerable<TSource> source) =>
        source is TSource[] { Length: 0 } ? [] : source.Select((x, i) => (i, x));
}
#endif
