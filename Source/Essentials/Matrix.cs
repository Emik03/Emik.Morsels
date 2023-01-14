#region Emik.MPL

// <copyright file="Matrix.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

#endregion

#if !NET20 && !NET30
namespace Emik.Morsels;

/// <summary>Maps a 1-dimensional collection as 2-dimensional.</summary>
/// <typeparam name="T">The type of item within the list.</typeparam>
sealed class Matrix<T> : IList<IList<T>>
{
    readonly int _countPerListEager;

    readonly Func<int>? _countPerListLazy;

    readonly IList<T>? _listEager;

    readonly Func<IList<T>>? _listLazy;

    /// <summary>Initializes a new instance of the <see cref="Matrix{T}"/> class.</summary>
    /// <param name="list">The list to encapsulate.</param>
    /// <param name="countPerList">The length per count.</param>
    public Matrix(IList<T> list, [NonNegativeValue] int countPerList)
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
    public Matrix(Func<IList<T>> list, [NonNegativeValue] int countPerList)
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
    public IList<T> this[[NonNegativeValue] int index]
    {
        [Pure] get => new Slice(this, index);
        set => Add(value);
    }

    /// <inheritdoc />
    public bool IsReadOnly
    {
        [Pure] get => List.IsReadOnly;
    }

    /// <inheritdoc />
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
    public int IndexOf(IList<T>? item) => item is null or { Count: 0 } ? -1 : List.IndexOf(item[0]);

    /// <inheritdoc />
    [Pure]
    public IEnumerator<IList<T>> GetEnumerator() =>
        Enumerable.Range(0, Count).Select(x => (IList<T>)new Slice(this, x)).GetEnumerator();

    /// <inheritdoc />
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Represents a slice of a matrix.</summary>
    sealed class Slice : IList<T>
    {
        [ProvidesContext]
        readonly Matrix<T> _matrix;

        [NonNegativeValue]
        readonly int _ordinal;

        /// <summary>Initializes a new instance of the <see cref="Slice"/> class.</summary>
        /// <param name="matrix">The matrix to reference.</param>
        /// <param name="ordinal">The first index of the matrix.</param>
        public Slice(Matrix<T> matrix, [NonNegativeValue] int ordinal)
        {
            _matrix = matrix;

            // Explicitly check, in case someone ignores the warning, or uses a variable.
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            _ordinal = ordinal >= 0
                ? ordinal
                : throw new ArgumentOutOfRangeException(nameof(ordinal), ordinal, "Value must be at least 0.");
        }

        /// <inheritdoc />
        public bool IsReadOnly
        {
            [Pure] get => _matrix.List.IsReadOnly;
        }

        /// <inheritdoc />
        public int Count
        {
            [Pure] get => _matrix.CountPerList;
        }

        /// <inheritdoc />
        public T this[[NonNegativeValue] int index]
        {
            [Pure] get => _matrix.List[Count * _ordinal + index];
            set => _matrix.List[Count * _ordinal + index] = value;
        }

        /// <inheritdoc />
        public void Add(T item) => _matrix.List.Add(item);

        /// <inheritdoc />
        public void Clear()
        {
            for (var i = 0; i < Count; i++)
                _matrix.List.RemoveAt(Count * _ordinal);
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, [NonNegativeValue] int arrayIndex)
        {
            for (var i = 0; i < Count; i++)
                array[arrayIndex + i] = this[i];
        }

        /// <inheritdoc />
        public void Insert([NonNegativeValue] int index, T item) => _matrix.List.Insert(Count * _ordinal + index, item);

        /// <inheritdoc />
        public void RemoveAt([NonNegativeValue] int index) => _matrix.List.RemoveAt(Count * _ordinal + index);

        /// <inheritdoc />
        [Pure]
        public bool Contains(T item) =>
            Enumerable.Range(0, Count).Any(x => EqualityComparer<T>.Default.Equals(_matrix.List[Count * _ordinal + x]));

        /// <inheritdoc />
        public bool Remove(T item) => Contains(item) && _matrix.List.Remove(item);

        /// <inheritdoc />
        [Pure, ValueRange(-1, int.MaxValue)]
        public int IndexOf(T item) => Contains(item) ? _matrix.List.IndexOf(item) - Count * _ordinal : -1;

        /// <inheritdoc />
        [Pure]
        public IEnumerator<T> GetEnumerator() => _matrix.List.Skip(Count * _ordinal).Take(Count).GetEnumerator();

        /// <inheritdoc />
        [Pure]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

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
    internal static Matrix<T>? AsMatrix<T>(this IEnumerable<T>? iterator, [NonNegativeValue] int countPerList) =>
        iterator is null
            ? null
            : new Matrix<T>(iterator as IList<T> ?? iterator.ToList(), countPerList);

    /// <summary>Wraps an <see cref="IList{T}"/> in a <see cref="Matrix{T}"/>.</summary>
    /// <typeparam name="T">The type of the <paramref name="iterator"/> and the <see langword="return"/>.</typeparam>
    /// <param name="iterator">The collection to turn into a <see cref="Matrix{T}"/>.</param>
    /// <param name="countPerList">The length per count.</param>
    /// <returns>A <see cref="Matrix{T}"/> that wraps the parameter <paramref name="iterator"/>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterator))]
    internal static Matrix<T>? AsMatrix<T>(this IEnumerable<T>? iterator, Func<int> countPerList) =>
        iterator is null
            ? null
            : new Matrix<T>(iterator as IList<T> ?? iterator.ToList(), countPerList);
}
#endif
