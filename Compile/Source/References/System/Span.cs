// SPDX-License-Identifier: MPL-2.0

// ReSharper disable BadPreprocessorIndent CheckNamespace EmptyNamespace StructCanBeMadeReadOnly

namespace System;
#if !(NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) || NO_SYSTEM_MEMORY
#pragma warning disable 8500
using Emik.Morsels; // ReSharper disable once RedundantNameQualifier
using static Runtime.CompilerServices.RuntimeHelpers;

/// <summary>Provides a type-safe and memory-safe representation of a contiguous region of arbitrary memory.</summary>
/// <remarks><para>This type delegates the responsibility of pinning the pointer to the consumer.</para></remarks>
/// <typeparam name="T">The type of items in the <see cref="Span{T}"/>.</typeparam>
[DebuggerTypeProxy(typeof(SpanDebugView<>)), DebuggerDisplay("{ToString(),raw}"), StructLayout(LayoutKind.Sequential)]
#if !NO_READONLY_STRUCTS
readonly
#endif
#if !NO_REF_STRUCTS
    ref
#endif
    partial struct Span<T>
#if UNMANAGED_SPAN
    where T : unmanaged
#endif
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Span{T}"/> struct from a specified number of
    /// <typeparamref name="T"/> elements starting at a specified memory address.
    /// </summary>
    /// <param name="pointer">A pointer to the starting address of a specified number of T elements in memory.</param>
    /// <param name="length">The length of the buffer.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is negative.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe Span(void* pointer, [NonNegativeValue] int length)
    {
        if (IsReferenceOrContainsReferences<T>())
            throw new ArgumentException("Invalid type with pointers not supported.", nameof(pointer));

        ValidateLength(length);
        Length = length;
        ByteOffset = (nint)pointer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Span{T}"/> struct over the entirety of the specified array.
    /// </summary>
    /// <param name="array">The array from which to create the <see cref="Span{T}"/> object.</param>
    /// <exception cref="ArrayTypeMismatchException">
    /// <typeparamref name="T"/> is a reference type, and <paramref name="array"/>
    /// is not an array of type <typeparamref name="T"/>.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span(T[]? array)
    {
        if (array is null)
            return;

        if (default(T) is null && array.GetType() != typeof(T[]))
            throw new ArrayTypeMismatchException();

        Length = array.Length;
        Pinnable = Unsafe.As<Pinnable<T>>(array);
        ByteOffset = SpanHelpers.PerTypeValues<T>.ArrayAdjustment;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Span{T}"/> struct over the entirety of the specified array.
    /// </summary>
    /// <param name="array">The source array.</param>
    /// <param name="start">
    /// The zero-based index of the first element to include in the new <see cref="Span{T}"/>.
    /// </param>
    /// <param name="length">The number of elements to include in the new <see cref="Span{T}"/>.</param>
    /// <exception cref="ArrayTypeMismatchException">
    /// <typeparamref name="T"/> is a reference type, and <paramref name="array"/>
    /// is not an array of type <typeparamref name="T"/>.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe Span(T[]? array, int start, int length)
    {
        if (array is null)
        {
            if (start is 0 && length is 0)
                return;

            throw new ArgumentOutOfRangeException(nameof(start), start, "start is out of range");
        }

        if ((uint)start > (uint)array.Length || (uint)length > (uint)(array.Length - start))
            throw new ArgumentOutOfRangeException(nameof(length), length, "length is out of range");

        if (default(T) is null && array.GetType() != typeof(T[]))
            throw new ArrayTypeMismatchException();

        Length = length;
        Pinnable = Unsafe.As<Pinnable<T>>(array);
        ByteOffset = (nint)((T*)SpanHelpers.PerTypeValues<T>.ArrayAdjustment + start);
    }

    /// <summary>Initializes a new instance of the <see cref="Span{T}"/> struct.</summary>
    /// <param name="pinnable">The pinnable.</param>
    /// <param name="byteOffset">The byte offset.</param>
    /// <param name="length">The length.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Span(Pinnable<T>? pinnable, nint byteOffset, int length)
    {
        Length = length;
        Pinnable = pinnable;
        ByteOffset = byteOffset;
    }

    /// <summary>Gets the element at the specified zero-based index.</summary>
    /// <param name="index">The zero-based index of the element.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="index"/> is less than zero or is greater than or equal to <see cref="Length"/>.
    /// </exception>
    public unsafe T this[[NonNegativeValue] int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get
        {
            ValidateIndex(index);

            fixed (T* ptr = this)
                return this.Align(ptr)[index];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            ValidateIndex(index);

            fixed (T* ptr = this)
                this.Align(ptr)[index] = value;
        }
    }

    /// <summary>Gets an empty <see cref="Span{T}"/> object.</summary>
    public static Span<T> Empty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => default;
    }

    /// <summary>Gets a value indicating whether the current <see cref="Span{T}"/> is empty.</summary>
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => Length is 0;
    }

    /// <summary>Gets the length of the current span.</summary>
    public int Length { [MethodImpl(MethodImplOptions.AggressiveInlining), NonNegativeValue, Pure] get; }

    /// <summary>Gets the byte offset.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public nint ByteOffset { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; }

    /// <summary>Gets the object to be pinned.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Pinnable<T>? Pinnable { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; }

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
    public static unsafe bool operator ==(Span<T> left, Span<T> right)
    {
        if (left.Length != right.Length)
            return false;

        fixed (T* l = left)
        fixed (T* r = right)
            return left.Align(l) == right.Align(r);
    }

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

    /// <summary>
    /// Implicitly converts the parameter by creating the new instance of
    /// <see cref="Span{T}"/> by using the constructor <see cref="System.Span{T}(T[])"/>.
    /// </summary>
    /// <param name="array">The parameter to pass onto the constructor.</param>
    /// <returns>
    /// The new instance of <see cref="Enumerator"/> by passing the parameter
    /// <paramref name="array"/> to the constructor <see cref="Span{T}"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator Span<T>(T[] array) => new(array);

    /// <summary>
    /// Implicitly converts the parameter by creating the new instance of
    /// <see cref="Span{T}"/> by using the constructor <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="segment">The parameter to pass onto the constructor.</param>
    /// <returns>
    /// The new instance of <see cref="Enumerator"/> by passing the parameter
    /// <paramref name="segment"/> to the constructor <see cref="System.Span{T}(T[], int, int)"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator Span<T>(ArraySegment<T> segment) =>
        new(segment.Array, segment.Offset, segment.Count);

    /// <summary>Defines an implicit conversion of a <see cref="Span{T}"/> to a <see cref="ReadOnlySpan{T}"/>.</summary>
    /// <param name="span">The object to convert to a <see cref="ReadOnlySpan{T}"/>.</param>
    /// <returns>A read-only span that corresponds to the current instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator ReadOnlySpan<T>(Span<T> span) => new(span.Pinnable, span.ByteOffset, span.Length);

    /// <summary>Clears the contents of this <see cref="Span{T}"/> object.</summary>
    /// <remarks><para>
    /// The <see cref="Clear"/> method sets the items in the <see cref="Span{T}"/> object to their default values.
    /// It does not remove items from the <see cref="Span{T}"/>.
    /// </para></remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable 8604, CA1855
    public unsafe void Clear()
    {
        if (Length is 0)
            return;

        var byteLength = (nuint)(ulong)((uint)Length * Unsafe.SizeOf<T>());

        if ((Unsafe.SizeOf<T>() & Unsafe.SizeOf<nint>() - 1) is not 0)
        {
            if (Pinnable is null)
                SpanHelpers.ClearLessThanPointerSized((byte*)ByteOffset, byteLength);
            else
                fixed (T* ptr = &Pinnable.Data)
                    SpanHelpers.ClearLessThanPointerSized((byte*)ptr + ByteOffset, byteLength);
        }
        else
            fixed (T* ptr = this)
                if (SpanHelpers.IsReferenceOrContainsReferences<T>())
                    SpanHelpers.ClearPointerSizedWithReferences(
                        (nint*)this.Align(ptr),
                        (nuint)(ulong)(Length * Unsafe.SizeOf<T>() / Unsafe.SizeOf<nint>())
                    );
                else
                    SpanHelpers.ClearPointerSizedWithoutReferences((byte*)this.Align(ptr), byteLength);
    }
