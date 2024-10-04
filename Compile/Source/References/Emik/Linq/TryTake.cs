// SPDX-License-Identifier: MPL-2.0

// ReSharper disable CheckNamespace ConditionIsAlwaysTrueOrFalse RedundantNameQualifier ReturnTypeCanBeEnumerable.Global UseIndexFromEndExpression
namespace Emik.Morsels;

/// <summary>Extension methods to attempt to grab ranges from enumerables.</summary>
static partial class TryTake
{
    /// <summary>Takes the last item lazily, or a fallback value.</summary>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    /// <param name="fallback">The fallback item.</param>
    /// <returns>The last item, or the parameter <paramref name="fallback"/>.</returns>
    [Pure]
    public static T EnumerateOr<T>([InstantHandle] this IEnumerable<T> iterable, T fallback)
    {
#if NETCOREAPP || ROSLYN
        if (iterable is ImmutableArray<T> { IsDefaultOrEmpty: true })
            return fallback;
#endif
        using var iterator = iterable.GetEnumerator();

        if (!iterator.MoveNext())
            return fallback;

        var last = iterator.Current;

        while (iterator.MoveNext())
            last = iterator.Current;

        return last;
    }
#if !(NET20 || NET30)
    /// <summary>Takes the first item, or a fallback value.</summary>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    /// <param name="fallback">The fallback item.</param>
    /// <returns>The first item, or the parameter <paramref name="fallback"/>.</returns>
    [MustUseReturnValue]
    public static T FirstOr<T>([InstantHandle] this IEnumerable<T> iterable, T fallback)
    {
        switch (iterable)
        {
            case string str:
                return str.Length is 0 ? fallback : Reinterpret<T>(str[0]);
#if NETCOREAPP || ROSLYN
            case ImmutableArray<T> array:
                return array.IsDefaultOrEmpty ? fallback : array[0];
#endif
            case IList<T> list:
                return list.Count is 0 ? fallback : list[0];
            case IReadOnlyList<T> list:
                return list.Count is 0 ? fallback : list[0];
            case var _ when iterable.TryGetNonEnumeratedCount(out var count):
                return count is 0 ? fallback : iterable.First();
            default:
            {
                using var iterator = iterable.GetEnumerator();
                return iterator.MoveNext() ? iterator.Current : fallback;
            }
        }
    }

    /// <summary>Takes the last item, or a fallback value.</summary>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    /// <param name="fallback">The fallback item.</param>
    /// <returns>The last item, or the parameter <paramref name="fallback"/>.</returns>
    [MustUseReturnValue]
    public static T LastOr<T>([InstantHandle] this IEnumerable<T> iterable, T fallback) =>
        iterable switch
        {
            string str => str is [.., var last] ? Reinterpret<T>(last) : fallback,
#if NETCOREAPP || ROSLYN
            ImmutableArray<T> array => array is [.., var last] ? last : fallback,
#endif
            IReadOnlyList<T> list => list is [.., var last] ? last : fallback,
            IList<T> list => list is [.., var last] ? last : fallback,
            _ when iterable.TryCount() is { } count => count is 0 ? fallback : iterable.Last(),
            _ => iterable.EnumerateOr(fallback),
        };
#endif
    /// <summary>Gets a specific item from a collection.</summary>
    /// <typeparam name="TKey">The key item in the collection.</typeparam>
    /// <typeparam name="TValue">The value item in the collection.</typeparam>
    /// <param name="dictionary">The <see cref="IEnumerable{T}"/> to get an item from.</param>
    /// <param name="key">The key to use to get the value.</param>
    /// <returns>An element from the parameter <paramref name="dictionary"/>, or <see langword="default"/>.</returns>
    [MustUseReturnValue]
    public static TValue? Nth<TKey, TValue>([InstantHandle] this IDictionary<TKey, TValue> dictionary, TKey key)
        where TKey : notnull =>
        dictionary.TryGetValue(key, out var value) ? value : default;
#if !NET20 && !NET30
    /// <summary>Returns the item, or a fallback.</summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="self">The item to potentially return.</param>
    /// <param name="fallback">The fallback item.</param>
    /// <returns>The parameter <paramref name="self"/>, or <paramref name="fallback"/>.</returns>
    [Pure]
    public static T Or<T>(this T? self, T fallback)
        where T : class =>
        self ?? fallback;

    /// <summary>Returns the item, or a fallback.</summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="self">The item to potentially return.</param>
    /// <param name="fallback">The fallback item.</param>
    /// <returns>The parameter <paramref name="self"/>, or <paramref name="fallback"/>.</returns>
    [Pure]
    public static T Or<T>(this T? self, T fallback)
        where T : struct =>
        self ?? fallback;

