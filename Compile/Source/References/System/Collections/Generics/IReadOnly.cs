// SPDX-License-Identifier: MPL-2.0

// ReSharper disable CheckNamespace EmptyNamespace
namespace System.Collections.Generic;
#pragma warning disable CA1710, SA1649, MA0048
#if NETFRAMEWORK && !NET45_OR_GREATER
/// <summary>Provides a read-only, covariant view of a generic list.</summary>
/// <typeparam name="T">The type of item on the list.</typeparam>
partial interface IReadOnlyCollection<
#if NET40_OR_GREATER
    out
#endif
    T> : IEnumerable<T>
{
    /// <summary>Gets the amount of items on the list.</summary>
    int Count { [Pure] get; }
}

/// <summary>Represents a generic read-only collection of key/value pairs.</summary>
/// <typeparam name="TKey">The type of keys in the read-only dictionary.</typeparam>
/// <typeparam name="TValue">The type of values in the read-only dictionary.</typeparam>
partial interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
{
    /// <summary>Gets an enumerable collection that contains the keys in the read-only dictionary.</summary>
    IEnumerable<TKey> Keys { [Pure] get; }

    /// <summary>Gets an enumerable collection that contains the values in the read-only dictionary.</summary>
    IEnumerable<TValue> Values { [Pure] get; }

    /// <summary>Gets the element that has the specified key in the read-only dictionary.</summary>
    /// <param name="key">The key to locate.</param>
    TValue this[TKey key] { [Pure] get; }

    /// <summary>Determines whether the read-only dictionary contains an element that has the specified key.</summary>
    /// <param name="key">The key to locate.</param>
    /// <returns>
    /// <see langword="true"/> if the read-only dictionary contains an element that has the specified key;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
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
    [Pure]
    bool TryGetValue(TKey key, out TValue value);
}

/// <summary>Represents a read-only collection of elements that can be accessed by index.</summary>
/// <typeparam name="T">The type of elements in the read-only list.</typeparam>
partial interface IReadOnlyList<
#if NET40_OR_GREATER
    out
#endif
    T> : IReadOnlyCollection<T>
{
    /// <summary>Performs an index operation on the list.</summary>
    /// <param name="index">The item to retrieve.</param>
    T this[int index] { [Pure] get; }
}
#endif
#if !NET5_0_OR_GREATER
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
    [Pure]
    bool IsProperSubsetOf(IEnumerable<T> other);

    /// <summary>Determines whether the current set is a proper (strict) superset of a specified collection.</summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns>
    /// <see langword="true"/> if the collection is a proper superset of other; otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">other is <see langword="null"/>.</exception>
    [Pure]
    bool IsProperSupersetOf(IEnumerable<T> other);

    /// <summary>Determine whether the current set is a subset of a specified collection.</summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns><see langword="true"/> if the current set is a subset of other; otherwise <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">other is <see langword="null"/>.</exception>
    [Pure]
    bool IsSubsetOf(IEnumerable<T> other);

    /// <summary>Determine whether the current set is a super set of a specified collection.</summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns><see langword="true"/> if the current set is a subset of other; otherwise <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">other is <see langword="null"/>.</exception>
    [Pure]
    bool IsSupersetOf(IEnumerable<T> other);

    /// <summary>Determines whether the current set overlaps with the specified collection.</summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns>
    /// <see langword="true"/> if the current set and other share at least one common element;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">other is <see langword="null"/>.</exception>
    [Pure]
    bool Overlaps(IEnumerable<T> other);

    /// <summary>Determines whether the current set and the specified collection contain the same elements.</summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns>
    /// <see langword="true"/> if the current set is equal to other; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">other is <see langword="null"/>.</exception>
    [Pure]
    bool SetEquals(IEnumerable<T> other);
}
#endif
#if NETFRAMEWORK && !NET40_OR_GREATER
/// <summary>Provides the base interface for the abstraction of sets.</summary>
/// <remarks><para>
/// This interface provides methods for implementing sets,
/// which are collections that have unique elements and specific operations.
/// </para></remarks>
/// <typeparam name="T">The type of elements in the set.</typeparam>
partial interface ISet<T> : ICollection<T>
{
    /// <summary>Removes all elements in the specified collection from the current set.</summary>
    /// <remarks><para>
    /// This method is an O(<c>n</c>) operation,
    /// where <c>n</c> is the number of elements in the <paramref name="other"/> parameter.
    /// </para></remarks>
    /// <param name="other">The collection of items to remove from the set.</param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    void ExceptWith(IEnumerable<T> other);

