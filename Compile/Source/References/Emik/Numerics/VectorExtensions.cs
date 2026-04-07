// SPDX-License-Identifier: MPL-2.0
#if NET8_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods for <see cref="Vector{T}"/>.</summary>
static partial class NumberInterfaceExtensions
{
    extension<T>(Vector<T> x)
    {
        /// <inheritdoc cref="IFloatingPoint{TSelf}.Ceiling"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public Vector<T> Ceiling() =>
            typeof(T) == typeof(float) ? (Vector<T>)(object)Vector.Ceiling((Vector<float>)(object)x) :
            typeof(T) == typeof(double) ? (Vector<T>)(object)Vector.Ceiling((Vector<double>)(object)x) :
            default;

        /// <inheritdoc cref="IFloatingPoint{TSelf}.Floor"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public Vector<T> Floor() =>
            typeof(T) == typeof(float) ? (Vector<T>)(object)Vector.Floor((Vector<float>)(object)x) :
            typeof(T) == typeof(double) ? (Vector<T>)(object)Vector.Floor((Vector<double>)(object)x) :
            default;
    }

#if NET9_0_OR_GREATER
    extension<T>(Vector<T> x)
    {
        /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.Cos"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public Vector<T> Cos() =>
            typeof(T) == typeof(float) ? (Vector<T>)(object)Vector.Cos((Vector<float>)(object)x) :
            typeof(T) == typeof(double) ? (Vector<T>)(object)Vector.Cos((Vector<double>)(object)x) :
            default;

        /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.DegreesToRadians"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public Vector<T> DegreesToRadians() =>
            typeof(T) == typeof(float) ? (Vector<T>)(object)Vector.DegreesToRadians((Vector<float>)(object)x) :
            typeof(T) == typeof(double) ? (Vector<T>)(object)Vector.DegreesToRadians((Vector<double>)(object)x) :
            default;

        /// <inheritdoc cref="IExponentialFunctions{TSelf}.Exp"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public Vector<T> Exp() =>
            typeof(T) == typeof(float) ? (Vector<T>)(object)Vector.Exp((Vector<float>)(object)x) :
            typeof(T) == typeof(double) ? (Vector<T>)(object)Vector.Exp((Vector<double>)(object)x) :
            default;

        /// <inheritdoc cref="IFloatingPointIeee754{TSelf}.FusedMultiplyAdd"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public Vector<T> FusedMultiplyAdd(Vector<T> y, Vector<T> z) =>
            typeof(T) == typeof(float) ? (Vector<T>)(object)Vector.FusedMultiplyAdd(
                (Vector<float>)(object)x,
                (Vector<float>)(object)y,
                (Vector<float>)(object)z
            ) :
            typeof(T) == typeof(double) ? (Vector<T>)(object)Vector.FusedMultiplyAdd(
                (Vector<double>)(object)x,
                (Vector<double>)(object)y,
                (Vector<double>)(object)z
            ) :
            default;

        /// <inheritdoc cref="IRootFunctions{TSelf}.Hypot"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public Vector<T> Hypot(Vector<T> y) =>
            typeof(T) == typeof(float) ?
                (Vector<T>)(object)Vector.Hypot((Vector<float>)(object)x, (Vector<float>)(object)y) :
                typeof(T) == typeof(double) ?
                    (Vector<T>)(object)Vector.Hypot((Vector<double>)(object)x, (Vector<double>)(object)y) : default;

        /// <inheritdoc cref="ILogarithmicFunctions{TSelf}.Log(TSelf)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public Vector<T> Log() =>
            typeof(T) == typeof(float) ? (Vector<T>)(object)Vector.Log((Vector<float>)(object)x) :
            typeof(T) == typeof(double) ? (Vector<T>)(object)Vector.Log((Vector<double>)(object)x) :
            default;

        /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.DegreesToRadians"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public Vector<T> Log2() =>
            typeof(T) == typeof(float) ? (Vector<T>)(object)Vector.Log2((Vector<float>)(object)x) :
            typeof(T) == typeof(double) ? (Vector<T>)(object)Vector.Log2((Vector<double>)(object)x) :
            default;

        /// <inheritdoc cref="INumberBase{TSelf}.MultiplyAddEstimate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public Vector<T> MultiplyAddEstimate(Vector<T> y, Vector<T> z) =>
            typeof(T) == typeof(float) ? (Vector<T>)(object)Vector.MultiplyAddEstimate(
                (Vector<float>)(object)x,
                (Vector<float>)(object)y,
                (Vector<float>)(object)z
            ) :
            typeof(T) == typeof(double) ? (Vector<T>)(object)Vector.MultiplyAddEstimate(
                (Vector<double>)(object)x,
                (Vector<double>)(object)y,
                (Vector<double>)(object)z
            ) :
            default;

        /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.RadiansToDegrees"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public Vector<T> RadiansToDegrees() =>
            typeof(T) == typeof(float) ? (Vector<T>)(object)Vector.RadiansToDegrees((Vector<float>)(object)x) :
            typeof(T) == typeof(double) ? (Vector<T>)(object)Vector.RadiansToDegrees((Vector<double>)(object)x) :
            default;

        /// <inheritdoc cref="IFloatingPoint{TSelf}.Round(TSelf)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public Vector<T> Round() =>
            typeof(T) == typeof(float) ? (Vector<T>)(object)Vector.Round((Vector<float>)(object)x) :
            typeof(T) == typeof(double) ? (Vector<T>)(object)Vector.Round((Vector<double>)(object)x) :
            default;

        /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.Sin"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public Vector<T> Sin() =>
            typeof(T) == typeof(float) ? (Vector<T>)(object)Vector.Sin((Vector<float>)(object)x) :
            typeof(T) == typeof(double) ? (Vector<T>)(object)Vector.Sin((Vector<double>)(object)x) :
            default;

        /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.SinCos"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public (Vector<T> Sin, Vector<T> Cos) SinCos() =>
            typeof(T) == typeof(float) ? ((Vector<T>, Vector<T>))(object)Vector.SinCos((Vector<float>)(object)x) :
            typeof(T) == typeof(double) ? ((Vector<T>, Vector<T>))(object)Vector.SinCos((Vector<double>)(object)x) :
            default;

        /// <inheritdoc cref="IFloatingPoint{TSelf}.Truncate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        public Vector<T> Truncate() =>
            typeof(T) == typeof(float) ? (Vector<T>)(object)Vector.Truncate((Vector<float>)(object)x) :
            typeof(T) == typeof(double) ? (Vector<T>)(object)Vector.Truncate((Vector<double>)(object)x) :
            default;
    }

#endif
}
#endif