#pragma warning restore 8604, CA1855
    /// <summary>Copies the contents of this <see cref="Span{T}"/> into a destination <see cref="Span{T}"/>.</summary>
    /// <param name="destination">The destination <see cref="Span{T}"/> object.</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="destination"/> is shorter than the source <see cref="Span{T}"/>.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(Span<T> destination) => this.ReadOnly().CopyTo(destination);

    /// <summary>Fills the elements of this span with a specified value.</summary>
    /// <param name="value">The value to assign to each element of the span.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Fill(T value)
    {
        if (Length is 0)
            return;

        fixed (T* s = this)
        {
            var source = this.Align(s);
            int i;

            for (i = 0; i < (Length & -8); i += 8)
            {
                source[i] = value;
                source[i + 1] = value;
                source[i + 2] = value;
                source[i + 3] = value;
                source[i + 4] = value;
                source[i + 5] = value;
                source[i + 6] = value;
                source[i + 7] = value;
            }

            if (i < (Length & -4))
            {
                source[i] = value;
                source[i + 1] = value;
                source[i + 2] = value;
                source[i + 3] = value;
                i += 4;
            }

            for (; i < Length; i++)
                source[i] = value;
        }
    }
#if !NO_REF_STRUCTS
    /// <inheritdoc />
    [ContractAnnotation("=> halt"),
     DoesNotReturn,
     EditorBrowsable(EditorBrowsableState.Never),
     MethodImpl(MethodImplOptions.AggressiveInlining),
     Obsolete("Equals() on Span will always throw an exception. Use the equality operator instead.")]
    public override bool Equals(object? obj) => throw new NotSupportedException();
