// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides the deconstruction to extract the head and tail of a collection.</summary>
static partial class Headless
{
    /// <summary>Separates the head from the tail of an <see cref="ICollection{T}"/>.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="collection">The enumerable to split.</param>
    /// <param name="head">The first element of the parameter <paramref name="collection"/>.</param>
    /// <param name="tail">The rest of the parameter <paramref name="collection"/>.</param>
    public static void Deconstruct<T>(
        this ICollection<T>? collection,
        out T? head,
        [NotNullIfNotNull(nameof(collection))] out ICollection<T>? tail
    )
    {
        head = collection is null ? default : collection.FirstOrDefault();
        tail = collection.Tail();
    }

    /// <summary>Gets the tail of the <see cref="ICollection{T}"/>.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="collection">The collection to extract the tail from.</param>
    /// <returns>
    /// The encapsulation of the parameter <paramref name="collection"/> that prevents the head from being accessed.
    /// </returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(collection))]
    public static HeadlessCollection<T>? Tail<T>(this ICollection<T>? collection) =>
        collection is null ? null : new(collection);
}

/// <summary>Represents a collection with no head.</summary>
/// <typeparam name="T">The type of collection to encapsulate.</typeparam>
#pragma warning disable MA0048
sealed partial class HeadlessCollection<T> : ICollection<T>
#pragma warning restore MA0048
{
    readonly ICollection<T> _collection;

    /// <summary>Initializes a new instance of the <see cref="HeadlessCollection{T}"/> class.</summary>
    /// <param name="collection">The collection to encapsulate.</param>
    public HeadlessCollection(ICollection<T> collection) => _collection = collection;

    /// <inheritdoc />
    public bool IsReadOnly => _collection.IsReadOnly;

    /// <inheritdoc />
    public int Count => _collection.Count - 1;

    /// <inheritdoc />
    public void Add(T item) => _collection.Add(item);

    /// <inheritdoc />
    public void Clear() => _collection.Clear();

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex)
    {
        using var a = GetEnumerator();

        while (arrayIndex < array.Length && a.MoveNext())
            array[arrayIndex++] = a.Current;
    }

    /// <inheritdoc />
    public bool Contains(T item) => _collection.Contains(item);

    /// <inheritdoc />
    public bool Remove(T item) => _collection.Remove(item);

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        var ret = ((IEnumerable)_collection).GetEnumerator();
        ret.MoveNext();
        return ret;
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        var ret = _collection.GetEnumerator();
        ret.MoveNext();
        return ret;
    }
}
