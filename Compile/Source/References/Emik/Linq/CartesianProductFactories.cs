// SPDX-License-Identifier: MPL-2.0
#if !NET20 && !NET30
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods to create cartesian products.</summary>
static partial class CartesianProductFactories
{
    /// <summary>Creates a cartesian product from two collections.</summary>
    /// <remarks><para>The cartesian product is defined as the set of ordered pairs.</para></remarks>
    /// <typeparam name="T1">The type of item in the first set.</typeparam>
    /// <typeparam name="T2">The type of item in the second set.</typeparam>
    /// <param name="first">The first set to create a cartesian product of.</param>
    /// <param name="second">The second set to create a cartesian product of.</param>
    /// <returns>
    /// The cartesian product of the parameter <paramref name="first"/> and <paramref name="second"/>.
    /// </returns>
    [LinqTunnel, Pure]
    public static IEnumerable<(T1 First, T2 Second)> CartesianProduct<T1, T2>(
        this IEnumerable<T1> first,
        IEnumerable<T2> second
    ) =>
        first.SelectMany(_ => second, (x, y) => (x, y));

    /// <summary>Creates a cartesian product from three collections.</summary>
    /// <remarks><para>The cartesian product is defined as the set of ordered pairs.</para></remarks>
    /// <typeparam name="T1">The type of item in the first set.</typeparam>
    /// <typeparam name="T2">The type of item in the second set.</typeparam>
    /// <typeparam name="T3">The type of item in the third set.</typeparam>
    /// <param name="first">The first set to create a cartesian product of.</param>
    /// <param name="second">The second set to create a cartesian product of.</param>
    /// <param name="third">The third set to create a cartesian product of.</param>
    /// <returns>
    /// The cartesian product of the parameter <paramref name="first"/>,
    /// <paramref name="second"/>, and <paramref name="third"/>.
    /// </returns>
    [LinqTunnel, Pure]
    public static IEnumerable<(T1 First, T2 Second, T3 Third)> CartesianProduct<T1, T2, T3>(
        this IEnumerable<T1> first,
        IEnumerable<T2> second,
        IEnumerable<T3> third
    ) =>
        first
           .SelectMany(_ => second, (x, y) => (x, y))
           .SelectMany(_ => third, (xy, z) => (xy.x, xy.y, z));

    /// <summary>Creates a cartesian product from four collections.</summary>
    /// <remarks><para>The cartesian product is defined as the set of ordered pairs.</para></remarks>
    /// <typeparam name="T1">The type of item in the first set.</typeparam>
    /// <typeparam name="T2">The type of item in the second set.</typeparam>
    /// <typeparam name="T3">The type of item in the third set.</typeparam>
    /// <typeparam name="T4">The type of item in the fourth set.</typeparam>
    /// <param name="first">The first set to create a cartesian product of.</param>
    /// <param name="second">The second set to create a cartesian product of.</param>
    /// <param name="third">The third set to create a cartesian product of.</param>
    /// <param name="fourth">The fourth set to create a cartesian product of.</param>
    /// <returns>
    /// The cartesian product of the parameter <paramref name="first"/>, <paramref name="second"/>,
    /// <paramref name="third"/>, and <paramref name="fourth"/>.
    /// </returns>
    [LinqTunnel, Pure]
    public static IEnumerable<(T1 First, T2 Second, T3 Third, T4 Fourth)> CartesianProduct<T1, T2, T3, T4>(
        this IEnumerable<T1> first,
        IEnumerable<T2> second,
        IEnumerable<T3> third,
        IEnumerable<T4> fourth
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
            Enumerable.Repeat((IEnumerable<T>)[], 1),
            (sum, next) => sum.SelectMany(_ => next, (s, n) => s.Concat(Enumerable.Repeat(n, 1)))
        );
}
#endif
