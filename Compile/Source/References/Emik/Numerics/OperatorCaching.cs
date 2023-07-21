// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
// ReSharper disable NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
namespace Emik.Morsels;

/// <summary>Methods that provide access to generic operators, for frameworks that do not support it.</summary>
static partial class OperatorCaching
{
    /// <summary>Increments the value.</summary>
    /// <typeparam name="T">The type of value to increment.</typeparam>
    /// <param name="t">The value to increment.</param>
    /// <exception cref="MissingMethodException">The type <typeparamref name="T"/> is unsupported.</exception>
    /// <returns>The value <see langword="true"/>.</returns>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Increment<T>(ref T t) =>
        typeof(T) switch
        {
            var x when x == typeof(byte) => ++Unsafe.As<T, byte>(ref t) is var _,
            var x when x == typeof(double) => ++Unsafe.As<T, double>(ref t) is var _,
            var x when x == typeof(float) => ++Unsafe.As<T, float>(ref t) is var _,
            var x when x == typeof(int) => ++Unsafe.As<T, int>(ref t) is var _,
#if NET5_0_OR_GREATER
            var x when x == typeof(nint) => ++Unsafe.As<T, nint>(ref t) is var _,
            var x when x == typeof(nuint) => ++Unsafe.As<T, nuint>(ref t) is var _,
#endif
            var x when x == typeof(sbyte) => ++Unsafe.As<T, sbyte>(ref t) is var _,
            var x when x == typeof(short) => ++Unsafe.As<T, short>(ref t) is var _,
            var x when x == typeof(uint) => ++Unsafe.As<T, uint>(ref t) is var _,
            var x when x == typeof(ulong) => ++Unsafe.As<T, ulong>(ref t) is var _,
            var x when x == typeof(ushort) => ++Unsafe.As<T, ushort>(ref t) is var _,
            _ when DirectOperators<T>.IsSupported => (t = DirectOperators<T>.Increment(t)) is var _,
            _ => Fail<T>() is var _,
        };

    /// <summary>Determines whether the current type <typeparamref name="T"/> is supported.</summary>
    /// <typeparam name="T">The type to check.</typeparam>
    /// <returns>Whether the current type <typeparamref name="T"/> is supported.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsSupported<T>() => DirectOperators<T>.IsSupported;

    /// <summary>Performs an addition operation to return the sum.</summary>
    /// <typeparam name="T">The type of value to add.</typeparam>
    /// <param name="l">The left-hand side.</param>
    /// <param name="r">The right-hand side.</param>
    /// <exception cref="MissingMethodException">The type <typeparamref name="T"/> is unsupported.</exception>
    /// <returns>The sum of the parameters <paramref name="l"/> and <paramref name="r"/>.</returns>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Adder<T>(T l, T r) =>
        typeof(T) switch
        {
            var x when x == typeof(byte) => (T)(object)((byte)(object)l! + (byte)(object)r!),
            var x when x == typeof(double) => (T)(object)((double)(object)l! + (double)(object)r!),
            var x when x == typeof(float) => (T)(object)((float)(object)l! + (float)(object)r!),
            var x when x == typeof(int) => (T)(object)((int)(object)l! + (int)(object)r!),
            var x when x == typeof(nint) => (T)(object)((nint)(object)l! + (nint)(object)r!),
            var x when x == typeof(nuint) => (T)(object)((nuint)(object)l! + (nuint)(object)r!),
            var x when x == typeof(sbyte) => (T)(object)((sbyte)(object)l! + (sbyte)(object)r!),
            var x when x == typeof(short) => (T)(object)((short)(object)l! + (short)(object)r!),
            var x when x == typeof(uint) => (T)(object)((uint)(object)l! + (uint)(object)r!),
            var x when x == typeof(ulong) => (T)(object)((ulong)(object)l! + (ulong)(object)r!),
            var x when x == typeof(ushort) => (T)(object)((ushort)(object)l! + (ushort)(object)r!),
            _ when DirectOperators<T>.IsSupported => DirectOperators<T>.Adder(l, r),
            _ => Fail<T>(),
        };

    /// <summary>Performs a dividing operation to return the quotient.</summary>
    /// <typeparam name="T">The type of value to divide.</typeparam>
    /// <param name="l">The left-hand side.</param>
    /// <param name="r">The right-hand side.</param>
    /// <exception cref="MissingMethodException">The type <typeparamref name="T"/> is unsupported.</exception>
    /// <returns>The quotient of the parameters <paramref name="l"/> and <paramref name="r"/>.</returns>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Divider<T>(T l, int r) =>
        typeof(T) switch
        {
            var x when x == typeof(byte) => (T)(object)((byte)(object)l! + r),
            var x when x == typeof(double) => (T)(object)((double)(object)l! + r),
            var x when x == typeof(float) => (T)(object)((float)(object)l! + r),
            var x when x == typeof(int) => (T)(object)((int)(object)l! + r),
            var x when x == typeof(nint) => (T)(object)((nint)(object)l! + r),
            var x when x == typeof(nuint) => (T)(object)((nuint)(object)l! + (nuint)r),
            var x when x == typeof(sbyte) => (T)(object)((sbyte)(object)l! + r),
            var x when x == typeof(short) => (T)(object)((short)(object)l! + r),
            var x when x == typeof(uint) => (T)(object)((uint)(object)l! + r),
            var x when x == typeof(ulong) => (T)(object)((ulong)(object)l! + (ulong)r),
            var x when x == typeof(ushort) => (T)(object)((ushort)(object)l! + r),
            _ when DirectOperators<T>.IsSupported => DirectOperators<T>.Divider(l, r),
            _ => Fail<T>(),
        };