#endif
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
    public bool TryCopyTo(Span<T> destination) => this.ReadOnly().TryCopyTo(destination);
#if !NO_REF_STRUCTS
    /// <inheritdoc />
    [ContractAnnotation("=> halt"),
     DoesNotReturn,
     EditorBrowsable(EditorBrowsableState.Never),
     MethodImpl(MethodImplOptions.AggressiveInlining),
     Obsolete("Equals() on Span will always throw an exception. Use the equality operator instead.")]
    public override int GetHashCode() => throw new NotSupportedException();
#endif
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override unsafe string ToString()
    {
        if (typeof(T) != typeof(char)) // ReSharper disable once UseStringInterpolation
            return string.Format("System.Span<{0}>[{1}]", typeof(T).Name, Length);

        if (ByteOffset == MemoryExtensions.StringAdjustment)
        {
            var obj = Unsafe.As<object>(Pinnable);

            if (obj is string text && Length == text.Length)
                return text;
        }

        fixed (T* ptr = this)
            return new((char*)this.Align(ptr), 0, Length);
    }

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
    public unsafe Span<T> Slice([NonNegativeValue] int start)
    {
        if ((uint)start > (uint)Length)
            throw new ArgumentOutOfRangeException(nameof(start));

        var byteOffset = (nint)((T*)ByteOffset + start);
        var length = Length - start;
        return new(Pinnable, byteOffset, length);
    }

    /// <summary>Creates the slice of this buffer.</summary>
    /// <param name="start">The start of the slice from this buffer.</param>
    /// <param name="length">The length of the slice from this buffer.</param>
    /// <exception cref="ArgumentOutOfRangeException">An out-of-range buffer is created.</exception>
    /// <returns>The <see cref="Span{T}"/> which is a slice of this buffer.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public unsafe Span<T> Slice([NonNegativeValue] int start, [NonNegativeValue] int length)
    {
        if ((uint)start > (uint)Length || (uint)length > (uint)(Length - start))
            throw new ArgumentOutOfRangeException(nameof(start));

        var byteOffset = (nint)((T*)ByteOffset + start);
        return new(Pinnable, byteOffset, length);
    }

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
            return [];

        var destination = new T[Length];
        CopyTo(destination);
        return destination;
    }

    /// <summary>Creates the span over the provided array.</summary>
    /// <param name="array">The array to create the <see cref="Span{T}"/> over.</param>
    /// <param name="start">The starting index.</param>
    /// <returns>The <see cref="Span{T}"/> over the parameter <paramref name="array"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The parameter <paramref name="start"/> is negative or exceeds
    /// the length of the parameter <paramref name="array"/>.
    /// </exception>
    /// <exception cref="ArrayTypeMismatchException">The parameter <paramref name="array"/> is downcast.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe Span<T> Create(T[]? array, int start) =>
        array is null ? start is 0 ? default : throw new ArgumentOutOfRangeException(nameof(start)) :
        default(T) is null && array.GetType() != typeof(T[]) ? throw new ArrayTypeMismatchException() :
        (uint)start > (uint)array.Length ? throw new ArgumentOutOfRangeException(nameof(start)) : new(
            Unsafe.As<Pinnable<T>>(array),
            (nint)((T*)SpanHelpers.PerTypeValues<T>.ArrayAdjustment + start),
            array.Length - start
        );

    /// <summary>
    /// Returns a reference to an object of type T that can be used for pinning.
    /// This method is intended to support .NET compilers and is not intended to be called by user code.
    /// </summary>
    /// <remarks><para>
    /// Applications should not directly call <see cref="GetPinnableReference"/>. Instead, callers should
    /// use their language's normal pinning syntax, such as C#'s <see langword="fixed"/> statement.
    /// If pinning a span of <see cref="char"/>, the resulting <c>char*</c> <b>is not</b> assumed to
    /// be null-terminated. This behavior is different from pinning a <see cref="string"/>,
    /// where the resulting <c>char*</c> is guaranteed to be null-terminated.
    /// </para></remarks>
    /// <returns>
    /// A reference to the element of the span at index 0, or <see langword="null"/> if the span is empty.
    /// </returns>
    [Inline]
    internal ref T GetPinnableReference() => ref (Pinnable ?? Pinnable<T>.Default).Data;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void ValidateLength(int length)
    {
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length), length, "Must be non-negative.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ValidateIndex(int index)
    {
        if ((uint)index >= (uint)Length)
            throw new ArgumentOutOfRangeException(
                nameof(index),
                index, // ReSharper disable once UseStringInterpolation
                string.Format("Must be non-zero and below length {0}.", Length)
            );
    }

    /// <summary>Enumerates the elements of a <see cref="Span{T}"/>.</summary>
    [StructLayout(LayoutKind.Auto)]
    public
