// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent CheckNamespace StructCanBeMadeReadOnly
namespace Emik.Morsels;
#pragma warning disable 8500, MA0102, SA1137
using static Math;
using static Span;

/// <summary>Provides methods for determining similarity between two sequences.</summary>
static partial class Similarity
{
    const StringComparison DefaultCharComparer = StringComparison.Ordinal;

    /// <summary>Calculates the Jaro similarity between two strings.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static double Jaro(this string? left, string? right) =>
        string.Equals(left, right, DefaultCharComparer) ? 1 : left.Jaro(right, EqualityComparer<char>.Default);

    /// <summary>Calculates the Jaro similarity between two strings.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static double Jaro(this string? left, string? right, [InstantHandle] Func<char, char, bool>? comparer) =>
        ReferenceEquals(left, right) ? 1 :
        left is null || right is null ? 0 :
        Jaro(left, right, static x => x.Length, static (x, i) => x[i], comparer);

    /// <summary>Calculates the Jaro similarity between two strings.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static double Jaro(this string? left, string? right, IEqualityComparer<char>? comparer) =>
        left.Jaro(right, comparer is null ? null : comparer.Equals);

    /// <summary>Calculates the Jaro-Emik similarity between two strings.</summary>
    /// <remarks><para>Like <see cref="Jaro(string, string)"/>, but with a bias to common sub-slices.</para></remarks>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static double JaroEmik(this string? left, string? right) =>
        string.Equals(left, right, DefaultCharComparer) ? 1 : left.JaroEmik(right, EqualityComparer<char>.Default);

    /// <summary>Calculates the Jaro-Emik similarity between two strings.</summary>
    /// <remarks><para>
    /// Like <see cref="Jaro(string, string, Func{char, char, bool})"/>, but with a bias to common sub-slices.
    /// </para></remarks>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static double JaroEmik(this string? left, string? right, [InstantHandle] Func<char, char, bool>? comparer) =>
        ReferenceEquals(left, right) ? 1 :
        left is null || right is null ? 0 :
        JaroEmik(left, right, static x => x.Length, static (x, i) => x[i], comparer);

    /// <summary>Calculates the Jaro-Emik similarity between two strings.</summary>
    /// <remarks><para>
    /// Like <see cref="Jaro(string, string, IEqualityComparer{char})"/>, but with a bias to common sub-slices.
    /// </para></remarks>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static double JaroEmik(this string? left, string? right, IEqualityComparer<char>? comparer) =>
        left.JaroEmik(right, comparer is null ? null : comparer.Equals);

    /// <summary>Calculates the Jaro-Winkler similarity between two strings.</summary>
    /// <remarks><para>Like <see cref="Jaro(string, string)"/>, but with a bias to common prefixes.</para></remarks>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static double JaroWinkler(this string? left, string? right) =>
        string.Equals(left, right, DefaultCharComparer) ? 1 : left.JaroWinkler(right, EqualityComparer<char>.Default);

