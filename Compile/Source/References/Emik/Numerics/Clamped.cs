// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods to clamp numbers.</summary>
static partial class Clamped
{
#if !NET7_0_OR_GREATER
    /// <summary>Clamps a value such that it is no smaller or larger than the defined amount.</summary>
    /// <param name="number">The number to clip.</param>
    /// <param name="min">If specified, the smallest number to return.</param>
    /// <param name="max">If specified, the greatest number to return.</param>
    /// <returns>
    /// The parameter <paramref name="min"/> if <paramref name="number"/> is smaller than <paramref name="min"/>,
    /// otherwise, the parameter <paramref name="max"/> if <paramref name="number"/> is greater than
    /// <paramref name="max"/>, otherwise the parameter <paramref name="number"/>.
    /// </returns>
    [Pure]
    public static int Clamp(this int number, int? min = null, int? max = null) =>
        (min ?? number) is var small &&
        (max ?? number) is var big &&
        number <= small ? small :
        number >= big ? big : number;

    /// <inheritdoc cref="Clamp(int, int?, int?)"/>
    [Pure]
    public static float Clamp(this float number, float? min = null, float? max = null) =>
        (min ?? number) is var small &&
        (max ?? number) is var big &&
        number <= small ? small :
        number >= big ? big : number;
#else
    /// <summary>Clamps a value such that it is no smaller or larger than the defined amount.</summary>
    /// <typeparam name="T">The type of numeric value for comparisons.</typeparam>
    /// <param name="number">The number to clip.</param>
    /// <param name="min">If specified, the smallest number to return.</param>
    /// <param name="max">If specified, the greatest number to return.</param>
    /// <returns>
    /// The parameter <paramref name="min"/> if <paramref name="number"/> is smaller than <paramref name="min"/>,
    /// otherwise, the parameter <paramref name="max"/> if <paramref name="number"/> is greater than
    /// <paramref name="max"/>, otherwise the parameter <paramref name="number"/>.
    /// </returns>
    [Pure]
    public static T Clamp<T>(this T number, T? min = null, T? max = null)
        where T : class, IComparisonOperators<T, T, bool> =>
        (min ?? number) is var small &&
        (max ?? number) is var big &&
        number <= small ? small :
        number >= big ? big : number;

    /// <inheritdoc cref="Clamp{T}(T, T?, T?)"/>
    [Pure]
    public static T Clamp<T>(this T number, T? min = null, T? max = null)
        where T : struct, IComparisonOperators<T, T, bool> =>
        (min ?? number) is var small &&
        (max ?? number) is var big &&
        number <= small ? small :
        number >= big ? big : number;

#endif
}
