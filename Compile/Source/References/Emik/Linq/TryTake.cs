// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods to attempt to grab values from enumerables.</summary>
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
        using var iterator = iterable.GetEnumerator();

        if (!iterator.MoveNext())
            return fallback;

        var last = iterator.Current;

        while (iterator.MoveNext())
            last = iterator.Current;

        return last;
    }

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
            case IList<T> list:
                return list.Count is 0 ? fallback : list[0];
            case IReadOnlyList<T> list:
                return list.Count is 0 ? fallback : list[0];
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
            // ReSharper disable once UseIndexFromEndExpression
#pragma warning disable IDE0056
            string str => str.Length is 0 ? fallback : Reinterpret<T>(str[str.Length - 1]),
#pragma warning restore IDE0056
            IReadOnlyList<T> list => list.Count is 0 ? fallback : list[0],
            IList<T> list => list.Count is 0 ? fallback : list[0],
            _ => iterable.EnumerateOr(fallback),
        };

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
    public static IEnumerable<T> OrEmpty<T>([NoEnumeration] this IEnumerable<T>? iterable) =>
        iterable ?? Enumerable.Empty<T>();

    /// <summary>Gets a specific character from a string.</summary>
    /// <param name="str">The string to get the character from.</param>
    /// <param name="index">The index to use.</param>
    /// <returns>The character based on the parameters <paramref name="str"/> and <paramref name="index"/>.</returns>
    // ReSharper disable ConditionIsAlwaysTrueOrFalse
    [Pure]
    public static char? Nth(this string str, [NonNegativeValue] int index) =>
        index >= 0 && index < str.Length ? str[index] : null;

    /// <summary>Gets a specific item from a collection.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="iterable"/>, or <see langword="default"/>.</returns>
    [MustUseReturnValue] // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public static T? Nth<T>([InstantHandle] this IEnumerable<T> iterable, [NonNegativeValue] int index)
    {
        // Runtime check.
        if (index < 0)
            return default;

        return iterable switch
        {
            string str => index < str.Length ? Reinterpret<T>(str[index]) : default,
            IReadOnlyList<T> list => index < list.Count ? list[index] : default,
            IList<T> list => index < list.Count ? list[index] : default,
            _ => iterable.Skip(index).FirstOrDefault(),
        };
    }

    /// <summary>Gets a specific character from a string.</summary>
    /// <param name="str">The string to get the character from.</param>
    /// <param name="index">The index to use.</param>
    /// <returns>The character based on the parameters <paramref name="str"/> and <paramref name="index"/>.</returns>
    // ReSharper disable ConditionIsAlwaysTrueOrFalse UseIndexFromEndExpression
    [Pure]
    public static char? NthLast(this string str, [NonNegativeValue] int index) =>
#pragma warning disable IDE0056
        index >= 0 && index < str.Length ? str[str.Length - index - 1] : null;
#pragma warning restore IDE0056

    /// <summary>Gets a specific item from a collection.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to get an item from.</param>
    /// <param name="index">The index to get.</param>
    /// <returns>An element from the parameter <paramref name="iterable"/>, or <see langword="default"/>.</returns>
    [MustUseReturnValue] // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public static T? NthLast<T>([InstantHandle] this IEnumerable<T> iterable, [NonNegativeValue] int index)
    {
        // Runtime check.
        if (index < 0)
            return default;

        return iterable switch
        {
            string str => index < str.Length ? Reinterpret<T>(str[str.Length - index - 1]) : default,
            IReadOnlyList<T> list => index < list.Count ? list[list.Count - index - 1] : default,
            IList<T> list => index < list.Count ? list[list.Count - index - 1] : default,
            _ when iterable.ToList() is var list => list[list.Count - index - 1],
            _ => throw Unreachable,
        };
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static unsafe T Reinterpret<T>(char c)
    {
        // ReSharper disable once InvocationIsSkipped RedundantNameQualifier
        System.Diagnostics.Debug.Assert(typeof(T) == typeof(char), "T must be char");
#pragma warning disable 8500
        return *(T*)&c;
#pragma warning restore 8500
    }
}