    /// <summary>Returns the item, or a fallback.</summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="self">The item to potentially return.</param>
    /// <returns>The parameter <paramref name="self"/>, or a new instance.</returns>
    [Pure]
    public static T OrNew<T>(this T? self)
        where T : class, new() =>
        self ?? new();

    /// <summary>Returns the string, or an empty string.</summary>
    /// <param name="str">The string to potentially return.</param>
    /// <returns>The parameter <paramref name="str"/>, or <see cref="string.Empty"/>.</returns>
    [Pure]
    public static string OrEmpty(this string? str) => str ?? "";

    /// <summary>Returns the enumeration, or an empty enumeration.</summary>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <param name="iterable">The enumeration to potentially return.</param>
    /// <returns>The parameter <paramref name="iterable"/>, or <see cref="Enumerable.Empty{T}"/>.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<T> OrEmpty<T>([NoEnumeration] this IEnumerable<T>? iterable) => iterable ?? [];
#if NETCOREAPP || ROSLYN
    /// <summary>Returns the array, or an empty array.</summary>
    /// <typeparam name="T">The type of array.</typeparam>
    /// <param name="array">The array to potentially return.</param>
    /// <returns>The parameter <paramref name="array"/>, or <see cref="ImmutableArray{T}.Empty"/>.</returns>
    [Pure]
    public static ImmutableArray<T> OrEmpty<T>(this ImmutableArray<T> array) =>
        array.IsDefault ? ImmutableArray<T>.Empty : array;
#endif
    /// <summary>Gets a specific character from a string.</summary>
    /// <param name="str">The string to get the character from.</param>
    /// <param name="index">The index to use.</param>
    /// <returns>The character based on the parameters <paramref name="str"/> and <paramref name="index"/>.</returns>
    [Pure]
    public static char? Nth(this string str, [NonNegativeValue] int index) =>
        index >= 0 && index < str.Length ? str[index] : null;

    /// <summary>Gets a specific item from a collection.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="iterable"/>, or <see langword="default"/>.</returns>
    [MustUseReturnValue]
    public static T? Nth<T>([InstantHandle] this IEnumerable<T> iterable, [NonNegativeValue] int index)
    {
        // Runtime check.
        if (index < 0)
            return default;

        return iterable switch
        {
            string str => index < str.Length ? Reinterpret<T>(str[index]) : default,
#if NETCOREAPP || ROSLYN
            ImmutableArray<T> array => !array.IsDefault && index < array.Length ? array[index] : default,
#endif
            IReadOnlyList<T> list => index < list.Count ? list[index] : default,
            IList<T> list => index < list.Count ? list[index] : default,
            _ => iterable.Skip(index).FirstOrDefault(),
        };
    }

    /// <summary>Gets a specific character from a string.</summary>
    /// <param name="str">The string to get the character from.</param>
    /// <param name="index">The index to use.</param>
    /// <returns>The character based on the parameters <paramref name="str"/> and <paramref name="index"/>.</returns>
    [Pure]
    public static char? NthLast(this string str, [NonNegativeValue] int index) =>
        index >= 0 && index < str.Length ? str[str.Length - index - 1] : null;

    /// <summary>Gets a specific item from a collection.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="iterable"/>, or <see langword="default"/>.</returns>
    [MustUseReturnValue]
    public static T? NthLast<T>([InstantHandle] this IEnumerable<T> iterable, [NonNegativeValue] int index)
    {
        // Runtime check.
        if (index < 0)
            return default;

        return iterable switch
        {
            string str => index < str.Length ? Reinterpret<T>(str[str.Length - index - 1]) : default,
#if NETCOREAPP || ROSLYN
            ImmutableArray<T> array =>
                !array.IsDefault && index < array.Length ? array[array.Length - index - 1] : default,
#endif
            IReadOnlyList<T> list => index < list.Count ? list[list.Count - index - 1] : default,
            IList<T> list => index < list.Count ? list[list.Count - index - 1] : default,
            _ when iterable.TryGetNonEnumeratedCount(out var count) =>
                index < count ? iterable.Skip(count - index - 1).FirstOrDefault() : default,
            _ => iterable.Reverse().Skip(index).FirstOrDefault(),
        };
    }
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    // ReSharper disable once RedundantUnsafeContext UnusedMember.Local
    static unsafe T Reinterpret<T>(char c)
    {
        // ReSharper disable once RedundantNameQualifier UseSymbolAlias
        System.Diagnostics.Debug.Assert(typeof(T) == typeof(char), "T must be char");
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
#pragma warning disable 8500
        return *(T*)&c;
#pragma warning restore 8500
#else
        return Unsafe.As<char, T>(ref c);
#endif
    }
}
