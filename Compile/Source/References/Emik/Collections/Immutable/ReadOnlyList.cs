// SPDX-License-Identifier: MPL-2.0

// ReSharper disable RedundantExtendsListEntry
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

using static CollectionAccessType;

#if !NET20 && !NET30
/// <summary>Extension methods that act as factories for read-only lists.</summary>
#pragma warning disable MA0048
static partial class ReadOnlyFactory
#pragma warning restore MA0048
{
    /// <summary>Wraps an <see cref="IList{T}"/> (upcasted/created) to a read-only list.</summary>
    /// <typeparam name="T">The type of the <paramref name="iterable"/> and the <see langword="return"/>.</typeparam>
    /// <param name="iterable">The collection to turn into a read-only list.</param>
    /// <returns>A read-only list of <paramref name="iterable"/>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterable))]
    public static IReadOnlyList<T>? ToReadOnly<T>(this IEnumerable<T>? iterable) =>
        iterable is null
            ? null
            : iterable as IReadOnlyList<T> ?? new ReadOnlyList<T>(iterable as IList<T> ?? iterable.ToList());
}
#endif

/// <summary>Encapsulates an <see cref="IList{T}"/> and make all mutating methods a no-op.</summary>
/// <typeparam name="T">The type of element in the list.</typeparam>
sealed partial class ReadOnlyList<T> : IList<T>, IReadOnlyList<T>
{
    [ProvidesContext]
    readonly IList<T> _list;

    /// <summary>Initializes a new instance of the <see cref="ReadOnlyList{T}"/> class.</summary>
    /// <param name="list">The list to encapsulate.</param>
    public ReadOnlyList([ProvidesContext] IList<T> list) => _list = list;

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
    [CollectionAccess(Read)]
    public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    [CollectionAccess(None)]
    void ICollection<T>.Add(T? item) { }

    /// <inheritdoc />
    [CollectionAccess(None)]
    void ICollection<T>.Clear() { }

    /// <inheritdoc />
    [CollectionAccess(None)]
    void IList<T>.Insert(int index, T? item) { }

    /// <inheritdoc />
    [CollectionAccess(None)]
    void IList<T>.RemoveAt(int index) { }

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    public bool Contains(T item) => _list.Contains(item);

    /// <inheritdoc />
    [CollectionAccess(None), Pure]
    bool ICollection<T>.Remove(T? item) => false;

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    public int IndexOf(T item) => _list.IndexOf(item);

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [CollectionAccess(Read), Pure] // ReSharper disable once ReturnTypeCanBeNotNullable
    public override string? ToString() => _list.ToString();
}
