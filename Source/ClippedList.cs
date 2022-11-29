// <copyright file="ClippedList.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>
namespace Emik.Morsels;

using static CollectionAccessType;

/// <summary>
/// Encapsulates an <see cref="IList{T}"/> where indices are always clamped and therefore never be out of range.
/// </summary>
/// <typeparam name="T">The generic type of the encapsulated <see cref="IList{T}"/>.</typeparam>
sealed class ClippedList<T> : IList<T>, IReadOnlyList<T>
{
    [ProvidesContext]
    readonly IList<T> _list;

    /// <summary>Initializes a new instance of the <see cref="ClippedList{T}"/> class.</summary>
    /// <param name="list">The <see cref="IList{T}"/> to encapsulate.</param>
    /// <exception cref="ArgumentOutOfRangeException"><see cref="Count"/> returns a non-positive number.</exception>
    public ClippedList([ProvidesContext] IList<T> list) => _list = list;

    /// <inheritdoc/>
    [CollectionAccess(None), Pure]
    public bool IsReadOnly => _list.IsReadOnly;

    /// <inheritdoc cref="ICollection{T}.Count"/>
    [CollectionAccess(None), Pure, ValueRange(1, int.MaxValue)]
    public int Count => _list.Count;

    /// <inheritdoc cref="IList{T}.this"/>
    [Pure]
    public T this[int index]
    {
        [CollectionAccess(Read)] get => _list[Clamp(index)];
        [CollectionAccess(ModifyExistingContent)] set => _list[Clamp(index)] = value;
    }

    /// <inheritdoc/>
    [CollectionAccess(UpdatedContent)]
    public void Add(T item) => _list.Add(item);

    /// <inheritdoc/>
    [CollectionAccess(ModifyExistingContent)]
    public void Clear() => _list.Clear();

    /// <inheritdoc/>
    [CollectionAccess(Read)]
    public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    [CollectionAccess(UpdatedContent)]
    public void Insert(int index, T item) => _list.Insert(Clamp(index), item);

    /// <inheritdoc/>
    [CollectionAccess(ModifyExistingContent)]
    public void RemoveAt(int index) => _list.RemoveAt(Clamp(index));

    /// <inheritdoc cref="ICollection{T}.Contains"/>
    [CollectionAccess(Read), Pure]
    public bool Contains(T item) => _list.Contains(item);

    /// <inheritdoc/>
    [CollectionAccess(Read | ModifyExistingContent), Pure]
    public bool Remove(T item) => _list.Remove(item);

    /// <inheritdoc/>
    [CollectionAccess(Read), Pure]
    public int IndexOf(T item) => _list.IndexOf(item);

    /// <inheritdoc />
    [CollectionAccess(Read), Pure] // ReSharper disable once ReturnTypeCanBeNotNullable
    public override string? ToString() => _list.ToString();

    /// <inheritdoc/>
    [CollectionAccess(Read), Pure]
    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

    /// <inheritdoc/>
    [CollectionAccess(Read), Pure]
    IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

    [NonNegativeValue, Pure]
    int Clamp(int index) => Count is var i && i is not 0 ? index.Clamp(0, i) : throw CannotBeEmpty;
}

/// <summary>Extension methods that act as factories for <see cref="ClippedList{T}"/>.</summary>
#pragma warning disable MA0048
static class ClippedFactory
#pragma warning restore MA0048
{
    /// <summary>Wraps an <see cref="IList{T}"/> (upcasted/created) to <see cref="ClippedList{T}"/>.</summary>
    /// <typeparam name="T">The type of the <paramref name="iterable"/> and the <see langword="return"/>.</typeparam>
    /// <param name="iterable">The collection to turn into a <see cref="ClippedList{T}"/>.</param>
    /// <returns>A <see cref="ClippedList{T}"/> of <paramref name="iterable"/>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterable))]
    internal static ClippedList<T>? ToClippedLazily<T>(this IEnumerable<T>? iterable) =>
        iterable is null
            ? null
            : iterable as ClippedList<T> ?? new ClippedList<T>(iterable as IList<T> ?? iterable.ToList());
}
