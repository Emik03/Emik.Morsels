#region Emik.MPL

// <copyright file="Force.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

#endregion

// ReSharper disable InvertIf
#pragma warning disable IDE0059
namespace Emik.Morsels;

/// <summary>Extension methods to force full enumerations.</summary>
static partial class Force
{
    /// <summary>Forces an enumeration, meant for enumerations that have side effects.</summary>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    internal static void Enumerate([InstantHandle] this IEnumerable? iterable)
    {
        if (iterable is not null)
            foreach (var unused in iterable) { }
    }

    /// <summary>Forces an enumeration, meant for enumerations that have side effects.</summary>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    internal static void Enumerate<T>([InstantHandle] this IEnumerable<T>? iterable)
    {
        if (iterable is not null)
            foreach (var unused in iterable) { }
    }
}
