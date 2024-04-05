// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent CheckNamespace StructCanBeMadeReadOnly RedundantExtendsListEntry RedundantReadonlyModifier
#pragma warning disable 8500, CA1710, CA1815, IDE0250, IDE0250, IDE0251, MA0048, MA0102, RCS1085, SA1137
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
#if KTANE
        where T : struct
#else
        where T : unmanaged
#endif
        =>
            source;

    /// <summary>Computes the Bitwise-AND of the <see cref="IEnumerable{T}"/>.</summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="source">The item.</param>
    /// <returns>The value <typeparamref name="T"/> containing the Bitwise-OR of <paramref name="source"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T BitwiseAnd<T>(this IEnumerable<T> source)
#if KTANE
        where T : struct
#else
        where T : unmanaged
#endif
    {
        T t = default;

        foreach (var next in source)
            Bits<T>.And(next, ref t);

        return t;
    }

    /// <summary>Computes the Bitwise-AND-NOT of the <see cref="IEnumerable{T}"/>.</summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="source">The item.</param>
    /// <returns>The value <typeparamref name="T"/> containing the Bitwise-OR of <paramref name="source"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T BitwiseAndNot<T>(this IEnumerable<T> source)
#if KTANE
        where T : struct
#else
        where T : unmanaged
#endif
    {
        T t = default;

        foreach (var next in source)
            Bits<T>.AndNot(next, ref t);

        return t;
    }
#if !(NETFRAMEWORK && !NET45_OR_GREATER || NETSTANDARD1_0)
    /// <summary>Returns the reference that contains the most bits.</summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="source">The item.</param>
    /// <returns>The value <typeparamref name="T"/> containing the most bits of <paramref name="source"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T BitwiseMax<T>(this IEnumerable<T> source)
        where T : unmanaged =>
        source.Aggregate(default(T), (acc, next) => Bits<T>.Max(acc, next));

    /// <summary>Returns the reference that contains the least bits.</summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="source">The item.</param>
    /// <returns>The value <typeparamref name="T"/> containing the least bits of <paramref name="source"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T BitwiseMin<T>(this IEnumerable<T> source)
        where T : unmanaged =>
        source.Aggregate(default(T), (acc, next) => Bits<T>.Min(acc, next));
#endif

    /// <summary>Computes the Bitwise-OR of the <see cref="IEnumerable{T}"/>.</summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="source">The item.</param>
    /// <returns>The value <typeparamref name="T"/> containing the Bitwise-OR of <paramref name="source"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T BitwiseOr<T>(this IEnumerable<T> source)
#if KTANE
        where T : struct
#else
        where T : unmanaged
#endif
    {
        T t = default;

        foreach (var next in source)
            Bits<T>.Or(next, ref t);

        return t;
    }

    /// <summary>Computes the Bitwise-XOR of the <see cref="IEnumerable{T}"/>.</summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="source">The item.</param>
    /// <returns>The value <typeparamref name="T"/> containing the Bitwise-OR of <paramref name="source"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T BitwiseXor<T>(this IEnumerable<T> source)
#if KTANE
        where T : struct
#else
        where T : unmanaged
#endif
    {
        T t = default;

        foreach (var next in source)
            Bits<T>.Xor(next, ref t);

        return t;
    }
}

/// <summary>Provides the enumeration of individual bits from the given <typeparamref name="T"/>.</summary>
/// <typeparam name="T">The type of the item to yield.</typeparam>
/// <param name="bits">The item to use.</param>
[StructLayout(LayoutKind.Auto), NoStructuralTyping]
#if CSHARPREPL
public
#endif
#if !NO_READONLY_STRUCTS
readonly
#endif
    partial struct Bits<T>([ProvidesContext] T bits) : IReadOnlyList<T>, IReadOnlySet<T>, ISet<T>, IList<T>
#if KTANE
    where T : struct
#else
    where T : unmanaged
