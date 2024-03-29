﻿// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods for nullable types and booleans.</summary>
static partial class Conditionals
{
#if NET7_0_OR_GREATER
    /// <summary>Converts <see cref="bool"/> to <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The type of number to convert to.</typeparam>
    /// <param name="that">Whether or not to return the one value, or zero.</param>
    /// <returns>
    /// The value <see cref="INumberBase{T}.One"/> or <see cref="INumberBase{T}.Zero"/>,
    /// based on the value of <paramref name="that"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T As<T>(this bool that)
        where T : INumberBase<T> =>
        that ? T.One : T.Zero;
#endif
#if NETCOREAPP || ROSLYN
    /// <summary>Determines whether two sequences are equal.</summary>
    /// <typeparam name="TDerived">The type of element in the compared array.</typeparam>
    /// <typeparam name="TBase">The type of element contained by the collection.</typeparam>
    /// <param name="first">The first <see cref="ImmutableArray{TBase}"/> to compare.</param>
    /// <param name="second">The second <see cref="ImmutableArray{TDerived}"/> to compare.</param>
    /// <returns>
    /// The value <see langword="true"/> if both sequences have the same
    /// values, or are both default; otherwise, <see langword="false"/>.
    /// </returns>
    [MustUseReturnValue]
    public static bool GuardedSequenceEqual<TDerived, TBase>(
        this ImmutableArray<TBase> first,
        ImmutableArray<TDerived> second
    )
        where TDerived : TBase =>
        first.IsDefault || second.IsDefault ? first.IsDefault && second.IsDefault : first.SequenceEqual(second);

    /// <summary>Determines whether two sequences are equal according to an equality comparer.</summary>
    /// <typeparam name="TDerived">The type of element in the compared array.</typeparam>
    /// <typeparam name="TBase">The type of element contained by the collection.</typeparam>
    /// <param name="first">The first <see cref="ImmutableArray{TBase}"/> to compare.</param>
    /// <param name="second">The second <see cref="ImmutableArray{TDerived}"/> to compare.</param>
    /// <param name="comparer">The comparer to use to check for equality.</param>
    /// <returns>
    /// The value <see langword="true"/> if both sequences have the same
    /// values, or are both default; otherwise, <see langword="false"/>.
    /// </returns>
    [MustUseReturnValue]
    public static bool GuardedSequenceEqual<TDerived, TBase>(
        this ImmutableArray<TBase> first,
        ImmutableArray<TDerived> second,
        Func<TBase, TBase, bool>? comparer
    )
        where TDerived : TBase =>
        first.IsDefault || second.IsDefault ? first.IsDefault && second.IsDefault :
        comparer is null ? first.SequenceEqual(second) : first.SequenceEqual(second, comparer);

    /// <summary>Determines whether two sequences are equal according to an equality comparer.</summary>
    /// <typeparam name="TDerived">The type of element in the compared array.</typeparam>
    /// <typeparam name="TBase">The type of element contained by the collection.</typeparam>
    /// <param name="first">The first <see cref="ImmutableArray{TBase}"/> to compare.</param>
    /// <param name="second">The second <see cref="ImmutableArray{TDerived}"/> to compare.</param>
    /// <param name="comparer">The comparer to use to check for equality.</param>
    /// <returns>
    /// The value <see langword="true"/> if both sequences have the same
    /// values, or are both default; otherwise, <see langword="false"/>.
    /// </returns>
    [MustUseReturnValue]
    public static bool GuardedSequenceEqual<TDerived, TBase>(
        this ImmutableArray<TBase> first,
        ImmutableArray<TDerived> second,
        IEqualityComparer<TBase>? comparer
    )
        where TDerived : TBase =>
        first.IsDefault || second.IsDefault ? first.IsDefault && second.IsDefault :
        comparer is null ? first.SequenceEqual(second) : first.SequenceEqual(second, comparer);
#endif

    /// <summary>Determines whether the inner value of a nullable value matches a given predicate.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="predicate">The predicate to determine the return value.</param>
    /// <returns>
    /// The value <see langword="true"/> if <paramref name="value"/> is not <see langword="null"/>
    /// and returned <see langword="true"/> from the predicate; otherwise, <see langword="false"/>.
    /// </returns>
    [MustUseReturnValue]
    public static bool IsAnd<T>([NotNullWhen(true)] this T? value, [InstantHandle] Predicate<T> predicate) =>
        value is not null && predicate(value);

