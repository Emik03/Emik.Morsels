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
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Bits<T> AsBits<T>(this T source)
        where T : unmanaged =>
        source;

    /// <summary>Computes the Bitwise-OR of the <see cref="IEnumerable{T}"/>.</summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="source">The item.</param>
    /// <returns>The value <typeparamref name="T"/> containing the Bitwise-OR of <paramref name="source"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T BitwiseOr<T>(this IEnumerable<T> source)
        where T : unmanaged
    {
        T t = default;

        foreach (var next in source)
            Bits<T>.Or(next, ref t);

        return t;
    }
}

/// <summary>Provides the enumeration of individual bits from the given <typeparamref name="T"/>.</summary>
/// <typeparam name="T">The type of the item to yield.</typeparam>
/// <param name="value">The item to use.</param>
[StructLayout(LayoutKind.Auto), NoStructuralTyping]
#if CSHARPREPL
public
#endif
#if !NO_READONLY_STRUCTS
readonly
#endif
    partial struct Bits<T>([ProvidesContext] T value) : IList<T>, IReadOnlyList<T>, IReadOnlySet<T>, ISet<T>
    where T : unmanaged
{
    const int BitsPerByte = 8;

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
        foreach (var next in this)
            array[arrayIndex++] = next;
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

        var (mask, index) = (e.Mask, e.Index);

        if (e.MoveNext())
            return -1;

        using var that = GetEnumerator();

        for (var i = 0; that.MoveNext(); i++)
            if (that.Mask == mask && that.Index == index)
                return i;
            else if (that.Mask > mask || that.Index > index)
                return -1;

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
        const int Start = -1;

        readonly T _value = value;

        /// <summary>Gets the current mask.</summary>
        [CollectionAccess(None), Pure]
        public nuint Mask
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] private set;
        }

        /// <summary>Gets the current index.</summary>
        [CLSCompliant(false), CollectionAccess(None), Pure]
        public nint Index
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] private set;
        } = Start;

        /// <inheritdoc />
        [CollectionAccess(None), Pure]
        public readonly unsafe T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                T t = default;
                *((nuint*)&t + Index) ^= Mask;
                return t;
            }
        }

        /// <inheritdoc />
        [CollectionAccess(None), Pure]
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
        public void Reset() => (Index, Mask) = (Start, 0);

        /// <inheritdoc />
        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable MA0051 // ReSharper disable once CognitiveComplexity
        public unsafe bool MoveNext()
#pragma warning restore MA0051
        {
            Mask <<= 1;

            if (Mask is 0)
            {
                Index++;
                Mask++;
            }

            fixed (T* ptr = &_value)
                if (sizeof(T) / sizeof(nuint) is not 0 && FindNativelySized(ptr) ||
                    sizeof(T) % sizeof(nuint) is not 0 && FindRest(ptr))
                    return true;

            Index = sizeof(T) / sizeof(nuint);
            Mask = FalsyMask();
            return false;
        }

        [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static unsafe nuint FalsyMask() => (nuint)1 << sizeof(nuint) * BitsPerByte - 2;

        [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static unsafe nuint LastRest() => ((nuint)1 << sizeof(T) % sizeof(nuint) * BitsPerByte) - 1;

        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        unsafe bool FindNativelySized(T* ptr)
        {
            for (; Index < sizeof(T) / sizeof(nuint); Index++, Mask = 1)
                for (; Mask is not 0; Mask <<= 1)
                    if (IsNonZero(ptr))
                        return true;

            return false;
        }

        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        unsafe bool FindRest(T* ptr)
        {
            for (; (Mask & LastRest()) is not 0; Mask <<= 1)
                if (IsNonZero(ptr))
                    return true;

            return false;
        }

        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        unsafe bool IsNonZero(T* ptr) => (((nuint*)ptr)[Index] & Mask) is not 0;
    }
}
