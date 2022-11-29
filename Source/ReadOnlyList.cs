// <copyright file="ReadOnlyList.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>
namespace Emik.Morsels;

using static CollectionAccessType;

/// <summary>Encapsulates an <see cref="IList{T}"/> and make all mutating methods a no-op.</summary>
/// <typeparam name="T">The type of element in the list.</typeparam>
sealed class ReadOnlyList<T> : IList<T>, IReadOnlyList<T>
{
    [ProvidesContext]
    readonly IList<T> _list;

    /// <summary>Initializes a new instance of the <see cref="ReadOnlyList{T}"/> class.</summary>
    /// <param name="list">The list to encapsulate.</param>
    internal ReadOnlyList([ProvidesContext] IList<T> list) => _list = list;

    /// <inheritdoc />
    [Pure]
    public bool IsReadOnly => true;

    /// <inheritdoc cref="ICollection{T}.Count"/>
    [CollectionAccess(Read), Pure]
    public int Count => _list.Count;

    /// <inheritdoc cref="IList{T}.this" />
    [Pure]
    public T this[int index]
    {
        [CollectionAccess(Read)] get => _list[index];
        [CollectionAccess(None)] set { }
    }

    /// <inheritdoc />
    [CollectionAccess(None)]
    public void Add(T item) { }

    /// <inheritdoc />
    [CollectionAccess(None)]
    public void Clear() { }

    /// <inheritdoc />
    [CollectionAccess(Read)]
    public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    [CollectionAccess(None)]
    public void Insert(int index, T item) { }

    /// <inheritdoc />
    [CollectionAccess(None)]
    public void RemoveAt(int index) { }

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    public bool Contains(T item) => _list.Contains(item);

    /// <inheritdoc />
    [CollectionAccess(None), Pure]
    public bool Remove(T item) => false;

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    public int IndexOf(T item) => _list.IndexOf(item);

    /// <inheritdoc />
    [CollectionAccess(Read), Pure] // ReSharper disable once ReturnTypeCanBeNotNullable
    public override string? ToString() => _list.ToString();

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>Extension methods that act as factories for <see cref="IReadOnlyList{T}"/>.</summary>
#pragma warning disable MA0048
static class ReadOnlyFactory
#pragma warning restore MA0048
{
    /// <summary>Wraps an <see cref="IList{T}"/> (upcasted/created) to <see cref="IReadOnlyList{T}"/>.</summary>
    /// <typeparam name="T">The type of the <paramref name="iterable"/> and the <see langword="return"/>.</typeparam>
    /// <param name="iterable">The collection to turn into a <see cref="IReadOnlyList{T}"/>.</param>
    /// <returns>A <see cref="IReadOnlyList{T}"/> of <paramref name="iterable"/>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterable))]
    internal static IReadOnlyList<T>? ToReadOnly<T>(this IEnumerable<T>? iterable) =>
        iterable is null
            ? null
            : iterable as IReadOnlyList<T> ?? new ReadOnlyList<T>(iterable as IList<T> ?? iterable.ToList());
}
