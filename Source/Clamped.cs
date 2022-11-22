// <copyright file="Clamped.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>
namespace Emik.Morsels;

/// <summary>Extension methods to clamp numbers.</summary>
static class Clamped
{
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
    internal static int Clamp(this int number, int? min = null, int? max = null) =>
        (min ?? number) is var small &&
        (max ?? number) is var big &&
        number <= small ? small :
        number >= big ? big : number;

    /// <inheritdoc cref="Clamp(int, int?, int?)"/>
    [Pure]
    internal static float Clamp(this float number, float? min = null, float? max = null) =>
        (min ?? number) is var small &&
        (max ?? number) is var big &&
        number <= small ? small :
        number >= big ? big : number;
}
