// <copyright file="ManyQueries.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>
namespace Emik.Morsels;

/// <summary>Methods that creates enumerations from individual items.</summary>
static partial class ManyQueries
{
    /// <summary>Uses the callback if the parameter is non-<see langword="null"/>.</summary>
    /// <typeparam name="T">The source of the item.</typeparam>
    /// <typeparam name="TResult">The resulting type.</typeparam>
    /// <param name="item">The item to check.</param>
    /// <param name="map">The callback to use when <paramref name="item"/> is non-<see langword="null"/>.</param>
    /// <returns>The result of the parameter <paramref name="map"/>, or <see cref="Enumerable.Empty{T}"/>.</returns>
    [LinqTunnel, MustUseReturnValue]
    internal static IEnumerable<TResult> ManyOrEmpty<T, TResult>(
        this T? item,
        [InstantHandle] Converter<T, IEnumerable<TResult>?> map
    ) =>
        item is not null && map(item) is { } iterable ? iterable : Enumerable.Empty<TResult>();

    /// <summary>Uses the callback if the parameter is non-<see langword="null"/>.</summary>
    /// <typeparam name="T">The source of the item.</typeparam>
    /// <typeparam name="TResult">The resulting type.</typeparam>
    /// <param name="item">The item to check.</param>
    /// <param name="map">The callback to use when <paramref name="item"/> is non-<see langword="null"/>.</param>
    /// <returns>The result of the parameter <paramref name="map"/>, or <see cref="Enumerable.Empty{T}"/>.</returns>
    [LinqTunnel, MustUseReturnValue]
    internal static IEnumerable<TResult> ManyOrEmpty<T, TResult>(
        this T? item,
        [InstantHandle] Converter<T, IEnumerable<TResult>?> map
    )
        where T : struct =>
        item.HasValue && map(item.Value) is { } iterable ? iterable : Enumerable.Empty<TResult>();

    /// <summary>
    /// <see cref="Enumerable.SelectMany{TSource, TResult}(IEnumerable{TSource}, Func{TSource, IEnumerable{TResult}})"/>
    /// but with exhaustive null guards that fall back to empty enumerables.
    /// </summary>
    /// <typeparam name="T">The source of the item.</typeparam>
    /// <typeparam name="TResult">The resulting type.</typeparam>
    /// <param name="iterator">The item to check.</param>
    /// <param name="map">The callback to use when <paramref name="iterator"/> is non-<see langword="null"/>.</param>
    /// <returns>The result of the parameter <paramref name="map"/>, or <see cref="Enumerable.Empty{T}"/>.</returns>
    [LinqTunnel, Pure]
    internal static IEnumerable<TResult> ManyOrEmpty<T, TResult>(
        [NoEnumeration] this IEnumerable<T?>? iterator,
        Func<T, IEnumerable<TResult?>?> map
    ) =>
        iterator?.Filter().Select(map).SelectMany(x => x ?? Enumerable.Empty<TResult?>()).Filter() ??
        Enumerable.Empty<TResult>();

    /// <summary>
    /// <see cref="Enumerable.SelectMany{TSource, TResult}(IEnumerable{TSource}, Func{TSource, IEnumerable{TResult}})"/>
    /// but with exhaustive null guards that fall back to empty enumerables.
    /// </summary>
    /// <typeparam name="T">The source of the item.</typeparam>
    /// <typeparam name="TResult">The resulting type.</typeparam>
    /// <param name="iterator">The item to check.</param>
    /// <param name="map">The callback to use when <paramref name="iterator"/> is non-<see langword="null"/>.</param>
    /// <returns>The result of the parameter <paramref name="map"/>, or <see cref="Enumerable.Empty{T}"/>.</returns>
    [LinqTunnel, Pure]
    internal static IEnumerable<TResult> ManyOrEmpty<T, TResult>(
        [NoEnumeration] this IEnumerable<T?>? iterator,
        Func<T, IEnumerable<TResult?>?> map
    )
        where T : struct =>
        iterator?.Filter().Select(map).SelectMany(x => x ?? Enumerable.Empty<TResult?>()).Filter() ??
        Enumerable.Empty<TResult>();

    /// <summary>
    /// <see cref="Enumerable.SelectMany{TSource, TResult}(IEnumerable{TSource}, Func{TSource, IEnumerable{TResult}})"/>
    /// but with exhaustive null guards that fall back to empty enumerables.
    /// </summary>
    /// <typeparam name="T">The source of the item.</typeparam>
    /// <typeparam name="TResult">The resulting type.</typeparam>
    /// <param name="iterator">The item to check.</param>
    /// <param name="map">The callback to use when <paramref name="iterator"/> is non-<see langword="null"/>.</param>
    /// <returns>The result of the parameter <paramref name="map"/>, or <see cref="Enumerable.Empty{T}"/>.</returns>
    [LinqTunnel, Pure]
    internal static IEnumerable<TResult> ManyOrEmpty<T, TResult>(
        [NoEnumeration] this IEnumerable<T?>? iterator,
        Func<T, IEnumerable<TResult?>?> map
    )
        where TResult : struct =>
        iterator?.Filter().Select(map).SelectMany(x => x ?? Enumerable.Empty<TResult?>()).Filter() ??
        Enumerable.Empty<TResult>();

