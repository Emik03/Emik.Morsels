// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
// ReSharper disable NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
namespace Emik.Morsels;

static partial class OperatorCaching
{
    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Increment<T>(ref T t)
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
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
                _ => (t = DirectOperators<T>.Increment(t)) is var _,
            };

    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Adder<T>(T l, T r)
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
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
                _ => DirectOperators<T>.Adder(l, r),
            };

    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Divider<T>(T l, int r)
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
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
                _ => DirectOperators<T>.Divider(l, r),
            };

    [Inline, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T MinValue<T>()
#if UNMANAGED_SPAN
        where T : unmanaged
#elif !NET8_0_OR_GREATER
        where T : struct
#endif
        =>
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

        static readonly Type[] s_args = new[] { typeof(T), typeof(T) };

        /// <summary>Gets the minimum value.</summary>
        // ReSharper disable once NullableWarningSuppressionIsUsed
        public static T MinValue { get; } =
            (T?)typeof(T).GetField(nameof(MinValue), Flags)?.GetValue(null)!;

        /// <summary>Gets the function for adding.</summary>
        public static Func<T?, T?, T> Adder { get; } = Make<T>("op_Addition", Expression.AddChecked);

        /// <summary>Gets the function for dividing.</summary>
        public static Func<T?, int, T> Divider { get; } =
            Make<int>("op_Division", (x, y) => Expression.Divide(x, Expression.Convert(y, typeof(T))));

        /// <summary>Gets the function for dividing.</summary>
        public static Func<T?, T> Increment { get; } = Make("op_Increment", Expression.Increment);

        static Func<T?, T> Make(string name, Func<Expression, UnaryExpression> go) =>
            typeof(T).GetMethod(name, Flags, s_args) is not { } x && Expression.Parameter(typeof(T), "unit") is var unit
                ? Expression.Lambda<Func<T?, T>>(go(unit), unit).Compile()
                : (Func<T?, T>)Delegate.CreateDelegate(typeof(Func<T?, T>), x);

        static Func<T?, TRight?, T> Make<TRight>(string name, Func<Expression, Expression, BinaryExpression> go) =>
            typeof(T).GetMethod(name, Flags, s_args) is not { } x &&
            Expression.Parameter(typeof(T), "left") is var left &&
            Expression.Parameter(typeof(TRight), "right") is var right
                ? Expression.Lambda<Func<T?, TRight?, T>>(go(left, right), left, right).Compile()
                : (Func<T?, TRight?, T>)Delegate.CreateDelegate(typeof(Func<T?, TRight?, T>), x);
    }
}
