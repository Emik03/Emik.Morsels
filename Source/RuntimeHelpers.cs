#region Emik.MPL

// <copyright file="RuntimeHelpers.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

#endregion

#if !(NETSTANDARD2_1_OR_GREATER || NETCOREAPP)
namespace System.Runtime.CompilerServices;

/// <summary>
/// Provides a set of static methods and properties that provide support for compilers. This class cannot be inherited.
/// </summary>
static class RuntimeHelpers
{
    /// <summary>Slices the specified array using the specified range.</summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array to slice.</param>
    /// <param name="range">
    /// An object that determines the portion of <paramref name="array"/> to include in the slice.
    /// </param>
    /// <returns>The subarray defined by <paramref name="range"/>.</returns>
    [Pure]
    internal static T[] GetSubArray<T>(T[] array, Range range)
    {
        range.GetOffsetAndLength(array.Length, out var offset, out var length);

        var isArrayTypeEqual = default(T) is not null || typeof(T[]) == array.GetType();

        if (isArrayTypeEqual && length is 0)
#if NET35
            return ArrayEx.Empty<T>();
#elif NET46_OR_GREATER || NETSTANDARD1_3_OR_GREATER || NETCOREAPP
            return Array.Empty<T>();
#else
            return new T[0];
#endif

        var dest = isArrayTypeEqual ? new T[length] :
            array.GetType().GetElementType() is { } element ? (T[])Array.CreateInstance(element, length) :
            throw Unreachable;

        Array.Copy(array, offset, dest, 0, length);
        return dest;
    }
}
#endif
