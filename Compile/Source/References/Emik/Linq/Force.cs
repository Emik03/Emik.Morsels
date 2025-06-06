// SPDX-License-Identifier: MPL-2.0

// ReSharper disable InvertIf

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods to force full enumerations.</summary>
static partial class Force
{
    /// <summary>Forces an enumeration, meant for enumerations that have side effects.</summary>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    public static void Enumerate([InstantHandle] this IEnumerable? iterable)
    {
        if (iterable is not null)
            foreach (var _ in iterable) { }
    }

    /// <summary>Forces an enumeration, meant for enumerations that have side effects.</summary>
    /// <typeparam name="T">The type of iterator.</typeparam>
    /// <param name="iterable">The collection of items to go through one-by-one.</param>
    public static void Enumerate<T>([InstantHandle] this IEnumerable<T>? iterable)
    {
        if (iterable is not null)
            foreach (var _ in iterable) { }
    }
}
