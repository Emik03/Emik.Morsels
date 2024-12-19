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
    partial struct Bits<T>([ProvidesContext] T bits) :
#if NET7_0_OR_GREATER
    IBitwiseOperators<Bits<T>, Bits<T>, Bits<T>>,
    IEqualityOperators<Bits<T>, Bits<T>, bool>,
#endif
    IEquatable<Bits<T>>,
    IReadOnlyList<T>,
    IReadOnlySet<T>,
    ISet<T>,
    IList<T>
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

    /// <summary>Determines whether both bits of <typeparamref name="T"/> do not contain the same bits.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameters <paramref name="left"/> and <paramref name="right"/>
    /// do not have the same bits as each other; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator !=(Bits<T> left, Bits<T> right) =>
        !Eq(Unsafe.As<Bits<T>, T>(ref Unsafe.AsRef(left)), Unsafe.As<Bits<T>, T>(ref Unsafe.AsRef(right)));

    /// <summary>Determines whether both bits of <typeparamref name="T"/> contain the same bits.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameters <paramref name="left"/> and <paramref name="right"/>
    /// have the same bits as each other; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator ==(Bits<T> left, Bits<T> right) =>
        Eq(Unsafe.As<Bits<T>, T>(ref Unsafe.AsRef(left)), Unsafe.As<Bits<T>, T>(ref Unsafe.AsRef(right)));

    /// <summary>Computes the Bitwise-NOT computation.</summary>
    /// <param name="value">The set of bits.</param>
    /// <returns>The result of the computation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Bits<T> operator ~(Bits<T> value)
    {
        Not(ref Unsafe.As<Bits<T>, T>(ref value));
        return value;
    }

    /// <summary>Computes the Bitwise-AND computation.</summary>
    /// <param name="left">The first set of bits.</param>
    /// <param name="right">The second set of bits.</param>
    /// <returns>The result of the computation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Bits<T> operator &(Bits<T> left, Bits<T> right)
    {
        And(Unsafe.As<Bits<T>, T>(ref Unsafe.AsRef(left)), ref Unsafe.As<Bits<T>, T>(ref right));
        return right;
    }

    /// <summary>Computes the Bitwise-OR computation.</summary>
    /// <param name="left">The first set of bits.</param>
    /// <param name="right">The second set of bits.</param>
    /// <returns>The result of the computation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Bits<T> operator |(Bits<T> left, Bits<T> right)
    {
        Or(Unsafe.As<Bits<T>, T>(ref Unsafe.AsRef(left)), ref Unsafe.As<Bits<T>, T>(ref right));
        return right;
    }

    /// <summary>Computes the Bitwise-XOR computation.</summary>
    /// <param name="left">The first set of bits.</param>
    /// <param name="right">The second set of bits.</param>
    /// <returns>The result of the computation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Bits<T> operator ^(Bits<T> left, Bits<T> right)
    {
        Xor(Unsafe.As<Bits<T>, T>(ref Unsafe.AsRef(left)), ref Unsafe.As<Bits<T>, T>(ref right));
        return right;
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
    readonly void ICollection<T>.Add(T item) { }

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly void ICollection<T>.Clear() { }

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly void IList<T>.Insert(int index, T item) { }

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly void IList<T>.RemoveAt(int index) { }

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly void ISet<T>.ExceptWith(IEnumerable<T>? other) { }

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly void ISet<T>.IntersectWith(IEnumerable<T>? other) { }

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly void ISet<T>.SymmetricExceptWith(IEnumerable<T>? other) { }

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly void ISet<T>.UnionWith(IEnumerable<T>? other) { }

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly bool ICollection<T>.Remove(T item) => false;

    /// <inheritdoc />
    [CollectionAccess(None), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly bool ISet<T>.Add(T item) => false;

    /// <summary>Determines whether the reference of <typeparamref name="T"/> contains all zeros.</summary>
    /// <returns>
    /// The value <see langword="true"/> if this instance is all zeros; otherwise, <see langword="false"/>.
    /// </returns>
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly bool Eq0() => Eq0(Unsafe.As<Bits<T>, T>(ref Unsafe.AsRef(this)));

    /// <inheritdoc />
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly override bool Equals(object? other) => other is Bits<T> bit && this == bit;

    /// <inheritdoc />
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly bool Equals(Bits<T> other) => this == other;

    /// <inheritdoc />
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly override int GetHashCode() => Coerce<int>();

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

    /// <summary>Computes the Bitwise-AND-NOT computation.</summary>
    /// <param name="other">The other set of bits.</param>
    /// <returns>The result of the computation.</returns>
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly Bits<T> AndNot(Bits<T> other)
    {
        AndNot(Unsafe.As<Bits<T>, T>(ref Unsafe.AsRef(this)), ref Unsafe.As<Bits<T>, T>(ref other));
        return other;
    }

    /// <summary>Returns the greater bits.</summary>
    /// <returns>
    /// This instance if its bits are greater or equal to the parameter
    /// <paramref name="other"/>; otherwise, <paramref name="other"/>.
    /// </returns>
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly Bits<T> Max(Bits<T> other) =>
        Max(Unsafe.As<Bits<T>, T>(ref Unsafe.AsRef(this)), Unsafe.As<Bits<T>, T>(ref other));

    /// <summary>Returns the lesser bits.</summary>
    /// <returns>
    /// This instance if its bits are lesser or equal to the parameter
    /// <paramref name="other"/>; otherwise, <paramref name="other"/>.
    /// </returns>
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly Bits<T> Min(Bits<T> other) =>
        Min(Unsafe.As<Bits<T>, T>(ref Unsafe.AsRef(this)), Unsafe.As<Bits<T>, T>(ref other));

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
    public readonly TResult Coerce<TResult>()
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
    public readonly TResult CoerceLeft<TResult>()
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

    /// <summary>Converts the value to a hex <see cref="string"/>.</summary>
    /// <returns>The hex <see cref="string"/>.</returns>
    [CollectionAccess(Read), MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly string ToHexString()
    {
        Span<char> span = stackalloc char[Unsafe.SizeOf<T>() * 2 + 2];
        ref var first = ref MemoryMarshal.GetReference(span);
        const string Hex = "0123456789abcdef";

        first = '0';
        Unsafe.Add(ref first, 1) = 'x';

        for (int i = 0, j = Unsafe.SizeOf<T>() * 2; i < Unsafe.SizeOf<T>(); i++, j -= 2)
        {
            var b = Unsafe.Add(ref Unsafe.As<T, byte>(ref AsRef(bits)), i);
            Unsafe.Add(ref first, j) = Unsafe.Add(ref AsRef(MemoryMarshal.GetReference(Hex.AsSpan())), (b & 0xf0) >> 4);
            Unsafe.Add(ref first, j + 1) = Unsafe.Add(ref AsRef(MemoryMarshal.GetReference(Hex.AsSpan())), b & 0x0f);
        }

        return span.ToString();
    }
}
#endif