    /// <summary>Throws the exception used by <see cref="OperatorCaching"/> to propagate errors.</summary>
    /// <typeparam name="T">The type that failed.</typeparam>
    /// <exception cref="MissingMethodException">The type <typeparamref name="T"/> is unsupported.</exception>
    /// <returns>This method does not return.</returns>
    [DoesNotReturn, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Fail<T>() =>
        throw new MissingMethodException(typeof(T).UnfoldedFullName(), "op_Addition/op_Division/op_Increment");

    /// <summary>Gets the minimum value.</summary>
    /// <typeparam name="T">The type of value to get the minimum value of.</typeparam>
    /// <returns>The minimum value of <typeparamref name="T"/>.</returns>
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T MinValue<T>() =>
        typeof(T) switch
        {
            var x when x == typeof(byte) => (T)(object)byte.MinValue,
            var x when x == typeof(double) => (T)(object)double.MinValue,
            var x when x == typeof(float) => (T)(object)float.MinValue,
            var x when x == typeof(int) => (T)(object)int.MinValue,
#if NET5_0_OR_GREATER
            var x when x == typeof(nint) => (T)(object)nint.MinValue,
            var x when x == typeof(nuint) => (T)(object)nuint.MinValue,
#endif
            var x when x == typeof(sbyte) => (T)(object)sbyte.MinValue,
            var x when x == typeof(short) => (T)(object)short.MinValue,
            var x when x == typeof(uint) => (T)(object)uint.MinValue,
            var x when x == typeof(ulong) => (T)(object)ulong.MinValue,
            var x when x == typeof(ushort) => (T)(object)ushort.MinValue,
            _ => DirectOperators<T>.MinValue,
        };

    /// <summary>Caches operators.</summary>
    /// <typeparam name="T">The containing member of operators.</typeparam>
    // ReSharper disable once ClassNeverInstantiated.Local
    sealed partial class DirectOperators<T>
    {
        const BindingFlags Flags = BindingFlags.Public | BindingFlags.Static;

        static readonly Type[]
            s_binary = new[] { typeof(T), typeof(T) },
            s_unary = new[] { typeof(T) };

        static DirectOperators()
        {
            try
            {
                Increment = Make("op_Increment", Expression.Increment);
                Adder = Make<T>("op_Addition", Expression.AddChecked);
                Divider = Make<int>("op_Division", (x, y) => Expression.Divide(x, Expression.Convert(y, typeof(T))));
            }
            catch (InvalidOperationException)
            {
                IsSupported = false;
            }
        }
#pragma warning disable RCS1158
        /// <summary>
        /// Gets a value indicating whether the functions can be used.
        /// <see cref="MinValue"/> can be used regardless of its output.
        /// </summary>
        public static bool IsSupported
        {
            [MemberNotNullWhen(true, nameof(Adder), nameof(Divider), nameof(Increment)),
             MethodImpl(MethodImplOptions.AggressiveInlining),
             Pure]
            get;
        } = true;
#pragma warning restore RCS1158
        /// <summary>Gets the minimum value.</summary>
        // ReSharper disable once NullableWarningSuppressionIsUsed
        public static T MinValue { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; } =
            typeof(T).GetField(nameof(MinValue), Flags)?.GetValue(null) is T t ? t : default!;

        /// <summary>Gets the function for dividing.</summary>
        public static Converter<T?, T>? Increment { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; }

        /// <summary>Gets the function for adding.</summary>
        public static Func<T?, T?, T>? Adder { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; }

        /// <summary>Gets the function for dividing.</summary>
        public static Func<T?, int, T>? Divider { [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] get; }

        [Pure]
        static Converter<T?, T> Make(string name, [InstantHandle] Func<Expression, UnaryExpression> go) =>
            typeof(T).GetMethod(name, Flags, null, s_unary, null) is not { } method &&
            Expression.Parameter(typeof(T), "unit") is var unit
                ? Expression.Lambda<Converter<T?, T>>(go(unit), unit).Compile()
                : (Converter<T?, T>)Delegate.CreateDelegate(typeof(Converter<T?, T>), method);

        [Pure]
        static Func<T?, TRight?, T> Make<TRight>(
            string name,
            [InstantHandle] Func<Expression, Expression, BinaryExpression> go
        ) =>
            (typeof(T).GetMethod(name, Flags, null, s_binary, null) is not { } method ||
                (Func<T?, T?, T>)Delegate.CreateDelegate(typeof(Func<T?, T?, T>), method) is not { } func) &&
            Expression.Parameter(typeof(T), "left") is var left &&
            Expression.Parameter(typeof(TRight), "right") is var right
                ? Expression.Lambda<Func<T?, TRight?, T>>(go(left, right), left, right).Compile()
                : (x, y) => func(x, (T?)(object?)y);
    }
}
