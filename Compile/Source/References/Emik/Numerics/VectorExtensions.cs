// SPDX-License-Identifier: MPL-2.0
#if NETSTANDARD2_0_OR_GREATER || NETCOREAPP
// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods for <see cref="INumber{TSelf}"/>.</summary>
static partial class NumberInterfaceExtensions
{
#if NET9_0_OR_GREATER
    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.SinCos"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static Vector<TSelf> Hypot<TSelf>(this Vector<TSelf> x, Vector<TSelf> y)
    {
        if (typeof(TSelf) == typeof(float))
        {
            var f = Vector.Hypot(
                Unsafe.As<Vector<TSelf>, Vector<float>>(ref x),
                Unsafe.As<Vector<TSelf>, Vector<float>>(ref y)
            );

            return Unsafe.As<Vector<float>, Vector<TSelf>>(ref f);
        }

        if (typeof(TSelf) != typeof(double))
            return default;

        var d = Vector.Hypot(
            Unsafe.As<Vector<TSelf>, Vector<double>>(ref x),
            Unsafe.As<Vector<TSelf>, Vector<double>>(ref y)
        );

        return Unsafe.As<Vector<double>, Vector<TSelf>>(ref d);
    }

    /// <inheritdoc cref="ITrigonometricFunctions{TSelf}.SinCos"/>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static (Vector<TSelf> Sin, Vector<TSelf> Cos) SinCos<TSelf>(this Vector<TSelf> x)
    {
        if (typeof(TSelf) == typeof(float))
        {
            var f = Vector.SinCos(Unsafe.As<Vector<TSelf>, Vector<float>>(ref x));
            return Unsafe.As<(Vector<float>, Vector<float>), (Vector<TSelf>, Vector<TSelf>)>(ref f);
        }

        if (typeof(TSelf) != typeof(double))
            return default;

        var d = Vector.SinCos(Unsafe.As<Vector<TSelf>, Vector<double>>(ref x));

        return Unsafe.As<(Vector<double>, Vector<double>), (Vector<TSelf>, Vector<TSelf>)>(ref d);
    }
#endif
}
#endif
