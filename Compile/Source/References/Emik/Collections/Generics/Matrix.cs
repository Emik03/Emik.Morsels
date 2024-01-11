// SPDX-License-Identifier: MPL-2.0
#if !NET20 && !NET30
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods that act as factories for <see cref="Matrix{T}"/>.</summary>
#pragma warning disable MA0048
static partial class MatrixFactory
#pragma warning restore MA0048
{
    /// <summary>Wraps an <see cref="IList{T}"/> in a <see cref="Matrix{T}"/>.</summary>
    /// <typeparam name="T">The type of the <paramref name="iterator"/> and the <see langword="return"/>.</typeparam>
    /// <param name="iterator">The collection to turn into a <see cref="Matrix{T}"/>.</param>
    /// <param name="countPerList">The length per count.</param>
    /// <returns>A <see cref="Matrix{T}"/> that wraps the parameter <paramref name="iterator"/>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterator))]
    public static Matrix<T>? AsMatrix<T>(this IEnumerable<T>? iterator, [NonNegativeValue] int countPerList) =>
#if WAWA
        iterator is null ? null : new(iterator as IList<T> ?? [..iterator], countPerList);
#else
        iterator is null ? null : new(iterator.ToListLazily(), countPerList);
#endif

    /// <summary>Wraps an <see cref="IList{T}"/> in a <see cref="Matrix{T}"/>.</summary>
    /// <typeparam name="T">The type of the <paramref name="iterator"/> and the <see langword="return"/>.</typeparam>
    /// <param name="iterator">The collection to turn into a <see cref="Matrix{T}"/>.</param>
    /// <param name="countPerList">The length per count.</param>
    /// <returns>A <see cref="Matrix{T}"/> that wraps the parameter <paramref name="iterator"/>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterator))]
    public static Matrix<T>? AsMatrix<T>(this IEnumerable<T>? iterator, Func<int> countPerList) =>
#if WAWA
        iterator is null ? null : new(iterator as IList<T> ?? [..iterator], countPerList);
#else
        iterator is null ? null : new(iterator.ToListLazily(), countPerList);
#endif
}

/// <summary>Maps a 1-dimensional collection as 2-dimensional.</summary>
/// <typeparam name="T">The type of item within the list.</typeparam>
#if !WAWA
[NoStructuralTyping]
#endif
sealed partial class Matrix<T> : IList<IList<T>>
{
    /// <summary>Represents a slice of a matrix.</summary>
    /// <param name="matrix">The matrix to reference.</param>
    /// <param name="ordinal">The first index of the matrix.</param>
#pragma warning disable IDE0044
    sealed class Slice([ProvidesContext] Matrix<T> matrix, [NonNegativeValue] int ordinal) : IList<T>
#pragma warning restore IDE0044
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
        public void Add(T item) => matrix.List.Add(item);

        /// <inheritdoc />
        public void Clear()
        {
            for (var i = 0; i < Count; i++)
                matrix.List.RemoveAt(Count * ordinal);
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, [NonNegativeValue] int arrayIndex)
        {
            for (var i = 0; i < Count; i++)
                array[arrayIndex + i] = this[i];
        }

        /// <inheritdoc />
        public void Insert([NonNegativeValue] int index, T item) => matrix.List.Insert(Count * ordinal + index, item);

        /// <inheritdoc />
        public void RemoveAt([NonNegativeValue] int index) => matrix.List.RemoveAt(Count * ordinal + index);

        /// <inheritdoc />
        [Pure]
        public bool Contains(T item) =>
            Enumerable
               .Range(0, Count)
               .Any(x => EqualityComparer<T>.Default.Equals(matrix.List[Count * ordinal + x], item));

        /// <inheritdoc />
        public bool Remove(T item) => Contains(item) && matrix.List.Remove(item);

        /// <inheritdoc />
        [Pure, ValueRange(-1, int.MaxValue)]
        public int IndexOf(T item) => Contains(item) ? matrix.List.IndexOf(item) - Count * ordinal : -1;

        /// <inheritdoc />
        [Pure]
        public IEnumerator<T> GetEnumerator() => matrix.List.Skip(Count * ordinal).Take(Count).GetEnumerator();

        /// <inheritdoc />
        [Pure]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    readonly int _countPerListEager;

    readonly Func<int>? _countPerListLazy;

    readonly IList<T>? _listEager;

    readonly Func<IList<T>>? _listLazy;

    /// <summary>Initializes a new instance of the <see cref="Matrix{T}"/> class.</summary>
    /// <param name="list">The list to encapsulate.</param>
    /// <param name="countPerList">The length per count.</param>
    public Matrix(IList<T> list, [ValueRange(1, int.MaxValue)] int countPerList)
    {
        // Explicitly check, in case someone ignores the warning, or uses a variable.
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        _countPerListEager = countPerList > 0
            ? countPerList
            : throw new ArgumentOutOfRangeException(nameof(countPerList), countPerList, "Value must be at least 1.");

        _listEager = list;
    }

    /// <summary>Initializes a new instance of the <see cref="Matrix{T}"/> class.</summary>
    /// <param name="list">The list to encapsulate.</param>
    /// <param name="countPerList">The length per count.</param>
    public Matrix(IList<T> list, Func<int> countPerList)
    {
        _countPerListLazy = countPerList;
        _listEager = list;
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
            : throw new ArgumentOutOfRangeException(nameof(countPerList), countPerList, "Value must be at least 1.");

        _listLazy = list;
    }

    /// <summary>Initializes a new instance of the <see cref="Matrix{T}"/> class.</summary>
    /// <param name="list">The list to encapsulate.</param>
    /// <param name="countPerList">The length per count.</param>
    public Matrix(Func<IList<T>> list, Func<int> countPerList)
    {
        _countPerListLazy = countPerList;
        _listLazy = list;
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

    /// <inheritdoc cref="IList{T}.this"/>
    public IList<T> this[[NonNegativeValue] int index]
    {
        [Pure] get => new Slice(this, index);
        set => Add(value);
    }

    /// <summary>Gets the amount of items per list.</summary>
    public int CountPerList
    {
        [Pure] get => _countPerListLazy?.Invoke() ?? _countPerListEager;
    }

    /// <summary>Gets the encapsulated list.</summary>
    [ProvidesContext]
#pragma warning disable CS8603 // Unreachable.
    public IList<T> List
    {
        [Pure] // ReSharper disable once AssignNullToNotNullAttribute
        get => _listLazy?.Invoke() ?? _listEager;
    }
#pragma warning restore CS8603

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

    /// <inheritdoc />
    public void Add(IList<T>? item) =>
        item?.ToList()
#pragma warning disable SA1110
#if NETSTANDARD && !NETSTANDARD1_3_OR_GREATER
           .For
#else
           .ForEach
#endif
                (List.Add);
#pragma warning restore SA1110

    /// <inheritdoc />
    public void Clear() => List.Clear();

    /// <inheritdoc />
    [Pure]
    public bool Contains(IList<T>? item) => item?.All(List.Contains) ?? false;

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
    public bool Remove(IList<T>? item) => item?.Select(List.Remove).Any() ?? false;

    /// <inheritdoc />
    [Pure, ValueRange(-1, int.MaxValue)]
    public int IndexOf(IList<T>? item) => item?.Count > 0 ? List.IndexOf(item[0]) : -1;

    /// <inheritdoc />
    [Pure]
    public IEnumerator<IList<T>> GetEnumerator() =>
        Enumerable.Range(0, Count).Select(x => (IList<T>)new Slice(this, x)).GetEnumerator();

    /// <inheritdoc />
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
#endif