#endif
{
    static readonly unsafe int s_nativeSize = sizeof(nuint) * BitsInByte, s_typeSize = sizeof(T) * BitsInByte;

    // ReSharper disable once ReplaceWithPrimaryConstructorParameter
    readonly T _value = bits;

    /// <inheritdoc cref="ICollection{T}.IsReadOnly"/>
    [CollectionAccess(None)]
    bool ICollection<T>.IsReadOnly
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => true;
    }

    /// <summary>Gets the item to use.</summary>
    [CollectionAccess(Read), ProvidesContext] // ReSharper disable once ConvertToAutoProperty
    public readonly T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _value;
    }

    /// <summary>Implicitly calls the constructor.</summary>
    /// <param name="value">The value to pass into the constructor.</param>
    /// <returns>A new instance of <see cref="Bits{T}"/> with <paramref name="value"/> passed in.</returns>
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator Bits<T>([ProvidesContext] Enumerator value) => value.Current;

    /// <summary>Implicitly calls the constructor.</summary>
    /// <param name="value">The value to pass into the constructor.</param>
    /// <returns>A new instance of <see cref="Bits{T}"/> with <paramref name="value"/> passed in.</returns>
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
    public readonly void CopyTo(T[] array, int arrayIndex)
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
    public readonly int IndexOf(T item)
    {
        if ((Enumerator)item is var e && !e.MoveNext() ||
            e.Mask is var mask && e.Index is var index && e.MoveNext())
            return -1;

        var that = (Enumerator)this;
#pragma warning disable S1994
        for (var i = 0; that.MoveNext(); i++)
            if (that.Mask == mask && that.Index == index)
                return i;
            else if (that.Mask > mask || that.Index > index)
                return -1;
#pragma warning restore S1994
        return -1;
    }

    /// <inheritdoc />
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly override string ToString() => ((Enumerator)this).ToRemainingString();

    /// <summary>
    /// Returns itself. Used to tell the compiler that it can be used in a <see langword="foreach"/> loop.
    /// </summary>
    /// <returns>Itself.</returns>
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), MustDisposeResource(false), Pure]
    public readonly Enumerator GetEnumerator() => _value;

    /// <inheritdoc />
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), MustDisposeResource(false), Pure]
    readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), MustDisposeResource(false), Pure]
    readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#pragma warning disable DOC100
    /// <summary>Reinterprets the bits in <see cref="Current"/> as <typeparamref name="TResult"/>.</summary>
    /// <remarks><para>
    /// If the type <typeparamref name="TResult"/> is smaller than <typeparamref name="T"/>,
    /// the result is truncated to the left. Otherwise, if the type <typeparamref name="TResult"/>
    /// is larger than <typeparamref name="T"/>, the result is zero-padded to the left.
    /// </para>
    /// <example>
    /// <para>Visual description of how the coercion works:</para>
    /// <code lang="C#"><![CDATA[
    /// var bits = ((ushort)0b0101_0110).AsBits(); // 0b0000_1111_0101_0110
    /// var padding = bits.Coerce<int>(); // 0b0000_0000_0000_0000_0000_1111_0101_0110
    /// var truncation = bits.Coerce<byte>(); // 0b0101_0110
    /// ]]></code></example></remarks>
    /// <typeparam name="TResult">The type to reinterpret the bits as.</typeparam>
    /// <returns>The result of reinterpreting <see cref="Current"/> as <typeparamref name="TResult"/>.</returns>
#pragma warning restore DOC100
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public unsafe TResult Coerce<TResult>()
#if KTANE
        where TResult : struct
#else
        where TResult : unmanaged
#endif
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static TResult Copy(T value)
        {
            TResult ret = default;
            *(T*)&ret = value;
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static TResult Read(T value) => *(TResult*)&value;

        return sizeof(T) >= sizeof(TResult) ? Read(_value) : Copy(_value);
    }
#pragma warning disable DOC100
    /// <summary>Reinterprets the bits in <see cref="Current"/> as <typeparamref name="TResult"/>.</summary>
    /// <remarks><para>
    /// If the type <typeparamref name="TResult"/> is smaller than <typeparamref name="T"/>,
    /// the result is truncated to the right. Otherwise, if the type <typeparamref name="TResult"/>
    /// is larger than <typeparamref name="T"/>, the result is zero-padded to the right.
    /// </para>
    /// <example>
    /// <para>Visual description of how the coercion works:</para>
    /// <code lang="C#"><![CDATA[
    /// var bits = ((ushort)0b0101_0110).AsBits(); // 0b0000_1111_0101_0110
    /// var padding = bits.Coerce<int>(); // 0b0000_1111_0101_0110_0000_0000_0000_0000
    /// var truncation = bits.Coerce<byte>(); // 0b0000_1111
    /// ]]></code></example></remarks>
    /// <typeparam name="TResult">The type to reinterpret the bits as.</typeparam>
    /// <returns>The result of reinterpreting <see cref="Current"/> as <typeparamref name="TResult"/>.</returns>
