// SPDX-License-Identifier: MPL-2.0
#if NET5_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

using static Span;

/// <summary>Extension methods for <see cref="Vector{T}"/>.</summary>
static partial class NumberInterfaceExtensions
{
    /// <inheritdoc cref="IFloatingPoint{TSelf}.Ceiling"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Vector<T> Ceiling<T>(this Vector<T> x) =>
        typeof(T) == typeof(float) ? Ret<Vector<T>>.From(Vector.Ceiling(Ret<Vector<float>>.From(x))) :
        typeof(T) == typeof(double) ? Ret<Vector<T>>.From(Vector.Ceiling(Ret<Vector<double>>.From(x))) :
        default;

    /// <inheritdoc cref="IFloatingPoint{TSelf}.Floor"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Vector<T> Floor<T>(this Vector<T> x) =>
        typeof(T) == typeof(float) ? Ret<Vector<T>>.From(Vector.Floor(Ret<Vector<float>>.From(x))) :
        typeof(T) == typeof(double) ? Ret<Vector<T>>.From(Vector.Floor(Ret<Vector<double>>.From(x))) :
        default;
#if NET9_0_OR_GREATER
    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.Cos"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Vector<T> Cos<T>(this Vector<T> x) =>
        typeof(T) == typeof(float) ? Ret<Vector<T>>.From(Vector.Cos(Ret<Vector<float>>.From(x))) :
        typeof(T) == typeof(double) ? Ret<Vector<T>>.From(Vector.Cos(Ret<Vector<double>>.From(x))) :
        default;

    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.DegreesToRadians"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Vector<T> DegreesToRadians<T>(this Vector<T> x) =>
        typeof(T) == typeof(float) ? Ret<Vector<T>>.From(Vector.DegreesToRadians(Ret<Vector<float>>.From(x))) :
        typeof(T) == typeof(double) ? Ret<Vector<T>>.From(Vector.DegreesToRadians(Ret<Vector<double>>.From(x))) :
        default;

    /// <inheritdoc cref="IExponentialFunctions{TSelf}.Exp"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Vector<T> Exp<T>(this Vector<T> x) =>
        typeof(T) == typeof(float) ? Ret<Vector<T>>.From(Vector.Exp(Ret<Vector<float>>.From(x))) :
        typeof(T) == typeof(double) ? Ret<Vector<T>>.From(Vector.Exp(Ret<Vector<double>>.From(x))) :
        default;

    /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.FusedMultiplyAdd"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Vector<T> FusedMultiplyAdd<T>(this Vector<T> x, Vector<T> y, Vector<T> z) =>
        typeof(T) == typeof(float) ? Ret<Vector<T>>.From(
            Vector.FusedMultiplyAdd(
                Ret<Vector<float>>.From(x),
                Ret<Vector<float>>.From(y),
                Ret<Vector<float>>.From(z)
            )
        ) :
        typeof(T) == typeof(double) ? Ret<Vector<T>>.From(
            Vector.FusedMultiplyAdd(
                Ret<Vector<double>>.From(x),
                Ret<Vector<double>>.From(y),
                Ret<Vector<double>>.From(z)
            )
        ) :
        default;

    /// <inheritdoc cref="IRootFunctions{TSelf}.Hypot"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Vector<T> Hypot<T>(this Vector<T> x, Vector<T> y) =>
        typeof(T) == typeof(float) ?
            Ret<Vector<T>>.From(Vector.Hypot(Ret<Vector<float>>.From(x), Ret<Vector<float>>.From(y))) :
            typeof(T) == typeof(double) ?
                Ret<Vector<T>>.From(Vector.Hypot(Ret<Vector<double>>.From(x), Ret<Vector<double>>.From(y))) :
                default;

    /// <inheritdoc cref="ILogarithmicFunctions{TSelf}.Log(TSelf)"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Vector<T> Log<T>(this Vector<T> x) =>
        typeof(T) == typeof(float) ? Ret<Vector<T>>.From(Vector.Log(Ret<Vector<float>>.From(x))) :
        typeof(T) == typeof(double) ? Ret<Vector<T>>.From(Vector.Log(Ret<Vector<double>>.From(x))) :
        default;

    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.DegreesToRadians"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Vector<T> Log2<T>(this Vector<T> x) =>
        typeof(T) == typeof(float) ? Ret<Vector<T>>.From(Vector.Log2(Ret<Vector<float>>.From(x))) :
        typeof(T) == typeof(double) ? Ret<Vector<T>>.From(Vector.Log2(Ret<Vector<double>>.From(x))) :
        default;

    /// <inheritdoc cref="INumberBase{TSelf}.MultiplyAddEstimate"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Vector<T> MultiplyAddEstimate<T>(this Vector<T> x, Vector<T> y, Vector<T> z) =>
        typeof(T) == typeof(float) ? Ret<Vector<T>>.From(
            Vector.MultiplyAddEstimate(
                Ret<Vector<float>>.From(x),
                Ret<Vector<float>>.From(y),
                Ret<Vector<float>>.From(z)
            )
        ) :
        typeof(T) == typeof(double) ? Ret<Vector<T>>.From(
            Vector.MultiplyAddEstimate(
                Ret<Vector<double>>.From(x),
                Ret<Vector<double>>.From(y),
                Ret<Vector<double>>.From(z)
            )
        ) :
        default;

    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.RadiansToDegrees"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Vector<T> RadiansToDegrees<T>(this Vector<T> x) =>
        typeof(T) == typeof(float) ? Ret<Vector<T>>.From(Vector.RadiansToDegrees(Ret<Vector<float>>.From(x))) :
        typeof(T) == typeof(double) ? Ret<Vector<T>>.From(Vector.RadiansToDegrees(Ret<Vector<double>>.From(x))) :
        default;

    /// <inheritdoc cref="IFloatingPoint{TSelf}.Round(TSelf)"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Vector<T> Round<T>(this Vector<T> x) =>
        typeof(T) == typeof(float) ? Ret<Vector<T>>.From(Vector.Round(Ret<Vector<float>>.From(x))) :
        typeof(T) == typeof(double) ? Ret<Vector<T>>.From(Vector.Round(Ret<Vector<double>>.From(x))) :
        default;

    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.Sin"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Vector<T> Sin<T>(this Vector<T> x) =>
        typeof(T) == typeof(float) ? Ret<Vector<T>>.From(Vector.Sin(Ret<Vector<float>>.From(x))) :
        typeof(T) == typeof(double) ? Ret<Vector<T>>.From(Vector.Sin(Ret<Vector<double>>.From(x))) :
        default;

    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.SinCos"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static (Vector<T> Sin, Vector<T> Cos) SinCos<T>(this Vector<T> x) =>
        typeof(T) == typeof(float) ?
            Ret<(Vector<T> Sin, Vector<T> Cos)>.From(Vector.SinCos(Ret<Vector<float>>.From(x))) :
            typeof(T) == typeof(double) ?
                Ret<(Vector<T> Sin, Vector<T> Cos)>.From(Vector.SinCos(Ret<Vector<double>>.From(x))) :
                default;

    /// <inheritdoc cref="IFloatingPoint{TSelf}.Truncate"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Vector<T> Truncate<T>(this Vector<T> x) =>
        typeof(T) == typeof(float) ? Ret<Vector<T>>.From(Vector.Truncate(Ret<Vector<float>>.From(x))) :
        typeof(T) == typeof(double) ? Ret<Vector<T>>.From(Vector.Truncate(Ret<Vector<double>>.From(x))) :
        default;
#endif
}
#endif
