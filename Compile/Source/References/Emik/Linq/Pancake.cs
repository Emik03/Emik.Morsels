// SPDX-License-Identifier: MPL-2.0
#if !NET20 && !NET30
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides methods to flatten <see cref="IEnumerable{T}"/> instances.</summary>
static partial class Pancake
{
    /// <summary>Flattens the nested collection.</summary>
    /// <typeparam name="T">The type of collection.</typeparam>
    /// <param name="enumerable">The collection to flatten.</param>
    /// <returns>The flattened collection of the parameter <paramref name="enumerable"/>.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable) =>
        enumerable.SelectMany(Enumerable.AsEnumerable);

    /// <summary>Flattens the nested collection.</summary>
    /// <typeparam name="T">The type of collection.</typeparam>
    /// <param name="enumerable">The collection to flatten.</param>
    /// <returns>The flattened collection of the parameter <paramref name="enumerable"/>.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<T> Flatten2<T>(this IEnumerable<IEnumerable<IEnumerable<T>>> enumerable) =>
        enumerable.SelectMany(Enumerable.AsEnumerable).SelectMany(Enumerable.AsEnumerable);

    /// <summary>Flattens the nested collection.</summary>
    /// <typeparam name="T">The type of collection.</typeparam>
    /// <param name="enumerable">The collection to flatten.</param>
    /// <returns>The flattened collection of the parameter <paramref name="enumerable"/>.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<T> Flatten3<T>(this IEnumerable<IEnumerable<IEnumerable<IEnumerable<T>>>> enumerable) =>
        enumerable
           .SelectMany(Enumerable.AsEnumerable)
           .SelectMany(Enumerable.AsEnumerable)
           .SelectMany(Enumerable.AsEnumerable);
}
#endif
