// SPDX-License-Identifier: MPL-2.0
namespace Emik.Morsels;
#pragma warning disable SA1114
using static Math;

/// <summary>Provides methods for determining similarity between two sequences.</summary>
static class Similarity
{
    /// <summary>Calculates the Jaro similarity between two strings.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <param name="winkler">If <see langword="true"/>, gives a boost to strings that have a common prefix.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure]
    public static double Jaro(
        this string? left,
        string? right,
        IEqualityComparer<char>? comparer = null,
        bool winkler = false
    ) =>
        left is null || right is null
            ? left is null && right is null ? 1 : 0
            : Jaro(left, right, static x => x.Length, static (x, i) => x[i], comparer, winkler);

    /// <summary>Calculates the Jaro similarity between two sequences.</summary>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <param name="winkler">If <see langword="true"/>, gives a boost to strings that have a common prefix.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure]
    public static double Jaro<T>(
        this IList<T>? left,
        IList<T>? right,
        IEqualityComparer<T>? comparer = null,
        bool winkler = false
    ) =>
        left is null || right is null
            ? left is null && right is null ? 1 : 0
            : Jaro(left, right, static x => x.Count, static (x, i) => x[i], comparer, winkler);

    /// <summary>Calculates the Jaro similarity between two sequences.</summary>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <typeparam name="TItem">The type of item within the sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="counter">The function that gets the count.</param>
    /// <param name="ind">The function that acts as an indexer.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <param name="winkler">If <see langword="true"/>, gives a boost to strings that have a common prefix.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [MustUseReturnValue]
    public static double Jaro<T, TItem>(
        T left,
        T right,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int> counter,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> ind,
        IEqualityComparer<TItem>? comparer = null,
        bool winkler = false
    ) =>
        Jaro(left, right, counter(left), counter(right), ind, comparer, winkler);

    /// <summary>Calculates the Jaro similarity between two instances.</summary>
    /// <typeparam name="T">The type of instance.</typeparam>
    /// <typeparam name="TItem">The type of item within the instance.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="leftLength">The left-hand side's length.</param>
    /// <param name="rightLength">The right-hand side's length.</param>
    /// <param name="ind">The function that acts as an indexer.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <param name="winkler">If <see langword="true"/>, gives a boost to strings that have a common prefix.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [MustUseReturnValue]
    public static double Jaro<T, TItem>(
        T left,
        T right,
        [NonNegativeValue] int leftLength,
        [NonNegativeValue] int rightLength,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> ind,
        IEqualityComparer<TItem>? comparer = null,
        bool winkler = false
    ) =>
        (comparer ?? EqualityComparer<TItem>.Default) is var c &&
        winkler
            ? JaroWinkler(left, right, leftLength, rightLength, c, ind)
            : JaroVanilla(left, right, leftLength, rightLength, c, ind);

    [Pure]
    static double JaroWinkler<T, TItem>(
        T a,
        T b,
        [NonNegativeValue] int aLen,
        [NonNegativeValue] int bLen,
        IEqualityComparer<TItem> eq,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> ind
    )
    {
        var jaroDistance = JaroVanilla(a, b, aLen, bLen, eq, ind);
        var prefixLength = NumberOfEquals(a, b, aLen, bLen, eq, ind);
        var distance = JaroWinklerDistance(jaroDistance, prefixLength);

        return Min(distance, 1);
    }

    [Pure]
    static double JaroVanilla<T, TItem>(
        T a,
        T b,
        [NonNegativeValue] int aLen,
        [NonNegativeValue] int bLen,
        IEqualityComparer<TItem> eq,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> ind
    ) =>
        aLen is 0 && bLen is 0 ? 1 :
        aLen is 0 || bLen is 0 ? 0 :
        aLen is 1 && bLen is 1 ? eq.Equals(ind(a, 0), ind(b, 0)) ? 1 : 0 :
        Span.Allocate(bLen, (a, b, aLen, bLen, eq, ind), JaroAllocated);

    [Pure, ValueRange(0, 1)]
    static double JaroAllocated<T, TItem>(
        Span<byte> buf,
        (T, T, int, int, IEqualityComparer<TItem>, Func<T, int, TItem>) tup
    )
    {
        var (a, b, aLen, bLen, eq, ind) = tup;
        int lastB = 0, transpose = 0;
        double matches = 0;

        buf.Clear();

        for (var i = 0; i < aLen; i++)
            if (InBounds(aLen, bLen, i))
                lastB = Next(buf, a, b, bLen, aLen, i, lastB, eq, ind, ref matches, ref transpose);

        return matches is 0 ? 0 : JaroDistance(aLen, bLen, matches, transpose);
    }

