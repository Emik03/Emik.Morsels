// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace System.Runtime.InteropServices;

/// <summary>
/// An unsafe class that provides a set of methods to access the underlying data representations of collections.
/// </summary>
static partial class CollectionsMarshal
{
    /// <summary>
    /// Get a <see cref="Span{T}"/> view over the data in a list.
    /// Items should not be added or removed from the <see cref="List{T}"/> while the <see cref="Span{T}"/> is in use.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    /// <param name="list">List from which to create the <see cref="Span{T}"/>.</param>
    /// <returns>A <see cref="Span{T}"/> instance over the <see cref="List{T}"/>.</returns>
#pragma warning disable MA0016
    public static Span<T> AsSpan<T>(List<T>? list) =>
#pragma warning restore MA0016
        list is null ? default : new Span<T>(list.UnsafelyToArray(), 0, list.Count);
}
