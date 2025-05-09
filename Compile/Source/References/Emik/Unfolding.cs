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
        while (value is not null)
        {
            yield return value;

            value = converter(value);
        }
    }

    /// <inheritdoc cref="FindPathToNull{T}"/>
    [DoesNotReturn, EditorBrowsable(EditorBrowsableState.Never), Obsolete("The return value is always not null.", true)]
    public static IEnumerable<T> FindPathToEmptyNullable<T>(this T _, Converter<T, T> converter)
        where T : struct =>
        throw Unreachable;

    /// <inheritdoc cref="FindPathToNull{T}"/>
    [Pure]
    public static IEnumerable<T> FindPathToEmptyNullable<T>(this T value, Converter<T, T?> converter)
        where T : struct
    {
        T? maybe = value;

        while (maybe is { } yes)
        {
            yield return yes;

            maybe = converter(yes);
        }
    }

    /// <inheritdoc cref="FindPathToNull{T}"/>
    [DoesNotReturn, EditorBrowsable(EditorBrowsableState.Never), Obsolete("The return value is always not null.", true)]
    public static IEnumerable<T> FindPathToEmptyNullable<T>(this T? _, Converter<T, T> converter)
        where T : struct =>
        throw Unreachable;

    /// <inheritdoc cref="FindPathToNull{T}"/>
    [Pure]
    public static IEnumerable<T> FindPathToEmptyNullable<T>(this T? value, Converter<T, T?> converter)
        where T : struct =>
        value is { } t ? FindPathToEmptyNullable(t, converter) : [];

    /// <inheritdoc cref="FindPathToNull{T}"/>
    [Pure]
    public static SmallList<T> FindSmallPathToNull<T>(this T? value, Converter<T, T?> converter)
        where T : class
    {
        SmallList<T> output = default;

        while (value is not null)
        {
            output.Add(value);
            value = converter(value);
        }

        return output;
    }

    /// <inheritdoc cref="FindPathToNull{T}"/>
    [DoesNotReturn, EditorBrowsable(EditorBrowsableState.Never), Obsolete("The return value is always not null.", true)]
    public static SmallList<T> FindSmallPathToEmptyNullable<T>(this T _, Converter<T, T> converter)
        where T : struct =>
        throw Unreachable;

    /// <inheritdoc cref="FindPathToNull{T}"/>
    [Pure]
    public static SmallList<T> FindSmallPathToEmptyNullable<T>(this T value, Converter<T, T?> converter)
        where T : struct
    {
        SmallList<T> output = [];
        T? maybe = value;

        while (maybe is { } yes)
        {
            output.Add(yes);
            maybe = converter(yes);
        }

        return output;
    }

    /// <inheritdoc cref="FindPathToNull{T}"/>
    [DoesNotReturn, EditorBrowsable(EditorBrowsableState.Never), Obsolete("The return value is always not null.", true)]
    public static SmallList<T> FindSmallPathToEmptyNullable<T>(this T? _, Converter<T, T> converter)
        where T : struct =>
        throw Unreachable;

    /// <inheritdoc cref="FindPathToNull{T}"/>
    [Pure]
    public static SmallList<T> FindSmallPathToEmptyNullable<T>(this T? value, Converter<T, T?> converter)
        where T : struct =>
        value is { } t ? FindSmallPathToEmptyNullable(t, converter) : [];
}
