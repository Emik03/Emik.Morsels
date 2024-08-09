// SPDX-License-Identifier: MPL-2.0

// ReSharper disable UnusedMember.Local
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

#pragma warning disable 8619
/// <summary>Extension methods for improving nullability awareness for enumerables.</summary>
static partial class NullableItems
{
    /// <summary>Annotates <c>ItemCanBeNullAttribute</c>.</summary>
    /// <typeparam name="T">The type of item to adjust nullability.</typeparam>
    /// <param name="iterable">The item to return with adjusted nullability.</param>
    /// <returns>The parameter <paramref name="iterable"/>, with <c>ItemCanBeNullAttribute</c>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterable))]
    public static IEnumerable<T?>? ItemCanBeNull<T>(this IEnumerable<T>? iterable) => iterable;

    /// <summary>Annotates <c>ItemCanBeNullAttribute</c>.</summary>
    /// <typeparam name="T">The type of item to adjust nullability.</typeparam>
    /// <param name="iterator">The item to return with adjusted nullability.</param>
    /// <returns>The parameter <paramref name="iterator"/>, with <c>ItemCanBeNullAttribute</c>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterator))]
    public static IEnumerator<T?>? ItemCanBeNull<T>(this IEnumerator<T>? iterator) => iterator;

    /// <summary>Annotates <c>ItemCanBeNullAttribute</c>.</summary>
    /// <typeparam name="T">The type of item to adjust nullability.</typeparam>
    /// <param name="collection">The item to return with adjusted nullability.</param>
    /// <returns>The parameter <paramref name="collection"/>, with <c>ItemCanBeNullAttribute</c>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(collection))]
    public static IReadOnlyCollection<T?>? ItemCanBeNull<T>(this IReadOnlyCollection<T>? collection) => collection;

    /// <summary>Annotates <c>ItemCanBeNullAttribute</c>.</summary>
    /// <typeparam name="T">The type of item to adjust nullability.</typeparam>
    /// <param name="list">The item to return with adjusted nullability.</param>
    /// <returns>The parameter <paramref name="list"/>, with <c>ItemCanBeNullAttribute</c>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(list))]
    public static IReadOnlyList<T?>? ItemCanBeNull<T>(this IReadOnlyList<T>? list) => list;

    /// <summary>Annotates <c>ItemCanBeNullAttribute</c>.</summary>
    /// <typeparam name="T">The type of item to adjust nullability.</typeparam>
    /// <param name="set">The item to return with adjusted nullability.</param>
    /// <returns>The parameter <paramref name="set"/>, with <c>ItemCanBeNullAttribute</c>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(set))]
    public static IReadOnlySet<T?>? ItemCanBeNull<T>(this IReadOnlySet<T>? set) => set;

    /// <summary>Returns the list if all items are non-null.</summary>
    /// <typeparam name="T">The type of list.</typeparam>
    /// <param name="list">The list to filter.</param>
    /// <returns>
    /// The parameter <paramref name="list"/> if all items are non-<see langword="null"/>,
    /// otherwise <see langword="null"/>.
    /// </returns>
    [Pure]
    public static IList<T>? ItemNotNull<T>(this IList<T?>? list)
    {
        if (list is null)
            return null;

        // ReSharper disable once ForCanBeConvertedToForeach LoopCanBeConvertedToQuery
        for (var i = 0; i < list.Count; i++)
            if (list[i] is null)
                return null;

        return list;
    }
}