#pragma warning restore DOC100
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public unsafe TResult CoerceLeft<TResult>()
#if KTANE
        where TResult : struct
#else
        where TResult : unmanaged
#endif
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static TResult Copy(T value)
        {
            TResult ret = default;
            ((T*)(&ret + 1))[-1] = value;
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static TResult Read(T value) => ((TResult*)(&value + 1))[-1];

        return sizeof(T) == sizeof(TResult) ? Coerce<TResult>() :
            sizeof(T) > sizeof(TResult) ? Read(_value) : Copy(_value);
    }

    /// <summary>An enumerator over <see cref="Bits{T}"/>.</summary>
    /// <param name="value">The item to use.</param>
    [StructLayout(LayoutKind.Auto)]
    public partial struct Enumerator(T value) : IEnumerator<T>
    {
        const int Start = -1;

        readonly T _value = value;

        /// <summary>Gets the current mask.</summary>
        [CollectionAccess(None)]
        public nuint Mask
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] private set;
        }

        /// <summary>Gets the current index.</summary>
        [CLSCompliant(false), CollectionAccess(None)]
        public nint Index
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] private set;
        } = Start;

        /// <summary>Gets the reconstruction of the original enumerable that can create this instance.</summary>
        [CollectionAccess(Read)]
        public readonly Bits<T> AsBits
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _value;
        }

        /// <summary>Gets the underlying value that is being enumerated.</summary>
        [CollectionAccess(Read)]
        public readonly T AsValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _value;
        }

        /// <inheritdoc />
        [CollectionAccess(None)]
        public readonly unsafe T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
            get
            {
                T t = default;
                *((nuint*)&t + Index) ^= Mask;
                return t;
            }
        }

        /// <inheritdoc />
        [CollectionAccess(None)]
        readonly object IEnumerator.Current
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => Current;
        }

        /// <summary>Implicitly calls the constructor.</summary>
        /// <param name="value">The value to pass into the constructor.</param>
        /// <returns>A new instance of <see cref="Enumerator"/> with <paramref name="value"/> passed in.</returns>
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
        public void Reset()
        {
            Index = Start;
            Mask = 0;
        }

        /// <inheritdoc />
        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool MoveNext()
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

        /// <inheritdoc />
        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public readonly override string ToString()
        {
            var that = this;
            return that.ToRemainingString();
        }

        /// <summary>Enumerates over the remaining elements to give a <see cref="string"/> result.</summary>
        /// <returns>The <see cref="string"/> result of this instance.</returns>
        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), MustUseReturnValue]
        public unsafe string ToRemainingString()
        {
            var ptr = stackalloc char[s_typeSize];
            new Span<char>(ptr, s_typeSize).Fill('0');
            var last = ptr + s_typeSize - 1;

            while (MoveNext())
                *(last - (int)(Index * s_nativeSize) - TrailingZeroCount(Mask)) ^= '\x01';

            return new(ptr, 0, s_typeSize);
        }

        [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static nuint FalsyMask() => (nuint)1 << s_nativeSize - 2;

        [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static unsafe nuint LastRest() => ((nuint)1 << sizeof(T) % sizeof(nuint) * BitsInByte) - 1;

        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        unsafe bool FindNativelySized(T* ptr)
        {
            // This check is normally unreachable, however it protects against out-of-bounds
            // reads if this enumerator instance was created through unsafe means.
            if (Index < 0)
                return false;

            for (; Index < sizeof(T) / sizeof(nuint); Index++, Mask = 1)
                for (; Mask is not 0; Mask <<= 1)
                    if (IsNonZero(ptr))
                        return true;

            return false;
        }

        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        unsafe bool FindRest(T* ptr)
        {
            // This check is normally unreachable, however it protects against out-of-bounds
            // reads if this enumerator instance was created through unsafe means.
            if (Index != sizeof(T) / sizeof(nuint))
                return false;

            for (; (Mask & LastRest()) is not 0; Mask <<= 1)
                if (IsNonZero(ptr))
                    return true;

            return false;
        }

        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        unsafe bool IsNonZero(T* ptr) => (((nuint*)ptr)[Index] & Mask) is not 0;
    }
}
