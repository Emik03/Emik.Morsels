// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace EmptyNamespace
namespace System.Numerics;
#if !NETCOREAPP3_0_OR_GREATER
/// <summary>
/// Utility methods for intrinsic bit-twiddling operations.
/// The methods use hardware intrinsics when available on the underlying platform,
/// otherwise they use optimized software fallbacks.
/// </summary>
static partial class BitOperations
{
    /// <summary>Evaluate whether a given integral value is a power of 2.</summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="value"/> is a power of 2; <see langword="false"/> otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPow2(int value) => (value & value - 1) is 0 && value > 0;

    /// <inheritdoc cref="IsPow2(int)"/>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPow2(uint value) => (value & value - 1) is 0 && value is not 0;

    /// <inheritdoc cref="IsPow2(int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPow2(long value) => (value & value - 1) is 0 && value > 0;

    /// <inheritdoc cref="IsPow2(int)"/>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPow2(ulong value) => (value & value - 1) is 0 && value is not 0;

    /// <inheritdoc cref="IsPow2(int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPow2(nint value) => (value & value - 1) is 0 && value > 0;

    /// <inheritdoc cref="IsPow2(int)"/>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPow2(nuint value) => (value & value - 1) is 0 && value is not 0;

    /// <summary>Returns the population count (number of bits set) of a mask.</summary>
    /// <remarks><para>Similar in behavior to the x86 instruction POPCNT.</para></remarks>
    /// <param name="value">The value.</param>
    /// <returns>The population count of the mask.</returns>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int PopCount(nuint value) =>
        sizeof(nint) is 8 ? PopCount((ulong)value) : PopCount((uint)value);

    /// <summary>Returns the population count (number of bits set) of an unsigned 32-integer mask.</summary>
    /// <remarks><para>Similar in behavior to the x86 instruction POPCNT.</para></remarks>
    /// <param name="value">The value.</param>
    /// <returns>The population count of the mask.</returns>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PopCount(uint value)
    {
        const uint
            C1 = 0x_55555555u,
            C2 = 0x_33333333u,
            C3 = 0x_0F0F0F0Fu,
            C4 = 0x_01010101u;

        value -= value >> 1 & C1;
        value = (value & C2) + (value >> 2 & C2);
        value = (value + (value >> 4) & C3) * C4 >> 24;
        return (int)value;
    }

    /// <summary>Returns the population count (number of bits set) of an unsigned 64-integer mask.</summary>
    /// <remarks><para>Similar in behavior to the x86 instruction POPCNT.</para></remarks>
    /// <param name="value">The value.</param>
    /// <returns>The population count of the mask.</returns>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PopCount(ulong value)
    {
        const ulong
            C1 = 0x_55555555_55555555ul,
            C2 = 0x_33333333_33333333ul,
            C3 = 0x_0F0F0F0F_0F0F0F0Ful,
            C4 = 0x_01010101_01010101ul;

        value -= value >> 1 & C1;
        value = (value & C2) + (value >> 2 & C2);
        value = (value + (value >> 4) & C3) * C4 >> 56;
        return (int)value;
    }

    /// <summary>Round the given integral value up to a power of 2.</summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The smallest power of 2 which is greater than or equal to <paramref name="value"/>.
    /// If <paramref name="value"/> is 0 or the result overflows, returns 0.
    /// </returns>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RoundUpToPowerOf2(uint value)
    {
        // Based on https://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2
        --value;
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;
        return value + 1;
    }

    /// <summary>Round the given integral value up to a power of 2.</summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The smallest power of 2 which is greater than or equal to <paramref name="value"/>.
    /// If <paramref name="value"/> is 0 or the result overflows, returns 0.
    /// </returns>
    [CLSCompliant(false), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong RoundUpToPowerOf2(ulong value)
    {
        // Based on https://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2
        --value;
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;
        value |= value >> 32;
        return value + 1;
    }
}
#endif
