// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
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
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure]
    public static double Jaro(
        this string? left,
        string? right,
        IEqualityComparer<char>? comparer = null
    ) =>
        left is null || right is null
            ? left is null && right is null ? 1 : 0
            : Jaro(left, right, static x => x.Length, static (x, i) => x[i], comparer);

    /// <summary>Calculates the Jaro-Winkler similarity between two strings.</summary>
    /// <remarks><para>Like <see cref="Jaro"/>, but with a bias to common prefixes.</para></remarks>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure]
    public static double JaroWinkler(
        this string? left,
        string? right,
        IEqualityComparer<char>? comparer = null
    ) =>
        left is null || right is null
            ? left is null && right is null ? 1 : 0
            : JaroWinkler(left, right, static x => x.Length, static (x, i) => x[i], comparer);

    /// <summary>Calculates the Jaro similarity between two sequences.</summary>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure]
    public static double Jaro<T>(
        this IList<T>? left,
        IList<T>? right,
        IEqualityComparer<T>? comparer = null
    ) =>
        left is null || right is null
            ? left is null && right is null ? 1 : 0
            : Jaro(left, right, static x => x.Count, static (x, i) => x[i], comparer);

    /// <summary>Calculates the Jaro-Winkler similarity between two sequences.</summary>
    /// <remarks><para>Like <see cref="Jaro"/>, but with a bias to common prefixes.</para></remarks>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure]
    public static double JaroWinkler<T>(
        this IList<T>? left,
        IList<T>? right,
        IEqualityComparer<T>? comparer = null
    ) =>
        left is null || right is null
            ? left is null && right is null ? 1 : 0
            : JaroWinkler(left, right, static x => x.Count, static (x, i) => x[i], comparer);

    /// <summary>Calculates the Jaro similarity between two sequences.</summary>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <typeparam name="TItem">The type of item within the sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="counter">The function that gets the count.</param>
    /// <param name="indexer">The function that acts as an indexer.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [MustUseReturnValue]
    public static double Jaro<T, TItem>(
        T left,
        T right,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int> counter,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> indexer,
        IEqualityComparer<TItem>? comparer = null
    ) =>
        Jaro(left, right, counter(left), counter(right), indexer, comparer);

    /// <summary>Calculates the Jaro-Winkler similarity between two sequences.</summary>
    /// <remarks><para>Like <see cref="Jaro"/>, but with a bias to common prefixes.</para></remarks>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <typeparam name="TItem">The type of item within the sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="counter">The function that gets the count.</param>
    /// <param name="indexer">The function that acts as an indexer.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [MustUseReturnValue]
    public static double JaroWinkler<T, TItem>(
        T left,
        T right,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int> counter,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> indexer,
        IEqualityComparer<TItem>? comparer = null
    ) =>
        JaroWinkler(left, right, counter(left), counter(right), indexer, comparer);

    /// <summary>Calculates the Jaro similarity between two instances.</summary>
    /// <typeparam name="T">The type of instance.</typeparam>
    /// <typeparam name="TItem">The type of item within the instance.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="leftLength">The left-hand side's length.</param>
    /// <param name="rightLength">The right-hand side's length.</param>
    /// <param name="indexer">The function that acts as an indexer.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [MustUseReturnValue]
    public static double Jaro<T, TItem>(
        T left,
        T right,
        [NonNegativeValue] int leftLength,
        [NonNegativeValue] int rightLength,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> indexer,
        IEqualityComparer<TItem>? comparer = null
    ) =>
        JaroInner(left, right, leftLength, rightLength, indexer, comparer ?? EqualityComparer<TItem>.Default);

    /// <summary>Calculates the Jaro-Winkler similarity between two instances.</summary>
    /// <remarks><para>Like <see cref="Jaro"/>, but with a bias to common prefixes.</para></remarks>
    /// <typeparam name="T">The type of instance.</typeparam>
    /// <typeparam name="TItem">The type of item within the instance.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="leftLength">The left-hand side's length.</param>
    /// <param name="rightLength">The right-hand side's length.</param>
    /// <param name="indexer">The function that acts as an indexer.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [MustUseReturnValue]
    public static double JaroWinkler<T, TItem>(
        T left,
        T right,
        [NonNegativeValue] int leftLength,
        [NonNegativeValue] int rightLength,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> indexer,
        IEqualityComparer<TItem>? comparer = null
    )
    {
        comparer ??= EqualityComparer<TItem>.Default;

        var jaroDistance = JaroInner(left, right, leftLength, rightLength, indexer, comparer);
        var prefixLength = NumberOfEquals(left, right, leftLength, rightLength, indexer, comparer);
        var distance = JaroWinklerDistance(jaroDistance, prefixLength);

        return Min(distance, 1);
    }

    [MustUseReturnValue, ValueRange(0, 1)]
    static double JaroAllocated<T, TItem>(
        in Span<byte> visited,
        (T, T, int, int, Func<T, int, TItem>, IEqualityComparer<TItem>) args
    )
    {
        var (left, right, leftLength, rightLength, indexer, comparer) = args;
        int rightPreviousIndex = 0, transpositionCount = 0;
        double matchCount = 0;
        visited.Clear();

        for (var i = 0; i < leftLength; i++)
            if (InBounds(leftLength, rightLength, i))
                rightPreviousIndex = Next(
                    visited,
                    left,
                    right,
                    leftLength,
                    rightLength,
                    i,
                    rightPreviousIndex,
                    comparer,
                    indexer,
                    ref matchCount,
                    ref transpositionCount
                );

        return matchCount is 0 ? 0 : JaroDistance(leftLength, rightLength, matchCount, transpositionCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    static double JaroInner<T, TItem>(
        T left,
        T right,
        [NonNegativeValue] int leftLength,
        [NonNegativeValue] int rightLength,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> indexer,
        IEqualityComparer<TItem> comparer
    ) =>
        leftLength is 0 && rightLength is 0 ? 1 :
            leftLength is 0 || rightLength is 0 ? 0 :
                leftLength is 1 && rightLength is 1 ?
                    comparer.Equals(indexer(left, 0), indexer(right, 0)) ? 1 : 0 :
                    Span.Allocate(
                        rightLength,
                        (left, right, leftLength, rightLength, indexer, comparer),
                        JaroAllocated
                    );

    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue, NonNegativeValue]
    static int Next<T, TItem>(
        in Span<byte> visited,
        T left,
        T right,
        [ValueRange(2, int.MaxValue)] int leftLength,
        [ValueRange(2, int.MaxValue)] int rightLength,
        [NonNegativeValue] int leftIndex,
        [NonNegativeValue] int rightPreviousIndex,
        IEqualityComparer<TItem> comparer,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> indexer,
        [NonNegativeValue] ref double matchCount,
        [NonNegativeValue] ref int transpositionCount
    )
    {
        for (var rightIndex = 0; rightIndex < rightLength; rightIndex++)
        {
            if (!ShouldProceed(visited, left, right, leftLength, rightLength, leftIndex, rightIndex, comparer, indexer))
                continue;

            visited[rightIndex]++;
            matchCount++;

            if (rightIndex < rightPreviousIndex)
                transpositionCount++;

            return rightIndex;
        }

        return rightPreviousIndex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    static bool ShouldProceed<T, TItem>(
        in Span<byte> visited,
        T leftLength,
        T rightLength,
        [ValueRange(2, int.MaxValue)] int aLen,
        [ValueRange(2, int.MaxValue)] int bLen,
        [NonNegativeValue] int leftIndex,
        [NonNegativeValue] int rightIndex,
        IEqualityComparer<TItem> comparer,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> indexer
    ) =>
        InBounds(aLen, bLen, leftIndex, rightIndex) &&
        visited[rightIndex] is 0 &&
        EqualsAt(leftLength, rightLength, leftIndex, rightIndex, comparer, indexer);

    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    static bool EqualsAt<T, TItem>(
        T left,
        T right,
        [NonNegativeValue] int leftIndex,
        [NonNegativeValue] int rightIndex,
        IEqualityComparer<TItem> comparer,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> indexer
    ) =>
        comparer.Equals(indexer(left, leftIndex), indexer(right, rightIndex));

    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
    static int NumberOfEquals<T, TItem>(
        T left,
        T right,
        [ValueRange(2, int.MaxValue)] int leftLength,
        [ValueRange(2, int.MaxValue)] int rightLength,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> indexer,
        IEqualityComparer<TItem> comparer
    )
    {
        var sharedLength = Min(leftLength, rightLength);

        for (var sharedIndex = 0; sharedIndex < sharedLength; sharedIndex++)
            if (!comparer.Equals(indexer(left, sharedIndex), indexer(right, sharedIndex)))
                return sharedIndex;

        return sharedLength;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static bool InBounds(
        [ValueRange(2, int.MaxValue)] int leftLength,
        [ValueRange(2, int.MaxValue)] int rightLength,
        [NonNegativeValue] int leftIndex
    ) =>
        MinBound(leftLength, rightLength, leftIndex) <= MaxBound(leftLength, rightLength, leftIndex);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static bool InBounds(
        [ValueRange(2, int.MaxValue)] int leftLength,
        [ValueRange(2, int.MaxValue)] int rightLength,
        [NonNegativeValue] int leftIndex,
        [NonNegativeValue] int rightIndex
    ) =>
        MinBound(leftLength, rightLength, leftIndex) <= rightIndex &&
        rightIndex <= MaxBound(leftLength, rightLength, leftIndex);

    [MethodImpl(MethodImplOptions.AggressiveInlining), NonNegativeValue, Pure]
    static int MaxBound(int leftLength, int rightLength, int leftIndex) =>
        Min(SearchRange(leftLength, rightLength) + leftIndex, rightLength - 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining), NonNegativeValue, Pure]
    static int MinBound(
        [ValueRange(2, int.MaxValue)] int leftLength,
        [ValueRange(2, int.MaxValue)] int rightLength,
        [NonNegativeValue] int leftIndex
    ) =>
        SearchRange(leftLength, rightLength) < leftIndex ? Max(0, leftIndex - SearchRange(leftLength, rightLength)) : 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining), NonNegativeValue, Pure]
    static int SearchRange([NonNegativeValue] int leftLength, [NonNegativeValue] int rightLength) =>
        Max(leftLength, rightLength) / 2 - 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure, ValueRange(0, 1)]
    static double JaroDistance(
        [ValueRange(2, int.MaxValue)] int leftLength,
        [ValueRange(2, int.MaxValue)] int rightLength,
        [NonNegativeValue] double matchCount,
        [NonNegativeValue] int transpositionCount
    ) =>
        1 / 3.0 * (matchCount / leftLength + matchCount / rightLength + (matchCount - transpositionCount) / matchCount);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure, ValueRange(0, 1)]
    static double JaroWinklerDistance([ValueRange(0, 1)] double jaroDistance, [NonNegativeValue] int prefixLength) =>
        jaroDistance + 0.1 * prefixLength * (1.0 - jaroDistance);
}
#pragma warning restore SA1114
