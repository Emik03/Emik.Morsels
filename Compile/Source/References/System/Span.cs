// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace System;
#if NETFRAMEWORK || NETSTANDARD && !NETSTANDARD2_1_OR_GREATER
#pragma warning disable 0809
/// <summary>Provides a type-safe and memory-safe representation of a contiguous region of arbitrary memory.</summary>
/// <remarks><para>This type delegates the responsibility of pinning the pointer to the consumer.</para></remarks>
/// <typeparam name="T">The type of items in the <see cref="Span{T}"/>.</typeparam>
[DebuggerTypeProxy(typeof(Span<>.SpanDebugView)), DebuggerDisplay("{ToString(),raw}"),
 StructLayout(LayoutKind.Auto)]
readonly unsafe ref struct Span<T>
    where T : unmanaged
{
    /// <summary>Gets the pointer representing the first element in the buffer.</summary>
    readonly T* _pointer;

    /// <summary>
    /// Initializes a new instance of the <see cref="Span{T}"/> struct from a specified number of
    /// <typeparamref name="T"/> elements starting at a specified memory address.
    /// </summary>
    /// <param name="pointer">A pointer to the starting address of a specified number of T elements in memory.</param>
    /// <param name="length">The length of the buffer.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is negative.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span(void* pointer, [NonNegativeValue] int length)
    {
        ValidateLength(length);

        _pointer = (T*)pointer;
        Length = length;
    }

    /// <summary>Gets the element at the specified zero-based index.</summary>
    /// <param name="index">The zero-based index of the element.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="index"/> is less than zero or is greater than or equal to <see cref="Length"/>.
    /// </exception>
    public T this[[NonNegativeValue] int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get
        {
            ValidateIndex(index);
            return _pointer[index];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            ValidateIndex(index);
            _pointer[index] = value;
        }
    }

    /// <summary>Gets an empty <see cref="Span{T}"/> object.</summary>
#pragma warning disable CA1000
    public static Span<T> Empty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => default;
    }
#pragma warning restore CA1000

    /// <summary>Gets a value indicating whether the current <see cref="Span{T}"/> is empty.</summary>
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => Length is 0;
    }

    /// <summary>Gets the length of the current span.</summary>
    public int Length { [MethodImpl(MethodImplOptions.AggressiveInlining), NonNegativeValue, Pure] get; }

    /// <summary>Returns a value that indicates whether two <see cref="Span{T}"/> objects are equal.</summary>
    /// <remarks><para>
    /// Two <see cref="Span{T}"/> objects are equal if they have the same length and the corresponding elements of
    /// <paramref name="left"/> and <paramref name="right"/> point to the same memory. Note that the test for equality
    /// does <i>not</i> attempt to determine whether the contents are equal.
    /// </para></remarks>
    /// <param name="left">The first span to compare.</param>
    /// <param name="right">The second span to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the two <see cref="Span{T}"/> objects are equal; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator ==(Span<T> left, Span<T> right) =>
        left.Length == right.Length && left._pointer == right._pointer;

    /// <summary>Returns a value that indicates whether two <see cref="Span{T}"/> objects are not equal.</summary>
    /// <remarks><para>
    /// Two <see cref="Span{T}"/> objects are equal if they have the same length and the corresponding elements of
    /// <paramref name="left"/> and <paramref name="right"/> point to the same memory.
    /// </para></remarks>
    /// <param name="left">The first span to compare.</param>
    /// <param name="right">The second span to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the two <see cref="Span{T}"/> objects are not equal;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator !=(Span<T> left, Span<T> right) => !(left == right);

    /// <summary>Clears the contents of this <see cref="Span{T}"/> object.</summary>
    /// <remarks><para>
    /// The <see cref="Clear"/> method sets the items in the <see cref="Span{T}"/> object to their default values.
    /// It does not remove items from the <see cref="Span{T}"/>.
    /// </para></remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => Fill(default);

    /// <summary>Copies the contents of this <see cref="Span{T}"/> into a destination <see cref="Span{T}"/>.</summary>
    /// <param name="destination">The destination <see cref="Span{T}"/> object.</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="destination"/> is shorter than the source <see cref="Span{T}"/>.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(Span<T> destination)
    {
        ValidateDestination(destination.Length);

        for (var i = 0; i < Length; i++)
            destination[i] = this[i];
    }

    /// <summary>Copies the contents of this <see cref="Span{T}"/> into a destination <see cref="IList{T}"/>.</summary>
    /// <param name="destination">The destination <see cref="IList{T}"/> object.</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="destination"/> is shorter than the source <see cref="Span{T}"/>.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(IList<T> destination)
    {
        ValidateDestination(destination.Count);

        for (var i = 0; i < Length; i++)
            destination[i] = this[i];
    }

    /// <summary>Fills the elements of this span with a specified value.</summary>
    /// <param name="value">The value to assign to each element of the span.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fill(T value)
    {
        for (var i = 0; i < Length; i++)
            _pointer[i] = value;
    }

    /// <inheritdoc />
    [ContractAnnotation("=> halt"),
     DoesNotReturn,
     MethodImpl(MethodImplOptions.AggressiveInlining),
     Obsolete("Equals() on Span will always throw an exception. Use the equality operator instead.")]
    public override bool Equals(object? obj) => throw new NotSupportedException();

    /// <summary>
    /// Attempts to copy the current <see cref="Span{T}"/> to a destination <see cref="Span{T}"/>
    /// and returns a value that indicates whether the copy operation succeeded.
    /// </summary>
    /// <remarks><para>
    /// This method copies all of <c>source</c> to <paramref name="destination"/> even if
    /// <c>source</c> and <paramref name="destination"/> overlap.
    /// If <paramref name="destination"/> is shorter than the source <see cref="Span{T}"/>, this method returns
    /// <see langword="false"/>, and no data is written to <paramref name="destination"/>.
    /// </para></remarks>
    /// <param name="destination">The target of the copy operation.</param>
    /// <returns><see langword="true"/> if the copy operation succeeded; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCopyTo(Span<T> destination)
    {
        if ((uint)Length > (uint)destination.Length)
            return false;

        for (var i = 0; i < Length; i++)
            destination[i] = this[i];

        return true;
    }

    /// <inheritdoc cref="TryCopyTo(Span{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCopyTo(IList<T> destination)
    {
        if ((uint)Length > (uint)destination.Count)
            return false;

        for (var i = 0; i < Length; i++)
            destination[i] = this[i];

        return true;
    }

    /// <inheritdoc />
    [ContractAnnotation("=> halt"),
     DoesNotReturn,
     MethodImpl(MethodImplOptions.AggressiveInlining),
     Obsolete("Equals() on Span will always throw an exception. Use the equality operator instead.")]
    public override int GetHashCode() => throw new NotSupportedException();

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override string ToString() =>
        typeof(T) == typeof(char)
            ? ToCharArray().Conjoin()
            : $"System.Span<{typeof(T).Name}>[{Length}]";

    /// <summary>Returns an enumerator of this <see cref="Span{T}"/>.</summary>
    /// <returns>An enumerator for this span.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public Enumerator GetEnumerator() => new(this);

    /// <summary>Forms a slice out of the current span that begins at a specified index.</summary>
    /// <param name="start">The index at which to begin the slice.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="start"/> is less than zero or greater than <see cref="Length"/>.
    /// </exception>
    /// <returns>
    /// A span that consists of all elements of the current span from <paramref name="start"/> to the end of the span.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public Span<T> Slice([NonNegativeValue] int start) =>
        (uint)start > (uint)Length
            ? throw new ArgumentOutOfRangeException(nameof(start))
            : new(_pointer + start, Length - start);

    /// <summary>Creates the slice of this buffer.</summary>
    /// <param name="start">The start of the slice from this buffer.</param>
    /// <param name="length">The length of the slice from this buffer.</param>
    /// <exception cref="ArgumentOutOfRangeException">An out-of-range buffer is created.</exception>
    /// <returns>The <see cref="Span{T}"/> which is a slice of this buffer.</returns>
