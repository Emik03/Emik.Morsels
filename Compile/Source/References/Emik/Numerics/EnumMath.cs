// SPDX-License-Identifier: MPL-2.0
#if !NETFRAMEWORK || NET35_OR_GREATER
// ReSharper disable CheckNamespace RedundantNameQualifier
namespace Emik.Morsels;
#if !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP3_0_OR_GREATER
using static System.Linq.Expressions.Expression;
using static Enum;
using Expression = System.Linq.Expressions.Expression;
#endif

/// <summary>Provides methods to do math on enums without overhead from boxing.</summary>
[UsedImplicitly]
static partial class EnumMath
{
#if !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP3_0_OR_GREATER
    enum Unknowable;
#endif
    static readonly Dictionary<Type, IList> s_dictionary = new();

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
        where T : Enum =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        (left.AsInt() & right.AsInt()) == left.AsInt();
#else
        left.Op(right, static (x, y) => (x & y) == x);
#endif

    /// <summary>Performs a conversion operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The <see cref="int"/> cast of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int AsInt<T>(this T value)
        where T : Enum =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        (typeof(T) == typeof(Enum) ? value.GetType() : typeof(T)).GetEnumUnderlyingType() switch
        {
            var x when x == typeof(byte) => (byte)(object)value,
            var x when x == typeof(sbyte) => (sbyte)(object)value,
            var x when x == typeof(short) => (short)(object)value,
            var x when x == typeof(ushort) => (ushort)(object)value,
            var x when x == typeof(int) => (int)(object)value,
            var x when x == typeof(uint) => (int)(uint)(object)value,
            var x when x == typeof(long) => (int)(long)(object)value,
            var x when x == typeof(ulong) => (int)(ulong)(object)value,
            var x when x == typeof(nint) => (int)(nint)(object)value,
            var x when x == typeof(nuint) => (int)(nuint)(object)value,
            _ => throw Unreachable,
        };
#else
        typeof(T) == typeof(Enum)
            ? value.GetType().GetEnumUnderlyingType() switch
            {
                var x when x == typeof(byte) => (byte)(object)value,
                var x when x == typeof(sbyte) => (sbyte)(object)value,
                var x when x == typeof(short) => (short)(object)value,
                var x when x == typeof(ushort) => (ushort)(object)value,
                var x when x == typeof(int) => (int)(object)value,
                var x when x == typeof(uint) => (int)(uint)(object)value,
                var x when x == typeof(long) => (int)(long)(object)value,
                var x when x == typeof(ulong) => (int)(ulong)(object)value,
                var x when x == typeof(nint) => (int)(nint)(object)value,
                var x when x == typeof(nuint) => (int)(nuint)(object)value,
                _ => throw Unreachable,
            }
            : MathCaching<T>.From(value);
#endif

    /// <summary>Gets the values of an enum cached and strongly-typed.</summary>
    /// <typeparam name="T">The type of enum to get the values from.</typeparam>
    /// <returns>All values in the type parameter <typeparamref name="T"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static IList<T> GetValues<T>()
        where T : Enum =>
        s_dictionary.TryGetValue(typeof(T), out var list)
            ? (IList<T>)list
            : (T[])(s_dictionary[typeof(T)] = typeof(T) == typeof(Enum)
#if NETFRAMEWORK && !NET46_OR_GREATER || NETSTANDARD && !NETSTANDARD1_3_OR_GREATER
                ? new T[0]
#else
                ? Array.Empty<T>()
#endif
                : Enum.GetValues(typeof(T)));

    /// <summary>Performs a conversion operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The <typeparamref name="T"/> cast of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T As<T>(this int value)
        where T : Enum =>
        typeof(T) == typeof(Enum)
#if !NETSTANDARD2_1_OR_GREATER || !NETCOREAPP3_0_OR_GREATER
            ? (T)(Enum)(Unknowable)value
            : typeof(T).GetEnumUnderlyingType() switch
            {
                var x when x == typeof(byte) => (T)(object)(byte)value,
                var x when x == typeof(sbyte) => (T)(object)(sbyte)value,
                var x when x == typeof(short) => (T)(object)(short)value,
                var x when x == typeof(ushort) => (T)(object)(ushort)value,
                var x when x == typeof(int) => (T)(object)value,
                var x when x == typeof(uint) => (T)(object)(uint)value,
                var x when x == typeof(long) => (T)(object)(long)value,
                var x when x == typeof(ulong) => (T)(object)(ulong)value,
                var x when x == typeof(nint) => (T)(object)(nint)value,
                var x when x == typeof(nuint) => (T)(object)(nuint)value,
                _ => throw Unreachable,
            };
#else
            ? (T)(Enum)MathCaching<Unknowable>.To(value)
            : MathCaching<T>.To(value);
#endif

    /// <summary>Performs a negation operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The negated value of the parameter <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Negate<T>(this T value)
        where T : Enum =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        (-value.AsInt()).As<T>();
#else
        value.Op(static x => unchecked(-x));
#endif

