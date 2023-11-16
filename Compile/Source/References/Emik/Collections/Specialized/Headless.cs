// SPDX-License-Identifier: MPL-2.0
#if !NET20 && !NET30
// ReSharper disable RedundantExtendsListEntry
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

using static CollectionAccessType;

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
[NoStructuralTyping]
#pragma warning disable MA0048
sealed partial class HeadlessList<T>([ProvidesContext] IList<T> list) : IList<T>
#pragma warning restore MA0048
{
    /// <inheritdoc cref="IList{T}.this" />
    [CollectionAccess(Read), Pure]
    public T this[int index]
    {
        get => index is not -1 ? list[index + 1] : throw new ArgumentOutOfRangeException(nameof(index));
        set => list[index + 1] = index is not -1 ? value : throw new ArgumentOutOfRangeException(nameof(index));
    }

    /// <inheritdoc />
    [CollectionAccess(None), Pure]
    public bool IsReadOnly => list.IsReadOnly;

    /// <inheritdoc cref="ICollection{T}.Count" />
    [CollectionAccess(None), Pure]
    public int Count => list.Count - 1;

    /// <inheritdoc />
    [CollectionAccess(UpdatedContent)]
    public void Add(T item) => list.Add(item);

    /// <inheritdoc />
    [CollectionAccess(ModifyExistingContent)]
    public void Clear() => list.Clear();

    /// <inheritdoc />
    [CollectionAccess(Read)]
    public void CopyTo(T[] array, int arrayIndex)
    {
        for (var i = 0; i < Count && arrayIndex + i < array.Length; i++)
            array[arrayIndex + i] = this[i];
    }

    /// <inheritdoc />
    [CollectionAccess(UpdatedContent)]
    public void Insert(int index, T item) =>
        list.Insert(index is not -1 ? index + 1 : throw new ArgumentOutOfRangeException(nameof(index)), item);

    /// <inheritdoc />
    [CollectionAccess(ModifyExistingContent)]
    public void RemoveAt(int index) =>
        list.RemoveAt(index is not -1 ? index + 1 : throw new ArgumentOutOfRangeException(nameof(index)));

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    public bool Contains(T item) => list.Contains(item);

    /// <inheritdoc />
    [CollectionAccess(Read | ModifyExistingContent), Pure]
    public bool Remove(T item) => list.Remove(item);

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    public int IndexOf(T item) =>
        list.IndexOf(item) switch
        {
            0 => Find(item),
            -1 => -1,
            var x => x - 1,
        };

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    IEnumerator IEnumerable.GetEnumerator()
    {
        var ret = ((IEnumerable)list).GetEnumerator();
        ret.MoveNext();
        return ret;
    }

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    public IEnumerator<T> GetEnumerator()
    {
        var ret = list.GetEnumerator();
        ret.MoveNext();
        return ret;
    }

    /// <inheritdoc />
    [CollectionAccess(Read), Pure] // ReSharper disable once ReturnTypeCanBeNotNullable
    public override string? ToString() => list.ToString();

    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure, ValueRange(-1, int.MaxValue)]
    int Find(T item)
    {
        for (var i = 0; i < Count; i++)
            if (EqualityComparer<T>.Default.Equals(this[i], item))
                return i;

        return -1;
    }
}
#endif
