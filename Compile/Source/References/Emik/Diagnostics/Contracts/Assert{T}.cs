// SPDX-License-Identifier: MPL-2.0
#if NETFRAMEWORK || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;
#pragma warning disable 1591, MA0048, SA1600 // Temporary because I don't feel like documenting yet.

/// <summary>Defines the base class for an assertion, where a value is expected to return true.</summary>
/// <typeparam name="T">The type of value to assert with.</typeparam>
abstract partial class Assert<T> : Assert
{
    /// <inheritdoc cref="Emik.Morsels.Assert.EqualTo{T}"/>
    [Format("Expected @x to be equal to @y, received #x and #y.")]
    public static bool EqualTo(T x, T y) => EqualTo<T>(x, y);

    /// <inheritdoc cref="Emik.Morsels.Assert.GreaterThan{T}"/>
    [Format("Expected @x to be strictly greater than @y, received #x which is less than or equal to #y.")]
    public static bool GreaterThan(T x, T y) => GreaterThan<T>(x, y);

    /// <inheritdoc cref="Emik.Morsels.Assert.GreaterThanOrEqualTo{T}"/>
    [Format("Expected @x to be greater than or equal to @y, received #x which is strictly less than #y.")]
    public static bool GreaterThanOrEqualTo(T x, T y) => GreaterThanOrEqualTo<T>(x, y);

    /// <inheritdoc cref="Emik.Morsels.Assert.LessThan{T}"/>
    [Format("Expected @x to be strictly less than @y, received #x which is greater than or equal to #y.")]
    public static bool LessThan(T x, T y) => LessThan<T>(x, y);

    /// <inheritdoc cref="Emik.Morsels.Assert.LessThanOrEqualTo{T}"/>
    [Format("Expected @x to be less than or equal to @y, received #x which is strictly greater than #y.")]
    public static bool LessThanOrEqualTo(T x, T y) => LessThanOrEqualTo<T>(x, y);

    /// <inheritdoc cref="Emik.Morsels.Assert.NotNull{T}"/>
    [Format("Expected @x to be not null, received null.")]
    public static bool NotNull(T x) => NotNull<T>(x);

    /// <inheritdoc cref="Emik.Morsels.Assert.Null{T}"/>
    [Format("Expected @x to be null, received #x.")]
    public static bool Null(T x) => Null<T>(x);

    /// <inheritdoc cref="Emik.Morsels.Assert.SequenceEqualTo{T}"/>
    [Format("Expected @x to have the same items as @y, received #x and #y.")]
    public static bool SequenceEqualTo([InstantHandle] IEnumerable<T> x, [InstantHandle] IEnumerable<T> y) =>
        SequenceEqualTo<T>(x, y);

    /// <inheritdoc cref="Emik.Morsels.Assert.UnequalTo{T}"/>
    [Format("Expected @x to not be equal to @y, received #x.")]
    public static bool UnequalTo(T x, T y) => UnequalTo<T>(x, y);

    /// <inheritdoc cref="Emik.Morsels.Assert.Compare{T}"/>
    [Pure]
    public static int Compare(T x, T y) => Compare<T>(x, y);

    /// <inheritdoc cref="Emik.Morsels.Assert.InRangeOf{T}(T, T)"/>
    [Pure]
    public static Predicate<T> InRangeOf(T low, T high) => InRangeOf<T>(low, high);

    /// <inheritdoc cref="Emik.Morsels.Assert.Structured{T}"/>
    [Pure]
    public static Predicate<IEnumerable<T>> Structured(params T[] expected) => Structured<T>(expected);

    /// <inheritdoc cref="Emik.Morsels.Assert.Params{T}"/>
    [Pure]
    public static T[] Params(params T[] items) => Params<T>(items);
}
#endif