#pragma warning disable CA2208, MA0015
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public Span<T> Slice([NonNegativeValue] int start, [NonNegativeValue] int length) =>
        (ulong)(uint)start + (uint)length > (uint)Length
            ? throw new ArgumentOutOfRangeException()
            : new(_pointer + start, length);
#pragma warning restore CA2208, MA0015

    /// <summary>Copies the contents of this span into a new array.</summary>
    /// <remarks><para>
    /// This method performs a heap allocation and therefore should be avoided if possible.
    /// Heap allocations are expected in APIs that work with arrays.
    /// Using such APIs is unavoidable if an alternative API overhead that takes a <see cref="Span{T}"/> does not exist.
    /// </para></remarks>
    /// <returns>An array containing the data in the current span.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public T[] ToArray()
    {
        if (IsEmpty)
#if NETFRAMEWORK && NET46_OR_GREATER || NETSTANDARD && NETSTANDARD1_3_OR_GREATER || NETCOREAPP
            return Array.Empty<T>();
#else
            return new T[0];
#endif

        var destination = new T[Length];
        CopyTo(destination);
        return destination;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void ValidateLength(int length)
    {
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length), length, "Non-negative");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ValidateDestination(int destination)
    {
        if ((uint)Length > (uint)destination)
            throw new ArgumentException(
                $"Destination length \"{destination}\" shorter than source \"{Length}\".",
                nameof(destination)
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ValidateIndex(int index)
    {
        if ((uint)index >= (uint)Length)
            throw new ArgumentOutOfRangeException(nameof(index), index, $"must be non-zero and below length {Length}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    char[] ToCharArray() => new Span<char>((char*)_pointer, Length).ToArray();

    /// <summary>Enumerates the elements of a <see cref="Span{T}"/>.</summary>
    [StructLayout(LayoutKind.Auto)]
#pragma warning disable CA1034
    public ref struct Enumerator
#pragma warning restore CA1034
    {
        readonly Span<T> _span;

        [ValueRange(-1, int.MaxValue)]
        int _index;

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
        /// <param name="span">The buffer to peek through.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(Span<T> span)
        {
            _span = span;
            _index = -1;
        }

        /// <inheritdoc cref="IEnumerator{T}.Current" />
        public readonly T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _span[_index];
        }

        /// <inheritdoc cref="IEnumerator.Reset" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => _index = -1;

        /// <inheritdoc cref="IEnumerator.MoveNext" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            var index = _index + 1;

            if (index >= _span.Length)
                return false;

            _index = index;
            return true;
        }
    }

    sealed class SpanDebugView
    {
        public SpanDebugView(Span<T> span) => Items = span.ToArray();

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden), Pure, UsedImplicitly]
        public T[] Items { get; }
    }
}
#endif
