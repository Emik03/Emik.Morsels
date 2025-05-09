﻿// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods for iterating over a set of elements, or for generating new ones.</summary>
static partial class Collected
{
#if !NETFRAMEWORK || NET35_OR_GREATER
    /// <summary>Upcasts or creates an <see cref="IList{T}"/>.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to upcast or encapsulate.</param>
    /// <returns>Itself as <see cref="IList{T}"/>, or collected.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterable))]
    public static T[]? ToArrayLazily<T>([InstantHandle] this IEnumerable<T>? iterable) =>
        iterable is null ? null : iterable as T[] ?? [..iterable];
#endif
    /// <summary>Wraps the <see cref="IEnumerable{T}"/> in a known-size collection type.</summary>
    /// <remarks><para>The parameter <paramref name="count"/> is assumed to be correct.</para></remarks>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to encapsulate.</param>
    /// <param name="count">The number of elements in the parameter <paramref name="iterable"/>.</param>
    /// <returns>The parameter <paramref name="iterable"/> as a <see cref="Collection{T}"/>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterable))]
    public static Collection<T>? WithCount<T>(this IEnumerable<T>? iterable, [NonNegativeValue] int count) =>
        iterable is null ? null : new(iterable, count);

    /// <summary>Upcasts or creates an <see cref="ICollection{T}"/>.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to upcast or encapsulate.</param>
    /// <returns>Itself as <see cref="ICollection{T}"/>, or collected.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterable))]
    public static ICollection<T>? ToICollection<T>([InstantHandle] this IEnumerable<T>? iterable) =>
        iterable is null
            ? null
            : iterable as ICollection<T> ??
            (iterable.TryCount() is { } count
                ? new Collection<T>(iterable, count)
#if NETFRAMEWORK && NET40_OR_GREATER
                : new List<T>(iterable));
#else
                : iterable.ToIList());
#endif
    /// <summary>Returns a fallback enumeration if the collection given is null or empty.</summary>
    /// <typeparam name="T">The type of item within the enumeration.</typeparam>
    /// <param name="iterable">The potentially empty collection.</param>
    /// <param name="fallback">The fallback value.</param>
    /// <returns>
    /// The parameter <paramref name="iterable"/> when non-empty, otherwise; <paramref name="fallback"/>.
    /// </returns>
    [Pure]
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

    /// <summary>Appends one element and returns the list.</summary>
    /// <typeparam name="T">The type of the list and element.</typeparam>
    /// <param name="list">The list to append to.</param>
    /// <param name="item">The item to append with.</param>
    /// <returns>The parameter <paramref name="list"/>.</returns>
    public static List<T> AndAdd<T>(this List<T> list, [CanBeNull] T item)
    {
        list.Add(item);
        return list;
    }

    /// <summary>Upcasts or creates an <see cref="IList{T}"/>.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to upcast or encapsulate.</param>
    /// <returns>Itself as <see cref="IList{T}"/>, or collected.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterable))]
    public static IList<T>? ToIList<T>([InstantHandle] this IEnumerable<T>? iterable) =>
#if !NET40_OR_GREATER && NETFRAMEWORK
        iterable is null ? null : iterable as IList<T> ?? new List<T>(iterable);
#else
        iterable is null ? null : iterable as IList<T> ?? [..iterable];
#endif
#if !NETFRAMEWORK || NET40_OR_GREATER
    /// <summary>Creates a <see cref="HashSet{T}"/>.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to encapsulate.</param>
    /// <param name="comparer">The comparer to use.</param>
    /// <returns>Itself as <see cref="ISet{T}"/>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterable))]
    public static HashSet<T>? ToSet<T>(
        [InstantHandle] this IEnumerable<T>? iterable,
        IEqualityComparer<T>? comparer = null
    ) =>
        iterable is null ? null : new HashSet<T>(iterable, comparer);

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
#endif
    /// <summary>Provides a wrapper to an <see cref="IEnumerable{T}"/> with a known count.</summary>
    /// <param name="enumerable">The enumerable to encapsulate.</param>
    /// <param name="count">The pre-computed count.</param>
    /// <typeparam name="T">The type of element in the <see cref="IEnumerable{T}"/>.</typeparam>
    internal sealed class Collection<T>([ProvidesContext] IEnumerable<T> enumerable, [NonNegativeValue] int count) :
        ICollection,
        ICollection<T>,
        IReadOnlyCollection<T>
    {
        /// <inheritdoc />
        [Pure]
        bool ICollection.IsSynchronized => true;

        /// <inheritdoc />
        [Pure]
        bool ICollection<T>.IsReadOnly => true;

        /// <inheritdoc cref="ICollection{T}.Count" />
        [NonNegativeValue, Pure]
        public int Count => count;

        /// <inheritdoc />
        [Pure]
        public object SyncRoot => enumerable;

        /// <inheritdoc />
        public void CopyTo(Array array, [NonNegativeValue] int index)
        {
            var i = index;

            foreach (var next in enumerable)
            {
                array.SetValue(next, i);
                _ = checked(i++);
            }
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, [NonNegativeValue] int arrayIndex)
        {
            var i = arrayIndex;

            foreach (var next in enumerable)
            {
                array[i] = next;
                _ = checked(i++);
            }
        }

        /// <inheritdoc />
        void ICollection<T>.Add(T? item) { }

        /// <inheritdoc />
        void ICollection<T>.Clear() { }

        /// <inheritdoc />
        [Pure]
        public bool Contains(T item) => enumerable.Contains(item);

        /// <inheritdoc />
        [Pure]
        bool ICollection<T>.Remove(T? item) => false;

        /// <inheritdoc />
        [Pure]
        IEnumerator IEnumerable.GetEnumerator() => enumerable.GetEnumerator();

        /// <inheritdoc />
        [Pure]
        public IEnumerator<T> GetEnumerator() => enumerable.GetEnumerator();
    }
}
