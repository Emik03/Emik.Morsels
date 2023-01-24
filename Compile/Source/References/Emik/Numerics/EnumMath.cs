// SPDX-License-Identifier: MPL-2.0
#if !NETFRAMEWORK || NET35_OR_GREATER
namespace Emik.Morsels;

using static Enum;
using static Expression;

/// <summary>Provides methods to do math on enums without overhead from boxing.</summary>
[UsedImplicitly]
static partial class EnumMath
{
    /// <summary>Performs a conversion operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The <see cref="int"/> cast of <paramref name="value"/>.</returns>
    [Pure]
    public static int AsInt<T>(this T value)
        where T : Enum =>
        Convert<T>.From(value);

    /// <summary>Performs a conversion operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The <typeparamref name="T"/> cast of <paramref name="value"/>.</returns>
    [Pure]
    public static T As<T>(this int value)
        where T : Enum =>
        Convert<T>.To(value);

    /// <summary>Performs a negation operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The negated value of the parameter <paramref name="value"/>.</returns>
    [Pure]
    public static T Negate<T>(this T value)
        where T : Enum =>
        value.Op(static x => unchecked(-x));

    /// <summary>Performs an decrement operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The predecessor of the parameter <paramref name="value"/>; the number immediately before it.</returns>
    [Pure]
    public static T Predecessor<T>(this T value)
        where T : Enum =>
        value.Op(static x => unchecked(--x));

    /// <summary>Performs a increment operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The predecessor of the parameter <paramref name="value"/>; the number immediately after it.</returns>
    [Pure]
    public static T Successor<T>(this T value)
        where T : Enum =>
        value.Op(static x => unchecked(++x));

    /// <summary>Performs an addition operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>The sum of the parameters <paramref name="left"/> and <paramref name="right"/>.</returns>
    [Pure]
    public static T Add<T>(this T left, T right)
        where T : Enum =>
        left.Op(right, static (x, y) => unchecked(x + y));

    /// <summary>Performs a subtraction operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>The difference of the parameters <paramref name="left"/> and <paramref name="right"/>.</returns>
    [Pure]
    public static T Subtract<T>(this T left, T right)
        where T : Enum =>
        left.Op(right, static (x, y) => unchecked(x - y));

    /// <summary>Performs a multiplication operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>The product of the parameters <paramref name="left"/> and <paramref name="right"/>.</returns>
    [Pure]
    public static T Multiply<T>(this T left, T right)
        where T : Enum =>
        left.Op(right, static (x, y) => unchecked(x * y));

    /// <summary>Performs a division operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>The quotient of the parameters <paramref name="left"/> and <paramref name="right"/>.</returns>
    [Pure]
    public static T Divide<T>(this T left, T right)
        where T : Enum =>
        left.Op(right, static (x, y) => x / y);

    /// <summary>Performs a modulo operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>The remainder of the parameters <paramref name="left"/> and <paramref name="right"/>.</returns>
    [Pure]
    public static T Modulo<T>(this T left, T right)
        where T : Enum =>
        left.Op(right, static (x, y) => x % y);

    [Pure]
    static T Op<T>(this T value, [InstantHandle, RequireStaticDelegate(IsError = true)] Func<int, int> op)
        where T : Enum =>
        op(value.AsInt()).As<T>();

    [Pure]
    static T Op<T>(this T left, T right, [InstantHandle, RequireStaticDelegate(IsError = true)] Func<int, int, int> op)
        where T : Enum =>
        op(left.AsInt(), right.AsInt()).As<T>();

    static class Convert<T>
        where T : Enum
    {
        public static Converter<T, int> From { get; } = Make<Converter<T, int>>(false);

        public static Converter<int, T> To { get; } = Make<Converter<int, T>>(true);

        static TFunc Make<TFunc>(bool isReverse)
            where TFunc : Delegate
        {
            var parameter = Parameter(isReverse ? typeof(int) : typeof(T), nameof(T));
            var underlying = GetUnderlyingType(typeof(T));
            var cast = isReverse ? (Expression)parameter : Convert(parameter, underlying);

            cast = underlying != typeof(int) ? Convert(parameter, isReverse ? underlying : typeof(int)) : cast;
            cast = isReverse ? Convert(cast, typeof(T)) : cast;

            return Lambda<TFunc>(cast, parameter).Compile();
        }
    }
}
#endif
