// SPDX-License-Identifier: MPL-2.0
#if !NET20 && !NET30
#pragma warning disable CA1710, CA1815

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

using static CollectionAccessType;

/// <summary>A factory for creating iterator types that yields an item once.</summary>
/// <typeparam name="T">The type of the item to yield.</typeparam>
[StructLayout(LayoutKind.Auto)]
readonly partial struct Once<T> : IList<T>, IReadOnlyList<T>, IReadOnlySet<T>, ISet<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Once{T}"/> struct. Prepares enumeration of a single item forever.
    /// </summary>
    /// <param name="value">The item to use.</param>
    public Once([ProvidesContext] T value) => Current = value;

    /// <inheritdoc cref="ICollection{T}.IsReadOnly"/>
    [CollectionAccess(None), Pure]
    bool ICollection<T>.IsReadOnly => true;

    /// <inheritdoc cref="IReadOnlyCollection{T}.Count"/>
    [CollectionAccess(None), Pure]
    int IReadOnlyCollection<T>.Count => 1;

    /// <inheritdoc cref="ICollection{T}.Count"/>
    [CollectionAccess(None), Pure]
    int ICollection<T>.Count => 1;

    /// <summary>Gets the item to use.</summary>
    [CollectionAccess(Read), ProvidesContext, Pure]
    public T Current { get; }

    /// <inheritdoc cref="IList{T}.this"/>
    [Pure]
    T IList<T>.this[int _]
    {
        [CollectionAccess(Read)] get => Current;
        [CollectionAccess(None)] set { }
    }

    /// <inheritdoc cref="IReadOnlyList{T}.this[int]"/>
    [CollectionAccess(Read), Pure]
    T IReadOnlyList<T>.this[int _] => Current;

    /// <summary>Implicitly calls the constructor.</summary>
    /// <param name="value">The value to pass into the constructor.</param>
    /// <returns>A new instance of <see cref="Yes{T}"/> with <paramref name="value"/> passed in.</returns>
    [CollectionAccess(None), Pure]
    public static implicit operator Once<T>([ProvidesContext] T value) => new(value);

    /// <summary>Implicitly calls <see cref="Current"/>.</summary>
    /// <param name="value">The value to call <see cref="Current"/>.</param>
    /// <returns>The value that was passed in to this instance.</returns>
    [CollectionAccess(Read), Pure]
    public static implicit operator T(Once<T> value) => value.Current;

    /// <inheritdoc />
    [CollectionAccess(Read)]
    public void CopyTo(T[] array, int arrayIndex) => array[arrayIndex] = Current;

    /// <inheritdoc />
    [CollectionAccess(None)]
    void ICollection<T>.Add(T item) { }

    /// <inheritdoc />
    [CollectionAccess(None)]
    void ICollection<T>.Clear() { }

    /// <inheritdoc />
    [CollectionAccess(None)]
    void IList<T>.Insert(int index, T item) { }

    /// <inheritdoc />
    [CollectionAccess(None)]
    void IList<T>.RemoveAt(int index) { }

    /// <inheritdoc />
    [CollectionAccess(None)]
    void ISet<T>.ExceptWith(IEnumerable<T> other) { }

    /// <inheritdoc />
    [CollectionAccess(None)]
    void ISet<T>.IntersectWith(IEnumerable<T> other) { }

    /// <inheritdoc />
    [CollectionAccess(None)]
    void ISet<T>.SymmetricExceptWith(IEnumerable<T> other) { }

    /// <inheritdoc />
    [CollectionAccess(None)]
    void ISet<T>.UnionWith(IEnumerable<T> other) { }

    /// <inheritdoc cref="ICollection{T}.Contains"/>
    [CollectionAccess(Read), Pure]
    public bool Contains(T item) => EqualityComparer<T>.Default.Equals(Current, item);

    /// <inheritdoc cref="ISet{T}.IsProperSubsetOf" />
    [CollectionAccess(Read), Pure]
    public bool IsProperSubsetOf([InstantHandle] IEnumerable<T> other) =>
        other.ToCollectionLazily() is { Count: > 1 } c && Overlaps(c);

    /// <inheritdoc cref="ISet{T}.IsProperSupersetOf" />
    [CollectionAccess(Read), Pure]
    public bool IsProperSupersetOf([InstantHandle] IEnumerable<T> other) => !other.Any();

    /// <inheritdoc cref="ISet{T}.IsSubsetOf" />
    [CollectionAccess(Read), Pure]
    public bool IsSubsetOf([InstantHandle] IEnumerable<T> other) => Overlaps(other);

    /// <inheritdoc cref="ISet{T}.IsSupersetOf" />
    [CollectionAccess(Read), Pure]
    public bool IsSupersetOf([InstantHandle] IEnumerable<T> other) =>
        other.ToCollectionLazily() is { Count: <= 1 } c && Overlaps(c);

    /// <inheritdoc cref="ISet{T}.Overlaps" />
    [CollectionAccess(Read), Pure]
    public bool Overlaps([InstantHandle] IEnumerable<T> other) => other.Contains(Current);

    /// <inheritdoc cref="ISet{T}.SetEquals" />
    [CollectionAccess(Read), Pure]
    public bool SetEquals([InstantHandle] IEnumerable<T> other) => other.SequenceEqual(this);

    /// <inheritdoc />
    [CollectionAccess(None), Pure]
    bool ICollection<T>.Remove(T item) => false;

    /// <inheritdoc />
    [CollectionAccess(None), Pure]
    bool ISet<T>.Add(T item) => false;

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    public int IndexOf(T item) => Contains(item) ? 0 : -1;

    /// <summary>
    /// Returns itself. Used to tell the compiler that it can be used in a <see langword="foreach"/> loop.
    /// </summary>
    /// <returns>Itself.</returns>
    [CollectionAccess(Read), Pure]
    public Enumerator GetEnumerator() => new(Current);

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>An enumerator over <see cref="Once{T}"/>.</summary>
    [StructLayout(LayoutKind.Auto)]
    public partial struct Enumerator : IEnumerator<T>
    {
        bool _hasMoved;

        /// <summary>
        /// Initializes a new instance of the <see cref="Enumerator"/> struct.
        /// Prepares enumeration of a single item forever.
        /// </summary>
        /// <param name="value">The item to use.</param>
        public Enumerator(T value) => Current = value;

        /// <inheritdoc />
        [CollectionAccess(Read), Pure]
        public T Current { get; }

        /// <inheritdoc />
        [CollectionAccess(Read), Pure]
        readonly object? IEnumerator.Current => Current;

        /// <summary>Implicitly calls the constructor.</summary>
        /// <param name="value">The value to pass into the constructor.</param>
        /// <returns>A new instance of <see cref="Yes{T}"/> with <paramref name="value"/> passed in.</returns>
        [CollectionAccess(None), Pure]
        public static implicit operator Enumerator(T value) => new(value);

        /// <summary>Implicitly calls <see cref="Current"/>.</summary>
        /// <param name="value">The value to call <see cref="Current"/>.</param>
        /// <returns>The value that was passed in to this instance.</returns>
        [CollectionAccess(Read), Pure]
        public static implicit operator T(Enumerator value) => value.Current;

        /// <inheritdoc />
        [CollectionAccess(None)]
        readonly void IDisposable.Dispose() { }

        /// <inheritdoc />
        [CollectionAccess(None)]
        public bool MoveNext() => !_hasMoved && (_hasMoved = true);

        /// <inheritdoc />
        [CollectionAccess(None)]
        public void Reset() => _hasMoved = false;
    }
}

/// <summary>Extension methods that act as factories for <see cref="Once{T}"/>.</summary>
#pragma warning disable MA0048
static partial class OnceFactory
#pragma warning restore MA0048
{
    /// <summary>Creates a <see cref="Once{T}"/> from an item.</summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="source">The item.</param>
    /// <returns>The <see cref="Once{T}"/> instance that can be yielded once.</returns>
    [Pure]
    internal static Once<T> Yield<T>(this T source) => source;
}
#endif