    /// <summary>Performs an decrement operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The predecessor of the parameter <paramref name="value"/>; the number immediately before it.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Predecessor<T>(this T value)
        where T : Enum =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        (value.AsInt() - 1).As<T>();
#else
        value.Op(static x => unchecked(x - 1));
#endif

    /// <summary>Performs a increment operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The predecessor of the parameter <paramref name="value"/>; the number immediately after it.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Successor<T>(this T value)
        where T : Enum =>
#if NETCOREAPP
        (value.AsInt() + 1).As<T>();
#else
        value.Op(static x => unchecked(x + 1));
#endif

    /// <summary>Performs an addition operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>The sum of the parameters <paramref name="left"/> and <paramref name="right"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Add<T>(this T left, T right)
        where T : Enum =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        (left.AsInt() + right.AsInt()).As<T>();
#else
        left.Op(right, static (x, y) => unchecked(x + y));
#endif

    /// <summary>Performs a subtraction operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>The difference of the parameters <paramref name="left"/> and <paramref name="right"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Subtract<T>(this T left, T right)
        where T : Enum =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        (left.AsInt() - right.AsInt()).As<T>();
#else
        left.Op(right, static (x, y) => unchecked(x - y));
#endif

    /// <summary>Performs a multiplication operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>The product of the parameters <paramref name="left"/> and <paramref name="right"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Multiply<T>(this T left, T right)
        where T : Enum =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        (left.AsInt() * right.AsInt()).As<T>();
#else
        left.Op(right, static (x, y) => unchecked(x * y));
#endif

    /// <summary>Performs a division operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>The quotient of the parameters <paramref name="left"/> and <paramref name="right"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Divide<T>(this T left, T right)
        where T : Enum =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        (left.AsInt() / right.AsInt()).As<T>();
#else
        left.Op(right, static (x, y) => x / y);
#endif

    /// <summary>Performs a modulo operation.</summary>
    /// <remarks><para>The conversion and operation are unchecked, and treated as <see cref="int"/>.</para></remarks>
    /// <typeparam name="T">The type of <see cref="Enum"/> to perform the operation on.</typeparam>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>The remainder of the parameters <paramref name="left"/> and <paramref name="right"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Modulo<T>(this T left, T right)
        where T : Enum =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        (left.AsInt() % right.AsInt()).As<T>();
#else
        left.Op(right, static (x, y) => x % y);
#endif

    /// <summary>Computes the product of a sequence of <typeparamref name="T"/> values.</summary>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <param name="source">A sequence of <typeparamref name="T"/> values to calculate the product of.</param>
    /// <returns>The product of the values in the sequence.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Product<T>(this IEnumerable<T> source)
        where T : Enum =>
        source.Aggregate(Multiply);

    /// <summary>Computes the sum of a sequence of <typeparamref name="T"/> values.</summary>
    /// <typeparam name="T">The type of sequence.</typeparam>
    /// <param name="source">A sequence of <typeparamref name="T"/> values to calculate the sum of.</param>
    /// <returns>The sum of the values in the sequence.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Sum<T>(this IEnumerable<T> source)
        where T :
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        unmanaged,
#endif
        Enum
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        =>
            source.TryGetSpan(out var span) ? span.Sum() : source.Aggregate(Add);
#else
        =>
            source.Aggregate(Add);
#endif
#if !NETCOREAPP
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static T Op<T>(this T value, [InstantHandle, RequireStaticDelegate(IsError = true)] Func<int, int> op)
        where T : Enum =>
        op(value.AsInt()).As<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static T Op<T>(this T left, T right, [InstantHandle, RequireStaticDelegate(IsError = true)] Func<int, int, int> op)
        where T : Enum =>
        op(left.AsInt(), right.AsInt()).As<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    static TResult Op<T, TResult>(
        this T left,
        T right,
        [InstantHandle, RequireStaticDelegate(IsError = true)] Func<int, int, TResult> op
    )
        where T : Enum =>
        op(left.AsInt(), right.AsInt());

    static class MathCaching<T>
        where T : Enum
    {
        public static Converter<T, int> From { [Pure] get; } = Make<Converter<T, int>>(false);

        public static Converter<int, T> To { [Pure] get; } = Make<Converter<int, T>>(true);

        [MustUseReturnValue]
        static TFunc Make<TFunc>(bool isReverse)
            where TFunc : Delegate
        {
            var parameter = Parameter(isReverse ? typeof(int) : typeof(T), nameof(T));
            var underlying = GetUnderlyingType(typeof(T));
            Expression cast = isReverse ? parameter : Convert(parameter, underlying);

            cast = underlying != typeof(int) ? Convert(parameter, isReverse ? underlying : typeof(int)) : cast;
            cast = isReverse ? Convert(cast, typeof(T)) : cast;

            return Lambda<TFunc>(cast, parameter).Compile();
        }
    }
#endif
}
#endif
