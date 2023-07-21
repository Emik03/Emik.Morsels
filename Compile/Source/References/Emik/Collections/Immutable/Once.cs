// SPDX-License-Identifier: MPL-2.0
#if !NET20 && !NET30
// ReSharper disable BadPreprocessorIndent CheckNamespace StructCanBeMadeReadOnly RedundantExtendsListEntry
#pragma warning disable CA1710, CA1815, IDE0250, IDE0251, MA0048, MA0102, SA1137
namespace Emik.Morsels;

using static CollectionAccessType;

/// <summary>Extension methods that act as factories for <see cref="Once{T}"/>.</summary>
static partial class OnceFactory
{
    /// <summary>Creates a <see cref="Once{T}"/> from an item.</summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="source">The item.</param>
    /// <returns>The <see cref="Once{T}"/> instance that can be yielded once.</returns>
    [Pure]
    public static Once<T> Yield<T>(this T source) => source;
}

/// <summary>A factory for creating iterator types that yields an item once.</summary>
/// <param name="value">The item to use.</param>
/// <typeparam name="T">The type of the item to yield.</typeparam>
[StructLayout(LayoutKind.Auto)]
#if !NO_READONLY_STRUCTS
readonly
#endif
partial struct Once<T>([ProvidesContext] T value) : IList<T>, IReadOnlyList<T>, IReadOnlySet<T>, ISet<T>
{
    /// <inheritdoc cref="ICollection{T}.IsReadOnly"/>
    [CollectionAccess(None), Pure]
    bool ICollection<T>.IsReadOnly => true;

    /// <inheritdoc cref="ICollection{T}.Count"/>
    [CollectionAccess(None), Pure]
    int IReadOnlyCollection<T>.Count => 1;

    /// <inheritdoc cref="ICollection{T}.Count"/>
    [CollectionAccess(None), Pure]
    int ICollection<T>.Count => 1;

    /// <summary>Gets the item to use.</summary>
    [CollectionAccess(Read), ProvidesContext, Pure]
    public T Current => value;

    /// <inheritdoc cref="IList{T}.this"/>
    [Pure]
    T IList<T>.this[int _]
    {
        [CollectionAccess(Read)] get => value;
        [CollectionAccess(None)] set { }
    }

    /// <inheritdoc cref="IList{T}.this[int]"/>
    [CollectionAccess(Read), Pure]
    T IReadOnlyList<T>.this[int _] => value;

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
    public void CopyTo(T[] array, int arrayIndex) => array[arrayIndex] = value;

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
    [CollectionAccess(None)]
    void ISet<T>.ExceptWith(IEnumerable<T>? other) { }

    /// <inheritdoc />
    [CollectionAccess(None)]
    void ISet<T>.IntersectWith(IEnumerable<T>? other) { }

    /// <inheritdoc />
    [CollectionAccess(None)]
    void ISet<T>.SymmetricExceptWith(IEnumerable<T>? other) { }

    /// <inheritdoc />
    [CollectionAccess(None)]
    void ISet<T>.UnionWith(IEnumerable<T>? other) { }

    /// <inheritdoc cref="ICollection{T}.Contains"/>
    [CollectionAccess(Read), Pure]
    public bool Contains(T item) => EqualityComparer<T>.Default.Equals(value, item);

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
    public bool Overlaps([InstantHandle] IEnumerable<T> other) => other.Contains(value);

    /// <inheritdoc cref="ISet{T}.SetEquals" />
    [CollectionAccess(Read), Pure]
    public bool SetEquals([InstantHandle] IEnumerable<T> other) => other.SequenceEqual(this);

    /// <inheritdoc />
    [CollectionAccess(None), Pure]
    bool ICollection<T>.Remove(T? item) => false;

    /// <inheritdoc />
    [CollectionAccess(None), Pure]
    bool ISet<T>.Add(T? item) => false;

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    public int IndexOf(T item) => Contains(item) ? 0 : -1;

    /// <summary>
    /// Returns itself. Used to tell the compiler that it can be used in a <see langword="foreach"/> loop.
    /// </summary>
    /// <returns>Itself.</returns>
    [CollectionAccess(Read), Pure]
    public Enumerator GetEnumerator() => new(value);

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [CollectionAccess(Read), Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>An enumerator over <see cref="Once{T}"/>.</summary>
    /// <param name="value">The item to use.</param>
    [StructLayout(LayoutKind.Auto)]
    public partial struct Enumerator(T value) : IEnumerator<T>
    {
        static readonly object s_fallback = new();

        bool _hasMoved;

        /// <inheritdoc />
        [CollectionAccess(Read), Pure]
        public readonly T Current => value;

        /// <inheritdoc />
        [CollectionAccess(Read), Pure]
        readonly object IEnumerator.Current => value ?? s_fallback;

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
#endif
