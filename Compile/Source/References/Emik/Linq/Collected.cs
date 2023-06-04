// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods for iterating over a set of elements, or for generating new ones.</summary>
static partial class Collected
{
    /// <summary>Returns a fallback enumeration if the collection given is null or empty.</summary>
    /// <typeparam name="T">The type of item within the enumeration.</typeparam>
    /// <param name="iterable">The potentially empty collection.</param>
    /// <param name="fallback">The fallback value.</param>
    /// <returns>
    /// The parameter <paramref name="iterable"/> when non-empty, otherwise; <paramref name="fallback"/>.
    /// </returns>
    [LinqTunnel, Pure]
    public static IEnumerable<T> DefaultIfEmpty<T>(this IEnumerable<T>? iterable, IEnumerable<T> fallback)
    {
        using var a = iterable?.GetEnumerator();

        if (a?.MoveNext() ?? false)
            do
                yield return a.Current;
            while (a.MoveNext());
        else
            foreach (var b in fallback)
                yield return b;
    }

    /// <summary>Upcasts or creates an <see cref="ICollection{T}"/>.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to upcast or encapsulate.</param>
    /// <returns>Itself as <see cref="ICollection{T}"/>, or collected.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterable))]
    public static ICollection<T>? ToCollectionLazily<T>([InstantHandle] this IEnumerable<T>? iterable) =>
        iterable is null
            ? null
            : iterable as ICollection<T> ??
            (iterable.TryGetNonEnumeratedCount(out var count)
                ? new Collection<T>(iterable, count)
                : new List<T>(iterable));

    /// <summary>Upcasts or creates an <see cref="IList{T}"/>.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to upcast or encapsulate.</param>
    /// <returns>Itself as <see cref="IList{T}"/>, or collected.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterable))]
    public static IList<T>? ToListLazily<T>([InstantHandle] this IEnumerable<T>? iterable) =>
        iterable is null ? null : iterable as IList<T> ?? new List<T>(iterable);

    /// <summary>Upcasts or creates an <see cref="ISet{T}"/>.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to upcast or encapsulate.</param>
    /// <returns>Itself as <see cref="IList{T}"/>, or collected.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterable))]
    public static ISet<T>? ToSetLazily<T>([InstantHandle] this IEnumerable<T>? iterable) =>
        iterable is null ? null : iterable as ISet<T> ?? new HashSet<T>(iterable);

    /// <summary>Upcasts or creates an <see cref="ISet{T}"/>.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to upcast or encapsulate.</param>
    /// <param name="comparer">The comparer to use if one needs to be generated.</param>
    /// <returns>Itself as <see cref="ISet{T}"/>, or collected.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterable))]
    public static ISet<T>? ToSetLazily<T>(
        [InstantHandle] this IEnumerable<T>? iterable,
        IEqualityComparer<T> comparer
    ) =>
        iterable is null ? null : iterable as ISet<T> ?? new HashSet<T>(iterable, comparer);

    /// <summary>Attempts to create a list from an <see cref="IEnumerable{T}"/>.</summary>
    /// <typeparam name="T">The type of item in the <see cref="IEnumerable{T}"/>.</typeparam>
    /// <typeparam name="TList">The destination type.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to convert.</param>
    /// <param name="converter">The <see cref="IList{T}"/> to convert it to.</param>
    /// <returns>
    /// A <typeparamref name="TList"/> from <paramref name="converter"/>, as long as every element returned
    /// is not <paramref langword="null"/>, otherwise <paramref langword="default"/>.
    /// </returns>
    [MustUseReturnValue]
    public static TList? Collect<T, TList>(
        [InstantHandle] this IEnumerable<T?> iterable,
        [InstantHandle] Converter<IEnumerable<T>, TList> converter
    )
        where TList : IList<T> => // ReSharper disable once NullableWarningSuppressionIsUsed
#pragma warning disable CS8620 // Checked later, technically could cause problems, but most factory methods are fine.
        (TList?)converter(iterable);
#pragma warning restore CS8620

    /// <summary>Provides a wrapper to an <see cref="IEnumerable{T}"/> with a known count.</summary>
    /// <typeparam name="T">The type of element in the <see cref="IEnumerable{T}"/>.</typeparam>
    sealed class Collection<T> : ICollection, ICollection<T>, IReadOnlyCollection<T>
    {
        [ProvidesContext]
        readonly IEnumerable<T> _enumerable;

        /// <summary>Initializes a new instance of the <see cref="Collection{T}"/> class.</summary>
        /// <param name="enumerable">The enumerable to encapsulate.</param>
        /// <param name="count">The pre-computed count.</param>
        public Collection(IEnumerable<T> enumerable, [NonNegativeValue] int count)
        {
            _enumerable = enumerable;
            Count = count;
        }

        /// <inheritdoc />
        [Pure]
        public bool IsSynchronized => true;

        /// <inheritdoc cref="ICollection{T}.Count" />
        [NonNegativeValue, Pure]
        public int Count { get; }

        /// <inheritdoc />
        [Pure]
        public object SyncRoot => _enumerable;

        /// <inheritdoc />
        public void CopyTo(Array array, [NonNegativeValue] int index)
        {
            var i = 0;

            foreach (var next in _enumerable)
            {
                array.SetValue(next, index);
                _ = checked(i++);
            }
        }

        /// <inheritdoc />
        [Pure]
        public IEnumerator GetEnumerator() => _enumerable.GetEnumerator();

        /// <inheritdoc />
        [Pure]
        public bool IsReadOnly => true;

        /// <inheritdoc />
        public void Add(T item) { }

        /// <inheritdoc />
        public void Clear() { }

        /// <inheritdoc />
        public void CopyTo(T[] array, [NonNegativeValue] int arrayIndex)
        {
            var i = 0;

            foreach (var next in _enumerable)
            {
                array[arrayIndex] = next;
                _ = checked(i++);
            }
        }

        /// <inheritdoc />
        [Pure]
        public bool Contains(T item)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var next in _enumerable)
                if (EqualityComparer<T>.Default.Equals(next, item))
                    return true;

            return false;
        }

        /// <inheritdoc />
        [Pure]
        public bool Remove(T item) => false;

        /// <inheritdoc />
        [Pure]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => _enumerable.GetEnumerator();
    }
}
