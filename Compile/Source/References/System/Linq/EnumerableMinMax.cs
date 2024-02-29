// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace System.Linq;

#if !NET6_0_OR_GREATER
/// <summary>The backport of the MinBy and MaxBy methods for <see cref="IEnumerable{T}"/>.</summary>
static partial class EnumerableMinMax
{
    /// <summary>
    /// Returns the minimum value in a generic sequence according to a specified key selector function.
    /// </summary>
    /// <remarks><para>
    /// If <typeparamref name="TKey"/> is a reference type and the source sequence is empty or contains
    /// only values that are <see langword="null"/>, this method returns <see langword="null"/>.
    /// </para></remarks>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <typeparam name="TKey">The type of key to compare elements by.</typeparam>
    /// <param name="source">A sequence of values to determine the minimum value of.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <returns>The value with the minimum key in the sequence.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">
    /// No key extracted from <paramref name="source"/> implements the
    /// <see cref="IComparable"/> or <see cref="IComparable{TKey}"/> interface.
    /// </exception>
    public static TSource? MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) =>
        MinBy(source, keySelector, null);

    /// <summary>
    /// Returns the minimum value in a generic sequence according to a specified key selector function.
    /// </summary>
    /// <remarks><para>
    /// If <typeparamref name="TKey"/> is a reference type and the source sequence is empty or contains
    /// only values that are <see langword="null" />, this method returns <see langword="null"/>.
    /// </para></remarks>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <typeparam name="TKey">The type of key to compare elements by.</typeparam>
    /// <param name="source">A sequence of values to determine the minimum value of.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">The <see cref="IComparer{TKey}" /> to compare keys.</param>
    /// <returns>The value with the minimum key in the sequence.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">
    /// No key extracted from <paramref name="source" /> implements the <see cref="IComparable"/>
    /// or <see cref="IComparable{TKey}"/> interface.
    /// </exception>
#pragma warning disable MA0051 // ReSharper disable once CognitiveComplexity
    public static TSource? MinBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IComparer<TKey>? comparer
    )
#pragma warning restore MA0051
    {
        comparer ??= Comparer<TKey>.Default;

        using var e = source.GetEnumerator();

        if (!e.MoveNext())
            return default(TSource) is null ? default : throw CannotBeEmpty;

        var value = e.Current;
        var key = keySelector(value);

        if (default(TKey) is null)
        {
            if (key is null)
            {
                var firstValue = value;

                do
                {
                    if (!e.MoveNext()) // All keys are null, surface the first element.
                        return firstValue;

                    value = e.Current;
                    key = keySelector(value);
                } while (key is null);
            }

            while (e.MoveNext())
            {
                var nextValue = e.Current;
                var nextKey = keySelector(nextValue);

                if (nextKey is null || comparer.Compare(nextKey, key) >= 0)
                    continue;

                key = nextKey;
                value = nextValue;
            }
        }
        else
        {
            // ReSharper disable once PossibleUnintendedReferenceComparison
            if (comparer == Comparer<TKey>.Default)
                while (e.MoveNext())
                {
                    var nextValue = e.Current;
                    var nextKey = keySelector(nextValue);

                    if (Comparer<TKey>.Default.Compare(nextKey, key) >= 0)
                        continue;

                    key = nextKey;
                    value = nextValue;
                }
            else
                while (e.MoveNext())
                {
                    var nextValue = e.Current;
                    var nextKey = keySelector(nextValue);

                    if (comparer.Compare(nextKey, key) >= 0)
                        continue;

                    key = nextKey;
                    value = nextValue;
                }
        }

        return value;
    }

    /// <summary>
    /// Returns the maximum value in a generic sequence according to a specified key selector function.
    /// </summary>
    /// <remarks><para>
    /// If <typeparamref name="TKey"/> is a reference type and the source sequence is empty or contains
    /// only values that are <see langword="null"/>, this method returns <see langword="null"/>.
    /// </para></remarks>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <typeparam name="TKey">The type of key to compare elements by.</typeparam>
    /// <param name="source">A sequence of values to determine the maximum value of.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <exception cref="ArgumentException">
    /// No key extracted from <paramref name="source" /> implements the <see cref="IComparable"/>
    /// or <see cref="IComparable{TKey}"/> interface.
    /// </exception>
    /// <returns>The value with the maximum key in the sequence.</returns>
    public static TSource? MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) =>
        MaxBy(source, keySelector, null);

    /// <summary>
    /// Returns the maximum value in a generic sequence according to a specified key selector function.
    /// </summary>
    /// <remarks><para>
    /// If <typeparamref name="TKey"/> is a reference type and the source sequence is empty or contains
    /// only values that are <see langword="null"/>, this method returns <see langword="null"/>.
    /// </para></remarks>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <typeparam name="TKey">The type of key to compare elements by.</typeparam>
    /// <param name="source">A sequence of values to determine the maximum value of.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">The <see cref="IComparer{TKey}" /> to compare keys.</param>
    /// <exception cref="ArgumentException">
    /// No key extracted from <paramref name="source"/> implements the <see cref="IComparable"/>
    /// or <see cref="IComparable{TKey}"/> interface.
    /// </exception>
    /// <returns>The value with the maximum key in the sequence.</returns>
#pragma warning disable MA0051 // ReSharper disable once CognitiveComplexity
    public static TSource? MaxBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IComparer<TKey>? comparer
    )
#pragma warning restore MA0051
    {
        comparer ??= Comparer<TKey>.Default;

        using var e = source.GetEnumerator();

        if (!e.MoveNext())
            return default(TSource) is null ? default : throw CannotBeEmpty;

        var value = e.Current;
        var key = keySelector(value);

        if (default(TKey) is null)
        {
            if (key is null)
            {
                var firstValue = value;

                do
                {
                    if (!e.MoveNext()) // All keys are null, surface the first element.
                        return firstValue;

                    value = e.Current;
                    key = keySelector(value);
                } while (key is null);
            }

            while (e.MoveNext())
            {
                var nextValue = e.Current;
                var nextKey = keySelector(nextValue);

                if (nextKey is null || comparer.Compare(nextKey, key) <= 0)
                    continue;

                key = nextKey;
                value = nextValue;
            }
        }
        else
        {
            // ReSharper disable once PossibleUnintendedReferenceComparison
            if (comparer == Comparer<TKey>.Default)
                while (e.MoveNext())
                {
                    var nextValue = e.Current;
                    var nextKey = keySelector(nextValue);

                    if (Comparer<TKey>.Default.Compare(nextKey, key) <= 0)
                        continue;

                    key = nextKey;
                    value = nextValue;
                }
            else
                while (e.MoveNext())
                {
                    var nextValue = e.Current;
                    var nextKey = keySelector(nextValue);

                    if (comparer.Compare(nextKey, key) <= 0)
                        continue;

                    key = nextKey;
                    value = nextValue;
                }
        }

        return value;
    }
}
#endif