    /// <summary>Calculates the Jaro-Winkler similarity between two strings.</summary>
    /// <remarks><para>
    /// Like <see cref="Jaro(string, string, Func{char, char, bool})"/>, but with a bias to common prefixes.
    /// </para></remarks>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static double JaroWinkler(
        this string? left,
        string? right,
        [InstantHandle] Func<char, char, bool>? comparer
    ) =>
        ReferenceEquals(left, right) ? 1 :
        left is null || right is null ? 0 :
        JaroWinkler(left, right, static x => x.Length, static (x, i) => x[i], comparer);

    /// <summary>Calculates the Jaro-Winkler similarity between two strings.</summary>
    /// <remarks><para>
    /// Like <see cref="Jaro(string, string, IEqualityComparer{char})"/>, but with a bias to common prefixes.
    /// </para></remarks>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static double JaroWinkler(
        this string? left,
        string? right,
        IEqualityComparer<char>? comparer
    ) =>
        left.JaroWinkler(right, comparer is null ? null : comparer.Equals);

    /// <summary>Calculates the Jaro similarity between two sequences.</summary>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static double Jaro<T>(this IList<T>? left, IList<T>? right) => left.Jaro(right, EqualityComparer<T>.Default);

    /// <summary>Calculates the Jaro similarity between two sequences.</summary>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static double Jaro<T>(this IList<T>? left, IList<T>? right, [InstantHandle] Func<T, T, bool>? comparer) =>
        ReferenceEquals(left, right) ? 1 :
        left is null || right is null ? 0 :
        Jaro(left, right, static x => x.Count, static (x, i) => x[i], comparer);

    /// <summary>Calculates the Jaro similarity between two sequences.</summary>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static double Jaro<T>(this IList<T>? left, IList<T>? right, IEqualityComparer<T>? comparer) =>
        left.Jaro(right, comparer is null ? null : comparer.Equals);

    /// <summary>Calculates the Jaro-Emik similarity between two sequences.</summary>
    /// <remarks><para>
    /// Like <see cref="Jaro{T}(IList{T}, IList{T})"/>, but with a bias to common sub-slices.
    /// </para></remarks>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static double JaroEmik<T>(this IList<T>? left, IList<T>? right) =>
        left.Jaro(right, EqualityComparer<T>.Default);

    /// <summary>Calculates the Jaro-Emik similarity between two sequences.</summary>
    /// <remarks><para>
    /// Like <see cref="Jaro{T}(IList{T}, IList{T}, Func{T, T, bool})"/>, but with a bias to common sub-slices.
    /// </para></remarks>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static double JaroEmik<T>(
        this IList<T>? left,
        IList<T>? right,
        [InstantHandle] Func<T, T, bool>? comparer
    ) =>
        ReferenceEquals(left, right) ? 1 :
        left is null || right is null ? 0 :
        Jaro(left, right, static x => x.Count, static (x, i) => x[i], comparer);

    /// <summary>Calculates the Jaro-Emik similarity between two sequences.</summary>
    /// <remarks><para>
    /// Like <see cref="Jaro{T}(IList{T}, IList{T}, IEqualityComparer{T})"/>, but with a bias to common sub-slices.
    /// </para></remarks>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static double JaroEmik<T>(this IList<T>? left, IList<T>? right, IEqualityComparer<T>? comparer) =>
        left.Jaro(right, comparer is null ? null : comparer.Equals);

    /// <summary>Calculates the Jaro-Winkler similarity between two sequences.</summary>
    /// <remarks><para>
    /// Like <see cref="Jaro{T}(IList{T}, IList{T})"/>, but with a bias to common prefixes.
    /// </para></remarks>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static double JaroWinkler<T>(this IList<T>? left, IList<T>? right) =>
        left.JaroWinkler(right, EqualityComparer<T>.Default);

    /// <summary>Calculates the Jaro-Winkler similarity between two sequences.</summary>
    /// <remarks><para>
    /// Like <see cref="Jaro{T}(IList{T}, IList{T}, Func{T, T, bool})"/>, but with a bias to common prefixes.
    /// </para></remarks>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static double JaroWinkler<T>(
        this IList<T>? left,
        IList<T>? right,
        [InstantHandle] Func<T, T, bool>? comparer
    ) =>
        ReferenceEquals(left, right) ? 1 :
        left is null || right is null ? 0 :
        JaroWinkler(left, right, static x => x.Count, static (x, i) => x[i], comparer);

    /// <summary>Calculates the Jaro-Winkler similarity between two sequences.</summary>
    /// <remarks><para>
    /// Like <see cref="Jaro{T}(IList{T}, IList{T}, IEqualityComparer{T})"/>, but with a bias to common prefixes.
    /// </para></remarks>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static double JaroWinkler<T>(this IList<T>? left, IList<T>? right, IEqualityComparer<T>? comparer) =>
        left.JaroWinkler(right, comparer is null ? null : comparer.Equals);

    /// <summary>Calculates the Jaro similarity between two sequences.</summary>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static double Jaro<T>(this ReadOnlySpan<T> left, ReadOnlySpan<T> right, IEqualityComparer<T>? comparer)
#if UNMANAGED_SPAN || CSHARPREPL
        where T : unmanaged
