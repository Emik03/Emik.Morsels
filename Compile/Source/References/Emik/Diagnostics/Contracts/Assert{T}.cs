// SPDX-License-Identifier: MPL-2.0
#if NETFRAMEWORK || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;
#pragma warning disable 1591, MA0048, SA1600 // Temporary because I don't feel like documenting yet.

/// <summary>Defines the base class for an assertion, where a value is expected to return true.</summary>
/// <typeparam name="T">The type of value to assert with.</typeparam>
abstract partial class Assert<T> : Assert
{
    /// <inheritdoc cref="Assert.EqualTo{T}"/>
    [Format("Expected @x to be equal to @y, received #x and #y.")]
    public static bool EqualTo(T x, T y) => EqualityComparer<T>.Default.Equals(x, y);

    /// <inheritdoc cref="Assert.GreaterThan{T}"/>
    [Format("Expected @x to be strictly greater than @y, received #x which is less than or equal to #y.")]
    public static bool GreaterThan(T x, T y) => Compare(x, y) > 0;

    /// <inheritdoc cref="Assert.GreaterThanOrEqualTo{T}"/>
    [Format("Expected @x to be greater than or equal to @y, received #x which is strictly less than #y.")]
    public static bool GreaterThanOrEqualTo(T x, T y) => Compare(x, y) >= 0;

    /// <inheritdoc cref="Assert.LessThan{T}"/>
    [Format("Expected @x to be strictly less than @y, received #x which is greater than or equal to #y.")]
    public static bool LessThan(T x, T y) => Compare(x, y) < 0;

    /// <inheritdoc cref="Assert.LessThanOrEqualTo{T}"/>
    [Format("Expected @x to be less than or equal to @y, received #x which is strictly greater than #y.")]
    public static bool LessThanOrEqualTo(T x, T y) => Compare(x, y) <= 0;

    /// <inheritdoc cref="Assert.NotNull{T}"/>
    [Format("Expected @x to be not null, received null.")]
    public static bool NotNull(T x) => x is not null;

    /// <inheritdoc cref="Assert.Null{T}"/>
    [Format("Expected @x to be null, received #x.")]
    public static bool Null(T x) => x is null;

    /// <inheritdoc cref="Assert.SequenceEqualTo{T}"/>
    [Format("Expected @x to have the same items as @y, received #x and #y.")]
    public static bool SequenceEqualTo(IEnumerable<T> x, IEnumerable<T> y) => x.SequenceEqual(y);

    /// <inheritdoc cref="Assert.UnequalTo{T}"/>
    [Format("Expected @x to not be equal to @y, received #x.")]
    public static bool UnequalTo(T x, T y) => !EqualTo(x, y);

    /// <inheritdoc cref="Assert.Compare{T}"/>
    [Pure]
    public static int Compare(T x, T y) => Compare<T>(x, y);

    /// <inheritdoc cref="Assert.InRangeOf{T}(T, T)"/>
    [Pure]
    public static Predicate<T> InRangeOf(T low, T high) => InRangeOf<T>(low, high);

    /// <inheritdoc cref="Assert.Structured{T}"/>
    [Pure]
    public static Predicate<IEnumerable<T>> Structured(params T[] expected) => Structured<T>(expected);

    /// <inheritdoc cref="Assert.Params{T}"/>
    [Pure]
    public static T[] Params(params T[] items) => Params<T>(items);
}
#endif
