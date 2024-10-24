// SPDX-License-Identifier: MPL-2.0
#if !NET20 && !NET30
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods that act as factories for <see cref="Matrix{T}"/>.</summary>
static partial class MatrixFactory
{
    /// <summary>Maps a 1-dimensional collection as 2-dimensional.</summary>
    /// <typeparam name="T">The type of item within the list.</typeparam>
    // ReSharper disable once ArrangeTypeMemberModifiers
    internal sealed partial class Matrix<T> : IList<IList<T>>
    {
        /// <summary>Represents a slice of a matrix.</summary>
        /// <param name="matrix">The matrix to reference.</param>
        /// <param name="ordinal">The first index of the matrix.</param>
        sealed class Slice([ProvidesContext] Matrix<T> matrix, [NonNegativeValue] int ordinal) : IList<T>
        {
            /// <inheritdoc />
            public T this[[NonNegativeValue] int index]
            {
                [Pure] get => matrix.List[Count * ordinal + index];
                set => matrix.List[Count * ordinal + index] = value;
            }

            /// <inheritdoc />
            public bool IsReadOnly
            {
                [Pure] get => matrix.List.IsReadOnly;
            }

            /// <inheritdoc />
            public int Count
            {
                [Pure] get => matrix.CountPerList;
            }

            /// <inheritdoc />
            public void Add(T item) => matrix.List.Insert(Count * (ordinal + 1), item);

            /// <inheritdoc />
            public void Clear()
            {
                for (int i = 0, count = Count; i < count; i++)
                    matrix.List.RemoveAt(count * ordinal);
            }

            /// <inheritdoc />
            public void CopyTo(T[] array, [NonNegativeValue] int arrayIndex)
            {
                for (int i = 0, count = Count; i < count; i++)
                    array[arrayIndex + i] = matrix.List[count * ordinal + i];
            }

            /// <inheritdoc />
            public void Insert([NonNegativeValue] int index, T item)
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (Count is var count && index >= 0 && index < count)
                    matrix.List.Insert(Count * ordinal + index, item);
            }

            /// <inheritdoc />
            public void RemoveAt([NonNegativeValue] int index)
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (Count is var count && index >= 0 && index < count)
                    matrix.List.RemoveAt(Count * ordinal + index);
            }

            /// <inheritdoc />
            [Pure]
            public bool Contains(T item) => IndexOf(item) is not -1;

            /// <inheritdoc />
            public bool Remove(T item)
            {
                for (int i = 0, count = Count; i < count; i++)
                    if (count * ordinal + i is var view && EqualityComparer<T>.Default.Equals(matrix.List[view], item))
                    {
                        matrix.List.RemoveAt(view);
                        return true;
                    }

                return false;
            }

            /// <inheritdoc />
            [Pure, ValueRange(-1, int.MaxValue)]
            public int IndexOf(T item)
            {
                for (int i = 0, count = Count; i < count; i++)
                    if (EqualityComparer<T>.Default.Equals(matrix.List[count * ordinal + i], item))
                        return i;

                return -1;
            }

            /// <inheritdoc />
            [Pure]
            public IEnumerator<T> GetEnumerator()
            {
                var count = Count;
                return matrix.List.Skip(count * ordinal).Take(count).GetEnumerator();
            }

            /// <inheritdoc />
            [Pure]
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        readonly int _countPerListEager;

        readonly Func<int>? _countPerListLazy;

        readonly object? _list;

        /// <summary>Initializes a new instance of the <see cref="Matrix{T}"/> class.</summary>
        /// <param name="list">The list to encapsulate.</param>
        /// <param name="countPerList">The length per count.</param>
        public Matrix(IList<T> list, [ValueRange(1, int.MaxValue)] int countPerList)
        {
            // Explicitly check, in case someone ignores the warning, or uses a variable.
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            _countPerListEager = countPerList > 0
                ? countPerList
                : throw new ArgumentOutOfRangeException(nameof(countPerList), countPerList, "Must be at least 1.");

            _list = list;
        }

        /// <summary>Initializes a new instance of the <see cref="Matrix{T}"/> class.</summary>
        /// <param name="list">The list to encapsulate.</param>
        /// <param name="countPerList">The length per count.</param>
        public Matrix(IList<T> list, Func<int> countPerList)
        {
            _countPerListLazy = countPerList;
            _list = list;
        }