    /// <summary>Determines whether the inner value of a nullable value matches a given predicate.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="predicate">The predicate to determine the return value.</param>
    /// <returns>
    /// The value <see langword="true"/> if <paramref name="value"/> is not <see langword="null"/>
    /// and returned <see langword="true"/> from the predicate; otherwise, <see langword="false"/>.
    /// </returns>
    [MustUseReturnValue]
    public static bool IsAnd<T>([NotNullWhen(true)] this T? value, [InstantHandle] Predicate<T> predicate)
        where T : struct =>
        value is { } t && predicate(t);

    /// <summary>Conditionally invokes based on a condition.</summary>
    /// <param name="that">The value that must be <see langword="false"/>.</param>
    /// <param name="exThat">Filled by the compiler, the expression to assert.</param>
    /// <returns>The parameter <paramref name="that"/>.</returns>
    [AssertionMethod]
    public static bool IsFalse(
        [AssertionCondition(AssertionConditionType.IS_FALSE), DoesNotReturnIf(true)] this bool that,
        [CallerArgumentExpression(nameof(that))] string? exThat = null
    ) =>
        that ? throw new UnreachableException(exThat) : false;

#if NETFRAMEWORK || NETSTANDARD2_0_OR_GREATER || NETCOREAPP
    /// <summary>Determines whether the value is null or not.</summary>
    /// <typeparam name="T">The type of value to check.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="value"/>
    /// is <see langword="null"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsNull<T>([NotNullWhen(false)] this T? value) =>
        (!typeof(T).IsValueType || Nullable.GetUnderlyingType(typeof(T)) is not null) &&
        EqualityComparer<T?>.Default.Equals(value, default);
#endif

    /// <summary>Conditionally invokes based on a condition.</summary>
    /// <param name="that">The value that must be <see langword="true"/>.</param>
    /// <param name="exThat">Filled by the compiler, the expression to assert.</param>
    /// <returns>The parameter <paramref name="that"/>.</returns>
    [AssertionMethod]
    public static bool IsTrue(
        [AssertionCondition(AssertionConditionType.IS_TRUE), DoesNotReturnIf(false)] this bool that,
        [CallerArgumentExpression(nameof(that))] string? exThat = null
    ) =>
        that ? true : throw new UnreachableException(exThat);

    /// <summary>Conditionally invokes based on a condition.</summary>
    /// <param name="value">The value to check.</param>
    /// <param name="ifTrue">The value to invoke when <see langword="true"/>.</param>
    /// <param name="ifFalse">The value to invoke when <see langword="false"/>.</param>
    /// <returns>The parameter <paramref name="value"/>.</returns>
    public static bool Then(
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

    /// <summary>Gives an optional value based on a condition.</summary>
    /// <remarks><para>The parameter is eagerly evaluated.</para></remarks>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="onTrue">The value to return when <see langword="true"/>.</param>
    /// <returns>
    /// The value <paramref name="onTrue"/> if <paramref name="value"/>
    /// is <see langword="true"/>; otherwise, <see langword="default"/>.
    /// </returns>
    [Pure]
    public static T? Then<T>(this bool value, T onTrue) => value ? onTrue : default;

    /// <summary>Gives an optional value based on a condition.</summary>
    /// <remarks><para>The parameter is lazily evaluated.</para></remarks>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="ifTrue">The value to invoke when <see langword="true"/>.</param>
    /// <returns>
    /// The value returned from <paramref name="ifTrue"/> if <paramref name="value"/>
    /// is <see langword="true"/>; otherwise, <see langword="default"/>.
    /// </returns>
    [MustUseReturnValue]
    public static T? Then<T>(this bool value, Func<T> ifTrue) => value ? ifTrue() : default;
#if !NET20 && !NET30
    /// <summary>Filters an <see cref="IEnumerable{T}"/> to only non-null values.</summary>
    /// <typeparam name="T">The type of value to filter.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <returns>A filtered <see cref="IEnumerable{T}"/> with strictly non-null values.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<T> Filter<T>([NoEnumeration] this IEnumerable<T?>? iterable) =>
#pragma warning disable 8619
        iterable?.Where(x => x is not null) ?? [];
#pragma warning restore 8619

    /// <summary>Filters an <see cref="IEnumerable{T}"/> to only non-null values.</summary>
    /// <typeparam name="T">The type of value to filter.</typeparam>
    /// <param name="iterable">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <returns>A filtered <see cref="IEnumerable{T}"/> with strictly non-null values.</returns>
    [LinqTunnel, Pure]
    public static IEnumerable<T> Filter<T>([NoEnumeration] this IEnumerable<T?>? iterable)
        where T : struct =>
#pragma warning disable 8629
        iterable?.Where(x => x.HasValue).Select(x => x.Value) ?? [];
#pragma warning restore 8629
#endif
}
