// <copyright file="NegatedEnumerable.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>
namespace Emik.Morsels;

/// <summary>Extension methods that negate functions from <see cref="Enumerable"/>.</summary>
static class NegatedEnumerable
{
    /// <summary>Negated <see cref="Enumerable.TakeWhile{T}(IEnumerable{T}, Func{T, int, bool})"/>.</summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}" /> that contains elements from
    /// the input sequence that do not satisfy the condition.
    /// </returns>
    /// <inheritdoc cref="Enumerable.TakeWhile{T}(IEnumerable{T}, Func{T, int, bool})"/>
    [LinqTunnel, MustUseReturnValue]
    internal static IEnumerable<T> TakeUntil<T>(
        [InstantHandle] this IEnumerable<T> source,
        [InstantHandle] Func<T, bool> predicate
    ) =>
        source.TakeWhile(Not1(predicate));

    /// <summary>Negated <see cref="Enumerable.TakeWhile{T}(IEnumerable{T}, Func{T, int, bool})"/>.</summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}" /> that contains elements from
    /// the input sequence that do not satisfy the condition.
    /// </returns>
    /// <inheritdoc cref="Enumerable.TakeWhile{T}(IEnumerable{T}, Func{T, int, bool})"/>
    [LinqTunnel, MustUseReturnValue]
    internal static IEnumerable<T> TakeUntil<T>(
        [InstantHandle] this IEnumerable<T> source,
        [InstantHandle] Func<T, int, bool> predicate
    ) =>
        source.TakeWhile(Not2(predicate));

    /// <summary>Negated <see cref="Enumerable.Where{T}(IEnumerable{T}, Func{T, bool})"/>.</summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}" /> that contains elements from
    /// the input sequence that do not satisfy the condition.
    /// </returns>
    /// <inheritdoc cref="Enumerable.Where{T}(IEnumerable{T}, Func{T, bool})"/>
    [LinqTunnel, MustUseReturnValue]
    internal static IEnumerable<T> WhereNot<T>(
        [InstantHandle] this IEnumerable<T> source,
        [InstantHandle] Func<T, bool> predicate
    ) =>
        source.Where(Not1(predicate));

    /// <summary>Negated <see cref="Enumerable.Where{T}(IEnumerable{T}, Func{T, int, bool})"/>.</summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}" /> that contains elements from
    /// the input sequence that do not satisfy the condition.
    /// </returns>
    /// <inheritdoc cref="Enumerable.Where{T}(IEnumerable{T}, Func{T, int, bool})"/>
    [LinqTunnel, MustUseReturnValue]
    internal static IEnumerable<T> WhereNot<T>(
        [InstantHandle] this IEnumerable<T> source,
        [InstantHandle] Func<T, int, bool> predicate
    ) =>
        source.Where(Not2(predicate));
}