        /// <summary>Initializes a new instance of the <see cref="Matrix{T}"/> class.</summary>
        /// <param name="list">The list to encapsulate.</param>
        /// <param name="countPerList">The length per count.</param>
        public Matrix(Func<IList<T>> list, [ValueRange(1, int.MaxValue)] int countPerList)
        {
            // Explicitly check, in case someone ignores the warning, or uses a variable.
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            _countPerListEager = countPerList > 0
                ? countPerList
                : throw new ArgumentOutOfRangeException(nameof(countPerList), countPerList, "Must be at least 1.");

            _list = list;
        }

        /// <summary>Initializes a new instance of the <see cref="Matrix{T}"/> class.</summary>
        /// <param name="list">The list to encapsulate.</param>
        /// <param name="countPerList">The length per count.</param>
        public Matrix(Func<IList<T>> list, Func<int> countPerList)
        {
            _countPerListLazy = countPerList;
            _list = list;
        }

        /// <inheritdoc cref="IList{T}.this"/>
        public IList<T> this[[NonNegativeValue] int index]
        {
            [Pure] get => new Slice(this, index);
            set => Add(value);
        }
#if !WAWA
        /// <summary>Performs the index operation on the <see cref="Matrix{T}"/>.</summary>
        /// <param name="x">The <c>x</c> position, which is the list to take.</param>
        /// <param name="y">The <c>y</c> position, which is the element from the list to take.</param>
        public T this[[NonNegativeValue] int x, [NonNegativeValue] int y]
        {
            [Pure] get => List[Count * x + y];
            set => List[Count * x + y] = value;
        }
#endif
        /// <inheritdoc />
        public bool IsReadOnly
        {
            [Pure] get => List.IsReadOnly;
        }

        /// <inheritdoc cref="ICollection{T}.Count" />
        [NonNegativeValue]
        public int Count
        {
            [Pure] get => List.Count / CountPerList;
        }

        /// <summary>Gets the amount of items per list.</summary>
        public int CountPerList
        {
            [Pure] get => _countPerListLazy?.Invoke() ?? _countPerListEager;
        }

        /// <summary>Gets the encapsulated list.</summary>
        [ProvidesContext]
        public IList<T> List
        {
            [Pure]
#pragma warning disable 8603 // ReSharper disable once AssignNullToNotNullAttribute
            get => (_list as Func<IList<T>>)?.Invoke() ?? _list as IList<T>;
#pragma warning restore 8603
        }
#if !WAWA
        /// <summary>
        /// Implicitly converts the parameter by creating the new instance of <see cref="Matrix{T}"/>
        /// by using the constructor <see cref="Matrix{T}(IList{T}, int)"/>.
        /// </summary>
        /// <param name="tuple">The parameter to pass onto the constructor.</param>
        /// <returns>
        /// The new instance of Matrix{T} by passing the parameter <paramref name="tuple"/>
        /// to the constructor <see cref="Matrix{T}(IList{T}, int)"/>.
        /// </returns>
        [Pure]
        public static implicit operator Matrix<T>((IList<T> List, int CountPerList) tuple) =>
            new(tuple.List, tuple.CountPerList);

        /// <summary>
        /// Implicitly converts the parameter by creating the new instance of <see cref="Matrix{T}"/>
        /// by using the constructor <see cref="Matrix{T}(IList{T}, Func{int})"/>.
        /// </summary>
        /// <param name="tuple">The parameter to pass onto the constructor.</param>
        /// <returns>
        /// The new instance of Matrix{T} by passing the parameter <paramref name="tuple"/>
        /// to the constructor <see cref="Matrix{T}(IList{T}, Func{int})"/>.
        /// </returns>
        [Pure]
        public static implicit operator Matrix<T>((IList<T> List, Func<int> CountPerList) tuple) =>
            new(tuple.List, tuple.CountPerList);

        /// <summary>
        /// Implicitly converts the parameter by creating the new instance of <see cref="Matrix{T}"/>
        /// by using the constructor <see cref="Matrix{T}(Func{IList{T}}, int)"/>.
        /// </summary>
        /// <param name="tuple">The parameter to pass onto the constructor.</param>
        /// <returns>
        /// The new instance of <see cref="Matrix{T}"/> by passing the parameter <paramref name="tuple"/>
        /// to the constructor <see cref="Matrix{T}(Func{IList{T}}, int)"/>.
        /// </returns>
        [Pure]
        public static implicit operator Matrix<T>((Func<IList<T>> List, int CountPerList) tuple) =>
            new(tuple.List, tuple.CountPerList);

