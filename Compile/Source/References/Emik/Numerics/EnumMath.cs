// SPDX-License-Identifier: MPL-2.0
#if !NETFRAMEWORK || NET35_OR_GREATER
// ReSharper disable CheckNamespace RedundantNameQualifier
namespace Emik.Morsels;

/// <summary>Provides methods to do math on enums without overhead from boxing.</summary>
[UsedImplicitly]
static partial class EnumMath
{
    /// <summary>Checks if the left-hand side implements the right-hand side.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="left"/> has the values
    /// of the parameter <paramref name="right"/>; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool Has<T>(this T left, T right)
        where T : struct, Enum =>
        (left.AsInt() & right.AsInt()) == right.AsInt();

    /// <summary>Performs a conversion operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The <see cref="int"/> cast of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int AsInt<T>(this T value)
        where T : struct, Enum =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        0 switch
        {
            _ when typeof(T).GetEnumUnderlyingType() == typeof(byte) => (byte)(object)value,
            _ when typeof(T).GetEnumUnderlyingType() == typeof(sbyte) => (sbyte)(object)value,
            _ when typeof(T).GetEnumUnderlyingType() == typeof(short) => (short)(object)value,
            _ when typeof(T).GetEnumUnderlyingType() == typeof(ushort) => (ushort)(object)value,
            _ when typeof(T).GetEnumUnderlyingType() == typeof(int) => (int)(object)value,
            _ when typeof(T).GetEnumUnderlyingType() == typeof(uint) => (int)(uint)(object)value,
            _ when typeof(T).GetEnumUnderlyingType() == typeof(long) => (int)(long)(object)value,
            _ when typeof(T).GetEnumUnderlyingType() == typeof(ulong) => (int)(ulong)(object)value,
            _ when typeof(T).GetEnumUnderlyingType() == typeof(nint) => (int)(nint)(object)value,
            _ when typeof(T).GetEnumUnderlyingType() == typeof(nuint) => (int)(nuint)(object)value,
            _ => throw Unreachable,
        };
#else
        MathCaching<T>.From(value);
#endif
    /// <summary>Performs a conversion operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The <typeparamref name="T"/> cast of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T As<T>(this int value)
        where T : struct, Enum =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        0 switch
        {
            _ when typeof(T).GetEnumUnderlyingType() == typeof(byte) => (T)(object)(byte)value,
            _ when typeof(T).GetEnumUnderlyingType() == typeof(sbyte) => (T)(object)(sbyte)value,
            _ when typeof(T).GetEnumUnderlyingType() == typeof(short) => (T)(object)(short)value,
            _ when typeof(T).GetEnumUnderlyingType() == typeof(ushort) => (T)(object)(ushort)value,
            _ when typeof(T).GetEnumUnderlyingType() == typeof(int) => (T)(object)value,
            _ when typeof(T).GetEnumUnderlyingType() == typeof(uint) => (T)(object)(uint)value,
            _ when typeof(T).GetEnumUnderlyingType() == typeof(long) => (T)(object)(long)value,
            _ when typeof(T).GetEnumUnderlyingType() == typeof(ulong) => (T)(object)(ulong)value,
            _ when typeof(T).GetEnumUnderlyingType() == typeof(nint) => (T)(object)(nint)value,
            _ when typeof(T).GetEnumUnderlyingType() == typeof(nuint) => (T)(object)(nuint)value,
            _ => throw Unreachable,
        };
#else
        MathCaching<T>.To(value);
#endif
    /// <summary>Performs a negation operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The negated value of the parameter <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Negate<T>(this T value)
        where T : struct, Enum =>
        (-value.AsInt()).As<T>();

    /// <summary>Performs an decrement operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The predecessor of the parameter <paramref name="value"/>; the number immediately before it.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Predecessor<T>(this T value)
        where T : struct, Enum =>
        (value.AsInt() - 1).As<T>();

    /// <summary>Performs a increment operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The predecessor of the parameter <paramref name="value"/>; the number immediately after it.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Successor<T>(this T value)
        where T : struct, Enum =>
        (value.AsInt() + 1).As<T>();

    /// <summary>Performs an addition operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>The sum of the parameters <paramref name="left"/> and <paramref name="right"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Add<T>(this T left, T right)
        where T : struct, Enum =>
        (left.AsInt() + right.AsInt()).As<T>();

    /// <summary>Performs a subtraction operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>The difference of the parameters <paramref name="left"/> and <paramref name="right"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Subtract<T>(this T left, T right)
        where T : struct, Enum =>
        (left.AsInt() - right.AsInt()).As<T>();

    /// <summary>Performs a multiplication operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>The product of the parameters <paramref name="left"/> and <paramref name="right"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Multiply<T>(this T left, T right)
        where T : struct, Enum =>
        (left.AsInt() * right.AsInt()).As<T>();

    /// <summary>Performs a division operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>The quotient of the parameters <paramref name="left"/> and <paramref name="right"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Divide<T>(this T left, T right)
        where T : struct, Enum =>
        (left.AsInt() / right.AsInt()).As<T>();

    /// <summary>Performs a modulo operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>The remainder of the parameters <paramref name="left"/> and <paramref name="right"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Modulo<T>(this T left, T right)
        where T : struct, Enum =>
        (left.AsInt() % right.AsInt()).As<T>();

    /// <summary>Computes the product of a sequence of <typeparamref name="T"/> values.</summary>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <param name="source">A sequence of <typeparamref name="T"/> values to calculate the product of.</param>
    /// <returns>The product of the values in the sequence.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Product<T>(this IEnumerable<T> source)
        where T : struct, Enum =>
        source.Aggregate(Multiply);

    /// <summary>Computes the sum of a sequence of <typeparamref name="T"/> values.</summary>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <param name="source">A sequence of <typeparamref name="T"/> values to calculate the sum of.</param>
    /// <returns>The sum of the values in the sequence.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Sum<T>(this IEnumerable<T> source)
        where T : struct, Enum
#if NET5_0_OR_GREATER
        =>
            source.TryGetSpan(out var span) ? span.Sum() : source.Aggregate(Add);
#else
        =>
            source.Aggregate(Add);
#endif
#if !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP3_0_OR_GREATER
    static class MathCaching<T>
        where T : struct, Enum
    {
        public static Converter<T, int> From { [Pure] get; } = Make<Converter<T, int>>(false);

        public static Converter<int, T> To { [Pure] get; } = Make<Converter<int, T>>(true);

        [MustUseReturnValue]
        static TFunc Make<TFunc>(bool isReverse)
            where TFunc : Delegate
        {
            var parameter =
                System.Linq.Expressions.Expression.Parameter(isReverse ? typeof(int) : typeof(T), nameof(T));

            var underlying = Enum.GetUnderlyingType(typeof(T));

            System.Linq.Expressions.Expression cast =
                isReverse ? parameter : System.Linq.Expressions.Expression.Convert(parameter, underlying);

            cast = underlying != typeof(int)
                ? System.Linq.Expressions.Expression.Convert(parameter, isReverse ? underlying : typeof(int))
                : cast;

            cast = isReverse ? System.Linq.Expressions.Expression.Convert(cast, typeof(T)) : cast;
            return System.Linq.Expressions.Expression.Lambda<TFunc>(cast, parameter).Compile();
        }
    }
#endif
}
#endif
