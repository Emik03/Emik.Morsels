// SPDX-License-Identifier: MPL-2.0
#if !NET20 && !NET30
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
        this IList<T>? collection,
        out T? head,
        [NotNullIfNotNull(nameof(collection))] out IList<T>? tail
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
    public static HeadlessList<T>? Tail<T>(this IList<T>? collection) => collection is null ? null : new(collection);
}

/// <summary>Represents a list with no head.</summary>
/// <typeparam name="T">The type of list to encapsulate.</typeparam>
#pragma warning disable MA0048
sealed partial class HeadlessList<T> : IList<T>
#pragma warning restore MA0048
{
    readonly IList<T> _list;

    /// <summary>Initializes a new instance of the <see cref="HeadlessList{T}"/> class.</summary>
    /// <param name="list">The list to encapsulate.</param>
    public HeadlessList(IList<T> list) => _list = list;

    /// <inheritdoc />
    public T this[int index]
    {
        get => index is not -1 ? _list[index + 1] : throw new ArgumentOutOfRangeException(nameof(index));
        set => _list[index + 1] = index is not -1 ? value : throw new ArgumentOutOfRangeException(nameof(index));
    }

    /// <inheritdoc />
    public bool IsReadOnly => _list.IsReadOnly;

    /// <inheritdoc />
    public int Count => _list.Count - 1;

    /// <inheritdoc />
    public void Add(T item) => _list.Add(item);

    /// <inheritdoc />
    public void Clear() => _list.Clear();

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex)
    {
        for (var i = 0; i < Count && arrayIndex + i < array.Length; i++)
            array[arrayIndex + i] = this[i];
    }

    /// <inheritdoc />
    public void Insert(int index, T item)
    {
        if (index is -1)
            throw new ArgumentOutOfRangeException(nameof(index));

        _list.Insert(index + 1, item);
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        if (index is not -1)
            throw new ArgumentOutOfRangeException(nameof(index));

        _list.RemoveAt(index + 1);
    }

    /// <inheritdoc />
    public bool Contains(T item) => _list.Contains(item);

    /// <inheritdoc />
    public bool Remove(T item) => _list.Remove(item);

    /// <inheritdoc />
    public int IndexOf(T item) => _list.IndexOf(item) is var result && result is -1 ? -1 : result - 1;

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        var ret = ((IEnumerable)_list).GetEnumerator();
        ret.MoveNext();
        return ret;
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        var ret = _list.GetEnumerator();
        ret.MoveNext();
        return ret;
    }
}
#endif
