// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods to clamp numbers.</summary>
static partial class Clamped
{
    /// <summary>Evaluate whether a given integral value is a power of 2.</summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="value"/>
    /// is a power of 2; otherwise, <see langword="false"/>.
    /// </returns>
    [CLSCompliant(false), Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsPow2(this int value) =>
#if NET6_0_OR_GREATER
        BitOperations.IsPow2(value);
#else
        (value & value - 1) is 0 && value > 0;
#endif

    /// <summary>Evaluate whether a given integral value is a power of 2.</summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="value"/>
    /// is a power of 2; otherwise, <see langword="false"/>.
    /// </returns>
    [CLSCompliant(false), Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsPow2(this uint value) =>
#if NET6_0_OR_GREATER
        BitOperations.IsPow2(value);
#else
        (value & value - 1) is 0 && value > 0;
#endif

    /// <summary>Evaluate whether a given integral value is a power of 2.</summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="value"/>
    /// is a power of 2; otherwise, <see langword="false"/>.
    /// </returns>
    [CLSCompliant(false), Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsPow2(this long value) =>
#if NET6_0_OR_GREATER
        BitOperations.IsPow2(value);
#else
        (value & value - 1) is 0 && value > 0;
#endif

    /// <summary>Evaluate whether a given integral value is a power of 2.</summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="value"/>
    /// is a power of 2; otherwise, <see langword="false"/>.
    /// </returns>
    [CLSCompliant(false), Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsPow2(this ulong value) =>
#if NET6_0_OR_GREATER
        BitOperations.IsPow2(value);
#else
        (value & value - 1) is 0 && value > 0;
#endif

    /// <summary>Evaluate whether a given integral value is a power of 2.</summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="value"/>
    /// is a power of 2; otherwise, <see langword="false"/>.
    /// </returns>
    [CLSCompliant(false), Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsPow2(this nint value) =>
#if NET7_0_OR_GREATER
        BitOperations.IsPow2(value);
#else
        (value & value - 1) is 0 && value > 0;
#endif

    /// <summary>Evaluate whether a given integral value is a power of 2.</summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="value"/>
    /// is a power of 2; otherwise, <see langword="false"/>.
    /// </returns>
    [CLSCompliant(false), Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsPow2(this nuint value) =>
#if NET7_0_OR_GREATER
        BitOperations.IsPow2(value);
#else
        (value & value - 1) is 0 && value > 0;
#endif
#if NET7_0_OR_GREATER
    /// <inheritdoc cref="IsPow2(IntPtr)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool IsPow2<T>(this T value)
        where T : IBitwiseOperators<T, T, T>, IComparisonOperators<T, T, bool>, INumberBase<T> =>
        (value & value - T.One) == T.Zero && value > T.Zero;
#endif

    /// <inheritdoc cref="RoundUpToPowerOf2(uint)"/>
    // ReSharper disable RedundantUnsafeContext
    [CLSCompliant(false), Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static unsafe uint RoundUpToPowerOf2(this int value) => RoundUpToPowerOf2(unchecked((uint)value));

    /// <summary>Round the given integral value up to a power of 2.</summary>
    /// <remarks><para>
    /// The fallback implementation is based on
    /// <a href="https://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2">
    /// Bit Twiddling Hacks by Sean Eron Anderson
    /// </a>.
    /// </para></remarks>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The smallest power of 2 which is greater than or equal to <paramref name="value"/>.
    /// If <paramref name="value"/> is 0 or the result overflows, returns 0.
    /// </returns>
    [CLSCompliant(false), Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static uint RoundUpToPowerOf2(this uint value)
#if NET6_0_OR_GREATER
        =>
            BitOperations.RoundUpToPowerOf2(value);
#else
    {
        --value;
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;
        return value + 1;
    }
#endif

    /// <inheritdoc cref="RoundUpToPowerOf2(uint)"/>
    // ReSharper disable RedundantUnsafeContext
    [CLSCompliant(false), Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static unsafe ulong RoundUpToPowerOf2(this long value) => RoundUpToPowerOf2(unchecked((ulong)value));

    /// <inheritdoc cref="RoundUpToPowerOf2(uint)"/>
    [CLSCompliant(false), Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static ulong RoundUpToPowerOf2(this ulong value)
#if NET6_0_OR_GREATER
        =>
            BitOperations.RoundUpToPowerOf2(value);
#else
    {
        --value;
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;
        value |= value >> 32;
        return value + 1;
    }
#endif

    /// <inheritdoc cref="RoundUpToPowerOf2(uint)"/>
    // ReSharper disable RedundantUnsafeContext
    [CLSCompliant(false), Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static unsafe nuint RoundUpToPowerOf2(this nint value) => RoundUpToPowerOf2(unchecked((nuint)value));

    /// <inheritdoc cref="RoundUpToPowerOf2(uint)"/>
    // ReSharper disable RedundantUnsafeContext
    [CLSCompliant(false), Inline, MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static unsafe nuint RoundUpToPowerOf2(this nuint value) =>
#if NET6_0_OR_GREATER // ReSharper restore RedundantUnsafeContext
#pragma warning disable IDE0004 // ReSharper disable once RedundantCast
        (nuint)BitOperations.RoundUpToPowerOf2(value);
#pragma warning restore IDE0004
#else
        sizeof(nuint) is 4 ? RoundUpToPowerOf2((uint)value) : (nuint)RoundUpToPowerOf2((ulong)value);
#endif
#if NET7_0_OR_GREATER
    /// <summary>Clamps a value such that it is no smaller or larger than the defined amount.</summary>
    /// <typeparam name="T">The type of numeric value for comparisons.</typeparam>
    /// <param name="number">The number to clip.</param>
    /// <param name="min">If specified, the smallest number to return.</param>
    /// <param name="max">If specified, the greatest number to return.</param>
    /// <returns>
    /// The parameter <paramref name="min"/> if <paramref name="number"/> is smaller than <paramref name="min"/>,
    /// otherwise, the parameter <paramref name="max"/> if <paramref name="number"/> is greater than
    /// <paramref name="max"/>, otherwise the parameter <paramref name="number"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Clip<T>(this T number, T? min = null, T? max = null)
        where T : class, IComparisonOperators<T, T, bool> =>
        (min ?? number) is var small &&
        (max ?? number) is var big &&
        number <= small ? small :
        number >= big ? big : number;

    /// <inheritdoc cref="Clip{T}(T,T?,T?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Clip<T>(this T number, T? min = null, T? max = null)
        where T : struct, IComparisonOperators<T, T, bool> =>
        (min ?? number) is var small &&
        (max ?? number) is var big &&
        number <= small ? small :
        number >= big ? big : number;

    /// <summary>
    /// Calculates the least nonnegative remainder of <paramref name="number"/> <c>%</c> <paramref name="radix"/>.
    /// </summary>
    /// <remarks><para>
    /// Implementation based on <a href="https://doc.rust-lang.org/src/core/num/int_macros.rs.html#2190">
    /// Rust standard library (core)'s rem_euclid function
    /// </a>.
    /// </para></remarks>
    /// <typeparam name="T">The type of numeric value.</typeparam>
    /// <param name="number">The number to calculate the remainder of.</param>
    /// <param name="radix">The radix to use.</param>
    /// <returns>The result of the Euclidean division algorithm.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static T Mod<T>(this T number, T radix)
        where T : IComparisonOperators<T, T, bool>, IModulusOperators<T, T, T>, INumberBase<T> =>
        number % radix is var r && r < T.Zero ? unchecked(r + radix) : r;
#else
    /// <summary>Clamps a value such that it is no smaller or larger than the defined amount.</summary>
    /// <param name="number">The number to clip.</param>
    /// <param name="min">If specified, the smallest number to return.</param>
    /// <param name="max">If specified, the greatest number to return.</param>
    /// <returns>
    /// The parameter <paramref name="min"/> if <paramref name="number"/> is smaller than <paramref name="min"/>,
    /// otherwise, the parameter <paramref name="max"/> if <paramref name="number"/> is greater than
    /// <paramref name="max"/>, otherwise the parameter <paramref name="number"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int Clamp(this int number, int? min = null, int? max = null) =>
        (min ?? number) is var small &&
        (max ?? number) is var big &&
        number <= small ? small :
        number >= big ? big : number;

    /// <inheritdoc cref="Clamp(int, int?, int?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static float Clamp(this float number, float? min = null, float? max = null) =>
        (min ?? number) is var small &&
        (max ?? number) is var big &&
        number <= small ? small :
        number >= big ? big : number;

    /// <summary>
    /// Calculates the least nonnegative remainder of <paramref name="number"/> <c>%</c> <paramref name="radix"/>.
    /// </summary>
    /// <remarks><para>
    /// Implementation based on <a href="https://doc.rust-lang.org/src/core/num/int_macros.rs.html#2190">
    /// Rust standard library (core)'s rem_euclid function
    /// </a>.
    /// </para></remarks>
    /// <param name="number">The number to calculate the remainder of.</param>
    /// <param name="radix">The radix to use.</param>
    /// <returns>The result of the Euclidean division algorithm.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int Mod(this int number, int radix) => number % radix is var r && r < 0 ? unchecked(r + radix) : r;

    /// <inheritdoc cref="Mod(int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static float Mod(this float number, float radix) => number % radix is var r && r < 0 ? r + radix : r;
#endif
}
