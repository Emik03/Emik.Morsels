// SPDX-License-Identifier: MPL-2.0

// ReSharper disable UnusedMember.Local
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

#pragma warning disable CA1508
/// <summary>Extension methods for improving nullability awareness for enumerables.</summary>
static partial class NullableItems
{
#pragma warning disable CS8619
    /// <summary>Annotates <c>ItemCanBeNullAttribute</c>.</summary>
    /// <typeparam name="T">The type of item to adjust nullability.</typeparam>
    /// <param name="iterable">The item to return with adjusted nullability.</param>
    /// <returns>The parameter <paramref name="iterable"/>, with <c>ItemCanBeNullAttribute</c>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterable))]
    internal static IEnumerable<T?>? ItemCanBeNull<T>(this IEnumerable<T>? iterable) => iterable;

    /// <summary>Annotates <c>ItemCanBeNullAttribute</c>.</summary>
    /// <typeparam name="T">The type of item to adjust nullability.</typeparam>
    /// <param name="iterator">The item to return with adjusted nullability.</param>
    /// <returns>The parameter <paramref name="iterator"/>, with <see cref="ItemCanBeNullAttribute"/>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterator))]
    internal static IEnumerator<T?>? ItemCanBeNull<T>(this IEnumerator<T>? iterator) => iterator;

#if !NET20 && !NET30
    /// <summary>Returns the list if all items are non-null.</summary>
    /// <typeparam name="T">The type of list.</typeparam>
    /// <param name="list">The list to filter.</param>
    /// <returns>
    /// The parameter <paramref name="list"/> if all items are non-<see langword="null"/>,
    /// otherwise <see langword="null"/>.
    /// </returns>
    [Pure]
    internal static IList<T>? ItemNotNull<T>(this IList<T?>? list) =>
        list?.All(x => x is not null) ?? false ? list : null;
#endif

    /// <summary>Annotates <c>ItemCanBeNullAttribute</c>.</summary>
    /// <typeparam name="T">The type of item to adjust nullability.</typeparam>
    /// <param name="collection">The item to return with adjusted nullability.</param>
    /// <returns>The parameter <paramref name="collection"/>, with <c>ItemCanBeNullAttribute</c>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(collection))]
    internal static IReadOnlyCollection<T?>? ItemCanBeNull<T>(this IReadOnlyCollection<T>? collection) => collection;

    /// <summary>Annotates <c>ItemCanBeNullAttribute</c>.</summary>
    /// <typeparam name="T">The type of item to adjust nullability.</typeparam>
    /// <param name="list">The item to return with adjusted nullability.</param>
    /// <returns>The parameter <paramref name="list"/>, with <c>ItemCanBeNullAttribute</c>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(list))]
    internal static IReadOnlyList<T?>? ItemCanBeNull<T>(this IReadOnlyList<T>? list) => list;

    /// <summary>Annotates <c>ItemCanBeNullAttribute</c>.</summary>
    /// <typeparam name="T">The type of item to adjust nullability.</typeparam>
    /// <param name="set">The item to return with adjusted nullability.</param>
    /// <returns>The parameter <paramref name="set"/>, with <c>ItemCanBeNullAttribute</c>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(set))]
    internal static IReadOnlySet<T?>? ItemCanBeNull<T>(this IReadOnlySet<T>? set) => set;
#pragma warning restore CS8619
}