    /// <summary>
    /// <see cref="Enumerable.SelectMany{TSource, TResult}(IEnumerable{TSource}, Func{TSource, IEnumerable{TResult}})"/>
    /// but with exhaustive null guards that fall back to empty enumerables.
    /// </summary>
    /// <typeparam name="T">The source of the item.</typeparam>
    /// <typeparam name="TResult">The resulting type.</typeparam>
    /// <param name="iterator">The item to check.</param>
    /// <param name="map">The callback to use when <paramref name="iterator"/> is non-<see langword="null"/>.</param>
    /// <returns>The result of the parameter <paramref name="map"/>, or <see cref="Enumerable.Empty{T}"/>.</returns>
    [LinqTunnel, Pure]
    internal static IEnumerable<TResult> ManyOrEmpty<T, TResult>(
        [NoEnumeration] this IEnumerable<T?>? iterator,
        Func<T, IEnumerable<TResult?>?> map
    )
        where T : struct
        where TResult : struct =>
        iterator?.Filter().Select(map).SelectMany(x => x ?? Enumerable.Empty<TResult?>()).Filter() ??
        Enumerable.Empty<TResult>();

    /// <summary>
    /// <see cref="Enumerable.SelectMany{TSource, TResult}(IEnumerable{TSource}, Func{TSource, IEnumerable{TResult}})"/>
    /// but with exhaustive null guards that fall back to empty enumerables.
    /// </summary>
    /// <typeparam name="T">The source of the item.</typeparam>
    /// <typeparam name="TResult">The resulting type.</typeparam>
    /// <param name="iterator">The item to check.</param>
    /// <param name="map">The callback to use when <paramref name="iterator"/> is non-<see langword="null"/>.</param>
    /// <returns>The result of the parameter <paramref name="map"/>, or <see cref="Enumerable.Empty{T}"/>.</returns>
    [LinqTunnel, Pure]
    internal static IEnumerable<TResult> ManyOrEmpty<T, TResult>(
        [NoEnumeration] this IEnumerable<T?>? iterator,
        Func<T, int, IEnumerable<TResult?>?> map
    ) =>
        iterator?.Filter().Select(map).SelectMany(x => x ?? Enumerable.Empty<TResult?>()).Filter() ??
        Enumerable.Empty<TResult>();

    /// <summary>
    /// <see cref="Enumerable.SelectMany{TSource, TResult}(IEnumerable{TSource}, Func{TSource, IEnumerable{TResult}})"/>
    /// but with exhaustive null guards that fall back to empty enumerables.
    /// </summary>
    /// <typeparam name="T">The source of the item.</typeparam>
    /// <typeparam name="TResult">The resulting type.</typeparam>
    /// <param name="iterator">The item to check.</param>
    /// <param name="map">The callback to use when <paramref name="iterator"/> is non-<see langword="null"/>.</param>
    /// <returns>The result of the parameter <paramref name="map"/>, or <see cref="Enumerable.Empty{T}"/>.</returns>
    [LinqTunnel, Pure]
    internal static IEnumerable<TResult> ManyOrEmpty<T, TResult>(
        [NoEnumeration] this IEnumerable<T?>? iterator,
        Func<T, int, IEnumerable<TResult?>?> map
    )
        where T : struct =>
        iterator?.Filter().Select(map).SelectMany(x => x ?? Enumerable.Empty<TResult?>()).Filter() ??
        Enumerable.Empty<TResult>();

    /// <summary>
    /// <see cref="Enumerable.SelectMany{TSource, TResult}(IEnumerable{TSource}, Func{TSource, IEnumerable{TResult}})"/>
    /// but with exhaustive null guards that fall back to empty enumerables.
    /// </summary>
    /// <typeparam name="T">The source of the item.</typeparam>
    /// <typeparam name="TResult">The resulting type.</typeparam>
    /// <param name="iterator">The item to check.</param>
    /// <param name="map">The callback to use when <paramref name="iterator"/> is non-<see langword="null"/>.</param>
    /// <returns>The result of the parameter <paramref name="map"/>, or <see cref="Enumerable.Empty{T}"/>.</returns>
    [LinqTunnel, Pure]
    internal static IEnumerable<TResult> ManyOrEmpty<T, TResult>(
        [NoEnumeration] this IEnumerable<T?>? iterator,
        Func<T, int, IEnumerable<TResult?>?> map
    )
        where TResult : struct =>
        iterator?.Filter().Select(map).SelectMany(x => x ?? Enumerable.Empty<TResult?>()).Filter() ??
        Enumerable.Empty<TResult>();

    /// <summary>
    /// <see cref="Enumerable.SelectMany{TSource, TResult}(IEnumerable{TSource}, Func{TSource, IEnumerable{TResult}})"/>
    /// but with exhaustive null guards that fall back to empty enumerables.
    /// </summary>
    /// <typeparam name="T">The source of the item.</typeparam>
    /// <typeparam name="TResult">The resulting type.</typeparam>
    /// <param name="iterator">The item to check.</param>
    /// <param name="map">The callback to use when <paramref name="iterator"/> is non-<see langword="null"/>.</param>
    /// <returns>The result of the parameter <paramref name="map"/>, or <see cref="Enumerable.Empty{T}"/>.</returns>
    [LinqTunnel, Pure]
    internal static IEnumerable<TResult> ManyOrEmpty<T, TResult>(
        [NoEnumeration] this IEnumerable<T?>? iterator,
        Func<T, int, IEnumerable<TResult?>?> map
    )
        where T : struct
        where TResult : struct =>
        iterator?.Filter().Select(map).SelectMany(x => x ?? Enumerable.Empty<TResult?>()).Filter() ??
        Enumerable.Empty<TResult>();
}