#if !NO_REF_STRUCTS
        ref
#endif
        partial struct Enumerator
    {
        readonly Span<T> _span;

        [ValueRange(-1, int.MaxValue)]
        int _index;

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
        /// <param name="span">The buffer to peek through.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator(Span<T> span)
        {
            _span = span;
            _index = -1;
        }

        /// <inheritdoc cref="IEnumerator{T}.Current" />
        public readonly T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _span[_index];
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => _index = -1;

        /// <summary>Advances the enumerator to the next element of the collection.</summary>
        /// <returns>
        /// <see langword="true"/> if the enumerator was successfully advanced to the next element;
        /// <see langword="false"/> if the enumerator has passed the end of the collection.
        /// </returns>
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
}

/// <summary>Provides a type-safe and memory-safe representation of a contiguous region of arbitrary memory.</summary>
/// <remarks><para>This type delegates the responsibility of pinning the pointer to the consumer.</para></remarks>
/// <typeparam name="T">The type of items in the <see cref="ReadOnlySpan{T}"/>.</typeparam>
[DebuggerTypeProxy(typeof(SpanDebugView<>)), DebuggerDisplay("{ToString(),raw}"), StructLayout(LayoutKind.Sequential)]
#if !NO_READONLY_STRUCTS
readonly
#endif
#if !NO_REF_STRUCTS
    ref
