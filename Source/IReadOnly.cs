#region Emik.MPL

// <copyright file="IReadOnly.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

#endregion

// ReSharper disable CheckNamespace EmptyNamespace
namespace System.Collections.Generic;
#pragma warning disable CA1710, SA1649, MA0048
#if NET40
/// <summary>Provides a read-only, covariant view of a generic list.</summary>
/// <typeparam name="T">The type of item on the list.</typeparam>
partial interface IReadOnlyCollection<out T> : IEnumerable<T>
{
    /// <summary>Gets the amount of items on the list.</summary>
    int Count { get; }
}

/// <summary>Represents a generic read-only collection of key/value pairs.</summary>
/// <typeparam name="TKey">The type of keys in the read-only dictionary.</typeparam>
/// <typeparam name="TValue">The type of values in the read-only dictionary.</typeparam>
partial interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
{
    /// <summary>Gets an enumerable collection that contains the keys in the read-only dictionary.</summary>
    IEnumerable<TKey> Keys { get; }

    /// <summary>Gets an enumerable collection that contains the values in the read-only dictionary.</summary>
    IEnumerable<TValue> Values { get; }

    /// <summary>Gets the element that has the specified key in the read-only dictionary.</summary>
    /// <param name="key">The key to locate.</param>
    TValue this[TKey key] { get; }

    /// <summary>Determines whether the read-only dictionary contains an element that has the specified key.</summary>
    /// <param name="key">The key to locate.</param>
    /// <returns>
    /// <see langword="true"/> if the read-only dictionary contains an element that has the specified key;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    bool ContainsKey(TKey key);

    /// <summary>Gets the value that is associated with the specified key.</summary>
    /// <param name="key">The key to locate.</param>
    /// <param name="value">
    /// When this method returns, the value associated with the specified key, if the key is found;
    /// otherwise, the default value for the type of the <paramref name="value"/> parameter.
    /// This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the object that implements the <see cref="IReadOnlyDictionary{TKey,TValue}"/>
    /// interface contains an element that has the specified key; otherwise, <see langword="false"/>.
    /// </returns>
    bool TryGetValue(TKey key, out TValue value);
}

/// <summary>Represents a read-only collection of elements that can be accessed by index.</summary>
/// <typeparam name="T">The type of elements in the read-only list.</typeparam>
partial interface IReadOnlyList<out T> : IReadOnlyCollection<T>
{
    /// <summary>Performs an index operation on the list.</summary>
    /// <param name="index">The item to retrieve.</param>
    T this[int index] { get; }
}
#endif
#if !NET35 && !NET5_0_OR_GREATER
/// <summary>Provides a readonly abstraction of a set.</summary>
/// <typeparam name="T">The type of elements in the set.</typeparam>
partial interface IReadOnlySet<T> : IReadOnlyCollection<T>
{
    /// <summary>Determines if the set contains a specific item.</summary>
    /// <param name="item">The item to check if the set contains.</param>
    /// <returns><see langword="true"/> if found; otherwise <see langword="false"/>.</returns>
    [Pure]
    bool Contains(T item);

    /// <summary>Determines whether the current set is a proper (strict) subset of a specified collection.</summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns>
    /// <see langword="true"/> if the current set is a proper subset of other; otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">other is <see langword="null"/>.</exception>
    bool IsProperSubsetOf(IEnumerable<T> other);

    /// <summary>Determines whether the current set is a proper (strict) superset of a specified collection.</summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns>
    /// <see langword="true"/> if the collection is a proper superset of other; otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">other is <see langword="null"/>.</exception>
    bool IsProperSupersetOf(IEnumerable<T> other);

    /// <summary>Determine whether the current set is a subset of a specified collection.</summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns><see langword="true"/> if the current set is a subset of other; otherwise <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">other is <see langword="null"/>.</exception>
    bool IsSubsetOf(IEnumerable<T> other);

    /// <summary>Determine whether the current set is a super set of a specified collection.</summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns><see langword="true"/> if the current set is a subset of other; otherwise <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">other is <see langword="null"/>.</exception>
    bool IsSupersetOf(IEnumerable<T> other);

    /// <summary>Determines whether the current set overlaps with the specified collection.</summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns>
    /// <see langword="true"/> if the current set and other share at least one common element;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">other is <see langword="null"/>.</exception>
    bool Overlaps(IEnumerable<T> other);

    /// <summary>Determines whether the current set and the specified collection contain the same elements.</summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns>
    /// <see langword="true"/> if the current set is equal to other; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">other is <see langword="null"/>.</exception>
    bool SetEquals(IEnumerable<T> other);
}
#endif
