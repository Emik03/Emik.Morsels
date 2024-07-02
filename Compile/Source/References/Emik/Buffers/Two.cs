// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent CheckNamespace RedundantNameQualifier RedundantUsingDirective StructCanBeMadeReadOnly
namespace Emik.Morsels;

using static Span;
using FieldInfo = System.Reflection.FieldInfo;

#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
/// <summary>
/// Provides implementations to turn nested <see cref="Two{T}"/> instances into a continuous <see cref="Span{T}"/>.
/// </summary>
static partial class Two
{
    /// <inheritdoc cref="PooledSmallList{T}.From{TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<T> two) =>
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        PooledSmallList<T>.AsSpan(ref AsRef(two));

    /// <inheritdoc cref="PooledSmallList{T}.From{TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<Two<T>> two) =>
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        PooledSmallList<T>.AsSpan(ref AsRef(two));

    /// <inheritdoc cref="PooledSmallList{T}.From{TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<Two<Two<T>>> two) =>
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        PooledSmallList<T>.AsSpan(ref AsRef(two));

    /// <inheritdoc cref="PooledSmallList{T}.From{TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<Two<Two<Two<T>>>> two) =>
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        PooledSmallList<T>.AsSpan(ref AsRef(two));

    /// <inheritdoc cref="PooledSmallList{T}.From{TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<Two<Two<Two<Two<T>>>>> two) =>
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        PooledSmallList<T>.AsSpan(ref AsRef(two));

    /// <inheritdoc cref="PooledSmallList{T}.From{TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<Two<Two<Two<Two<Two<T>>>>>> two) =>
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        PooledSmallList<T>.AsSpan(ref AsRef(two));

    /// <inheritdoc cref="PooledSmallList{T}.From{TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<Two<Two<Two<Two<Two<Two<T>>>>>>> two) =>
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        PooledSmallList<T>.AsSpan(ref AsRef(two));

    /// <inheritdoc cref="PooledSmallList{T}.From{TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>> two) =>
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        PooledSmallList<T>.AsSpan(ref AsRef(two));

    /// <inheritdoc cref="PooledSmallList{T}.From{TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>> two) =>
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        PooledSmallList<T>.AsSpan(ref AsRef(two));

    /// <inheritdoc cref="PooledSmallList{T}.From{TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>> two) =>
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        PooledSmallList<T>.AsSpan(ref AsRef(two));

    /// <inheritdoc cref="Two{T}.op_Implicit(ValueTuple{T, T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Two<T> AsTwo<T>(this (T First, T Second) tuple) => tuple;
}
#endif

/// <summary>
/// Represents two inlined elements, equivalent to <see cref="ValueTuple{T1, T2}"/>,
/// but the memory layout is guaranteed to be sequential, and both elements are of the same type.
/// </summary>
/// <remarks><para>
/// The name of this type may or may not derive from a specific algebralien from a show...
/// </para></remarks>
/// <typeparam name="T">The type of item to store.</typeparam>
/// <param name="left">The first item.</param>
/// <param name="right">The second item.</param>
[StructLayout(LayoutKind.Sequential)]
#pragma warning disable MA0102
#if !NO_READONLY_STRUCTS
readonly
#endif
partial struct Two<T>(T left, T right) :
#if NET7_0_OR_GREATER
    IComparisonOperators<Two<T>, Two<T>, bool>,
    IEqualityOperators<Two<T>, Two<T>, bool>,
#endif
#if NET471_OR_GREATER || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
    ITuple,
#endif
    IComparable<Two<T>>,
    IEquatable<Two<T>>
{
    /// <summary>The stored items.</summary>
    public readonly T First = left, Second = right;

    /// <summary>Applies the indexer and returns the instance according to the value.</summary>
    /// <param name="back">Whether or not to return <see cref="Second"/>.</param>
    [Pure]
    public T this[bool back] => back ? Second : First;

    /// <inheritdoc cref="Two{T}.op_Implicit(Two{T})"/>
    public (T First, T Second) Tuple => this;

#if NET471_OR_GREATER || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
    /// <inheritdoc />
    [Pure, ValueRange(2)]
    int ITuple.Length => 2;

    /// <inheritdoc />
    [Pure]
    object? ITuple.this[int index] =>
        index switch
        {
            0 => First,
            1 => Second,
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null),
        };