    /// <summary>
    /// Modifies the current set so that it contains only elements that are also in a specified collection.
    /// </summary>
    /// <remarks><para>This method ignores any duplicate elements in other.</para></remarks>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    void IntersectWith(IEnumerable<T> other);

    /// <summary>
    /// Modifies the current set so that it contains only elements that are present
    /// either in the current set or in the specified collection, but not both.
    /// </summary>
    /// <remarks><para>Any duplicate elements in <paramref name="other"/> are ignored.</para></remarks>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    void SymmetricExceptWith(IEnumerable<T> other);

    /// <summary>
    /// Modifies the current set so that it contains all elements that are present in the current set,
    /// in the specified collection, or in both.
    /// </summary>
    /// <remarks><para>Any duplicate elements in <paramref name="other"/> are ignored.</para></remarks>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    void UnionWith(IEnumerable<T> other);

    /// <summary>
    /// Adds an element to the current set and returns a value to indicate if the element was successfully added.
    /// </summary>
    /// <param name="item">The element to add to the set.</param>
    /// <returns>
    /// <see langword="true"/> if the element is added to the set;
    /// <see langword="false"/> if the element is already in the set.
    /// </returns>
    new bool Add(T item);

    /// <summary>Determines whether the current set is a proper (strict) superset of a specified collection.</summary>
    /// <remarks><para>
    /// If the current set is a proper superset of <paramref name="other"/>,
    /// <paramref name="other"/> must have at least one element that the current set does not have.
    /// </para><para>
    /// An empty set is a proper superset of any other collection. Therefore, this method returns <see langword="true"/>
    /// if the current set is empty, unless the <paramref name="other"/> parameter is also an empty set.
    /// </para><para>
    /// This method always returns <see langword="false"/> if the current set is
    /// less than or equal to the number of elements in <paramref name="other"/>.
    /// </para></remarks>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    /// <returns>
    /// <see langword="true"/> if the current set is a proper superset of <paramref name="other"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    bool IsProperSupersetOf(IEnumerable<T> other);

    /// <summary>Determines whether the current set is a proper (strict) subset of a specified collection.</summary>
    /// <remarks><para>
    /// If the current set is a proper subset of <paramref name="other"/>,
    /// <paramref name="other"/> must have at least one element that the current set does not have.
    /// </para><para>
    /// An empty set is a proper subset of any other collection. Therefore, this method returns <see langword="true"/>
    /// if the current set is empty, unless the <paramref name="other"/> parameter is also an empty set.
    /// </para><para>
    /// This method always returns <see langword="false"/> if the current set has
    /// more or the same number of elements than <paramref name="other"/>.
    /// </para></remarks>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    /// <returns>
    /// <see langword="true"/> if the current set is a proper subset of <paramref name="other"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    bool IsProperSubsetOf(IEnumerable<T> other);

    /// <summary>Determines whether a set is a subset of a specified collection.</summary>
    /// <remarks><para>
    /// If <paramref name="other"/> contains the same elements as the current set,
    /// the current set is still considered a subset of <paramref name="other"/>.
    /// </para><para>
    /// This method always returns <see langword="false"/> if the current
    /// set has elements that are not in <paramref name="other"/>.
    /// </para></remarks>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    /// <returns>
    /// <see langword="true"/> if the current set is a subset of <paramref name="other"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    bool IsSubsetOf(IEnumerable<T> other);

    /// <summary>Determines whether the current set is a superset of a specified collection.</summary>
    /// <remarks><para>
    /// If <paramref name="other"/> contains the same elements as the current set,
    /// the current set is still considered a superset of <paramref name="other"/>.
    /// </para><para>
    /// This method always returns <see langword="false"/> if the current
    /// set has fewer elements than <paramref name="other"/>.
    /// </para></remarks>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    /// <returns>
    /// <see langword="true"/> if the current set is a superset of <paramref name="other"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    bool IsSupersetOf(IEnumerable<T> other);

    /// <summary>Determines whether the current set overlaps with the specified collection.</summary>
    /// <remarks><para>Any duplicate elements in <paramref name="other"/> are ignored.</para></remarks>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    /// <returns>
    /// <see langword="true"/> if the current set and <paramref name="other"/> share at least one common element;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    bool Overlaps(IEnumerable<T> other);

    /// <summary>Determines whether the current set and the specified collection contain the same elements.</summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    /// <returns>
    /// <see langword="true"/> if the current set is equal to <paramref name="other"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    bool SetEquals(IEnumerable<T> other);
}
#endif