#endif
    partial struct ReadOnlySpan<T>
#if UNMANAGED_SPAN
    where T : unmanaged
#endif
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySpan{T}"/> struct from a specified number of
    /// <typeparamref name="T"/> elements starting at a specified memory address.
    /// </summary>
    /// <param name="pointer">A pointer to the starting address of a specified number of T elements in memory.</param>
    /// <param name="length">The length of the buffer.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is negative.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ReadOnlySpan(void* pointer, [NonNegativeValue] int length)
    {
        if (IsReferenceOrContainsReferences<T>())
            throw new ArgumentException("Invalid type with pointers not supported.", nameof(pointer));

        ValidateLength(length);
        Length = length;
        ByteOffset = (nint)pointer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySpan{T}"/> struct over the entirety of the specified array.
    /// </summary>
    /// <param name="array">The array from which to create the <see cref="ReadOnlySpan{T}"/> object.</param>
    /// <exception cref="ArrayTypeMismatchException">
    /// <typeparamref name="T"/> is a reference type, and <paramref name="array"/>
    /// is not an array of type <typeparamref name="T"/>.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan(T[]? array)
    {
        if (array is null)
            return;

        if (default(T) is null && array.GetType() != typeof(T[]))
            throw new ArrayTypeMismatchException();

        Length = array.Length;
        Pinnable = Unsafe.As<Pinnable<T>>(array);
        ByteOffset = SpanHelpers.PerTypeValues<T>.ArrayAdjustment;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySpan{T}"/> struct over the entirety of the specified array.
    /// </summary>
    /// <param name="array">The source array.</param>
    /// <param name="start">
    /// The zero-based index of the first element to include in the new <see cref="ReadOnlySpan{T}"/>.
    /// </param>
    /// <param name="length">The number of elements to include in the new <see cref="ReadOnlySpan{T}"/>.</param>
    /// <exception cref="ArrayTypeMismatchException">
    /// <typeparamref name="T"/> is a reference type, and <paramref name="array"/>
    /// is not an array of type <typeparamref name="T"/>.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ReadOnlySpan(T[]? array, int start, int length)
    {
        if (array is null)
        {
            if (start is 0 && length is 0)
                return;

            throw new ArgumentOutOfRangeException(nameof(start), start, "start is out of range");
        }

        if ((uint)start > (uint)array.Length || (uint)length > (uint)(array.Length - start))
            throw new ArgumentOutOfRangeException(nameof(length), length, "length is out of range");

        if (default(T) is null && array.GetType() != typeof(T[]))
            throw new ArrayTypeMismatchException();

        Length = length;
        Pinnable = Unsafe.As<Pinnable<T>>(array);
        ByteOffset = (nint)((T*)SpanHelpers.PerTypeValues<T>.ArrayAdjustment + start);
    }

    /// <summary>Initializes a new instance of the <see cref="ReadOnlySpan{T}"/> struct.</summary>
    /// <param name="pinnable">The pinnable.</param>
    /// <param name="byteOffset">The byte offset.</param>
    /// <param name="length">The length.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan(Pinnable<T>? pinnable, nint byteOffset, int length)
    {
        Length = length;
        Pinnable = pinnable;
        ByteOffset = byteOffset;
    }

    /// <summary>Gets the element at the specified zero-based index.</summary>
    /// <param name="index">The zero-based index of the element.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="index"/> is less than zero or is greater than or equal to <see cref="Length"/>.
    /// </exception>
    public unsafe T this[[NonNegativeValue] int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get
        {
            ValidateIndex(index);

            fixed (T* ptr = this)
                return this.Align(ptr)[index];
        }
    }

    /// <summary>Gets an empty <see cref="ReadOnlySpan{T}"/> object.</summary>
    public static ReadOnlySpan<T> Empty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => default;
    }

    /// <summary>Gets a value indicating whether the current <see cref="ReadOnlySpan{T}"/> is empty.</summary>
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => Length is 0;
    }

    /// <summary>Gets the length of the current span.</summary>
    public int Length { [MethodImpl(MethodImplOptions.AggressiveInlining), NonNegativeValue, Pure] get; }

    /// <summary>Gets the byte offset.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public nint ByteOffset { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; }

    /// <summary>Gets the object to be pinned.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Pinnable<T>? Pinnable { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; }

    /// <summary>Returns a value that indicates whether two <see cref="ReadOnlySpan{T}"/> objects are equal.</summary>
    /// <remarks><para>
    /// Two <see cref="ReadOnlySpan{T}"/> objects are equal if they have the same length and the corresponding elements of
    /// <paramref name="left"/> and <paramref name="right"/> point to the same memory. Note that the test for equality
    /// does <i>not</i> attempt to determine whether the contents are equal.
    /// </para></remarks>
    /// <param name="left">The first span to compare.</param>
    /// <param name="right">The second span to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the two <see cref="ReadOnlySpan{T}"/> objects are equal; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static unsafe bool operator ==(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
    {
        if (left.Length != right.Length)
            return false;

        fixed (T* l = left)
        fixed (T* r = right)
            return left.Align(l) == right.Align(r);
    }

    /// <summary>Returns a value that indicates whether two <see cref="ReadOnlySpan{T}"/> objects are not equal.</summary>
    /// <remarks><para>
    /// Two <see cref="ReadOnlySpan{T}"/> objects are equal if they have the same length and the corresponding elements of
    /// <paramref name="left"/> and <paramref name="right"/> point to the same memory.
    /// </para></remarks>
    /// <param name="left">The first span to compare.</param>
    /// <param name="right">The second span to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the two <see cref="ReadOnlySpan{T}"/> objects are not equal;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator !=(ReadOnlySpan<T> left, ReadOnlySpan<T> right) => !(left == right);

    /// <summary>
    /// Implicitly converts the parameter by creating the new instance of
    /// <see cref="ReadOnlySpan{T}"/> by using the constructor <see cref="System.ReadOnlySpan{T}(T[])"/>.
    /// </summary>
    /// <param name="array">The parameter to pass onto the constructor.</param>
    /// <returns>
    /// The new instance of <see cref="Enumerator"/> by passing the parameter
    /// <paramref name="array"/> to the constructor <see cref="ReadOnlySpan{T}"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator ReadOnlySpan<T>(T[] array) => new(array);

    /// <summary>
    /// Implicitly converts the parameter by creating the new instance of
    /// <see cref="ReadOnlySpan{T}"/> by using the constructor <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="segment">The parameter to pass onto the constructor.</param>
    /// <returns>
    /// The new instance of <see cref="Enumerator"/> by passing the parameter
    /// <paramref name="segment"/> to the constructor <see cref="System.ReadOnlySpan{T}(T[], int, int)"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator ReadOnlySpan<T>(ArraySegment<T> segment) =>
        new(segment.Array, segment.Offset, segment.Count);

    /// <summary>Copies the contents of this <see cref="ReadOnlySpan{T}"/> into a destination <see cref="ReadOnlySpan{T}"/>.</summary>
    /// <param name="destination">The destination <see cref="ReadOnlySpan{T}"/> object.</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="destination"/> is shorter than the source <see cref="ReadOnlySpan{T}"/>.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(Span<T> destination)
    {
        if (!TryCopyTo(destination))
            throw new ArgumentException("Destination too short", nameof(destination));
    }
