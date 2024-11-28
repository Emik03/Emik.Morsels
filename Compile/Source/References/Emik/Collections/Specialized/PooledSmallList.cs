// SPDX-License-Identifier: MPL-2.0
// ReSharper disable NullableWarningSuppressionIsUsed RedundantExtendsListEntry RedundantNameQualifier RedundantUnsafeContext UseSymbolAlias
// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;

// ReSharper disable RedundantNameQualifier RedundantUsingDirective
using static System.Runtime.CompilerServices.RuntimeHelpers;
using static Span;
using FieldInfo = System.Reflection.FieldInfo;

#if (NET45_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER) && !NO_SYSTEM_MEMORY
/// <summary>Provides the method needed for collection expressions in <see cref="PooledSmallList{T}"/>.</summary>
static class PooledSmallListBuilder
{
    /// <summary>Converts the buffer into an expandable buffer.</summary>
    /// <typeparam name="T">The type of span.</typeparam>
    /// <param name="span">The span.</param>
    /// <returns>The <see cref="PooledSmallList{T}"/> that encapsulates the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static PooledSmallList<T> From<T>(ReadOnlySpan<T> span) => default(PooledSmallList<T>).Append(span);
}

/// <summary>Inlines elements before falling back on the heap using <see cref="ArrayPool{T}"/>.</summary>
/// <typeparam name="T">The type of the collection.</typeparam>
[CollectionBuilder(typeof(PooledSmallListBuilder), nameof(PooledSmallListBuilder.From))]
#if !NO_REF_STRUCTS
ref
#endif
    partial struct PooledSmallList<T>
#if !NO_ALLOWS_REF_STRUCT
    : IDisposable
#endif
#if UNMANAGED_SPAN
    where T : unmanaged