#endif
        =>
            left.Jaro(right, comparer is null ? null : comparer.Equals);

    /// <summary>Calculates the Jaro similarity between two sequences.</summary>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static unsafe double Jaro<T>(
        this ReadOnlySpan<T> left,
        ReadOnlySpan<T> right,
        [InstantHandle] Func<T, T, bool>? comparer = null
    )
#if UNMANAGED_SPAN || CSHARPREPL
        where T : unmanaged
#endif
    {
        // ReSharper disable once WrongIndentSize
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
        var l = left.Pointer;
        var r = right.Pointer;
#else
        fixed (T* l = left)
        fixed (T* r = right)
#endif
        return Jaro(
            new Fat<T>(l, left.Length),
            new(r, right.Length),
            static x => x.Length,
            static (x, i) => x[i],
            comparer
        );
    }

    /// <summary>Calculates the Jaro-Emik similarity between two sequences.</summary>
    /// <remarks><para>
    /// Like <see cref="Jaro{T}(ReadOnlySpan{T}, ReadOnlySpan{T}, IEqualityComparer{T})"/>,
    /// but with a bias to common sub-slices.
    /// </para></remarks>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static double
        JaroEmik<T>(this ReadOnlySpan<T> left, ReadOnlySpan<T> right, IEqualityComparer<T>? comparer)
#if UNMANAGED_SPAN || CSHARPREPL
        where T : unmanaged
#endif
        =>
            left.JaroEmik(right, comparer is null ? null : comparer.Equals);

    /// <summary>Calculates the Jaro-Emik similarity between two sequences.</summary>
    /// <remarks><para>
    /// Like <see cref="Jaro{T}(ReadOnlySpan{T}, ReadOnlySpan{T}, Func{T, T, bool})"/>,
    /// but with a bias to common sub-slices.
    /// </para></remarks>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static unsafe double JaroEmik<T>(
        this ReadOnlySpan<T> left,
        ReadOnlySpan<T> right,
        [InstantHandle] Func<T, T, bool>? comparer = null
    )
#if UNMANAGED_SPAN || CSHARPREPL
        where T : unmanaged
#endif
    {
        // ReSharper disable once WrongIndentSize
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
        var l = left.Pointer;
        var r = right.Pointer;
#else
        fixed (T* l = left)
        fixed (T* r = right)
#endif
        return JaroEmik(
            new Fat<T>(l, left.Length),
            new(r, right.Length),
            static x => x.Length,
            static (x, i) => x[i],
            comparer
        );
    }

    /// <summary>Calculates the Jaro-Winkler similarity between two sequences.</summary>
    /// <remarks><para>
    /// Like <see cref="Jaro{T}(ReadOnlySpan{T}, ReadOnlySpan{T}, IEqualityComparer{T})"/>,
    /// but with a bias to common prefixes.
    /// </para></remarks>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static double JaroWinkler<T>(
        this ReadOnlySpan<T> left,
        ReadOnlySpan<T> right,
        IEqualityComparer<T>? comparer
    )
#if UNMANAGED_SPAN || CSHARPREPL
        where T : unmanaged
#endif
        =>
            left.JaroWinkler(right, comparer is null ? null : comparer.Equals);

    /// <summary>Calculates the Jaro-Winkler similarity between two sequences.</summary>
    /// <remarks><para>
    /// Like <see cref="Jaro{T}(ReadOnlySpan{T}, ReadOnlySpan{T}, Func{T, T, bool})"/>,
    /// but with a bias to common prefixes.
    /// </para></remarks>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [Pure, ValueRange(0, 1)]
    public static unsafe double JaroWinkler<T>(
        this ReadOnlySpan<T> left,
        ReadOnlySpan<T> right,
        [InstantHandle] Func<T, T, bool>? comparer = null
    )
#if UNMANAGED_SPAN || CSHARPREPL
        where T : unmanaged