#if !NO_REF_STRUCTS
    /// <inheritdoc />
    [ContractAnnotation("=> halt"),
     DoesNotReturn,
     EditorBrowsable(EditorBrowsableState.Never),
     MethodImpl(MethodImplOptions.AggressiveInlining),
     Obsolete("Equals() on Span will always throw an exception. Use the equality operator instead.")]
    public override bool Equals(object? obj) => throw new NotSupportedException();
#endif
    /// <summary>
    /// Attempts to copy the current <see cref="ReadOnlySpan{T}"/> to a destination <see cref="ReadOnlySpan{T}"/>
    /// and returns a value that indicates whether the copy operation succeeded.
    /// </summary>
    /// <remarks><para>
    /// This method copies all of <c>source</c> to <paramref name="destination"/> even if
    /// <c>source</c> and <paramref name="destination"/> overlap.
    /// If <paramref name="destination"/> is shorter than the source <see cref="ReadOnlySpan{T}"/>, this method returns
    /// <see langword="false"/>, and no data is written to <paramref name="destination"/>.
    /// </para></remarks>
    /// <param name="destination">The target of the copy operation.</param>
    /// <returns><see langword="true"/> if the copy operation succeeded; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe bool TryCopyTo(Span<T> destination)
    {
        if (Length is 0)
            return true;

        if ((uint)Length > (uint)destination.Length)
            return false;

        fixed (T* s = this)
        fixed (T* d = destination)
            SpanHelpers.CopyTo(destination.Align(d), destination.Length, this.Align(s), Length);

        return true;
    }