        /// <summary>
        /// Implicitly converts the parameter by creating the new instance of <see cref="Matrix{T}"/>
        /// by using the constructor <see cref="Matrix{T}(Func{IList{T}}, Func{int})"/>.
        /// </summary>
        /// <param name="tuple">The parameter to pass onto the constructor.</param>
        /// <returns>
        /// The new instance of <see cref="Matrix{T}"/> by passing the parameter <paramref name="tuple"/>
        /// to the constructor <see cref="Matrix{T}(Func{IList{T}}, Func{int})"/>.
        /// </returns>
        [Pure]
        public static implicit operator Matrix<T>((Func<IList<T>> List, Func<int> CountPerList) tuple) =>
            new(tuple.List, tuple.CountPerList);
#endif
        /// <inheritdoc />
        public void Add(IList<T>? item)
        {
            if (item is null)
                return;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0, count = item.Count; i < count; i++)
                List.Add(item[i]);
        }

        /// <inheritdoc />
        public void Clear() => List.Clear();

        /// <inheritdoc />
        [Pure]
        public bool Contains(IList<T>? item) => IndexOf(item) is not -1;

        /// <inheritdoc />
        public void CopyTo(IList<T>[] array, [NonNegativeValue] int arrayIndex)
        {
            for (var i = 0; i < Count; i++)
                array[arrayIndex + i] = this[i];
        }

        /// <inheritdoc />
        public void Insert([NonNegativeValue] int index, IList<T>? item)
        {
            if (item is not null)
                this[index] = item;
        }

        /// <inheritdoc />
        public void RemoveAt([NonNegativeValue] int index) => this[index].Clear();

        /// <inheritdoc />
        public bool Remove(IList<T>? item)
        {
            if (item is null)
                return false;

            for (int i = 0, count = Count; i < count; i++)
                if (this[i].SequenceEqual(item))
                {
                    RemoveAt(i);
                    return true;
                }

            return false;
        }

        /// <inheritdoc />
        [Pure, ValueRange(-1, int.MaxValue)]
        public int IndexOf(IList<T>? item)
        {
            if (item is null)
                return -1;

            for (int i = 0, count = Count; i < count; i++)
                if (this[i].SequenceEqual(item))
                    return i;

            return -1;
        }

        /// <inheritdoc />
        [Pure]
        public IEnumerator<IList<T>> GetEnumerator() =>
            Enumerable.Range(0, Count).Select(IList<T> (x) => new Slice(this, x)).GetEnumerator();

        /// <inheritdoc />
        [Pure]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
#endif
#if !WAWA
    /// <summary>Wraps an <see cref="IList{T}"/> in a <see cref="Matrix{T}"/>.</summary>
    /// <typeparam name="T">The type of the <paramref name="iterator"/> and the <see langword="return"/>.</typeparam>
    /// <param name="iterator">The collection to turn into a <see cref="Matrix{T}"/>.</param>
    /// <param name="countPerList">The length per count.</param>
    /// <returns>A <see cref="Matrix{T}"/> that wraps the parameter <paramref name="iterator"/>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterator))]
    public static Matrix<T>? AsMatrix<T>(this IEnumerable<T>? iterator, [NonNegativeValue] int countPerList) =>
        iterator is null ? null : new(iterator.ToIList(), countPerList);
#endif
    /// <summary>Wraps an <see cref="IList{T}"/> in a <see cref="Matrix{T}"/>.</summary>
    /// <typeparam name="T">The type of the <paramref name="iterator"/> and the <see langword="return"/>.</typeparam>
    /// <param name="iterator">The collection to turn into a <see cref="Matrix{T}"/>.</param>
    /// <param name="countPerList">The length per count.</param>
    /// <returns>A <see cref="Matrix{T}"/> that wraps the parameter <paramref name="iterator"/>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterator))]
    public static Matrix<T>? AsMatrix<T>(
        this
#if WAWA
            IList<T>?
#else
            IEnumerable<T>?
#endif
            iterator,
        Func<int> countPerList
    ) =>
#if WAWA
        iterator is null ? null : new(iterator, countPerList);
#else
        iterator is null ? null : new(iterator.ToIList(), countPerList);
#endif
}
