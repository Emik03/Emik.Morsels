// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;
#if !NET20 && !NET30
using static CollectionAccessType;

/// <summary>Extension methods that act as factories for read-only lists.</summary>
static partial class ReadOnlyFactory
{
    /// <summary>Encapsulates an <see cref="IList{T}"/> and make all mutating methods a no-op.</summary>
    /// <typeparam name="T">The type of element in the list.</typeparam>
    /// <param name="list">The list to encapsulate.</param>
    sealed partial class ReadOnlyList<T>([ProvidesContext] IList<T> list) : IList<T>, IReadOnlyList<T>
    {
        /// <inheritdoc />
        [Pure]
        public bool IsReadOnly => true;

        /// <inheritdoc cref="ICollection{T}.Count"/>
        [CollectionAccess(Read), Pure]
        public int Count => list.Count;

        /// <inheritdoc cref="IList{T}.this" />
        [Pure]
        public T this[int index]
        {
            [CollectionAccess(Read)] get => list[index];
            [CollectionAccess(None)] set { }
        }

        /// <inheritdoc />
        [CollectionAccess(Read)]
        public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

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
        public bool Contains(T item) => list.Contains(item);

        /// <inheritdoc />
        [CollectionAccess(None), Pure]
        bool ICollection<T>.Remove(T? item) => false;

        /// <inheritdoc />
        [CollectionAccess(Read), Pure]
        public int IndexOf(T item) => list.IndexOf(item);

        /// <inheritdoc />
        [CollectionAccess(Read), Pure]
        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

        /// <inheritdoc />
        [CollectionAccess(Read), Pure]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        [CollectionAccess(Read), Pure]
        public override string ToString() => list.ToString() ?? "";
    }

    /// <summary>Wraps an <see cref="IList{T}"/> (upcasted/created) to a read-only list.</summary>
    /// <typeparam name="T">The type of the <paramref name="iterable"/> and the <see langword="return"/>.</typeparam>
    /// <param name="iterable">The collection to turn into a read-only list.</param>
    /// <returns>A read-only list of <paramref name="iterable"/>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterable))]
    public static IReadOnlyList<T>? ReadOnly<T>(this IEnumerable<T>? iterable) =>
        iterable is null
            ? null
            : iterable as IReadOnlyList<T> ?? new ReadOnlyList<T>(iterable as IList<T> ?? [.. iterable]);
}
#endif