#if !NO_REF_STRUCTS
    /// <inheritdoc />
    [ContractAnnotation("=> halt"),
     DoesNotReturn,
     EditorBrowsable(EditorBrowsableState.Never),
     MethodImpl(MethodImplOptions.AggressiveInlining),
     Obsolete("Equals() on Span will always throw an exception. Use the equality operator instead.")]
    public override int GetHashCode() => throw new NotSupportedException();
#endif
    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override unsafe string ToString()
    {
        if (typeof(T) != typeof(char)) // ReSharper disable once UseStringInterpolation
            return string.Format("System.ReadOnlySpan<{0}>[{1}]", typeof(T).Name, Length);

        if (ByteOffset == MemoryExtensions.StringAdjustment)
        {
            var obj = Unsafe.As<object>(Pinnable);

            if (obj is string text && Length == text.Length)
                return text;
        }

        fixed (T* ptr = this)
            return new((char*)this.Align(ptr), 0, Length);
    }

    /// <summary>Returns an enumerator of this <see cref="ReadOnlySpan{T}"/>.</summary>
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
    public unsafe ReadOnlySpan<T> Slice([NonNegativeValue] int start)
    {
        if ((uint)start > (uint)Length)
            throw new ArgumentOutOfRangeException(nameof(start));

        var byteOffset = (nint)((T*)ByteOffset + start);
        var length = Length - start;
        return new(Pinnable, byteOffset, length);
    }

    /// <summary>Creates the slice of this buffer.</summary>
    /// <param name="start">The start of the slice from this buffer.</param>
    /// <param name="length">The length of the slice from this buffer.</param>
    /// <exception cref="ArgumentOutOfRangeException">An out-of-range buffer is created.</exception>
    /// <returns>The <see cref="ReadOnlySpan{T}"/> which is a slice of this buffer.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public unsafe ReadOnlySpan<T> Slice([NonNegativeValue] int start, [NonNegativeValue] int length)
    {
        if ((uint)start > (uint)Length || (uint)length > (uint)(Length - start))
            throw new ArgumentOutOfRangeException(nameof(start));

        var byteOffset = (nint)((T*)ByteOffset + start);
        return new(Pinnable, byteOffset, length);
    }

    /// <summary>Copies the contents of this span into a new array.</summary>
    /// <remarks><para>
    /// This method performs a heap allocation and therefore should be avoided if possible.
    /// Heap allocations are expected in APIs that work with arrays.
    /// Using such APIs is unavoidable if an alternative API overhead that takes a <see cref="ReadOnlySpan{T}"/> does not exist.
    /// </para></remarks>
    /// <returns>An array containing the data in the current span.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public T[] ToArray()
    {
        if (IsEmpty)
            return [];

        var destination = new T[Length];
        CopyTo(destination);
        return destination;
    }

    /// <summary>
    /// Returns a reference to an object of type T that can be used for pinning.
    /// This method is intended to support .NET compilers and is not intended to be called by user code.
    /// </summary>
    /// <remarks><para>
    /// Applications should not directly call <see cref="GetPinnableReference"/>. Instead, callers should
    /// use their language's normal pinning syntax, such as C#'s <see langword="fixed"/> statement.
    /// If pinning a span of <see cref="char"/>, the resulting <c>char*</c> <b>is not</b> assumed to
    /// be null-terminated. This behavior is different from pinning a <see cref="string"/>,
    /// where the resulting <c>char*</c> is guaranteed to be null-terminated.
    /// </para></remarks>
    /// <returns>
    /// A reference to the element of the span at index 0, or <see langword="null"/> if the span is empty.
    /// </returns>
    [Inline]
    internal ref readonly T GetPinnableReference() => ref (Pinnable ?? Pinnable<T>.Default).Data;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void ValidateLength(int length)
    {
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length), length, "Must be non-negative.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ValidateIndex(int index)
    {
        if ((uint)index >= (uint)Length)
            throw new ArgumentOutOfRangeException(
                nameof(index),
                index, // ReSharper disable once UseStringInterpolation
                string.Format("Must be non-zero and below length {0}.", Length)
            );
    }

    /// <summary>Enumerates the elements of a <see cref="ReadOnlySpan{T}"/>.</summary>
    [StructLayout(LayoutKind.Auto)]
    public
