// SPDX-License-Identifier: MPL-2.0
#if !NET20 && !NET30
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods to create cartesian products.</summary>
static partial class CartesianProductFactories
{
    /// <summary>Creates a cartesian product from two collections.</summary>
    /// <remarks><para>The cartesian product is defined as the set of ordered pairs.</para></remarks>
    /// <typeparam name="T">The type of item in the set.</typeparam>
    /// <param name="first">The first set to create a cartesian product of.</param>
    /// <param name="second">The second set to create a cartesian product of.</param>
    /// <returns>
    /// The cartesian product of the parameter <paramref name="first"/> and <paramref name="second"/>.
    /// </returns>
    [LinqTunnel, Pure]
    public static IEnumerable<(T, T)> CartesianProduct<T>(this IEnumerable<T> first, IEnumerable<T> second) =>
        first.SelectMany(_ => second, (x, y) => (x, y));

    /// <summary>Creates a cartesian product from three collections.</summary>
    /// <remarks><para>The cartesian product is defined as the set of ordered pairs.</para></remarks>
    /// <typeparam name="T">The type of item in the set.</typeparam>
    /// <param name="first">The first set to create a cartesian product of.</param>
    /// <param name="second">The second set to create a cartesian product of.</param>
    /// <param name="third">The third set to create a cartesian product of.</param>
    /// <returns>
    /// The cartesian product of the parameter <paramref name="first"/>,
    /// <paramref name="second"/>, and <paramref name="third"/>.
    /// </returns>
    [LinqTunnel, Pure]
    public static IEnumerable<(T, T, T)> CartesianProduct<T>(
        this IEnumerable<T> first,
        IEnumerable<T> second,
        IEnumerable<T> third
    ) =>
        first
           .SelectMany(_ => second, (x, y) => (x, y))
           .SelectMany(_ => third, (xy, z) => (xy.x, xy.y, z));

    /// <summary>Creates a cartesian product from four collections.</summary>
    /// <remarks><para>The cartesian product is defined as the set of ordered pairs.</para></remarks>
    /// <typeparam name="T">The type of item in the set.</typeparam>
    /// <param name="first">The first set to create a cartesian product of.</param>
    /// <param name="second">The second set to create a cartesian product of.</param>
    /// <param name="third">The third set to create a cartesian product of.</param>
    /// <param name="fourth">The fourth set to create a cartesian product of.</param>
    /// <returns>
    /// The cartesian product of the parameter <paramref name="first"/>, <paramref name="second"/>,
    /// <paramref name="third"/>, and <paramref name="fourth"/>.
    /// </returns>
    [LinqTunnel, Pure]
    public static IEnumerable<(T, T, T, T)> CartesianProduct<T>(
        this IEnumerable<T> first,
        IEnumerable<T> second,
        IEnumerable<T> third,
        IEnumerable<T> fourth
    ) =>
        first
           .SelectMany(_ => second, (x, y) => (x, y))
           .SelectMany(_ => third, (xy, z) => (xy, z))
           .SelectMany(_ => fourth, (xyz, w) => (xyz.xy.x, xyz.xy.y, xyz.z, w));

    /// <summary>Creates a cartesian product from n-collections.</summary>
    /// <remarks><para>The cartesian product is defined as the set of ordered pairs.</para></remarks>
    /// <typeparam name="T">The type of item in the set.</typeparam>
    /// <param name="first">The first set to create a cartesian product of.</param>
    /// <param name="rest">The rest of the sets to create a cartesian product of.</param>
    /// <returns>
    /// The cartesian product of the parameter <paramref name="first"/>, and all of <paramref name="rest"/>.
    /// </returns>
    public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(
        this IEnumerable<T> first,
        params IEnumerable<T>[] rest
    ) =>
        Enumerable.Repeat(first, 1).Concat(rest).CartesianProduct();

    /// <summary>Creates a cartesian product from n-collections.</summary>
    /// <remarks><para>The cartesian product is defined as the set of ordered pairs.</para></remarks>
    /// <typeparam name="T">The type of item in the set.</typeparam>
    /// <param name="iterable">The sets to create a cartesian product of.</param>
    /// <returns>The cartesian product of all of the parameter <paramref name="iterable"/>.</returns>
    public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> iterable) =>
        iterable.Aggregate(
            Enumerable.Repeat(Enumerable.Empty<T>(), 1),
            (sum, next) => sum.SelectMany(_ => next, (s, n) => s.Concat(Enumerable.Repeat(n, 1)))
        );
}
#endif
