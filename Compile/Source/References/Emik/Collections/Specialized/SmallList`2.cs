// SPDX-License-Identifier: MPL-2.0
// ReSharper disable NullableWarningSuppressionIsUsed RedundantExtendsListEntry RedundantUnsafeContext
// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;

using static Two;

#pragma warning disable CA1000, CA1065, CA1819, IDISP012, RCS1158
#if !NETFRAMEWORK
/// <summary>Inlines elements before falling back on the heap using <see cref="ArrayPool{T}"/>.</summary>
/// <typeparam name="T">The type of the collection.</typeparam>
/// <typeparam name="TRef">The type of reference containing a continuous region of <typeparamref name="T"/>.</typeparam>
/// <param name="view">The view to hold as the initial value.</param>
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
ref partial struct SmallList<T, TRef>(Span<T> view)
#if UNMANAGED_SPAN
        where T : unmanaged
#endif
{
    [NonNegativeValue]
    int _length;

    // ReSharper disable once ReplaceWithPrimaryConstructorParameter
    Span<T> _view = view;

    T[]? _rental;

    /// <summary>Initializes a new instance of the <see cref="SmallList{T, TRef}"/> struct.</summary>
    /// <param name="reference">The reference considered to be a continuous buffer of <typeparamref name="T"/>.</param>
    public SmallList(ref TRef reference)
        : this(AsSpan<T, TRef>(ref Unsafe.AsRef(reference))) { }

    /// <summary>Gets the amount of items that can be inlined before <see cref="ArrayPool{T}"/> is used.</summary>
    public static int InlinedLength { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; }
        = Unsafe.SizeOf<TRef>() / Unsafe.SizeOf<T>();

    /// <inheritdoc cref="Span{T}.Empty"/>
    public static SmallList<T, TRef> Empty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => default;
    }

    /// <inheritdoc cref="Span{T}.IsEmpty"/>
    public readonly bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => View.IsEmpty;
    }

    /// <summary>Gets a value indicating whether the elements are inlined.</summary>
    public readonly bool IsInlined
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining),
         MemberNotNullWhen(false, nameof(_rental), nameof(TransferOwnership)), Pure]
        get => _rental is null;
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

    /// <summary>Gets the buffer.</summary>
    public readonly Span<T> View
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get => _view[.._length];
    }

    /// <inheritdoc cref="IList{T}.Clear"/>
    public SmallList<T, TRef> Reset
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            {
                _length = 0;
                return this;
            }
        }
    }

    /// <summary>Gets the entire exposed view.</summary>
    public SmallList<T, TRef> Stretched
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            _length = _view.Length;
            return this;
        }
    }

    /// <summary>Gets and transfers responsibility of disposing the inner array to the caller.</summary>
    /// <returns>The inner array.</returns>
    public T[]? TransferOwnership
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining),
         MustUseReturnValue("Dispose array by passing it into System.Memory.ArrayPool<T>.Shared.Return")]
        get
        {
            if (IsInlined)
                return null;

            var rental = _rental;
            _length = 0;
            _view = default;
            _rental = null;
            return rental;
        }
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
    public static bool operator ==(SmallList<T, TRef> left, SmallList<T, TRef> right) => left.View == right.View;

    /// <inheritdoc cref="Span{T}.op_Inequality"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator !=(SmallList<T, TRef> left, SmallList<T, TRef> right) => !(left == right);

    /// <inheritdoc cref="IDisposable.Dispose"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (!IsInlined)
            ArrayPool<T>.Shared.Return(TransferOwnership);
    }

    /// <inheritdoc />
    [Obsolete("Will always throw", true), DoesNotReturn]
    public readonly override bool Equals(object? obj) => throw Unreachable;

    /// <inheritdoc />
    [Obsolete("Will always throw", true), DoesNotReturn]
    public readonly override int GetHashCode() => throw Unreachable;

    /// <inheritdoc cref="Span{T}.ToString"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly override string ToString() =>
        typeof(T) == typeof(char) ? View.ToString() : View.ToArray().Conjoin();

    /// <inheritdoc cref="IList{T}.Add"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallList<T, TRef> Append(T item) => Insert(_length, new ReadOnlySpan<T>(item));

    /// <inheritdoc cref="List{T}.AddRange"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallList<T, TRef> Append(scoped ReadOnlySpan<T> collection) => Insert(_length, collection);

    /// <inheritdoc cref="List{T}.AddRange"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallList<T, TRef> Append(IEnumerable<T> collection)
    {
        if (collection.TryGetNonEnumeratedCount(out var count))
            MakeRoom(count);

        foreach (var x in collection)
            Append(x);

        return this;
    }

    /// <inheritdoc cref="IList{T}.Add"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallList<T, TRef> Prepend(T item) => Insert(0, new ReadOnlySpan<T>(item));

    /// <inheritdoc cref="List{T}.AddRange"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallList<T, TRef> Prepend(scoped ReadOnlySpan<T> collection) => Insert(0, collection);

    /// <inheritdoc cref="List{T}.AddRange"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallList<T, TRef> Prepend(IEnumerable<T> collection)
    {
        MakeRoom(collection);

        using var e = collection.GetEnumerator();

        for (var i = 0; e.MoveNext(); i++)
            Insert(i, e.Current);

        return this;
    }

    /// <inheritdoc cref="IList{T}.Insert"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallList<T, TRef> Insert([NonNegativeValue] int offset, T item) =>
        Insert(offset, new ReadOnlySpan<T>(item));

    /// <inheritdoc cref="IList{T}.Insert"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallList<T, TRef> Insert([NonNegativeValue] int index, scoped ReadOnlySpan<T> items)
    {
        if (HasRoom(items.Length))
        {
            Copy(index, items, _view);
            return this;
        }

        var replacement = Rent(items.Length);
        Copy(index, items, replacement);
        Swap(replacement);
        return this;
    }

    /// <inheritdoc cref="IList{T}.RemoveAt"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallList<T, TRef> RemoveAt([NonNegativeValue] int index) => RemoveAt(index, 1);

    /// <summary>Removes the <see cref="SmallList{T, TRef}"/> item at the specified offset and length.</summary>
    /// <param name="offset">The offset of the slice to remove.</param>
    /// <param name="length">The length of the slice to remove.</param>
    /// <returns>Itself.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallList<T, TRef> RemoveAt([NonNegativeValue] int offset, [NonNegativeValue] int length)
    {
        View[(offset + length)..].CopyTo(View[offset..]);
        _length -= length;
        return this;
    }

    /// <inheritdoc cref="IList{T}.RemoveAt"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallList<T, TRef> RemoveAt(Index index)
    {
        var offset = index.GetOffset(View.Length);
        RemoveAt(offset, 1);
        return this;
    }

    /// <summary>Removes the <see cref="SmallList{T, TRef}"/> item at the specified range.</summary>
    /// <param name="range">The range of the slice to remove.</param>
    /// <returns>Itself.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallList<T, TRef> RemoveAt(Range range)
    {
        var (offset, length) = range.GetOffsetAndLength(View.Length);
        RemoveAt(offset, length);
        return this;
    }

    /// <summary>Shrinks the collection.</summary>
    /// <param name="amount">The amount of elements to shrink.</param>
    /// <returns>Itself.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallList<T, TRef> Shrink([NonNegativeValue] int amount)
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
    public readonly ref T Nth(int i)
    {
        if (i >= 0 && i < _length)
            return ref _view[i];

        return ref Unsafe.NullRef<T>();
    }

    /// <summary>Gets the specific element, returning the default value when out-of-bounds.</summary>
    /// <param name="i">The index.</param>
    /// <returns>The element, or default.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public readonly ref T Nth(Index i)
    {
        var offset = i.GetOffset(_length);
        return ref Nth(offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Copy([NonNegativeValue] int offset, scoped ReadOnlySpan<T> insertion, scoped Span<T> destination)
    {
        View[offset..].CopyTo(destination[offset..]);
        insertion.CopyTo(destination[offset..]);
        View[..offset].CopyTo(destination);
        _length += insertion.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Swap(T[] replacement)
    {
        if (!IsInlined)
            ArrayPool<T>.Shared.Return(_rental);

        _view = _rental = replacement;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void MakeRoom(int by)
    {
        if (HasRoom(by))
            return;

        var replacement = Rent(by);
        View.CopyTo(replacement);
        Swap(replacement);
        _length += by;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void MakeRoom([NoEnumeration] IEnumerable<T> collection)
    {
        if (collection.TryGetNonEnumeratedCount(out var count))
            MakeRoom(count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly bool HasRoom(int by) => _length + by <= _view.Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly T[] Rent([NonNegativeValue] int by)
    {
        var sum = unchecked((uint)(_view.Length + by));
        var length = unchecked((int)BitOperations.RoundUpToPowerOf2(sum));
        return ArrayPool<T>.Shared.Rent(length);
    }
}
#endif