#if !NO_REF_STRUCTS
        ref
#endif
        partial struct Enumerator
    {
        readonly ReadOnlySpan<T> _span;

        [ValueRange(-1, int.MaxValue)]
        int _index;

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
        /// <param name="span">The buffer to peek through.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator(ReadOnlySpan<T> span)
        {
            _span = span;
            _index = -1;
        }

        /// <inheritdoc cref="IEnumerator{T}.Current" />
        public readonly T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _span[_index];
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => _index = -1;

        /// <summary>Advances the enumerator to the next element of the collection.</summary>
        /// <returns>
        /// <see langword="true"/> if the enumerator was successfully advanced to the next element;
        /// <see langword="false"/> if the enumerator has passed the end of the collection.
        /// </returns>
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
}

/// <summary>Represents a debug view to this span.</summary>
/// <typeparam name="T">The type of element in the span.</typeparam>
sealed class SpanDebugView<T>
#if UNMANAGED_SPAN
    where T : unmanaged
#endif
{
    /// <summary>Initializes a new instance of the <see cref="SpanDebugView{T}"/> class.</summary>
    /// <param name="span">The span to collect.</param>
    public SpanDebugView(Span<T> span) => Items = [..span];

    /// <summary>Initializes a new instance of the <see cref="SpanDebugView{T}"/> class.</summary>
    /// <param name="span">The span to collect.</param>
    public SpanDebugView(ReadOnlySpan<T> span) => Items = [..span];

    /// <summary>Gets the items of this span.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Items { [Pure] get; }
}

[StructLayout(LayoutKind.Sequential)]
sealed class Pinnable<T>
{
    public static Pinnable<T> Default { get; } = new();

    // ReSharper disable once NullableWarningSuppressionIsUsed
    public T Data = default!;
}
#endif