#endif
{
    const int Inlined = 0, UnmanagedHeap = 1, ArrayPool = 2;

    [NonNegativeValue]
    int _length;

    Span<T> _view;

    T[]? _rental;

    /// <summary>Initializes a new instance of the <see cref="PooledSmallList{T}"/> struct.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PooledSmallList() { }

    /// <summary>Initializes a new instance of the <see cref="PooledSmallList{T}"/> struct.</summary>
    /// <param name="capacity">
    /// The initial allocation, which puts it on the heap immediately but can save future resizing.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PooledSmallList([NonNegativeValue] int capacity)
    {
        MakeRoom(capacity);
        _length = 0;
    }

    /// <summary>Initializes a new instance of the <see cref="PooledSmallList{T}"/> struct.</summary>
    /// <param name="view">The view to hold as the initial value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PooledSmallList(Span<T> view) => _view = view;

    /// <inheritdoc cref="Span{T}.Empty"/>
    public static PooledSmallList<T> Empty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => default;
    }

    /// <inheritdoc cref="Span{T}.IsEmpty"/>
    public readonly bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => View.IsEmpty;
    }

    /// <summary>Gets a value indicating whether the elements are inlined.</summary>
    [CLSCompliant(false)]
    public readonly bool IsInlined
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining),
         MemberNotNullWhen(false, nameof(_rental), nameof(DangerouslyTransferOwnership)),
         Pure]
        get => _rental is null;
    }

    /// <summary>Gets a value indicating whether the elements are using the unmanaged heap.</summary>
    [CLSCompliant(false)]
    public readonly bool IsUnmanagedHeap
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining),
         Pure]
        get => _rental.ToAddress() is UnmanagedHeap;
    }

    /// <summary>Gets a value indicating whether the elements are inlined.</summary>
    [CLSCompliant(false)]
    public readonly bool IsArrayPool
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining),
         MemberNotNullWhen(true, nameof(_rental), nameof(DangerouslyTransferOwnership)),
         Pure]
        get => _rental.ToAddress() >= ArrayPool;
    }

    /// <inheritdoc cref="Span{T}.Length"/>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), NonNegativeValue, Pure] readonly get => _length;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            var newLength = Math.Max(value, 0);
            var relativeLength = newLength - _length;
            MakeRoom(relativeLength);
            _length = newLength;
        }
    }

    /// <summary>Gets and transfers responsibility of disposing the inner unmanaged array to the caller.</summary>
    public nint DangerouslyTransferOwnershipUnmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining),
         MustUseReturnValue("Dispose unmanaged array with System.Runtime.InteropServices.Marshal.FreeHGlobal")]
        get
        {
            if (!IsUnmanagedHeap)
                return 0;

            var pointer = UnmanagedHeapPointer;
            _length = 0;
            _view = default;
            _rental = null;
            return pointer;
        }
    }

    /// <summary>Gets the buffer.</summary>
    public readonly Span<T> View
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _view[.._length];
    }

    /// <inheritdoc cref="ICollection{T}.Clear"/>
    public PooledSmallList<T> Reset
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            _length = 0;
            return this;
        }
    }

    /// <summary>Gets the entire exposed view.</summary>
    public PooledSmallList<T> Stretched
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            _length = _view.Length;
            return this;
        }
    }

    /// <summary>Gets the mutable reference to the <see cref="PooledSmallList{T}"/>.</summary>
    public unsafe ref PooledSmallList<T> AsRef
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get
        {
#pragma warning disable 8500
            fixed (PooledSmallList<T>* ptr = &this)
#pragma warning restore 8500
                return ref *ptr;
        }
    }

    /// <summary>Gets the inner heap array, or a copy of the inlined array.</summary>
    public readonly T[] ToArrayLazily
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get => IsArrayPool ? _rental : _view.ToArray();
    }

    /// <summary>Gets and transfers responsibility of disposing the inner array to the caller.</summary>
    public T[]? DangerouslyTransferOwnership
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining),
         MustUseReturnValue("Dispose array with System.Memory.ArrayPool<T>.Shared.Return.")]
        get
        {
            if (!IsArrayPool)
                return null;

            var rental = _rental;
            _length = 0;
            _view = default;
            _rental = null;
            return rental;
        }
    }

    readonly unsafe nint UnmanagedHeapPointer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get => (nint)Unsafe.AsPointer(ref MemoryMarshal.GetReference(_view));
    }

    /// <inheritdoc cref="Span{T}.Slice(int, int)"/>
    public readonly Span<T> this[[NonNegativeValue] int start, [NonNegativeValue] int length]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => View.Slice(start, length);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] set => value.CopyTo(View.Slice(start, length));
    }

    /// <inheritdoc cref="Span{T}.this"/>
    public readonly Span<T> this[Range range]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => View[range];
        [MethodImpl(MethodImplOptions.AggressiveInlining)] set => value.CopyTo(View[range]);
    }

    /// <inheritdoc cref="Span{T}.this"/>
    public readonly ref T this[[NonNegativeValue] int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => ref View[index];
    }

    /// <inheritdoc cref="Span{T}.this"/>
    public readonly ref T this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => ref View[index];
    }

    /// <inheritdoc cref="Span{T}.op_Equality"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator ==(PooledSmallList<T> left, PooledSmallList<T> right) => left.View == right.View;

    /// <inheritdoc cref="Span{T}.op_Inequality"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator !=(PooledSmallList<T> left, PooledSmallList<T> right) => !(left == right);

    /// <summary>
    /// Implicitly converts the parameter by creating the new instance of <see cref="PooledSmallList{T}"/>
    /// by using the constructor <see cref="Emik.Morsels.PooledSmallList{T}(int)"/>.
    /// </summary>
    /// <param name="capacity">The parameter to pass onto the constructor.</param>
    /// <returns>
    /// The new instance of <see cref="PooledSmallList{T}"/> by passing the parameter <paramref name="capacity"/>
    /// to the constructor <see cref="Emik.Morsels.PooledSmallList{T}(int)"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator PooledSmallList<T>(int capacity) => new(capacity);

    /// <summary>Implicitly converts the buffer into an expandable buffer.</summary>
    /// <param name="span">The span.</param>
    /// <returns>The <see cref="PooledSmallList{T}"/> that encapsulates the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator PooledSmallList<T>(Span<T> span) => new(span);

    /// <inheritdoc cref="AsSpan{TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static PooledSmallList<T> From<TRef>(ref TRef reference)
        where TRef : struct =>
        AsSpan(ref reference);

    /// <summary>Reinterprets the reference as the continuous buffer of <typeparamref name="T"/>.</summary>
    /// <typeparam name="TRef">The generic representing the continuous buffer of <typeparamref name="T"/>.</typeparam>
    /// <param name="reference">The reference.</param>
    /// <returns>The span.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Span<T> AsSpan<TRef>(ref TRef reference)
        where TRef : struct =>
        Validate<TRef>.AsSpan(ref reference);

    /// <inheritdoc cref="IDisposable.Dispose"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        switch (_rental.ToAddress())
        {
            case Inlined: break;
            case UnmanagedHeap:
                Marshal.FreeHGlobal(DangerouslyTransferOwnershipUnmanaged);
                break;
            default:
                ArrayPool<T>.Shared.Return(DangerouslyTransferOwnership!);
                break;
        }
    }

    /// <inheritdoc />
    [DoesNotReturn, Obsolete("Will always throw", true)]
    public readonly override bool Equals(object? obj) => throw Unreachable;

    /// <inheritdoc />
    [DoesNotReturn, Obsolete("Will always throw", true)]
    public readonly override int GetHashCode() => throw Unreachable;

    /// <inheritdoc cref="Span{T}.ToString"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly override string ToString() =>
        typeof(T) == typeof(char) ? View.ToString() : View.ToArray().Conjoin();

    /// <inheritdoc cref="ICollection{T}.Add"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PooledSmallList<T> Append(T item)
    {
        if (HasRoom(1))
        {
            _view[_length++] = item;
            return this;
        }

        if (CanAllocateInUnmanagedHeap(1, out var length, out var bytes))
        {
            var unmanaged = Rent(length, bytes);
            _view.CopyTo(unmanaged);
            unmanaged[_length++] = item;
            Swap(unmanaged);
            return this;
        }

        var replacement = ArrayPool<T>.Shared.Rent(length);
        _view.CopyTo(replacement);
        replacement[_length++] = item;
        Swap(replacement);
        return this;
    }

    /// <inheritdoc cref="List{T}.AddRange"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PooledSmallList<T> Append(scoped ReadOnlySpan<T> collection)
    {
        if (HasRoom(collection.Length))
        {
            collection.CopyTo(_view[_length..]);
            _length += collection.Length;
            return this;
        }

        if (CanAllocateInUnmanagedHeap(collection.Length, out var length, out var bytes))
        {
            var unmanaged = Rent(length, bytes);
            _view.CopyTo(unmanaged);
            collection.CopyTo(unmanaged[_length..]);
            _length += collection.Length;
            Swap(unmanaged);
            return this;
        }

        var replacement = ArrayPool<T>.Shared.Rent(length);
        _view.CopyTo(replacement);
        collection.CopyTo(replacement.AsSpan()[_length..]);
        _length += collection.Length;
        Swap(replacement);
        return this;
    }

    /// <inheritdoc cref="List{T}.AddRange"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PooledSmallList<T> Append([InstantHandle] IEnumerable<T>? collection)
    {
        if (collection is null)
            return this;

        if (collection.TryGetNonEnumeratedCount(out var count))
            MakeRoom(count);

        foreach (var x in collection)
            Append(x);

        return this;
    }

    /// <inheritdoc cref="ICollection{T}.Add"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PooledSmallList<T> Prepend(T item)
    {
        if (HasRoom(1))
        {
            View.CopyTo(_view[1..]);
            _length++;
            _view[0] = item;
            return this;
        }

        if (CanAllocateInUnmanagedHeap(1, out var length, out var bytes))
        {
            var unmanaged = Rent(length, bytes);
            _view.CopyTo(unmanaged[1..]);
            unmanaged[0] = item;
            _length++;
            Swap(unmanaged);
            return this;
        }

        var replacement = ArrayPool<T>.Shared.Rent(length);
        _view.CopyTo(replacement.AsSpan()[1..]);
        replacement[0] = item;
        _length++;
        Swap(replacement);
        return this;
    }

    /// <inheritdoc cref="List{T}.AddRange"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PooledSmallList<T> Prepend(scoped ReadOnlySpan<T> collection)
    {
        if (HasRoom(collection.Length))
        {
            View.CopyTo(_view[collection.Length..]);
            _length += collection.Length;
            collection.CopyTo(_view);
            return this;
        }

        if (CanAllocateInUnmanagedHeap(collection.Length, out var length, out var bytes))
        {
            var unmanaged = Rent(length, bytes);
            _view.CopyTo(unmanaged[collection.Length..]);
            collection.CopyTo(unmanaged);
            _length += collection.Length;
            Swap(unmanaged);
            return this;
        }

        var replacement = ArrayPool<T>.Shared.Rent(length);
        _view.CopyTo(replacement.AsSpan()[collection.Length..]);
        collection.CopyTo(replacement);
        _length += collection.Length;
        Swap(replacement);
        return this;
    }

    /// <inheritdoc cref="List{T}.AddRange"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PooledSmallList<T> Prepend([InstantHandle] IEnumerable<T> collection) => Insert(0, collection);

    /// <inheritdoc cref="IList{T}.Insert"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PooledSmallList<T> Insert([NonNegativeValue] int offset, T item)
    {
        if (HasRoom(1))
        {
            Copy(offset, item, _view);
            return this;
        }

        if (CanAllocateInUnmanagedHeap(1, out var length, out var bytes))
        {
            var unmanaged = Rent(length, bytes);
            Copy(offset, item, unmanaged);
            Swap(unmanaged);
            return this;
        }

        var replacement = ArrayPool<T>.Shared.Rent(length);
        Copy(offset, item, replacement);
        Swap(replacement);
        return this;
    }

    /// <inheritdoc cref="IList{T}.Insert"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PooledSmallList<T> Insert([NonNegativeValue] int index, scoped ReadOnlySpan<T> collection)
    {
        if (HasRoom(collection.Length))
        {
            Copy(index, collection, _view);
            return this;
        }

        if (CanAllocateInUnmanagedHeap(collection.Length, out var length, out var bytes))
        {
            var unmanaged = Rent(length, bytes);
            Copy(index, collection, unmanaged);
            Swap(unmanaged);
            return this;
        }

        var replacement = ArrayPool<T>.Shared.Rent(length);
        Copy(index, collection, replacement);
        Swap(replacement);
        return this;
    }

    /// <inheritdoc cref="List{T}.AddRange"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PooledSmallList<T> Insert([NonNegativeValue] int index, [InstantHandle] IEnumerable<T> collection)
    {
        if (collection.TryCount() is { } count)
            MakeRoom(count);

        using var e = collection.GetEnumerator();

        for (var i = index; e.MoveNext(); i++)
            Insert(i, e.Current);

        return this;
    }

    /// <inheritdoc cref="IList{T}.RemoveAt"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PooledSmallList<T> RemoveAt([NonNegativeValue] int index) => RemoveAt(index, 1);

    /// <summary>Removes the <see cref="PooledSmallList{T}"/> item at the specified offset and length.</summary>
    /// <param name="offset">The offset of the slice to remove.</param>
    /// <param name="length">The length of the slice to remove.</param>
    /// <returns>Itself.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PooledSmallList<T> RemoveAt([NonNegativeValue] int offset, [NonNegativeValue] int length)
    {
        View[(offset + length)..].CopyTo(View[offset..]);
        _length -= length;
        return this;
    }

    /// <inheritdoc cref="IList{T}.RemoveAt"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PooledSmallList<T> RemoveAt(Index index)
    {
        var offset = index.GetOffset(_length);
        RemoveAt(offset, 1);
        return this;
    }

    /// <summary>Removes the <see cref="PooledSmallList{T}"/> item at the specified range.</summary>
    /// <param name="range">The range of the slice to remove.</param>
    /// <returns>Itself.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PooledSmallList<T> RemoveAt(Range range)
    {
        range.GetOffsetAndLength(_length, out var offset, out var length);
        RemoveAt(offset, length);
        return this;
    }

    /// <summary>Shrinks the collection.</summary>
    /// <param name="amount">The amount of elements to shrink.</param>
    /// <returns>Itself.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PooledSmallList<T> Shrink([NonNegativeValue] int amount)
    {
        Length -= amount;
        return this;
    }

    /// <inheritdoc cref="Span{T}.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly Span<T>.Enumerator GetEnumerator() => View.GetEnumerator();

    /// <summary>Gets the specific element, returning the default value when out-of-bounds.</summary>
    /// <param name="i">The index.</param>
    /// <returns>The element, or default.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly ref T Nth([NonNegativeValue] int i) => // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        ref i >= 0 && i < _length ? ref _view[i] : ref Unsafe.NullRef<T>();

    /// <summary>Gets the specific element, returning the default value when out-of-bounds.</summary>
    /// <param name="i">The index.</param>
    /// <returns>The element, or default.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly ref T Nth(Index i) => ref Nth(i.GetOffset(_length));

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static unsafe Span<T> Rent([NonNegativeValue] int length, [NonNegativeValue] int bytes) =>
        new((void*)Marshal.AllocHGlobal(bytes), length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Copy([NonNegativeValue] int offset, T insertion, scoped Span<T> destination)
    {
        switch (offset)
        {
            case 0:
                View.CopyTo(destination[1..]);
                break;
            case var _ when offset == _length:
                View.CopyTo(destination);
                break;
            default:
                View[offset..].CopyTo(destination[(offset + 1)..]);
                View[..offset].CopyTo(destination);
                break;
        }

        destination[offset] = insertion;
        _length++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Copy([NonNegativeValue] int offset, scoped ReadOnlySpan<T> insertion, scoped Span<T> destination)
    {
        switch (offset)
        {
            case 0:
                View.CopyTo(destination[insertion.Length..]);
                insertion.CopyTo(destination);
                break;
            case var _ when offset == _length:
                insertion.CopyTo(destination[offset..]);
                View.CopyTo(destination);
                break;
            default:
                View[offset..].CopyTo(destination[(offset + insertion.Length)..]);
                insertion.CopyTo(destination[offset..]);
                View[..offset].CopyTo(destination);
                break;
        }

        _length += insertion.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Swap(T[] replacement)
    {
        if (IsArrayPool)
            ArrayPool<T>.Shared.Return(_rental);

        _view = _rental = replacement;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe void Swap(Span<T> replacement)
    {
        if (IsUnmanagedHeap)
            Marshal.FreeHGlobal((nint)Unsafe.AsPointer(ref MemoryMarshal.GetReference(_view)));

        UnsafelySetNullishTo(out _rental, 1);
        _view = replacement;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void MakeRoom([NonNegativeValue] int by)
    {
        if (HasRoom(by))
            return;

        if (CanAllocateInUnmanagedHeap(by, out var length, out var bytes))
        {
            var unmanaged = Rent(length, bytes);
            View.CopyTo(unmanaged);
            Swap(unmanaged);
        }
        else
        {
            var replacement = ArrayPool<T>.Shared.Rent(length);
            View.CopyTo(replacement);
            Swap(replacement);
        }

        _length += by;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly bool CanAllocateInUnmanagedHeap([NonNegativeValue] int by, out int length, out int bytes)
    {
        length = unchecked((int)((uint)(_view.Length + by)).RoundUpToPowerOf2());

        if (IsReferenceOrContainsReferences<T>())
        {
            Unsafe.SkipInit(out bytes);
            return false;
        }

        bytes = length * Unsafe.SizeOf<T>();

        if (length >= 0 && bytes >= 0)
            return true;

        // Swaps to ArrayPool.
        Marshal.FreeHGlobal(UnmanagedHeapPointer);
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly bool HasRoom(int by) => _length + by <= _view.Length;

    /// <summary>Validator of generics representing the continuous buffer over the element type.</summary>
    /// <typeparam name="TRef">The generic representing the continuous buffer over the element type.</typeparam>
    public static class Validate<TRef>
        where TRef : struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Validate() => Go(typeof(TRef));

        /// <summary>Gets the inlined length.</summary>
        [NonNegativeValue]
        public static int InlinedLength { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; } =
            Unsafe.SizeOf<TRef>() / Unsafe.SizeOf<T>();

        /// <summary>Reinterprets the reference as the continuous buffer over the element type.</summary>
        /// <param name="reference">The reference.</param>
        /// <returns>The span.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public static unsafe Span<T> AsSpan(ref TRef reference) =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            MemoryMarshal.CreateSpan(ref Unsafe.As<TRef, T>(ref reference), InlinedLength);
#else
            new(Unsafe.AsPointer(ref Unsafe.As<TRef, T>(ref reference)), InlinedLength);
#endif

        // ReSharper disable once SuggestBaseTypeForParameter
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool Go(Type type) =>
            type
               .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
               .Where(x => x.FieldType is var y && y != typeof(T) && (!y.IsValueType || y != x.DeclaringType && Go(y)))
               .Select(Throw)
               .Any();

        [DoesNotReturn, MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool Throw(FieldInfo _) =>
            throw new TypeLoadException($"\"{typeof(TRef).Name}\" contains fields other than {typeof(T).Name}.");
    }
}
#endif
