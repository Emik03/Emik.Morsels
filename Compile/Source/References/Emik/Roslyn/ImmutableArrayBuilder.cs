// SPDX-License-Identifier: MPL-2.0
#if ROSLYN
// ReSharper disable NullableWarningSuppressionIsUsed
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>A helper type to build sequences of values with pooled buffers.</summary>
/// <typeparam name="T">The type of items to create sequences for.</typeparam>
[StructLayout(LayoutKind.Auto)]
ref partial struct ImmutableArrayBuilder<T>
{
    /// <summary>The rented <see cref="Writer"/> instance to use.</summary>
    Writer? _writer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImmutableArrayBuilder{T}"/> struct with the specified parameters.
    /// </summary>
    /// <param name="writer">The target data writer to use.</param>
    ImmutableArrayBuilder(Writer writer) => _writer = writer;

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Count"/>
    public readonly int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _writer!.Count;
    }

    /// <summary>Gets the data written to the underlying buffer so far, as a <see cref="ReadOnlySpan{T}"/>.</summary>
    [UnscopedRef]
    public readonly ReadOnlySpan<T> WrittenSpan
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _writer!.WrittenSpan;
    }

    /// <summary>Creates a <see cref="ImmutableArrayBuilder{T}"/> value with a pooled underlying data writer.</summary>
    /// <returns>A <see cref="ImmutableArrayBuilder{T}"/> instance to write data to.</returns>
    public static ImmutableArrayBuilder<T> Rent() => new(new());

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Add(T)"/>
    public readonly void Add(T item) => _writer!.Add(item);

    /// <summary>Adds the specified items to the end of the array.</summary>
    /// <param name="items">The items to add at the end of the array.</param>
    public readonly void AddRange(scoped ReadOnlySpan<T> items) => _writer!.AddRange(items);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.ToImmutable"/>
    // ReSharper disable once ArrangeStaticMemberQualifier
    public readonly ImmutableArray<T> ToImmutable() => ImmutableCollectionsMarshal.AsImmutableArray(ToArray());

    /// <inheritdoc cref="ImmutableArray{T}.Builder.ToArray"/>
    public readonly T[] ToArray() => WrittenSpan.ToArray();

    /// <summary>Gets an <see cref="IEnumerable{T}"/> instance for the current builder.</summary>
    /// <remarks><para>The builder should not be mutated while an enumerator is in use.</para></remarks>
    /// <returns>An <see cref="IEnumerable{T}"/> instance for the current builder.</returns>
    public readonly IEnumerable<T> AsEnumerable() => _writer!;

    /// <inheritdoc/>
    public readonly override string ToString() => WrittenSpan.ToString();

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        var writer = _writer;
        _writer = null;
        writer?.Dispose();
    }

    /// <summary>A class handling the actual buffer writing.</summary>
    sealed class Writer : ICollection<T>, IDisposable
    {
        /// <summary>The underlying <typeparamref name="T"/> array.</summary>
        T?[]? _array = ArrayPool<T?>.Shared.Rent(typeof(T) == typeof(char) ? 1024 : 8);

        /// <inheritdoc/>
        bool ICollection<T>.IsReadOnly => true;

        /// <inheritdoc cref="ImmutableArrayBuilder{T}.Count"/>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] private set;
        }

        /// <inheritdoc cref="ImmutableArrayBuilder{T}.WrittenSpan"/>
        public ReadOnlySpan<T> WrittenSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new(_array!, 0, Count);
        }

        /// <inheritdoc cref="ImmutableArrayBuilder{T}.Add"/>
        public void Add(T item)
        {
            EnsureCapacity(1);
            _array![Count++] = item;
        }

        /// <inheritdoc cref="ImmutableArrayBuilder{T}.AddRange"/>
        public void AddRange(ReadOnlySpan<T> items)
        {
            EnsureCapacity(items.Length);
            items.CopyTo(_array.AsSpan(Count)!);
            Count += items.Length;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            var array = _array;
            _array = null;

            if (array is not null)
                ArrayPool<T?>.Shared.Return(array, typeof(T) != typeof(char));
        }

        /// <inheritdoc/>
        void ICollection<T>.Clear() => throw new NotSupportedException();

        /// <inheritdoc/>
        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => Array.Copy(_array!, 0, array, arrayIndex, Count);

        /// <summary>
        /// Ensures that <see cref="_array"/> has enough free space to contain a given number of new items.
        /// </summary>
        /// <param name="requestedSize">The minimum number of items to ensure space for in <see cref="_array"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void EnsureCapacity(int requestedSize)
        {
            if (requestedSize > _array!.Length - Count)
                ResizeBuffer(requestedSize);
        }

        /// <summary>Resizes <see cref="_array"/> to ensure it can fit the specified number of new items.</summary>
        /// <param name="sizeHint">The minimum number of items to ensure space for in <see cref="_array"/>.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        void ResizeBuffer(int sizeHint)
        {
            var minimumSize = Count + sizeHint;
            var oldArray = _array!;
            var newArray = ArrayPool<T?>.Shared.Rent(minimumSize);

            Array.Copy(oldArray, newArray, Count);
            _array = newArray;
            ArrayPool<T?>.Shared.Return(oldArray, typeof(T) != typeof(char));
        }

        /// <inheritdoc/>
        bool ICollection<T>.Contains(T item) => throw new NotSupportedException();

        /// <inheritdoc/>
        bool ICollection<T>.Remove(T item) => throw new NotSupportedException();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();

        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            var array = _array!;
            var length = Count;

            for (var i = 0; i < length; i++)
                yield return array[i]!;
        }
    }
}
#endif
