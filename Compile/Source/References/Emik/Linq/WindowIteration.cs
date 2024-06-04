// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides methods to create views of <see cref="IEnumerable{T}"/> instances.</summary>
static partial class WindowIteration
{
    /// <summary>
    /// Transforms the <see cref="IEnumerable{T}"/> into views of the current and next items.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="source">The collection to iterate over.</param>
    /// <returns>The <see cref="IEnumerable{T}"/> containing the current and next items.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<(T Current, T Next)> Window<T>(this IEnumerable<T> source) =>
        source.TryCount() is { } x ? Iterator(source).WithCount(x - 1) : Iterator(source);

    /// <summary>
    /// Transforms the <see cref="IEnumerable{T}"/> into views of the specified length.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="source">The collection to iterate over.</param>
    /// <param name="size">The size of the window.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of windows.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<T[]> Window<T>(this IEnumerable<T> source, int size) =>
        size <= 0
            ? []
            : source.TryCount() switch
            {
                0 => [],
                { } x when x < size => [],
                1 => source.Select(x => new[] { x }),
                { } x => Iterator(source, size).WithCount(x - size + 1),
                null => Iterator(source, size),
            };

    [Pure]
    static IEnumerable<(T Current, T Next)> Iterator<T>(IEnumerable<T> source)
    {
        using var e = source.GetEnumerator();

        if (!e.MoveNext())
            yield break;

        var previous = e.Current;

        while (e.MoveNext())
            yield return (previous, previous = e.Current);
    }

    [Pure]
    static IEnumerable<T[]> Iterator<T>(IEnumerable<T> source, int size)
    {
        using var e = source.GetEnumerator();
        var window = new T[size];

        for (var i = 0; i < size; i++)
            if (e.MoveNext())
                window[i] = e.Current;
            else
                yield break;

        yield return (T[])window.Clone();

        while (e.MoveNext())
        {
            for (var i = 1; i < window.Length; i++)
                window[i - 1] = window[i];

            window[^1] = e.Current;
            yield return (T[])window.Clone();
        }
    }
}
