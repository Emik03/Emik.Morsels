// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;
#if !NETFRAMEWORK
/// <summary>
/// Provides implementations to turn nested <see cref="Two{T}"/> instances into a continuous <see cref="Span{T}"/>.
/// </summary>
static partial class Two
{
    /// <inheritdoc cref="AsSpan{T, TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<T> two)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            MemoryMarshal.CreateSpan(
                ref Unsafe.As<Two<T>, T>(ref Unsafe.AsRef(two)),
                SmallList<T, Two<T>>.InlinedLength
            );

    /// <inheritdoc cref="AsSpan{T, TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<Two<T>> two)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            MemoryMarshal.CreateSpan(
                ref Unsafe.As<Two<Two<T>>, T>(ref Unsafe.AsRef(two)),
                SmallList<T, Two<Two<T>>>.InlinedLength
            );

    /// <inheritdoc cref="AsSpan{T, TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<Two<Two<T>>> two)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            MemoryMarshal.CreateSpan(
                ref Unsafe.As<Two<Two<Two<T>>>, T>(ref Unsafe.AsRef(two)),
                SmallList<T, Two<Two<Two<T>>>>.InlinedLength
            );

    /// <inheritdoc cref="AsSpan{T, TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<Two<Two<Two<T>>>> two)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            MemoryMarshal.CreateSpan(
                ref Unsafe.As<Two<Two<Two<Two<T>>>>, T>(ref Unsafe.AsRef(two)),
                SmallList<T, Two<Two<Two<Two<T>>>>>.InlinedLength
            );

    /// <inheritdoc cref="AsSpan{T, TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<Two<Two<Two<Two<T>>>>> two)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            MemoryMarshal.CreateSpan(
                ref Unsafe.As<Two<Two<Two<Two<Two<T>>>>>, T>(ref Unsafe.AsRef(two)),
                SmallList<T, Two<Two<Two<Two<Two<T>>>>>>.InlinedLength
            );

    /// <inheritdoc cref="AsSpan{T, TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<Two<Two<Two<Two<Two<T>>>>>> two) =>
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        MemoryMarshal.CreateSpan(
            ref Unsafe.As<Two<Two<Two<Two<Two<Two<T>>>>>>, T>(ref Unsafe.AsRef(two)),
            SmallList<T, Two<Two<Two<Two<Two<Two<T>>>>>>>.InlinedLength
        );

    /// <inheritdoc cref="AsSpan{T, TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<Two<Two<Two<Two<Two<Two<T>>>>>>> two)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            MemoryMarshal.CreateSpan(
                ref Unsafe.As<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>, T>(ref Unsafe.AsRef(two)),
                SmallList<T, Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>.InlinedLength
            );

    /// <inheritdoc cref="AsSpan{T, TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>> two)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            MemoryMarshal.CreateSpan(
                ref Unsafe.As<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>, T>(ref Unsafe.AsRef(two)),
                SmallList<T, Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>.InlinedLength
            );

    /// <inheritdoc cref="AsSpan{T, TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>> two)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            MemoryMarshal.CreateSpan(
                ref Unsafe.As<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>, T>(ref Unsafe.AsRef(two)),
                SmallList<T, Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>.InlinedLength
            );

    /// <inheritdoc cref="AsSpan{T, TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>> two)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            MemoryMarshal.CreateSpan(
                ref Unsafe.As<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>, T>(ref Unsafe.AsRef(two)),
                SmallList<T, Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>.InlinedLength
            );

    /// <inheritdoc cref="AsSpan{T, TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>> two)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            MemoryMarshal.CreateSpan(
                ref Unsafe.As<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>, T>(ref Unsafe.AsRef(two)),
                SmallList<T, Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>>.InlinedLength
            );

    /// <inheritdoc cref="AsSpan{T, TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>> two)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            MemoryMarshal.CreateSpan(
                ref Unsafe.As<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>>, T>(ref Unsafe.AsRef(two)),
                SmallList<T, Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>>>.InlinedLength
            );

    /// <inheritdoc cref="AsSpan{T, TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>>> two)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            MemoryMarshal.CreateSpan(
                ref Unsafe.As<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>>>, T>(
                    ref Unsafe.AsRef(two)
                ),
                SmallList<T, Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>>>>.InlinedLength
            );

    /// <inheritdoc cref="AsSpan{T, TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this in Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>>>> two)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            MemoryMarshal.CreateSpan(
                ref Unsafe.As<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>>>>, T>(
                    ref Unsafe.AsRef(two)
                ),
                SmallList<T, Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>>>>>.InlinedLength
            );

    /// <inheritdoc cref="AsSpan{T, TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(
        this in Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>>>>> two
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            MemoryMarshal.CreateSpan(
                ref Unsafe.As<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>>>>>, T>(
                    ref Unsafe.AsRef(two)
                ),
                SmallList<T, Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>>>>>>.InlinedLength
            );

    /// <inheritdoc cref="AsSpan{T, TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(
        this in Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>>>>>> two
    )
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            MemoryMarshal.CreateSpan(
                ref Unsafe.As<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>>>>>>, T>(
                    ref Unsafe.AsRef(two)
                ),
                SmallList<T, Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>>>>>>.InlinedLength
            );

    /// <summary>Turns the reference into a continuous <see cref="Span{T}"/>.</summary>
    /// <typeparam name="T">The type of the collection.</typeparam>
    /// <typeparam name="TRef">The type of reference containing a continuous region of <typeparamref name="T"/>.</typeparam>
    /// <param name="reference">The instance to turn into the <see cref="Span{T}"/>.</param>
    /// <returns>
    /// The <see cref="Span{T}"/> going over the continuous memory of the parameter <paramref name="reference"/>.
    /// </returns>
    public static Span<T> AsSpan<T, TRef>(ref TRef reference)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
        =>
            typeof(TRef) switch
            {
                _ when typeof(TRef) == typeof(T) =>
                    new(ref Unsafe.As<TRef, T>(ref reference)),
                _ when typeof(TRef) == typeof(Two<T>) =>
                    Unsafe.As<TRef, Two<T>>(ref reference).AsSpan(),
                _ when typeof(TRef) == typeof(Two<Two<T>>) =>
                    Unsafe.As<TRef, Two<Two<T>>>(ref reference).AsSpan(),
                _ when typeof(TRef) == typeof(Two<Two<Two<T>>>) =>
                    Unsafe.As<TRef, Two<Two<Two<T>>>>(ref reference).AsSpan(),
                _ when typeof(TRef) == typeof(Two<Two<Two<Two<T>>>>) =>
                    Unsafe.As<TRef, Two<Two<Two<Two<T>>>>>(ref reference).AsSpan(),
                _ when typeof(TRef) == typeof(Two<Two<Two<Two<Two<T>>>>>) =>
                    Unsafe.As<TRef, Two<Two<Two<Two<Two<T>>>>>>(ref reference).AsSpan(),
                _ when typeof(TRef) == typeof(Two<Two<Two<Two<Two<Two<T>>>>>>) =>
                    Unsafe.As<TRef, Two<Two<Two<Two<Two<Two<T>>>>>>>(ref reference).AsSpan(),
                _ when typeof(TRef) == typeof(Two<Two<Two<Two<Two<Two<Two<T>>>>>>>) =>
                    Unsafe.As<TRef, Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>(ref reference).AsSpan(),
                _ when typeof(TRef) == typeof(Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>) =>
                    Unsafe.As<TRef, Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>(ref reference).AsSpan(),
                _ when typeof(TRef) == typeof(Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>) =>
                    Unsafe.As<TRef, Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>(ref reference).AsSpan(),
                _ when typeof(TRef) == typeof(Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>) =>
                    Unsafe.As<TRef, Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>(ref reference).AsSpan(),
                _ when typeof(TRef) == typeof(Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>) =>
                    Unsafe.As<TRef, Two<Two<Two<Two<Two<Two<Two<Two<Two<Two<T>>>>>>>>>>>(ref reference).AsSpan(),
                _ => MemoryMarshal.CreateSpan(
                    ref Unsafe.As<TRef, T>(ref Unsafe.AsRef(reference)),
                    SmallList<T, TRef>.InlinedLength
                ),
            };
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
/// <param name="first">The first item.</param>
/// <param name="second">The second item.</param>
[StructLayout(LayoutKind.Sequential)]
public readonly struct Two<T>(T first, T second) :
#if NET471_OR_GREATER || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER
    ITuple,
#endif
    IComparable<Two<T>>,
    IEquatable<Two<T>>
{
    /// <summary>The stored items.</summary>
    public readonly T First = first, Second = second;

    /// <summary>Applies the indexer and returns the instance according to the value.</summary>
    /// <param name="back">Whether or not to return <see cref="Second"/>.</param>
    [Pure]
    public T this[bool back] => back ? Second : First;
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
    public static implicit operator Two<T>((T First, T Second) tuple) => (tuple.First, tuple.Second);

    /// <inheritdoc />
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
        unchecked
        {
            var hashCode = 0;

            if (First is not null)
                hashCode = EqualityComparer<T>.Default.GetHashCode(First);

            if (Second is not null)
                hashCode ^= EqualityComparer<T>.Default.GetHashCode(Second);

            return hashCode;
        }
    }
}