#endif
    {
        // ReSharper disable once WrongIndentSize
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
        var l = left.Pointer;
        var r = right.Pointer;
#else
        fixed (T* l = left)
        fixed (T* r = right)
#endif
        return JaroWinkler(
            new Fat<T>(l, left.Length),
            new(r, right.Length),
            static x => x.Length,
            static (x, i) => x[i],
            comparer
        );
    }

    /// <summary>Calculates the Jaro similarity between two sequences.</summary>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <typeparam name="TItem">The type of item within the sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="counter">The function that gets the count.</param>
    /// <param name="indexer">The function that acts as an indexer.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [MustUseReturnValue, ValueRange(0, 1)]
    public static double Jaro<T, TItem>(
        T left,
        T right,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int> counter,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> indexer,
        [InstantHandle] Func<TItem, TItem, bool>? comparer = null
    ) =>
        Jaro(left, right, counter(left), counter(right), indexer, comparer);

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
    [MustUseReturnValue, ValueRange(0, 1)]
    public static double Jaro<T, TItem>(
        T left,
        T right,
        [NonNegativeValue] int leftLength,
        [NonNegativeValue] int rightLength,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> indexer,
        [InstantHandle] Func<TItem, TItem, bool>? comparer = null
    ) =>
        JaroInner(left, right, leftLength, rightLength, indexer, comparer ?? EqualityComparer<TItem>.Default.Equals);

    /// <summary>Calculates the Jaro-Emik similarity between two sequences.</summary>
    /// <remarks><para>
    /// Like <see cref="Jaro{T, TItem}(T, T, Func{T, int}, Func{T, int, TItem}, Func{TItem, TItem, bool})"/>,
    /// but with a bias to common sub-slices.
    /// </para></remarks>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <typeparam name="TItem">The type of item within the sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="counter">The function that gets the count.</param>
    /// <param name="indexer">The function that acts as an indexer.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [MustUseReturnValue, ValueRange(0, 1)]
    public static double JaroEmik<T, TItem>(
        T left,
        T right,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int> counter,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> indexer,
        [InstantHandle] Func<TItem, TItem, bool>? comparer = null
    ) =>
        JaroEmik(left, right, counter(left), counter(right), indexer, comparer);

    /// <summary>Calculates the Jaro-Emik similarity between two instances.</summary>
    /// <remarks><para>
    /// Like <see cref="Jaro{T, TItem}(T, T, int, int, Func{T, int, TItem}, Func{TItem, TItem, bool})"/>,
    /// but with a bias to common sub-slices.
    /// </para></remarks>
    /// <typeparam name="T">The type of instance.</typeparam>
    /// <typeparam name="TItem">The type of item within the instance.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="leftLength">The left-hand side's length.</param>
    /// <param name="rightLength">The right-hand side's length.</param>
    /// <param name="indexer">The function that acts as an indexer.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [MustUseReturnValue, ValueRange(0, 1)]
    public static double JaroEmik<T, TItem>(
        T left,
        T right,
        [NonNegativeValue] int leftLength,
        [NonNegativeValue] int rightLength,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> indexer,
        [InstantHandle] Func<TItem, TItem, bool>? comparer = null
    )
    {
        comparer ??= EqualityComparer<TItem>.Default.Equals;

        var jaro = JaroInner(left, right, leftLength, rightLength, indexer, comparer);

        if (leftLength is 0 || rightLength is 0)
            return jaro;

        var slice = Slice(left, right, leftLength, rightLength, indexer, comparer) * Grade(leftLength, rightLength);

        return Max(jaro, slice);
    }

    /// <summary>Calculates the Jaro-Winkler similarity between two sequences.</summary>
    /// <remarks><para>
    /// Like <see cref="Jaro{T, TItem}(T, T, Func{T, int}, Func{T, int, TItem}, Func{TItem, TItem, bool})"/>,
    /// but with a bias to common prefixes.
    /// </para></remarks>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <typeparam name="TItem">The type of item within the sequence.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="counter">The function that gets the count.</param>
    /// <param name="indexer">The function that acts as an indexer.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [MustUseReturnValue, ValueRange(0, 1)]
    public static double JaroWinkler<T, TItem>(
        T left,
        T right,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int> counter,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> indexer,
        [InstantHandle] Func<TItem, TItem, bool>? comparer = null
    ) =>
        JaroWinkler(left, right, counter(left), counter(right), indexer, comparer);

    /// <summary>Calculates the Jaro-Winkler similarity between two instances.</summary>
    /// <remarks><para>
    /// Like <see cref="Jaro{T, TItem}(T, T, int, int, Func{T, int, TItem}, Func{TItem, TItem, bool})"/>,
    /// but with a bias to common prefixes.
    /// </para></remarks>
    /// <typeparam name="T">The type of instance.</typeparam>
    /// <typeparam name="TItem">The type of item within the instance.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="leftLength">The left-hand side's length.</param>
    /// <param name="rightLength">The right-hand side's length.</param>
    /// <param name="indexer">The function that acts as an indexer.</param>
    /// <param name="comparer">The comparer to determine equality, or <see cref="EqualityComparer{T}.Default"/>.</param>
    /// <returns>Between 0.0 and 1.0 (higher value means more similar).</returns>
    [MustUseReturnValue, ValueRange(0, 1)]
    public static double JaroWinkler<T, TItem>(
        T left,
        T right,
        [NonNegativeValue] int leftLength,
        [NonNegativeValue] int rightLength,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> indexer,
        [InstantHandle] Func<TItem, TItem, bool>? comparer = null
    )
    {
        comparer ??= EqualityComparer<TItem>.Default.Equals;

        var jaroDistance = JaroInner(left, right, leftLength, rightLength, indexer, comparer);
        var prefixLength = NumberOfEquals(left, right, leftLength, rightLength, indexer, comparer);
        var distance = JaroWinklerDistance(jaroDistance, prefixLength);

        return Min(distance, 1);
    }

    [MustUseReturnValue, ValueRange(0, 1)]
    static double JaroAllocated<T, TItem>(
        Span<byte> visited,
        (T, T, int, int, Func<T, int, TItem>, Func<TItem, TItem, bool>) args
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

    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue, ValueRange(0, 1)]
    static double JaroInner<T, TItem>(
        T left,
        T right,
        [NonNegativeValue] int leftLength,
        [NonNegativeValue] int rightLength,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> indexer,
        [InstantHandle] Func<TItem, TItem, bool> comparer
    ) =>
        leftLength is 0 && rightLength is 0 ? 1 :
            leftLength is 0 || rightLength is 0 ? 0 :
                leftLength is 1 && rightLength is 1 ?
                    EqualsAt(left, right, 0, 0, comparer, indexer) ? 1 : 0 :
                    Allocate(rightLength, (left, right, leftLength, rightLength, indexer, comparer), Fun<T, TItem>());

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static SpanFunc<byte, (T, T, int, int, Func<T, int, TItem>, Func<TItem, TItem, bool>), double> Fun<T, TItem>() =>
        static (span, tuple) => JaroAllocated(span, tuple);

    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue, NonNegativeValue]
    static int Next<T, TItem>(
        Span<byte> visited,
        T left,
        T right,
        [ValueRange(2, int.MaxValue)] int leftLength,
        [ValueRange(2, int.MaxValue)] int rightLength,
        [NonNegativeValue] int leftIndex,
        [NonNegativeValue] int rightPreviousIndex,
        [InstantHandle] Func<TItem, TItem, bool> comparer,
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
        Span<byte> visited,
        T leftLength,
        T rightLength,
        [ValueRange(2, int.MaxValue)] int aLen,
        [ValueRange(2, int.MaxValue)] int bLen,
        [NonNegativeValue] int leftIndex,
        [NonNegativeValue] int rightIndex,
        [InstantHandle] Func<TItem, TItem, bool> comparer,
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
        [InstantHandle] Func<TItem, TItem, bool> comparer,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> indexer
    ) =>
        comparer(indexer(left, leftIndex), indexer(right, rightIndex));

    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue, ValueRange(0, 1)]
    static double Slice<T, TItem>(
        T left,
        T right,
        [NonNegativeValue] int leftLength,
        [NonNegativeValue] int rightLength,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> indexer,
        [InstantHandle] Func<TItem, TItem, bool> comparer
    )
    {
        var score = 0;
        var isLeftSmaller = leftLength < rightLength;

        var small = isLeftSmaller ? left : right;
        var smallLength = isLeftSmaller ? leftLength : rightLength;

        var big = isLeftSmaller ? right : left;
        var bigLength = isLeftSmaller ? rightLength : leftLength;

        for (var i = 0; i < bigLength; i++)
        {
            var highestPossibleScore = Min(bigLength - i - 1, smallLength);

            if (score >= highestPossibleScore)
                break;

            score = SliceInner(big, small, bigLength, smallLength, indexer, comparer, i, score);
        }

        return (double)score / smallLength;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue, NonNegativeValue]
    static int SliceInner<T, TItem>(
        T big,
        T small,
        [NonNegativeValue] int bigLength,
        [NonNegativeValue] int smallLength,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> indexer,
        [InstantHandle] Func<TItem, TItem, bool> comparer,
        [NonNegativeValue] int i,
        [NonNegativeValue] int score
    )
    {
        var lower = -1;

        for (var j = 0; j < smallLength && i + j < bigLength; j++)
            if (EqualsAt(big, small, i + j, j, comparer, indexer))
                score = Max(score, j - lower);
            else
                lower = j;

        return score;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue, NonNegativeValue]
    static int NumberOfEquals<T, TItem>(
        T left,
        T right,
        [ValueRange(2, int.MaxValue)] int leftLength,
        [ValueRange(2, int.MaxValue)] int rightLength,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<T, int, TItem> indexer,
        [InstantHandle] Func<TItem, TItem, bool> comparer
    )
    {
        var sharedLength = Min(leftLength, rightLength);

        for (var sharedIndex = 0; sharedIndex < sharedLength; sharedIndex++)
            if (!EqualsAt(left, right, sharedIndex, sharedIndex, comparer, indexer))
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
    static int MaxBound(
        [ValueRange(2, int.MaxValue)] int leftLength,
        [ValueRange(2, int.MaxValue)] int rightLength,
        [NonNegativeValue] int leftIndex
    ) =>
        Min(SearchRange(leftLength, rightLength) + leftIndex, rightLength - 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining), NonNegativeValue, Pure]
    static int MinBound(
        [ValueRange(2, int.MaxValue)] int leftLength,
        [ValueRange(2, int.MaxValue)] int rightLength,
        [NonNegativeValue] int leftIndex
    ) =>
        SearchRange(leftLength, rightLength) < leftIndex ? Max(0, leftIndex - SearchRange(leftLength, rightLength)) : 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining), NonNegativeValue, Pure]
    static int SearchRange(
        [ValueRange(2, int.MaxValue)] int leftLength,
        [ValueRange(2, int.MaxValue)] int rightLength
    ) =>
        Max(leftLength, rightLength) / 2 - 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure, ValueRange(0, 1)]
    static double JaroDistance(
        [ValueRange(2, int.MaxValue)] int leftLength,
        [ValueRange(2, int.MaxValue)] int rightLength,
        [NonNegativeValue] double matchCount,
        [NonNegativeValue] int transpositionCount
    ) =>
        1 / 3.0 * (matchCount / leftLength + matchCount / rightLength + (matchCount - transpositionCount) / matchCount);

    [MustUseReturnValue, ValueRange(0, 1)]
    static double Grade([NonNegativeValue] int leftLength, [NonNegativeValue] int rightLength) =>
        1 - 1.0 / Min(leftLength + 1, rightLength + 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure, ValueRange(0, 1)]
    static double JaroWinklerDistance([ValueRange(0, 1)] double jaroDistance, [NonNegativeValue] int prefixLength) =>
        jaroDistance + 0.1 * prefixLength * (1.0 - jaroDistance);

    /// <summary>Represents a pointer with a length.</summary>
    [StructLayout(LayoutKind.Auto)]
#if !NO_READONLY_STRUCTS
    readonly
#endif
        unsafe partial struct Fat<T>
#if UNMANAGED_SPAN || CSHARPREPL
        where T : unmanaged
#endif
    {
        const string E = "Value must be non-negative and less than the length.";

        readonly void* _pointer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fat(void* pointer, [NonNegativeValue] int length)
        {
            _pointer = pointer;
            Length = length;
        }

        public T this[int i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
            get => (uint)i < (uint)Length ? ((T*)_pointer)[i] : throw new ArgumentOutOfRangeException(nameof(i), i, E);
        }

        public int Length { [MethodImpl(MethodImplOptions.AggressiveInlining), NonNegativeValue, Pure] get; }
    }
}
#pragma warning restore SA1114
