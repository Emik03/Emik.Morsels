// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace NullnessAnnotationConflictWithJetBrainsAnnotations
namespace Emik.Morsels;

using static CollectionAccessType;

#if !NET20 && !NET30
/// <summary>Extension methods that act as factories for <see cref="ClampedList{T}"/>.</summary>
#pragma warning disable MA0048
static partial class ClippedFactory
#pragma warning restore MA0048
{
    /// <summary>Wraps an <see cref="IList{T}"/> (upcasted/created) to <see cref="ClampedList{T}"/>.</summary>
    /// <typeparam name="T">The type of the <paramref name="iterable"/> and the <see langword="return"/>.</typeparam>
    /// <param name="iterable">The collection to turn into a <see cref="ClampedList{T}"/>.</param>
    /// <returns>A <see cref="ClampedList{T}"/> of <paramref name="iterable"/>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterable))]
    public static ClampedList<T>? ToClamped<T>(this IEnumerable<T>? iterable) =>
        iterable is null ? null : iterable as ClampedList<T> ?? new(iterable.ToIList());
}
#endif

/// <summary>
/// Encapsulates an <see cref="IList{T}"/> where indices are always clamped and therefore never be out of range.
/// </summary>
/// <typeparam name="T">The generic type of the encapsulated <see cref="IList{T}"/>.</typeparam>
/// <param name="list">The <see cref="IList{T}"/> to encapsulate.</param>
sealed partial class ClampedList<T>([ProvidesContext] IList<T> list) : IList<T>, IReadOnlyList<T>
{
    /// <inheritdoc cref="IList{T}.this"/>
    [Pure]
    public T this[int index]
    {
        [CollectionAccess(Read)] get => list[Clamp(index)];
        [CollectionAccess(ModifyExistingContent)] set => list[Clamp(index)] = value;
    }

    /// <inheritdoc/>
    [CollectionAccess(None), Pure]
    public bool IsReadOnly => list.IsReadOnly;

    /// <inheritdoc cref="ICollection{T}.Count"/>
    [CollectionAccess(None), Pure, ValueRange(1, int.MaxValue)]
    public int Count => list.Count;

    /// <inheritdoc/>
    [CollectionAccess(UpdatedContent)]
    public void Add(T item) => list.Add(item);

    /// <inheritdoc/>
    [CollectionAccess(ModifyExistingContent)]
    public void Clear() => list.Clear();

    /// <inheritdoc/>
    [CollectionAccess(Read)]
    public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    [CollectionAccess(UpdatedContent)]
    public void Insert(int index, T item) => list.Insert(Clamp(index), item);

    /// <inheritdoc/>
    [CollectionAccess(ModifyExistingContent)]
    public void RemoveAt(int index) => list.RemoveAt(Clamp(index));

    /// <inheritdoc cref="ICollection{T}.Contains"/>
    [CollectionAccess(Read), Pure]
    public bool Contains(T item) => list.Contains(item);

    /// <inheritdoc/>
    [CollectionAccess(Read | ModifyExistingContent), Pure]
    public bool Remove(T item) => list.Remove(item);

    /// <inheritdoc/>
    [CollectionAccess(Read), Pure]
    public int IndexOf(T item) => list.IndexOf(item);

    /// <inheritdoc/>
    [CollectionAccess(Read), Pure]
    public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

    /// <inheritdoc/>
    [CollectionAccess(Read), Pure]
    IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    public override string ToString() => list.ToString().OrEmpty();

    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), NonNegativeValue, Pure]
    int Clamp(int index) => Count is var i && i is not 0 ? index.Clamp(0, i) : throw CannotBeEmpty;
}
