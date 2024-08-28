// SPDX-License-Identifier: MPL-2.0
#if !NET20 && !NET30
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Methods that creates enumerations from individual items.</summary>
static partial class ManyQueries
{
#if !NETSTANDARD || NETSTANDARD1_5_OR_GREATER
    /// <summary>Gets the types from an assembly even if type loads occur.</summary>
    /// <param name="assembly">The assembly to get the types from.</param>
    /// <returns>
    /// The enumeration of all successfully loaded types from the parameter <paramref name="assembly"/>.
    /// </returns>
    [MustUseReturnValue]
    public static IEnumerable<Type> TryGetTypes(this Assembly? assembly)
    {
        try
        {
            return assembly?.GetTypes() ?? [];
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Filter();
        }
    }
#endif
    /// <summary>Uses the callback if the parameter is non-<see langword="null"/>.</summary>
    /// <typeparam name="T">The source of the item.</typeparam>
    /// <typeparam name="TResult">The resulting type.</typeparam>
    /// <param name="item">The item to check.</param>
    /// <param name="map">The callback to use when <paramref name="item"/> is non-<see langword="null"/>.</param>
    /// <returns>The result of the parameter <paramref name="map"/>, or <see cref="Enumerable.Empty{T}"/>.</returns>
    [LinqTunnel, MustUseReturnValue]
    public static IEnumerable<TResult> ManyOrEmpty<T, TResult>(
        this T? item,
        [InstantHandle] Converter<T, IEnumerable<TResult>?> map
    ) =>
        item is not null && map(item) is { } iterable ? iterable : [];

    /// <summary>Uses the callback if the parameter is non-<see langword="null"/>.</summary>
    /// <typeparam name="T">The source of the item.</typeparam>
    /// <typeparam name="TResult">The resulting type.</typeparam>
    /// <param name="item">The item to check.</param>
    /// <param name="map">The callback to use when <paramref name="item"/> is non-<see langword="null"/>.</param>
    /// <returns>The result of the parameter <paramref name="map"/>, or <see cref="Enumerable.Empty{T}"/>.</returns>
    [LinqTunnel, MustUseReturnValue]
    public static IEnumerable<TResult> ManyOrEmpty<T, TResult>(
        this T? item,
        [InstantHandle] Converter<T, IEnumerable<TResult>?> map
    )
        where T : struct =>
        item.HasValue && map(item.Value) is { } iterable ? iterable : [];

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
    public static IEnumerable<TResult> ManyOrEmpty<T, TResult>(
        [NoEnumeration] this IEnumerable<T?>? iterator,
        Func<T, IEnumerable<TResult?>?> map
    ) =>
        iterator?.Filter().Select(map).SelectMany(x => x ?? []).Filter() ?? [];

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
    public static IEnumerable<TResult> ManyOrEmpty<T, TResult>(
        [NoEnumeration] this IEnumerable<T?>? iterator,
        Func<T, IEnumerable<TResult?>?> map
    )
        where T : struct =>
        iterator?.Filter().Select(map).SelectMany(x => x ?? []).Filter() ?? [];

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
    public static IEnumerable<TResult> ManyOrEmpty<T, TResult>(
        [NoEnumeration] this IEnumerable<T?>? iterator,
        Func<T, IEnumerable<TResult?>?> map
    )
        where TResult : struct =>
        iterator?.Filter().Select(map).SelectMany(x => x ?? []).Filter() ?? [];

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
    public static IEnumerable<TResult> ManyOrEmpty<T, TResult>(
        [NoEnumeration] this IEnumerable<T?>? iterator,
        Func<T, IEnumerable<TResult?>?> map
    )
        where T : struct
        where TResult : struct =>
        iterator?.Filter().Select(map).SelectMany(x => x ?? []).Filter() ?? [];

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
    public static IEnumerable<TResult> ManyOrEmpty<T, TResult>(
        [NoEnumeration] this IEnumerable<T?>? iterator,
        Func<T, int, IEnumerable<TResult?>?> map
    ) =>
        iterator?.Filter().Select(map).SelectMany(x => x ?? []).Filter() ?? [];

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
    public static IEnumerable<TResult> ManyOrEmpty<T, TResult>(
        [NoEnumeration] this IEnumerable<T?>? iterator,
        Func<T, int, IEnumerable<TResult?>?> map
    )
        where T : struct =>
        iterator?.Filter().Select(map).SelectMany(x => x ?? []).Filter() ?? [];

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
    public static IEnumerable<TResult> ManyOrEmpty<T, TResult>(
        [NoEnumeration] this IEnumerable<T?>? iterator,
        Func<T, int, IEnumerable<TResult?>?> map
    )
        where TResult : struct =>
        iterator?.Filter().Select(map).SelectMany(x => x ?? []).Filter() ?? [];

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
    public static IEnumerable<TResult> ManyOrEmpty<T, TResult>(
        [NoEnumeration] this IEnumerable<T?>? iterator,
        Func<T, int, IEnumerable<TResult?>?> map
    )
        where T : struct
        where TResult : struct =>
        iterator?.Filter().Select(map).SelectMany(x => x ?? []).Filter() ?? [];
}
#endif
