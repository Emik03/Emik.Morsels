// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides methods to create and operate on <see cref="AggregateException"/> instances.</summary>
static partial class AggregateExceptionFactory
{
    /// <summary>Asserts that the <paramref name="exception"/> is <see langword="null"/>.</summary>
    /// <param name="exception">The <see cref="AggregateException"/> to assert.</param>
    /// <exception cref="AggregateException">The <paramref name="exception"/> is not <see langword="null"/>.</exception>
    public static void ExpectNull(this AggregateException? exception)
    {
        if (exception is not null)
            throw exception;
    }

    /// <summary>Asserts that the <paramref name="exceptions"/> are empty.</summary>
    /// <param name="exceptions">The collection of <see cref="Exception"/> instances.</param>
    /// <param name="message">The message of the <see cref="AggregateException"/> instance.</param>
    /// <exception cref="AggregateException">The <paramref name="exceptions"/> are non-empty.</exception>
    public static void ThrowAny(this ICollection<Exception> exceptions, string? message = null) =>
        ExpectNull(Aggregate(exceptions));

    /// <summary>
    /// Creates an <see cref="AggregateException"/> from a collection of <see cref="Exception"/> instances.
    /// </summary>
    /// <param name="exceptions">The collection of <see cref="Exception"/> instances.</param>
    /// <param name="message">The message of the <see cref="AggregateException"/> instance.</param>
    /// <returns>
    /// The <see cref="AggregateException"/> instance from the parameter
    /// <paramref name="exceptions"/> if it is not empty; otherwise, <see langword="null"/>.
    /// </returns>
    [Pure]
    public static AggregateException? Aggregate(this ICollection<Exception> exceptions, string? message = null) =>
        exceptions.Count is 0 ? null : new(message, exceptions);
}