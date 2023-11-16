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

    /// <summary>
    /// Flattens the nested collection by taking all the first elements of the enumerations,
    /// then all the second elements of the enumerations, the third, and so on.
    /// When any enumeration runs out, it simply moves onto the next enumeration until all enumerations are finished.
    /// </summary>
    /// <typeparam name="T">The type of collection.</typeparam>
    /// <param name="enumerable">The collection to flatten.</param>
    /// <returns>
    /// The flattened collection by taking items in order of appearance of each individual enumerable,
    /// and only then by the outer enumerable.
    /// </returns>
    [Pure]
    public static IEnumerable<List<T>> Transpose<T>(this IEnumerable<IEnumerable<T>> enumerable)
    {
        // ReSharper disable once ObjectProducedWithMustDisposeAnnotatedMethodIsReturned
        var (truthy, falsy) = enumerable.Select(x => x.GetEnumerator()).SplitBy(x => x.MoveNext());

        falsy.For(x => x.Dispose());

        try
        {
            while (truthy is not [])
            {
#if NETFRAMEWORK || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
                yield return truthy.ConvertAll(x => x.Current);
#else
                yield return new(truthy.Select(x => x.Current));
#endif
                (truthy, falsy) = truthy.SplitBy(x => x.MoveNext());
                falsy.For(x => x.Dispose());
            }
        }
        finally
        {
            truthy.For(x => x.Dispose());
        }
    }
}
#endif
