// SPDX-License-Identifier: MPL-2.0
// ReSharper disable NullableWarningSuppressionIsUsed RedundantExtendsListEntry RedundantUnsafeContext
// ReSharper disable once CheckNamespace EmptyNamespace
namespace Emik.Morsels;

using FieldInfo = System.Reflection.FieldInfo;

#pragma warning disable CA1000, CA1065, CA1819, IDISP012, RCS1158
#if !NETFRAMEWORK
/// <summary>Inlines elements before falling back on the heap using <see cref="ArrayPool{T}"/>.</summary>
/// <typeparam name="T">The type of the collection.</typeparam>
/// <param name="view">The view to hold as the initial value.</param>
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
ref partial struct SmallerList<T>(Span<T> view)
#if UNMANAGED_SPAN
    where T : unmanaged
#endif
{
    [NonNegativeValue]
    int _length;

    // ReSharper disable once ReplaceWithPrimaryConstructorParameter
    Span<T> _view = view;

    T[]? _rental;

    /// <summary>Initializes a new instance of the <see cref="SmallerList{T}"/> struct.</summary>
    /// <param name="capacity">
    /// The initial allocation, which puts it on the heap immediately but can save future resizing.
    /// </param>
    public SmallerList(int capacity)
        : this(Span<T>.Empty) =>
        _view = _rental = Rent(capacity);

    /// <inheritdoc cref="Span{T}.Empty"/>
    public static SmallerList<T> Empty
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
    public SmallerList<T> Reset
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
    public SmallerList<T> Stretched
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
    public static bool operator ==(SmallerList<T> left, SmallerList<T> right) => left.View == right.View;

    /// <inheritdoc cref="Span{T}.op_Inequality"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator !=(SmallerList<T> left, SmallerList<T> right) => !(left == right);

    /// <summary>Implicitly converts the buffer into an expandable buffer.</summary>
    /// <param name="span">The span.</param>
    /// <returns>The <see cref="SmallerList{T}"/> that encapsulates the parameter <paramref name="span"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator SmallerList<T>(Span<T> span) => new(span);

    /// <inheritdoc cref="AsSpan{TRef}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static SmallerList<T> From<TRef>(ref TRef reference)
        where TRef : struct =>
        AsSpan(ref reference);

    /// <summary>Reinterprets the reference as the continuous buffer of <see cref="T"/>.</summary>
    /// <typeparam name="TRef">The generic representing the continuous buffer of <see cref="T"/>.</typeparam>
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
        if (!IsInlined)
            ArrayPool<T>.Shared.Return(TransferOwnership);
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

    /// <inheritdoc cref="IList{T}.Add"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallerList<T> Append(T item)
    {
        if (HasRoom(1))
        {
            _view[_length++] = item;
            return this;
        }

        var replacement = Rent(1);
        _view.CopyTo(replacement);
        replacement[_length++] = item;
        Swap(replacement);
        return this;
    }

    /// <inheritdoc cref="List{T}.AddRange"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallerList<T> Append(scoped ReadOnlySpan<T> collection)
    {
        if (HasRoom(collection.Length))
        {
            collection.CopyTo(_view[_length..]);
            _length += collection.Length;
            return this;
        }

        var replacement = Rent(collection.Length);
        _view.CopyTo(replacement);
        collection.CopyTo(_view[_length..]);
        _length += collection.Length;
        Swap(replacement);
        return this;
    }

    /// <inheritdoc cref="List{T}.AddRange"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallerList<T> Append([InstantHandle] IEnumerable<T> collection)
    {
        if (collection.TryGetNonEnumeratedCount(out var count))
            MakeRoom(count);

        foreach (var x in collection)
            Append(x);

        return this;
    }

    /// <inheritdoc cref="IList{T}.Add"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallerList<T> Prepend(T item)
    {
        if (HasRoom(1))
        {
            View.CopyTo(_view[1..]);
            _length++;
            _view[0] = item;
            return this;
        }

        var replacement = Rent(1);
        _view.CopyTo(replacement.AsSpan()[1..]);
        replacement[0] = item;
        _length++;
        Swap(replacement);
        return this;
    }

    /// <inheritdoc cref="List{T}.AddRange"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallerList<T> Prepend(scoped ReadOnlySpan<T> collection)
    {
        if (HasRoom(collection.Length))
        {
            View.CopyTo(_view[collection.Length..]);
            collection.CopyTo(_view);
            _length += collection.Length;
            return this;
        }

        var replacement = Rent(collection.Length);
        _view.CopyTo(replacement.AsSpan()[collection.Length..]);
        collection.CopyTo(_view);
        _length += collection.Length;
        Swap(replacement);
        return this;
    }

    /// <inheritdoc cref="List{T}.AddRange"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallerList<T> Prepend([InstantHandle] IEnumerable<T> collection) => Insert(0, collection);

    /// <inheritdoc cref="IList{T}.Insert"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallerList<T> Insert([NonNegativeValue] int offset, T item)
    {
        if (HasRoom(1))
        {
            Copy(offset, item, _view);
            return this;
        }

        var replacement = Rent(1);
        Copy(offset, item, replacement);
        Swap(replacement);
        return this;
    }

    /// <inheritdoc cref="IList{T}.Insert"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallerList<T> Insert([NonNegativeValue] int index, scoped ReadOnlySpan<T> items)
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

    /// <inheritdoc cref="List{T}.AddRange"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallerList<T> Insert([NonNegativeValue] int index, [InstantHandle] IEnumerable<T> collection)
    {
        MakeRoom(collection);

        using var e = collection.GetEnumerator();

        for (var i = index; e.MoveNext(); i++)
            Insert(i, e.Current);

        return this;
    }

    /// <inheritdoc cref="IList{T}.RemoveAt"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallerList<T> RemoveAt([NonNegativeValue] int index) => RemoveAt(index, 1);

    /// <summary>Removes the <see cref="SmallerList{T}"/> item at the specified offset and length.</summary>
    /// <param name="offset">The offset of the slice to remove.</param>
    /// <param name="length">The length of the slice to remove.</param>
    /// <returns>Itself.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallerList<T> RemoveAt([NonNegativeValue] int offset, [NonNegativeValue] int length)
    {
        View[(offset + length)..].CopyTo(View[offset..]);
        _length -= length;
        return this;
    }

    /// <inheritdoc cref="IList{T}.RemoveAt"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallerList<T> RemoveAt(Index index)
    {
        var offset = index.GetOffset(_length);
        RemoveAt(offset, 1);
        return this;
    }

    /// <summary>Removes the <see cref="SmallerList{T}"/> item at the specified range.</summary>
    /// <param name="range">The range of the slice to remove.</param>
    /// <returns>Itself.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallerList<T> RemoveAt(Range range)
    {
        var (offset, length) = range.GetOffsetAndLength(_length);
        RemoveAt(offset, length);
        return this;
    }

    /// <summary>Shrinks the collection.</summary>
    /// <param name="amount">The amount of elements to shrink.</param>
    /// <returns>Itself.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SmallerList<T> Shrink([NonNegativeValue] int amount)
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
    public readonly ref T Nth([NonNegativeValue] int i)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
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
        if (!IsInlined)
            ArrayPool<T>.Shared.Return(_rental);

        _view = _rental = replacement;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void MakeRoom([NonNegativeValue] int by)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    readonly T[] Rent([NonNegativeValue] int by)
    {
        var sum = unchecked((uint)(_view.Length + by));
        var length = unchecked((int)BitOperations.RoundUpToPowerOf2(sum));
        return ArrayPool<T>.Shared.Rent(length);
    }

    /// <summary>Validator of generics the continuous buffer of <see cref="T"/>.</summary>
    /// <typeparam name="TRef">The generic representing the continuous buffer of <see cref="T"/>.</typeparam>
    public static class Validate<TRef>
        where TRef : struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Validate() => Check(typeof(TRef));

        /// <summary>Gets the inlined length.</summary>
        [NonNegativeValue]
        public static int InlinedLength { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; } =
            Unsafe.SizeOf<TRef>() / Unsafe.SizeOf<T>();

        /// <summary>Reinterprets the reference as the continuous buffer of <see cref="T"/>.</summary>
        /// <param name="reference">The reference.</param>
        /// <returns>The span.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public static Span<T> AsSpan(ref TRef reference) =>
            MemoryMarshal.CreateSpan(ref Unsafe.As<TRef, T>(ref reference), InlinedLength);

        // ReSharper disable once SuggestBaseTypeForParameter
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool Check(Type type) =>
            !type
               .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
               .Where(x => x.FieldType != typeof(T) && x.FieldType != typeof(TRef) && Check(x.FieldType))
               .Select(Throw)
               .Any();

        [DoesNotReturn, MethodImpl(MethodImplOptions.AggressiveInlining)]
        static object Throw(FieldInfo _) =>
            throw new TypeLoadException(
                $"\"{typeof(TRef).UnfoldedName()}\" contains fields other than {typeof(T).UnfoldedName()}."
            );
    }
}
#endif