    [NonNegativeValue, Pure]
    static int Next<T, TItem>(
        Span<byte> buf,
        T a,
        T b,
        [ValueRange(2, int.MaxValue)] int bLen,
        [ValueRange(2, int.MaxValue)] int aLen,
        [NonNegativeValue] int i,
        [NonNegativeValue] int lastB,
        IEqualityComparer<TItem> c,
        [RequireStaticDelegate(IsError = true)] Func<T, int, TItem> index,
        [NonNegativeValue] ref double matches,
        [NonNegativeValue] ref int transpositions
    )
    {
        for (var j = 0; j < bLen; j++)
        {
            if (!ShouldProceed(buf, a, b, aLen, bLen, i, j, c, index))
                continue;

            buf[j]++;
            matches++;

            if (j < lastB)
                transpositions++;

            return j;
        }

        return lastB;
    }

    [Pure]
    static bool ShouldProceed<T, TItem>(
        Span<byte> buf,
        T a,
        T b,
        [ValueRange(2, int.MaxValue)] int aLen,
        [ValueRange(2, int.MaxValue)] int bLen,
        [NonNegativeValue] int i,
        [NonNegativeValue] int j,
        IEqualityComparer<TItem> eq,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> indexer
    ) =>
        InBounds(aLen, bLen, i, j) && buf[j] is 0 && EqualsAt(a, b, i, j, eq, indexer);

    [Pure]
    static bool EqualsAt<T, TItem>(
        T a,
        T b,
        [NonNegativeValue] int i,
        [NonNegativeValue] int j,
        IEqualityComparer<TItem> eq,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> indexer
    ) =>
        eq.Equals(indexer(a, i), indexer(b, j));

    [Pure]
    static int NumberOfEquals<T, TItem>(
        T a,
        T b,
        [ValueRange(2, int.MaxValue)] int aLen,
        [ValueRange(2, int.MaxValue)] int bLen,
        IEqualityComparer<TItem> eq,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> ind
    )
    {
        var len = Min(aLen, bLen);

        for (var i = 0; i < len; i++)
            if (!eq.Equals(ind(a, i), ind(b, i)))
                return i;

        return len;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static bool InBounds(
        [ValueRange(2, int.MaxValue)] int aLen,
        [ValueRange(2, int.MaxValue)] int bLen,
        [NonNegativeValue] int i
    ) =>
        MinBound(aLen, bLen, i) <= MaxBound(aLen, bLen, i);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static bool InBounds(
        [ValueRange(2, int.MaxValue)] int aLen,
        [ValueRange(2, int.MaxValue)] int bLen,
        [NonNegativeValue] int i,
        [NonNegativeValue] int j
    ) =>
        MinBound(aLen, bLen, i) <= j && j <= MaxBound(aLen, bLen, i);

    [MethodImpl(MethodImplOptions.AggressiveInlining), NonNegativeValue, Pure]
    static int MaxBound(int aLen, int bLen, int i) => Min(SearchRange(aLen, bLen) + i, bLen - 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining), NonNegativeValue, Pure]
    static int MinBound(
        [ValueRange(2, int.MaxValue)] int aLen,
        [ValueRange(2, int.MaxValue)] int bLen,
        [NonNegativeValue] int i
    ) =>
        SearchRange(aLen, bLen) < i ? Max(0, i - SearchRange(aLen, bLen)) : 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining), NonNegativeValue, Pure]
    static int SearchRange([NonNegativeValue] int aLen, [NonNegativeValue] int bLen) => Max(aLen, bLen) / 2 - 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure, ValueRange(0, 1)]
    static double JaroDistance(
        [ValueRange(2, int.MaxValue)] int aLen,
        [ValueRange(2, int.MaxValue)] int bLen,
        [NonNegativeValue] double matches,
        [NonNegativeValue] int transpositions
    ) =>
        1 / 3.0 * (matches / aLen + matches / bLen + (matches - transpositions) / matches);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure, ValueRange(0, 1)]
    static double JaroWinklerDistance([ValueRange(0, 1)] double jaroDistance, [NonNegativeValue] int prefixLength) =>
        jaroDistance + 0.1 * prefixLength * (1.0 - jaroDistance);
}
#pragma warning restore SA1114
