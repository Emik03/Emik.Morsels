// SPDX-License-Identifier: MPL-2.0
#if !NET20 && !NET30
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;
#pragma warning disable CS1574, CS1580
/// <summary>Extension methods that negate functions from <see cref="Enumerable"/>.</summary>
static partial class NegatedEnumerable
{
    /// <summary>Negated <see cref="Enumerable.Distinct{T}(IEnumerable{T}, IEqualityComparer{T})"/>.</summary>
    /// <remarks><para>
    /// Filters out unique elements within an <see cref="Enumerable{T}"/>.
    /// Each duplicate appears exactly once within the returned value.
    /// </para></remarks>
    /// <typeparam name="T">The type of <see cref="IEnumerable{T}"/> and <see cref="IEqualityComparer{T}"/>.</typeparam>
    /// <param name="source">The source to filter.</param>
    /// <param name="comparer">The comparer to assess distinctiveness.</param>
    /// <returns>The parameter <paramref name="source"/>, filtering out all elements that only appear once.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<T> DistinctDuplicates<T>(
        [NoEnumeration] this IEnumerable<T> source,
        IEqualityComparer<T>? comparer = null
    ) =>
        source.GroupDuplicates(comparer).Select(x => x.Key);

    /// <summary>Negated <see cref="Enumerable.Distinct{T}(IEnumerable{T}, IEqualityComparer{T})"/>.</summary>
    /// <remarks><para>
    /// Filters out unique elements within an <see cref="Enumerable{T}"/>.
    /// Each duplicate appears two or more times within the returned value.
    /// </para></remarks>
    /// <typeparam name="T">The type of <see cref="IEnumerable{T}"/> and <see cref="IEqualityComparer{T}"/>.</typeparam>
    /// <param name="source">The source to filter.</param>
    /// <param name="comparer">The comparer to assess distinctiveness.</param>
    /// <returns>The parameter <paramref name="source"/>, filtering out all elements that only appear once.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<T> Duplicates<T>([NoEnumeration] this IEnumerable<T> source, IEqualityComparer<T>? comparer = null) =>
        source.GroupDuplicates(comparer).SelectMany(x => x);

    /// <summary>Negated <see cref="Enumerable.Distinct{T}(IEnumerable{T}, IEqualityComparer{T})"/>.</summary>
    /// <remarks><para>Filters out unique elements within an <see cref="Enumerable{T}"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="IEnumerable{T}"/> and <see cref="IEqualityComparer{T}"/>.</typeparam>
    /// <param name="source">The source to filter.</param>
    /// <param name="comparer">The comparer to assess distinctiveness.</param>
    /// <returns>The parameter <paramref name="source"/>, filtering out all elements that only appear once.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<IGrouping<T, T>> GroupDuplicates<T>(
        [NoEnumeration] this IEnumerable<T> source,
        IEqualityComparer<T>? comparer = null
    ) =>
        source.GroupBy(x => x, comparer).Where(x => x.Skip(1).Any());

    /// <summary>Negated <see cref="Enumerable.SkipWhile{T}(IEnumerable{T}, Func{T, int, bool})"/>.</summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}" /> that contains the elements from the input sequence starting at
    /// the first element in the linear series that does pass the test specified by the predicate.
    /// </returns>
    /// <inheritdoc cref="Enumerable.SkipWhile{T}(IEnumerable{T}, Func{T, int, bool})"/>
    [LinqTunnel, Pure]
    public static IEnumerable<T> SkipUntil<T>([NoEnumeration] this IEnumerable<T> source, Func<T, bool> predicate) =>
        source.SkipWhile(Not1(predicate));

    /// <summary>Negated <see cref="Enumerable.SelectMany{T}(IEnumerable{T}, Func{T, IEnumerable{T}})"/>.</summary>
    /// <remarks><para>
    /// Splits the <see cref="IEnumerable{T}"/> into multiple <see cref="IEnumerable{T}"/>
    /// instances in at most the specified length.
    /// </para></remarks>
    /// <typeparam name="T">The type of the <see cref="IEnumerable{T}"/>.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to chop into slices.</param>
    /// <param name="count">The maximum length of any given returned <see cref="IEnumerable{T}"/> instances.</param>
    /// <returns>The wrapper of the parameter <paramref name="source"/> that returns slices of it.</returns>
    [Pure]
    public static IEnumerable<IEnumerable<T>> SplitEvery<T>(
        [InstantHandle] IEnumerable<T> source,
        [ValueRange(1, int.MaxValue)] int count
    )
    {
        using var e = source.GetEnumerator();

        while (e.MoveNext())
            yield return SplitEvery(e, count);
    }

    /// <summary>Negated <see cref="Enumerable.TakeWhile{T}(IEnumerable{T}, Func{T, int, bool})"/>.</summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}" /> that contains the elements from the input
    /// sequence that occur before the element at which the test no longer fails.
    /// </returns>
    /// <inheritdoc cref="Enumerable.TakeWhile{T}(IEnumerable{T}, Func{T, int, bool})"/>
    [LinqTunnel, Pure]
    public static IEnumerable<T> TakeUntil<T>([NoEnumeration] this IEnumerable<T> source, Func<T, bool> predicate) =>
        source.TakeWhile(Not1(predicate));

    /// <summary>Negated <see cref="Enumerable.TakeWhile{T}(IEnumerable{T}, Func{T, int, bool})"/>.</summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}" /> that contains elements from
    /// the input sequence that do not satisfy the condition.
    /// </returns>
    /// <inheritdoc cref="Enumerable.TakeWhile{T}(IEnumerable{T}, Func{T, int, bool})"/>
    [LinqTunnel, Pure]
    public static IEnumerable<T> TakeUntil<T>(
        [NoEnumeration] this IEnumerable<T> source,
        Func<T, int, bool> predicate
    ) =>
        source.TakeWhile(Not2(predicate));

    /// <summary>Negated <see cref="Enumerable.Where{T}(IEnumerable{T}, Func{T, bool})"/>.</summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}" /> that contains elements from
    /// the input sequence that do not satisfy the condition.
    /// </returns>
    /// <inheritdoc cref="Enumerable.Where{T}(IEnumerable{T}, Func{T, bool})"/>
    [LinqTunnel, Pure]
    public static IEnumerable<T> Omit<T>([NoEnumeration] this IEnumerable<T> source, Func<T, bool> predicate) =>
        source.Where(Not1(predicate));

    /// <summary>Negated <see cref="Enumerable.Where{T}(IEnumerable{T}, Func{T, int, bool})"/>.</summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}" /> that contains elements from
    /// the input sequence that do not satisfy the condition.
    /// </returns>
    /// <inheritdoc cref="Enumerable.Where{T}(IEnumerable{T}, Func{T, int, bool})"/>
    [LinqTunnel, Pure]
    public static IEnumerable<T> Omit<T>(
        [NoEnumeration] this IEnumerable<T> source,
        Func<T, int, bool> predicate
    ) =>
        source.Where(Not2(predicate));

    static IEnumerable<T> SplitEvery<T>(IEnumerator<T> e, [ValueRange(1, int.MaxValue)] int count)
    {
        do
        {
            yield return e.Current;

            count--;
        } while (count > 0 && e.MoveNext());
    }
}
#endif
