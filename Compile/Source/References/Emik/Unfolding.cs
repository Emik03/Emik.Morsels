// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides methods for unfolding.</summary>
static partial class Unfolding
{
    /// <summary>Applies a selector and collects the returned items recursively until the value becomes null.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The initial value.</param>
    /// <param name="converter">The converter to apply.</param>
    /// <returns>
    /// The parameter <paramref name="value"/>, followed by each non-null
    /// returned value from the parameter <paramref name="converter"/>.
    /// </returns>
    [Pure]
    public static IEnumerable<T> FindPathToNull<T>(this T? value, Converter<T, T?> converter)
        where T : class
    {
        if (value is null)
            yield break;

        do
            yield return value;
        while (converter(value) is { } newValue && (value = newValue) is var _);
    }

    /// <summary>Applies a selector and collects the returned items recursively until the value becomes null.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The initial value.</param>
    /// <param name="converter">The converter to apply.</param>
    /// <returns>
    /// The parameter <paramref name="value"/>, followed by each non-null
    /// returned value from the parameter <paramref name="converter"/>.
    /// </returns>
    [Pure]
    public static IEnumerable<T> FindPathToNull<T>(this T value, Converter<T, T?> converter)
        where T : struct
    {
        do
            yield return value;
        while (converter(value) is { } newValue && (value = newValue) is var _);
    }

    /// <summary>Applies a selector and collects the returned items recursively until the value becomes null.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The initial value.</param>
    /// <param name="converter">The converter to apply.</param>
    /// <returns>
    /// The parameter <paramref name="value"/>, followed by each non-null
    /// returned value from the parameter <paramref name="converter"/>.
    /// </returns>
    [Pure]
    public static IEnumerable<T> FindPathToNull<T>(this T? value, Converter<T, T?> converter)
        where T : struct =>
        value is { } t ? FindPathToNull(t, converter) : Enumerable.Empty<T>();
}
