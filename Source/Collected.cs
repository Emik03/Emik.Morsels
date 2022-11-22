// <copyright file="Iterators.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>
namespace Emik.Morsels;

/// <summary>Extension methods for iterating over a set of elements, or for generating new ones.</summary>
static class Iterators
{
    /// <summary>Upcasts or creates an <see cref="ICollection{T}"/>.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to upcast or encapsulate.</param>
    /// <returns>Itself as <see cref="ICollection{T}"/>, or a collected <see cref="Array"/>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterable))]
    internal static ICollection<T>? ToCollectionLazily<T>([InstantHandle] this IEnumerable<T>? iterable) =>
        iterable is null ? null : iterable as ICollection<T> ?? iterable.ToList();

    /// <summary>Upcasts or creates an <see cref="IList{T}"/>.</summary>
    /// <typeparam name="T">The item in the collection.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to upcast or encapsulate.</param>
    /// <returns>Itself as <see cref="IList{T}"/>, or a collected <see cref="Array"/>.</returns>
    [Pure]
    [return: NotNullIfNotNull(nameof(iterable))]
    internal static IList<T>? ToListLazily<T>([InstantHandle] this IEnumerable<T>? iterable) =>
        iterable is null ? null : iterable as IList<T> ?? iterable.ToList();

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
    internal static TList? Collect<T, TList>(
        [InstantHandle] this IEnumerable<T?> iterable,
        [InstantHandle] Converter<IEnumerable<T>, TList> converter
    )
        where TList : IList<T> => // ReSharper disable once NullableWarningSuppressionIsUsed
#pragma warning disable CS8620 // Checked later, technically could cause problems, but most factory methods are fine.
        (TList?)converter(iterable).ItemNotNull();
#pragma warning restore CS8620
}
