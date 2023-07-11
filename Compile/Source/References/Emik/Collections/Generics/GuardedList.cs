// SPDX-License-Identifier: MPL-2.0

// ReSharper disable RedundantExtendsListEntry
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

using static CollectionAccessType;

#if !NET20 && !NET30
/// <summary>Extension methods that act as factories for <see cref="GuardedList{T}"/>.</summary>
#pragma warning disable MA0048
static partial class GuardedFactory
#pragma warning restore MA0048
{
    /// <summary>Wraps an <see cref="IList{T}"/> (upcasted/created) to <see cref="GuardedList{T}"/>.</summary>
    /// <typeparam name="T">The type of the <paramref name="iterable"/> and the <see langword="return"/>.</typeparam>
    /// <param name="iterable">The collection to turn into a <see cref="GuardedList{T}"/>.</param>
    /// <returns>A <see cref="GuardedList{T}"/> of <paramref name="iterable"/>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterable))]
    public static GuardedList<T>? ToGuardedLazily<T>(this IEnumerable<T>? iterable) =>
        iterable is null ? null : iterable as GuardedList<T> ?? new(iterable.ToListLazily());
}
#endif

/// <summary>
/// Encapsulates an <see cref="IList{T}"/> where applying an index will always result in an optional value;
/// an out of range value will always give the <see langword="default"/> value.
/// </summary>
/// <param name="list">The <see cref="IList{T}"/> to encapsulate.</param>
/// <typeparam name="T">The generic type of the encapsulated <see cref="IList{T}"/>.</typeparam>
sealed partial class GuardedList<T>([ProvidesContext] IList<T> list) : IList<T?>, IReadOnlyList<T?>
{
    /// <inheritdoc/>
    [CollectionAccess(None), Pure]
    public bool IsReadOnly => list.IsReadOnly;

    /// <inheritdoc cref="ICollection{T}.Count"/>
    [CollectionAccess(None), NonNegativeValue, Pure]
    public int Count => list.Count;

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

    /// <inheritdoc/>
    [CollectionAccess(UpdatedContent)]
    public void Add(T? item)
    {
        if (item is not null)
            list.Add(item);
    }

    /// <inheritdoc/>
    [CollectionAccess(ModifyExistingContent)]
    public void Clear() => list.Clear();

    /// <inheritdoc/>
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

    /// <inheritdoc cref="ICollection{T}.Contains"/>
    [CollectionAccess(Read), Pure]
    public bool Contains(T? item) => item is not null && list.Contains(item);

    /// <inheritdoc/>
    [CollectionAccess(Read | ModifyExistingContent), Pure]
    public bool Remove(T? item) => item is not null && list.Remove(item);

    /// <inheritdoc/>
    [CollectionAccess(Read), Pure]
    public int IndexOf(T? item) => item is null ? -1 : list.IndexOf(item);

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    [CollectionAccess(Read), Pure]
#if NETFRAMEWORK && !NET40_OR_GREATER // Good job .NET 2.0 - 3.5 Nullable Analysis.
#pragma warning disable CS8619
#endif
    public IEnumerator<T?> GetEnumerator() => list.GetEnumerator();

    /// <inheritdoc/>
    [CollectionAccess(Read), Pure]
    IEnumerator<T> IEnumerable<T?>.GetEnumerator() => list.GetEnumerator();

    /// <inheritdoc/>
    [CollectionAccess(Read), Pure]
    IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

    /// <inheritdoc />
    [CollectionAccess(Read), Pure] // ReSharper disable once ReturnTypeCanBeNotNullable
    public override string? ToString() => list.ToString();

    [Pure]
    bool IsIn(int index) => index >= 0 && index < Count;
}
