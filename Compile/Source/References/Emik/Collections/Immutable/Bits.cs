// SPDX-License-Identifier: MPL-2.0
#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
// ReSharper disable BadPreprocessorIndent CheckNamespace StructCanBeMadeReadOnly RedundantReadonlyModifier
#pragma warning disable 8500, IDE0251, MA0102
namespace Emik.Morsels;

using static CollectionAccessType;
using static Span;

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

    /// <summary>Computes the Bitwise-AND of the <see cref="IEnumerable{T}"/>.</summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="source">The item.</param>
    /// <returns>The value <typeparamref name="T"/> containing the Bitwise-OR of <paramref name="source"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T BitwiseAnd<T>(this IEnumerable<T> source)
        where T : unmanaged
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
        where T : unmanaged
    {
        T t = default;

        foreach (var next in source)
            Bits<T>.AndNot(next, ref t);

        return t;
    }

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

    /// <summary>Computes the Bitwise-XOR of the <see cref="IEnumerable{T}"/>.</summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="source">The item.</param>
    /// <returns>The value <typeparamref name="T"/> containing the Bitwise-OR of <paramref name="source"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T BitwiseXor<T>(this IEnumerable<T> source)
        where T : unmanaged
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
[StructLayout(LayoutKind.Auto)]
#if CSHARPREPL
public
#endif
#if !NO_READONLY_STRUCTS
readonly
#endif
    partial struct Bits<T>([ProvidesContext] T bits) : IReadOnlyList<T>, IReadOnlySet<T>, ISet<T>, IList<T>
    where T : unmanaged
{
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
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => bits;
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

        for (var i = 0; that.MoveNext(); i++)
            if (that.Mask == mask && that.Index == index)
                return i;
            else if (that.Mask > mask || that.Index > index)
                return -1;

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
    public readonly Enumerator GetEnumerator() => bits;

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
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public TResult Coerce<TResult>()
        where TResult : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static TResult Copy(T value)
        {
            TResult ret = default;
            Unsafe.As<TResult, T>(ref ret) = value;
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static TResult Read(T value) => Unsafe.As<T, TResult>(ref value);

        return Unsafe.SizeOf<T>() >= Unsafe.SizeOf<TResult>() ? Read(bits) : Copy(bits);
    }

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
    public TResult CoerceLeft<TResult>()
        where TResult : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static TResult Copy(T value)
        {
            TResult ret = default;
            Unsafe.Subtract(ref Unsafe.As<TResult, T>(ref Unsafe.Add(ref ret, 1)), 1) = value;
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static TResult Read(T value) => Unsafe.Subtract(ref Unsafe.As<T, TResult>(ref Unsafe.Add(ref value, 1)), 1);

        return Unsafe.SizeOf<T>() == Unsafe.SizeOf<TResult>() ? Coerce<TResult>() :
            Unsafe.SizeOf<T>() > Unsafe.SizeOf<TResult>() ? Read(bits) : Copy(bits);
    }

    /// <summary>An enumerator over <see cref="Bits{T}"/>.</summary>
    /// <param name="value">The item to use.</param>
    [StructLayout(LayoutKind.Auto)]
    public partial struct Enumerator(T value) : IEnumerator<T>
    {
        const int Start = -1;

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
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => value;
        }

        /// <summary>Gets the underlying value that is being enumerated.</summary>
        [CollectionAccess(Read)]
        public readonly T AsValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => value;
        }

        /// <inheritdoc />
        [CollectionAccess(None)]
        public readonly T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
            get
            {
                T t = default;
                Unsafe.Add(ref Unsafe.As<T, nuint>(ref t), Index) = Mask;
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
        public static explicit operator T(Enumerator value) => value.Current;

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
        public bool MoveNext()
        {
            Mask <<= 1;

            if (Mask is 0)
            {
                Index++;
                Mask++;
            }

            if (Unsafe.SizeOf<T>() / Unsafe.SizeOf<nint>() is not 0 && FindNativelySized() ||
                Unsafe.SizeOf<T>() % Unsafe.SizeOf<nint>() is not 0 && FindRest())
                return true;

            Index = Unsafe.SizeOf<T>() / Unsafe.SizeOf<nint>();
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
        public string ToRemainingString()
        {
            Span<char> span = stackalloc char[Unsafe.SizeOf<T>() * BitsInByte];
            ref var last = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), Unsafe.SizeOf<T>() * BitsInByte - 1);
            span.Fill('0');

            while (MoveNext())
                Unsafe.Add(ref last, (int)(Index * (Unsafe.SizeOf<nint>() * BitsInByte) - TrailingZeroCount(Mask))) ^=
                    '\x01';

            return span.ToString();
        }

        [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static nuint FalsyMask() => (nuint)1 << Unsafe.SizeOf<nint>() * BitsInByte - 2;

        [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        static nuint LastRest() => ((nuint)1 << Unsafe.SizeOf<T>() % Unsafe.SizeOf<nint>() * BitsInByte) - 1;

        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        bool FindNativelySized()
        {
            // This check is normally unreachable, however it protects against out-of-bounds
            // reads if this enumerator instance was created through unsafe means.
            if (Index < 0)
                return false;

            for (; Index < Unsafe.SizeOf<T>() / Unsafe.SizeOf<nint>(); Index++, Mask = 1)
                for (; Mask is not 0; Mask <<= 1)
                    if (IsNonZero())
                        return true;

            return false;
        }

        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        bool FindRest()
        {
            // This check is normally unreachable, however it protects against out-of-bounds
            // reads if this enumerator instance was created through unsafe means.
            if (Index != Unsafe.SizeOf<T>() / Unsafe.SizeOf<nint>())
                return false;

            for (; (Mask & LastRest()) is not 0; Mask <<= 1)
                if (IsNonZero())
                    return true;

            return false;
        }

        [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        bool IsNonZero() => (Unsafe.Add(ref Unsafe.As<T, nuint>(ref AsRef(value)), Index) & Mask) is not 0;
    }
}
#endif
