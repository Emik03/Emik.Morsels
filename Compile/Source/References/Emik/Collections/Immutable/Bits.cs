// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent CheckNamespace StructCanBeMadeReadOnly RedundantExtendsListEntry
#pragma warning disable CA1710, CA1815, IDE0250, IDE0251, MA0048, MA0102, SA1137
namespace Emik.Morsels;

using static CollectionAccessType;

/// <summary>Extension methods that act as factories for <see cref="Bits{T}"/>.</summary>
static partial class BitsFactory
{
    /// <summary>Creates the <see cref="Bits{T}"/> from the item.</summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="source">The item.</param>
    /// <returns>The <see cref="Bits{T}"/> instance with the parameter <paramref name="source"/>.</returns>
    [Pure]
    public static Bits<T> AsBits<T>(this T source)
        where T : unmanaged =>
        source;
}

/// <summary>Provides the enumeration of individual bits from the given <typeparamref name="T"/>.</summary>
/// <typeparam name="T">The type of the item to yield.</typeparam>
/// <param name="value">The item to use.</param>
[StructLayout(LayoutKind.Auto)]
#if CSHARPREPL
public
#endif
#if !NO_READONLY_STRUCTS
readonly
#endif
    partial struct Bits<T>([ProvidesContext] T value) : IList<T>, IReadOnlyList<T>, IReadOnlySet<T>, ISet<T>
    where T : unmanaged
{
    [ProvidesContext]
    readonly T _value = value;

    /// <inheritdoc cref="ICollection{T}.IsReadOnly"/>
    [CollectionAccess(None), Pure]
    bool ICollection<T>.IsReadOnly
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => true;
    }

    /// <summary>Gets the item to use.</summary>
    [CollectionAccess(Read), ProvidesContext, Pure]
    public T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _value;
    }

    /// <summary>Implicitly calls the constructor.</summary>
    /// <param name="value">The value to pass into the constructor.</param>
    /// <returns>A new instance of <see cref="Once{T}"/> with <paramref name="value"/> passed in.</returns>
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator Bits<T>([ProvidesContext] Enumerator value) => value.Current;

    /// <summary>Implicitly calls the constructor.</summary>
    /// <param name="value">The value to pass into the constructor.</param>
    /// <returns>A new instance of <see cref="Once{T}"/> with <paramref name="value"/> passed in.</returns>
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator Bits<T>([ProvidesContext] T value) => new(value);

    /// <summary>Implicitly calls <see cref="Current"/>.</summary>
    /// <param name="value">The value to call <see cref="Current"/>.</param>
    /// <returns>The value that was passed in to this instance.</returns>
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator Enumerator([ProvidesContext] Bits<T> value) => value.Current;

    /// <summary>Implicitly calls <see cref="Current"/>.</summary>
    /// <param name="value">The value to call <see cref="Current"/>.</param>
    /// <returns>The value that was passed in to this instance.</returns>
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator T(Bits<T> value) => value.Current;

    /// <inheritdoc />
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(T[] array, int arrayIndex)
    {
        using var that = GetEnumerator();

        while (that.MoveNext())
            array[arrayIndex++] = that.Current;
    }

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ICollection<T>.Add(T item) { }

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ICollection<T>.Clear() { }

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IList<T>.Insert(int index, T item) { }

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IList<T>.RemoveAt(int index) { }

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ISet<T>.ExceptWith(IEnumerable<T>? other) { }

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ISet<T>.IntersectWith(IEnumerable<T>? other) { }

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ISet<T>.SymmetricExceptWith(IEnumerable<T>? other) { }

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ISet<T>.UnionWith(IEnumerable<T>? other) { }

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    bool ICollection<T>.Remove(T item) => false;

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    bool ISet<T>.Add(T item) => false;

    /// <inheritdoc />
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public int IndexOf(T item)
    {
        using var e = new Enumerator(item);

        if (!e.MoveNext())
            return -1;

        var offset = e.Offset;
        var mask = e.Mask;

        if (e.MoveNext())
            return -1;

        using var that = GetEnumerator();

        for (var i = 0; that.MoveNext(); i++)
            if (that.Offset == offset && that.Mask == mask)
                return i;

        return -1;
    }

    /// <summary>
    /// Returns itself. Used to tell the compiler that it can be used in a <see langword="foreach"/> loop.
    /// </summary>
    /// <returns>Itself.</returns>
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public Enumerator GetEnumerator() => value;

    /// <inheritdoc />
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>An enumerator over <see cref="Once{T}"/>.</summary>
    /// <param name="value">The item to use.</param>
    [StructLayout(LayoutKind.Auto)]
    public partial struct Enumerator(T value) : IEnumerator<T>
    {
        readonly T _value = value;

        /// <summary>Gets the current mask.</summary>
        [Pure]
        public byte Mask
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] private set;
        }

        /// <summary>Gets the current offset.</summary>
        [Pure]
        public int Offset
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] private set;
        }

        /// <inheritdoc />
        [CollectionAccess(Read), Pure]
        public readonly unsafe T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                T t = default;

                if (Offset < sizeof(T))
                    *((byte*)&t + Offset) = Mask;

                return t;
            }
        }

        /// <inheritdoc />
        [CollectionAccess(Read), Pure]
        readonly object IEnumerator.Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Current;
        }

        /// <summary>Implicitly calls the constructor.</summary>
        /// <param name="value">The value to pass into the constructor.</param>
        /// <returns>A new instance of <see cref="Yes{T}"/> with <paramref name="value"/> passed in.</returns>
        [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public static implicit operator Enumerator(T value) => new(value);

        /// <summary>Implicitly calls <see cref="Current"/>.</summary>
        /// <param name="value">The value to call <see cref="Current"/>.</param>
        /// <returns>The value that was passed in to this instance.</returns>
        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public static implicit operator T(Enumerator value) => value.Current;

        /// <inheritdoc />
        [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly void IDisposable.Dispose() { }

        /// <inheritdoc />
        [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool MoveNext()
        {
            if (Offset is 0 && Mask is 0)
                Mask = 1;
            else
                Mask <<= 1;

            fixed (T* val = &_value)
                for (; Offset < sizeof(T); Offset++, Mask = 1)
                    for (; Mask is not 0; Mask <<= 1)
                        if ((((byte*)val)[Offset] & Mask) is not 0)
                            return true;

            return false;
        }

        /// <inheritdoc />
        [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            Mask = 0;
            Offset = 0;
        }
    }
}
