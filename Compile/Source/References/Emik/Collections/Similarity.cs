// SPDX-License-Identifier: MPL-2.0
namespace Emik.Morsels;
#pragma warning disable CA1508 // Closures take care of this.
using static Math;

/// <summary>Provides methods for determining similarity between two sequences.</summary>
static class Similarity
{
    /// <summary>Calculates the Jaro similarity between two strings.</summary>
    /// <typeparam name="T">The type of element in both <see cref="IEnumerable{T}"/> instances.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <param name="useWinkler">If <see langword="true"/>, gives a boost to strings that have a common prefix.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    public static double Jaro<T>(
        this IEnumerable<T> left,
        IEnumerable<T> right,
        IEqualityComparer<T>? comparer = null,
        bool useWinkler = false
    ) =>
        left.ToCollectionLazily() is var a &&
        right.ToCollectionLazily() is var b &&
        (comparer ?? EqualityComparer<T>.Default) is var c &&
        useWinkler
            ? JaroWinkler(a, b, c)
            : JaroVanilla(a, b, c);

    static double JaroWinkler<T>(ICollection<T> a, ICollection<T> b, IEqualityComparer<T> c)
    {
        var jaroDistance = a.Jaro(b, c);
        var prefixLength = a.Zip(b).TakeWhile(a => c.Equals(a.First, a.Second)).Count();
        var jaroWinklerDistance = jaroDistance + 0.1 * prefixLength * (1.0 - jaroDistance);

        return Min(jaroWinklerDistance, 1);
    }

    static double JaroVanilla<T>(ICollection<T> a, ICollection<T> b, IEqualityComparer<T> c) =>
        a.Count is 0 && b.Count is 0 ? 1 :
        a.Count is 0 || b.Count is 0 ? 0 :
        a.Count is 1 && b.Count is 1 ? a.SequenceEqual(b) ? 1 : 0 :
        JaroInner(a, b, c);

    static double JaroInner<T>(ICollection<T> a, ICollection<T> b, IEqualityComparer<T> c)
    {
        int bMatchIndex = 0,
            transpositions = 0;

        double matches = 0;

        BitArray bConsumed = new(b.Count);

        bool WithinBounds(T _, int i) => MinBound(a, b, i) <= MaxBound(a, b, i);

        ControlFlow Process(T ax, T bx, int i, int j)
        {
            if (MinBound(a, b, i) > j || j > MaxBound(a, b, i) || !c.Equals(ax, bx) || bConsumed[j])
                return ControlFlow.Continue;

            bConsumed[j] = true;
            matches++;

            if (j < bMatchIndex)
                transpositions++;

            bMatchIndex = j;
            return ControlFlow.Break;
        }

        a.Where(WithinBounds).For((aElem, i) => b.For((bElem, j) => Process(aElem, bElem, i, j)));

        return matches is 0 ? 0 : Equivalence(a, b, matches, transpositions);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static double Equivalence<T>(ICollection<T> a, ICollection<T> b, double matches, int transpositions) =>
        1 / 3.0 * (matches / a.Count + matches / b.Count + (matches - transpositions) / matches);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int MaxBound<T>(ICollection<T> a, ICollection<T> b, int i) => Min(b.Count - 1, i + SearchRange(a, b));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int MinBound<T>(ICollection<T> a, ICollection<T> b, int i) =>
        i > SearchRange(a, b) ? Max(0, i - SearchRange(a, b)) : 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int SearchRange<T>(ICollection<T> a, ICollection<T> b) => Max(a.Count, b.Count) / 2 - 1;
}
