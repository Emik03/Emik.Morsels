// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

using static CollectionAccessType;

#if !NET20 && !NET30
/// <summary>Extension methods that act as factories for <see cref="GuardedList{T}"/>.</summary>
static partial class GuardedFactory
{
    /// <summary>Wraps an <see cref="IList{T}"/> (upcasted/created) to <see cref="GuardedList{T}"/>.</summary>
    /// <typeparam name="T">The type of the <paramref name="iterable"/> and the <see langword="return"/>.</typeparam>
    /// <param name="iterable">The collection to turn into a <see cref="GuardedList{T}"/>.</param>
    /// <returns>A <see cref="GuardedList{T}"/> of <paramref name="iterable"/>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterable))]
    public static GuardedList<T>? ToGuardedLazily<T>(this IEnumerable<T>? iterable) =>
        iterable is null ? null : iterable as GuardedList<T> ?? new(iterable.ToIList());
}
#endif

/// <summary>
/// Encapsulates an <see cref="IList{T}"/> where applying an index will always result in an optional value;
/// an out of range value will always give the <see langword="default"/> value.
/// </summary>
/// <typeparam name="T">The generic type of the encapsulated <see cref="IList{T}"/>.</typeparam>
/// <param name="list">The <see cref="IList{T}"/> to encapsulate.</param>
sealed partial class GuardedList<T>([ProvidesContext] IList<T> list) : IList<T?>, IReadOnlyList<T?>
{
    /// <inheritdoc cref="IList{T}.this"/>
    [Pure]
    public T? this[int index]
    {
        [CollectionAccess(Read)] get => IsIn(index) ? list[index] : default;
        [CollectionAccess(ModifyExistingContent)]
        set
        {
            if (value is not null && IsIn(index))
                list[index] = value;
        }
    }

    /// <inheritdoc cref="ICollection{T}.IsReadOnly"/>
    [CollectionAccess(None), Pure]
    public bool IsReadOnly => list.IsReadOnly;

    /// <inheritdoc />
    bool ICollection<T?>.IsReadOnly => IsReadOnly;

    /// <inheritdoc cref="ICollection{T}.Count"/>
    [CollectionAccess(None), NonNegativeValue, Pure]
    public int Count => list.Count;

    /// <inheritdoc />
    int ICollection<T?>.Count => Count;

    /// <inheritdoc cref="ICollection{T}.Add"/>
    [CollectionAccess(UpdatedContent)]
    public void Add(T? item)
    {
        if (item is not null)
            list.Add(item);
    }

    /// <inheritdoc cref="ICollection{T}.Clear"/>
    [CollectionAccess(ModifyExistingContent)]
    public void Clear() => list.Clear();

    /// <inheritdoc cref="ICollection{T}.CopyTo"/>
    [CollectionAccess(Read)]
    public void CopyTo(T?[] array, int arrayIndex)
    {
        if (Count <= array.Length - arrayIndex)
            list.CopyTo(array as T[], arrayIndex);
    }

    /// <inheritdoc/>
    [CollectionAccess(UpdatedContent)]
    public void Insert(int index, T? item)
    {
        if (item is not null && IsIn(index))
            list.Insert(index, item);
    }

    /// <inheritdoc/>
    [CollectionAccess(ModifyExistingContent)]
    public void RemoveAt(int index)
    {
        if (IsIn(index))
            list.RemoveAt(index);
    }

    /// <inheritdoc />
    void ICollection<T?>.Add(T? item) => Add(item);

    /// <inheritdoc />
    void ICollection<T?>.Clear() => Clear();

    /// <inheritdoc />
    void ICollection<T?>.CopyTo(T?[] array, int arrayIndex) => CopyTo(array, arrayIndex);

    /// <inheritdoc cref="ICollection{T}.Contains"/>
    [CollectionAccess(Read), Pure]
    public bool Contains(T? item) => item is not null && list.Contains(item);

    /// <inheritdoc cref="ICollection{T}.Remove"/>
    [CollectionAccess(Read | ModifyExistingContent), Pure]
    public bool Remove(T? item) => item is not null && list.Remove(item);

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    bool ICollection<T?>.Contains(T? item) => Contains(item);

    /// <inheritdoc />
    [CollectionAccess(Read | ModifyExistingContent), Pure]
    bool ICollection<T?>.Remove(T? item) => Remove(item);

    /// <inheritdoc cref="IList{T}.IndexOf"/>
    [CollectionAccess(Read), Pure]
    public int IndexOf(T? item) => item is null ? -1 : list.IndexOf(item);

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    [CollectionAccess(Read), MustDisposeResource, Pure]
#pragma warning disable 8619 // Good job .NET 2.0 - 3.5 Nullable Analysis.
    public IEnumerator<T?> GetEnumerator() => list.GetEnumerator();
#pragma warning restore 8619
    /// <inheritdoc/>
    [CollectionAccess(Read), Pure]
    IEnumerator<T?> IEnumerable<T?>.GetEnumerator() => list.GetEnumerator();

    /// <inheritdoc/>
    [CollectionAccess(Read), Pure]
    IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    public override string ToString() => list.ToString().OrEmpty();

    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    bool IsIn(int index) => index >= 0 && index < Count;
}