#endif

    /// <summary>Deconstructs this instance into the two inlined elements.</summary>
    /// <param name="first">The first item.</param>
    /// <param name="second">The second item.</param>
    public void Deconstruct(out T first, out T second)
    {
        first = First;
        second = Second;
    }

    /// <summary>Determines whether both instances contain the same two values.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether both instances have the same two values.</returns>
    [Pure]
    public static bool operator ==(Two<T> left, Two<T> right) => left.Equals(right);

    /// <summary>Determines whether both instances contain different values.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether both instances have different values.</returns>
    [Pure]
    public static bool operator !=(Two<T> left, Two<T> right) => !(left == right);

    /// <summary>Determines whether the left instance is less than the right.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether the left instance is less than the right.</returns>
    [Pure]
    public static bool operator <(Two<T> left, Two<T> right) => left.CompareTo(right) < 0;

    /// <summary>Determines whether the left instance is equal to or less than the right.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether the left instance is equal to or less than the right.</returns>
    [Pure]
    public static bool operator <=(Two<T> left, Two<T> right) => left.CompareTo(right) <= 0;

    /// <summary>Determines whether the left instance is greater than the right.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether the left instance is greater than the right.</returns>
    [Pure]
    public static bool operator >(Two<T> left, Two<T> right) => left.CompareTo(right) > 0;

    /// <summary>Determines whether the left instance is equal to or greater than the right.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>Whether the left instance is equal to or greater than the right.</returns>
    [Pure]
    public static bool operator >=(Two<T> left, Two<T> right) => left.CompareTo(right) >= 0;

    /// <summary>Implicitly converts the <see cref="Two{T}"/> into the <see cref="ValueTuple{T1, T2}"/>.</summary>
    /// <param name="two">The <see cref="Two{T}"/> to convert.</param>
    /// <returns>The equivalent tuple layout of the parameter <paramref name="two"/>.</returns>
    [Pure]
    public static implicit operator (T First, T Second)(Two<T> two) => (two.First, two.Second);

    /// <summary>Implicitly converts the <see cref="ValueTuple{T1, T2}"/> into the <see cref="Two{T}"/>.</summary>
    /// <param name="tuple">The <see cref="ValueTuple{T1, T2}"/> to convert.</param>
    /// <returns>The equivalent sequential layout of the parameter <paramref name="tuple"/>.</returns>
    [Pure]
    public static implicit operator Two<T>((T First, T Second) tuple) => new(tuple.First, tuple.Second);

    /// <inheritdoc cref="object.Equals(object)"/>
    [Pure]
    public override bool Equals(object? obj) => obj is Two<T> two && Equals(two);

    /// <inheritdoc />
    [Pure]
    public bool Equals(Two<T> other) =>
        EqualityComparer<T>.Default.Equals(First, other.First) &&
        EqualityComparer<T>.Default.Equals(Second, other.Second);

    /// <inheritdoc />
    [Pure]
    public int CompareTo(Two<T> other) =>
        Comparer<T>.Default.Compare(First, other.First) is var first and not 0 ? first :
        Comparer<T>.Default.Compare(Second, other.Second) is var second and not 0 ? second : 0;

    /// <inheritdoc />
    [Pure]
    public override int GetHashCode()
    {
        var hashCode = 2;

        if (First is not null)
            hashCode ^= EqualityComparer<T>.Default.GetHashCode(First);

        if (Second is not null)
            hashCode ^= EqualityComparer<T>.Default.GetHashCode(Second);

        return hashCode;
    }
}
