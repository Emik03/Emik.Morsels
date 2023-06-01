// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace System.Linq;

#if !NETCOREAPP3_0_OR_GREATER
/// <summary>The backport of Zip methods for <see cref="IEnumerable{T}"/>.</summary>
static partial class EnumerableZip
{
    /// <summary>
    /// Produces a sequence of tuples with elements from the two specified sequences.
    /// </summary>
    /// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
    /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
    /// <param name="first">The first sequence to merge.</param>
    /// <param name="second">The second sequence to merge.</param>
    /// <returns>A sequence of tuples with elements taken from the first and second sequence, in that order.</returns>
    public static IEnumerable<(TFirst First, TSecond Second)> Zip<TFirst, TSecond>(
        this IEnumerable<TFirst> first,
        IEnumerable<TSecond> second
    )
    {
        using var e1 = first.GetEnumerator();
        using var e2 = second.GetEnumerator();

        while (e1.MoveNext() && e2.MoveNext())
            yield return (e1.Current, e2.Current);
    }

    /// <summary>
    /// Produces a sequence of tuples with elements from the three specified sequences.
    /// </summary>
    /// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
    /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
    /// <typeparam name="TThird">The type of the elements of the third input sequence.</typeparam>
    /// <param name="first">The first sequence to merge.</param>
    /// <param name="second">The second sequence to merge.</param>
    /// <param name="third">The third sequence to merge.</param>
    /// <returns>
    /// A sequence of tuples with elements taken from the first, second, and third sequences, in that order.
    /// </returns>
    public static IEnumerable<(TFirst First, TSecond Second, TThird Third)> Zip<TFirst, TSecond, TThird>(
        this IEnumerable<TFirst> first,
        IEnumerable<TSecond> second,
        IEnumerable<TThird> third
    )
    {
        using var e1 = first.GetEnumerator();
        using var e2 = second.GetEnumerator();
        using var e3 = third.GetEnumerator();

        while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext())
            yield return (e1.Current, e2.Current, e3.Current);
    }

    /// <summary>
    /// Applies a specified function to the corresponding elements of two sequences,
    /// producing a sequence of the results.
    /// </summary>
    /// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
    /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
    /// <typeparam name="TResult">The type of the elements of the result sequence.</typeparam>
    /// <param name="first">The first sequence to merge.</param>
    /// <param name="second">The second sequence to merge.</param>
    /// <param name="resultSelector">A function that specifies how to merge the elements from the two sequences.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains merged elements of two input sequences.</returns>
    public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(
        this IEnumerable<TFirst> first,
        IEnumerable<TSecond> second,
        Func<TFirst, TSecond, TResult> resultSelector
    )
    {
        using var e1 = first.GetEnumerator();
        using var e2 = second.GetEnumerator();

        while (e1.MoveNext() && e2.MoveNext())
            yield return resultSelector(e1.Current, e2.Current);
    }
}
#endif
