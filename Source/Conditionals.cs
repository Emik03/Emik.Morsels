// <copyright file="Conditionals.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>
namespace Emik.Morsels;

/// <summary>Extension methods for nullable types and booleans.</summary>
static class Conditionals
{
    /// <summary>Determines whether the inner value of a nullable value matches a given predicate.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="predicate">The predicate to determine the return value.</param>
    /// <returns>
    /// The value <see langword="true"/> if <paramref name="value"/> is not <see langword="null"/>
    /// and returned <see langword="true"/> from the predicate, otherwise <see langword="false"/>.
    /// </returns>
    [MustUseReturnValue]
    internal static bool IsAnd<T>([NotNullWhen(true)] this T? value, [InstantHandle] Predicate<T> predicate) =>
        value is not null && predicate(value);

    /// <summary>Determines whether the inner value of a nullable value matches a given predicate.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="predicate">The predicate to determine the return value.</param>
    /// <returns>
    /// The value <see langword="true"/> if <paramref name="value"/> is not <see langword="null"/>
    /// and returned <see langword="true"/> from the predicate, otherwise <see langword="false"/>.
    /// </returns>
    [MustUseReturnValue]
    internal static bool IsAnd<T>([NotNullWhen(true)] this T? value, [InstantHandle] Predicate<T> predicate)
        where T : struct =>
        value is { } t && predicate(t);

    /// <summary>Conditionally invokes based on a condition.</summary>
    /// <param name="that">The value that must be <see langword="false"/>.</param>
    /// <param name="exThat">Filled by the compiler, the expression to assert.</param>
    /// <returns>The parameter <paramref name="that"/>.</returns>
    [AssertionMethod]
    internal static bool IsFalse(
        [AssertionCondition(AssertionConditionType.IS_FALSE)] this bool that,
        [CallerArgumentExpression(nameof(that))] string? exThat = null
    ) =>
        that ? throw new UnreachableException(exThat) : false;

    /// <summary>Conditionally invokes based on a condition.</summary>
    /// <param name="that">The value that must be <see langword="true"/>.</param>
    /// <param name="exThat">Filled by the compiler, the expression to assert.</param>
    /// <returns>The parameter <paramref name="that"/>.</returns>
    [AssertionMethod]
    internal static bool IsTrue(
        [AssertionCondition(AssertionConditionType.IS_TRUE)] this bool that,
        [CallerArgumentExpression(nameof(that))] string? exThat = null
    ) =>
        that ? true : throw new UnreachableException(exThat);

    /// <summary>Conditionally invokes based on a condition.</summary>
    /// <param name="value">The value to check.</param>
    /// <param name="ifFalse">The value to invoke when <see langword="false"/>.</param>
    /// <param name="ifTrue">The value to invoke when <see langword="true"/>.</param>
    /// <returns>The parameter <paramref name="value"/>.</returns>
    internal static bool NotThen(
        this bool value,
        [InstantHandle] Action ifFalse,
        [InstantHandle] Action? ifTrue = null
    )
    {
        if (value)
            ifTrue?.Invoke();
        else
            ifFalse();

        return value;
    }

    /// <summary>Conditionally invokes based on a condition.</summary>
    /// <param name="value">The value to check.</param>
    /// <param name="ifTrue">The value to invoke when <see langword="true"/>.</param>
    /// <param name="ifFalse">The value to invoke when <see langword="false"/>.</param>
    /// <returns>The parameter <paramref name="value"/>.</returns>
    internal static bool Then(
        this bool value,
        [InstantHandle] Action ifTrue,
        [InstantHandle] Action? ifFalse = null
    )
    {
        if (value)
            ifTrue();
        else
            ifFalse?.Invoke();

        return value;
    }

    /// <summary>Filters an <see cref="IEnumerable{T}"/> to only non-null values.</summary>
    /// <typeparam name="T">The type of value to filter.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <returns>A filtered <see cref="IEnumerable{T}"/> with strictly non-null values.</returns>
    [LinqTunnel, Pure]
    internal static IEnumerable<T> Filter<T>([NoEnumeration] this IEnumerable<T?>? iterable) =>
#pragma warning disable CS8619
        iterable?.Where(x => x is not null) ?? Enumerable.Empty<T>();
#pragma warning restore CS8619

    /// <summary>Filters an <see cref="IEnumerable{T}"/> to only non-null values.</summary>
    /// <typeparam name="T">The type of value to filter.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <returns>A filtered <see cref="IEnumerable{T}"/> with strictly non-null values.</returns>
    [LinqTunnel, Pure]
    internal static IEnumerable<T> Filter<T>([NoEnumeration] this IEnumerable<T?>? iterable)
        where T : struct =>
#pragma warning disable CS8629
        iterable?.Where(x => x.HasValue).Select(x => x.Value) ?? Enumerable.Empty<T>();
#pragma warning restore CS8629

    /// <summary>Gives an optional value based on a condition.</summary>
    /// <remarks><para>The parameter is eagerly evaluated.</para></remarks>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="ifFalse">The value to return when <see langword="false"/>.</param>
    /// <returns>
    /// The value <paramref name="ifFalse"/> if <paramref name="value"/>
    /// is <see langword="false"/>, else <see langword="default"/>.
    /// </returns>
    [Pure]
    internal static T? NotThen<T>(this bool value, T ifFalse) => value ? default : ifFalse;

    /// <summary>Gives an optional value based on a condition.</summary>
    /// <remarks><para>The parameter is lazily evaluated.</para></remarks>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="onFalse">The value to invoke when <see langword="false"/>.</param>
    /// <returns>
    /// The value returned from <paramref name="onFalse"/> if <paramref name="value"/>
    /// is <see langword="false"/>, else <see langword="default"/>.
    /// </returns>
    [MustUseReturnValue]
    internal static T? NotThen<T>(this bool value, Func<T> onFalse) => value ? default : onFalse();

    /// <summary>Gives an optional value based on a condition.</summary>
    /// <remarks><para>The parameter is eagerly evaluated.</para></remarks>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="onTrue">The value to return when <see langword="true"/>.</param>
    /// <returns>
    /// The value <paramref name="onTrue"/> if <paramref name="value"/>
    /// is <see langword="true"/>, else <see langword="default"/>.
    /// </returns>
    [Pure]
    internal static T? Then<T>(this bool value, T onTrue) => value ? onTrue : default;

    /// <summary>Gives an optional value based on a condition.</summary>
    /// <remarks><para>The parameter is lazily evaluated.</para></remarks>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="ifTrue">The value to invoke when <see langword="true"/>.</param>
    /// <returns>
    /// The value returned from <paramref name="ifTrue"/> if <paramref name="value"/>
    /// is <see langword="true"/>, else <see langword="default"/>.
    /// </returns>
    [MustUseReturnValue]
    internal static T? Then<T>(this bool value, Func<T> ifTrue) => value ? ifTrue() : default;
}
