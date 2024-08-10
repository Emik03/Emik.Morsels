// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods for nullable types and booleans.</summary>
static partial class Conditionals
{
#if NET7_0_OR_GREATER
    /// <summary>Converts <see cref="bool"/> to <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The type of number to convert to.</typeparam>
    /// <param name="that">Whether or not to return the one value, or zero.</param>
    /// <returns>
    /// The value <see cref="INumberBase{T}.One"/> or <see cref="INumberBase{T}.Zero"/>,
    /// based on the value of <paramref name="that"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T As<T>(this bool that)
        where T : INumberBase<T> =>
        that ? T.One : T.Zero;
#endif
#if NETCOREAPP || ROSLYN
    /// <summary>Determines whether two sequences are equal.</summary>
    /// <typeparam name="TDerived">The type of element in the compared array.</typeparam>
    /// <typeparam name="TBase">The type of element contained by the collection.</typeparam>
    /// <param name="first">The first <see cref="ImmutableArray{TBase}"/> to compare.</param>
    /// <param name="second">The second <see cref="ImmutableArray{TDerived}"/> to compare.</param>
    /// <returns>
    /// The value <see langword="true"/> if both sequences have the same
    /// values, or are both default; otherwise, <see langword="false"/>.
    /// </returns>
    [MustUseReturnValue]
    public static bool GuardedSequenceEqual<TDerived, TBase>(
        this ImmutableArray<TBase> first,
        ImmutableArray<TDerived> second
    )
        where TDerived : TBase =>
        first.IsDefault || second.IsDefault ? first.IsDefault && second.IsDefault : first.SequenceEqual(second);

    /// <summary>Determines whether two sequences are equal according to an equality comparer.</summary>
    /// <typeparam name="TDerived">The type of element in the compared array.</typeparam>
    /// <typeparam name="TBase">The type of element contained by the collection.</typeparam>
    /// <param name="first">The first <see cref="ImmutableArray{TBase}"/> to compare.</param>
    /// <param name="second">The second <see cref="ImmutableArray{TDerived}"/> to compare.</param>
    /// <param name="comparer">The comparer to use to check for equality.</param>
    /// <returns>
    /// The value <see langword="true"/> if both sequences have the same
    /// values, or are both default; otherwise, <see langword="false"/>.
    /// </returns>
    [MustUseReturnValue]
    public static bool GuardedSequenceEqual<TDerived, TBase>(
        this ImmutableArray<TBase> first,
        ImmutableArray<TDerived> second,
        Func<TBase, TBase, bool>? comparer
    )
        where TDerived : TBase =>
        first.IsDefault || second.IsDefault ? first.IsDefault && second.IsDefault :
        comparer is null ? first.SequenceEqual(second) : first.SequenceEqual(second, comparer);

    /// <summary>Determines whether two sequences are equal according to an equality comparer.</summary>
    /// <typeparam name="TDerived">The type of element in the compared array.</typeparam>
    /// <typeparam name="TBase">The type of element contained by the collection.</typeparam>
    /// <param name="first">The first <see cref="ImmutableArray{TBase}"/> to compare.</param>
    /// <param name="second">The second <see cref="ImmutableArray{TDerived}"/> to compare.</param>
    /// <param name="comparer">The comparer to use to check for equality.</param>
    /// <returns>
    /// The value <see langword="true"/> if both sequences have the same
    /// values, or are both default; otherwise, <see langword="false"/>.
    /// </returns>
    [MustUseReturnValue]
    public static bool GuardedSequenceEqual<TDerived, TBase>(
        this ImmutableArray<TBase> first,
        ImmutableArray<TDerived> second,
        IEqualityComparer<TBase>? comparer
    )
        where TDerived : TBase =>
        first.IsDefault || second.IsDefault ? first.IsDefault && second.IsDefault :
        comparer is null ? first.SequenceEqual(second) : first.SequenceEqual(second, comparer);
#endif

#if !NET20 && !NET30
    /// <summary>Filters an <see cref="IEnumerable{T}"/> to only non-null values.</summary>
    /// <typeparam name="T">The type of value to filter.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <returns>A filtered <see cref="IEnumerable{T}"/> with strictly non-null values.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<T> Filter<T>([NoEnumeration] this IEnumerable<T?>? iterable) =>
#pragma warning disable 8619
        iterable?.Where(x => x is not null) ?? [];
#pragma warning restore 8619

    /// <summary>Filters an <see cref="IEnumerable{T}"/> to only non-null values.</summary>
    /// <typeparam name="T">The type of value to filter.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <returns>A filtered <see cref="IEnumerable{T}"/> with strictly non-null values.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<T> Filter<T>([NoEnumeration] this IEnumerable<T?>? iterable)
        where T : struct =>
#pragma warning disable 8629
        iterable?.Where(x => x.HasValue).Select(x => x.Value) ?? [];
#pragma warning restore 8629
#endif
}
