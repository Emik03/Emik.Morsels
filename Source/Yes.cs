// <copyright file="Yes.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>
namespace Emik.Morsels;

using static CollectionAccessType;

/// <summary>A factory for creating iterator types that yield the same item forever.</summary>
/// <typeparam name="T">The type of the item to yield.</typeparam>
[StructLayout(LayoutKind.Auto)]
readonly partial struct Yes<T> : IEnumerable<T>, IEnumerator<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Yes{T}"/> struct. Prepares enumeration of a single item forever.
    /// </summary>
    /// <param name="value">The item to use.</param>
    public Yes([ProvidesContext] T value) => Current = value;

    /// <inheritdoc />
    [CollectionAccess(Read), ProvidesContext, Pure]
    public T Current { get; }

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    object? IEnumerator.Current => Current;

    /// <summary>Implicitly calls the constructor.</summary>
    /// <param name="value">The value to pass into the constructor.</param>
    /// <returns>A new instance of <see cref="Yes{T}"/> with <paramref name="value"/> passed in.</returns>
    [CollectionAccess(Read), Pure]
    public static implicit operator Yes<T>([ProvidesContext] T value) => new(value);

    /// <summary>Implicitly calls <see cref="Current"/>.</summary>
    /// <param name="value">The value to call <see cref="Current"/>.</param>
    /// <returns>The value that was passed in to this instance.</returns>
    [CollectionAccess(Read), Pure]
    public static implicit operator T(Yes<T> value) => value.Current;

    /// <summary>Returns itself.</summary>
    /// <remarks><para>Used to allow <see langword="foreach"/> to be used on <see cref="Yes{T}"/>.</para></remarks>
    /// <returns>Itself.</returns>
    [CollectionAccess(None), Pure]
    public Yes<T> GetEnumerator() => this;

    /// <inheritdoc />
    [CollectionAccess(None)]
    void IDisposable.Dispose() { }

    /// <inheritdoc />
    [CollectionAccess(None)]
    void IEnumerator.Reset() { }

    /// <inheritdoc />
    [CollectionAccess(None), Pure]
    bool IEnumerator.MoveNext() => true;

    /// <inheritdoc />
    [CollectionAccess(None), Pure]
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [CollectionAccess(None), Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>Extension methods that act as factories for <see cref="Yes{T}"/>.</summary>
#pragma warning disable MA0048
static partial class YesFactory
#pragma warning restore MA0048
{
    /// <summary>Creates a <see cref="Yes{T}"/> from an item.</summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="source">The item.</param>
    /// <returns>The <see cref="Yes{T}"/> instance that can be yielded forever.</returns>
    [Pure]
    internal static Yes<T> Forever<T>(this T source) => source;
}
