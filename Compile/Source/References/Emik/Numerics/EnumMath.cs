// SPDX-License-Identifier: MPL-2.0
#if !NETFRAMEWORK || NET35_OR_GREATER
namespace Emik.Morsels;

using static Enum;
using static Expression;

/// <summary>Provides methods to do math on enums without overhead from boxing.</summary>
[UsedImplicitly]
static partial class EnumMath
{
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
        Convert<T>.AsT(op(Convert<T>.AsInt(value)));

    [Pure]
    static T Op<T>(this T left, T right, [InstantHandle, RequireStaticDelegate(IsError = true)] Func<int, int, int> op)
        where T : Enum =>
        Convert<T>.AsT(op(Convert<T>.AsInt(left), Convert<T>.AsInt(right)));

    static class Convert<T>
        where T : Enum
    {
        public static Converter<T, int> AsInt { get; } = MakeAsInt();

        public static Converter<int, T> AsT { get; } = MakeAsT();

        static Converter<T, int> MakeAsInt()
        {
            var parameter = Parameter(typeof(T), nameof(T));
            var underlying = GetUnderlyingType(typeof(T));
            var cast = Convert(parameter, underlying);

            if (underlying != typeof(int))
                cast = Convert(cast, typeof(int));

            return Lambda<Converter<T, int>>(cast, parameter).Compile();
        }

        static Converter<int, T> MakeAsT()
        {
            var parameter = Parameter(typeof(int), nameof(T));
            var underlying = GetUnderlyingType(typeof(T));
            var cast = (Expression)parameter;

            if (underlying != typeof(int))
                cast = Convert(parameter, underlying);

            cast = Convert(cast, typeof(T));

            return Lambda<Converter<int, T>>(cast, parameter).Compile();
        }
    }
}
#endif
