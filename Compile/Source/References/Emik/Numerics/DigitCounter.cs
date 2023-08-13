// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Extension methods to count digits in numbers.</summary>
static partial class DigitCounter
{
    /// <summary>Gets the amount of digits of the number.</summary>
    /// <param name="number">The number to count.</param>
    /// <returns>The amount of digits in the parameter <paramref name="number"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure, ValueRange(1, 3)]
    public static byte DigitCount(this byte number) =>
        number switch
        {
            < 10 => 1,
            < 100 => 2,
            _ => 3,
        };

    /// <inheritdoc cref="DigitCount(byte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure, ValueRange(1, 3)]
    public static byte DigitCount(this sbyte number) =>
        number switch
        {
            < 10 and > -10 => 1,
            < 100 and > -100 => 2,
            _ => 3,
        };

    /// <inheritdoc cref="DigitCount(byte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure, ValueRange(1, 5)]
    public static byte DigitCount(this ushort number) =>
        number switch
        {
            < 10 => 1,
            < 100 => 2,
            < 1000 => 3,
            < 10000 => 4,
            _ => 5,
        };

    /// <inheritdoc cref="DigitCount(byte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure, ValueRange(1, 5)]
    public static byte DigitCount(this short number) =>
        number switch
        {
            < 10 and > -10 => 1,
            < 100 and > -100 => 2,
            < 1000 and > -1000 => 3,
            < 10000 and > -10000 => 4,
            _ => 5,
        };

    /// <inheritdoc cref="DigitCount(byte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure, ValueRange(1, 10)]
    public static byte DigitCount(this uint number) =>
        number switch
        {
            < 10 => 1,
            < 100 => 2,
            < 1000 => 3,
            < 10000 => 4,
            < 100000 => 5,
            < 1000000 => 6,
            < 10000000 => 7,
            < 100000000 => 8,
            < 1000000000 => 9,
            _ => 10,
        };

    /// <inheritdoc cref="DigitCount(byte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure, ValueRange(1, 10)]
    public static byte DigitCount(this int number) =>
        number switch
        {
            < 10 and > -10 => 1,
            < 100 and > -100 => 2,
            < 1000 and > -1000 => 3,
            < 10000 and > -10000 => 4,
            < 100000 and > -100000 => 5,
            < 1000000 and > -1000000 => 6,
            < 10000000 and > -10000000 => 7,
            < 100000000 and > -100000000 => 8,
            < 1000000000 and > -1000000000 => 9,
            _ => 10,
        };

    /// <inheritdoc cref="DigitCount(byte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure, ValueRange(1, 20)]
    public static byte DigitCount(this ulong number) =>
        number switch
        {
            < 10 => 1,
            < 100 => 2,
            < 1000 => 3,
            < 10000 => 4,
            < 100000 => 5,
            < 1000000 => 6,
            < 10000000 => 7,
            < 100000000 => 8,
            < 1000000000 => 9,
            < 10000000000 => 10,
            < 100000000000 => 11,
            < 1000000000000 => 12,
            < 10000000000000 => 13,
            < 100000000000000 => 14,
            < 1000000000000000 => 15,
            < 10000000000000000 => 16,
            < 100000000000000000 => 17,
            < 1000000000000000000 => 18,
            < 10000000000000000000 => 19,
            _ => 20,
        };

    /// <inheritdoc cref="DigitCount(byte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure, ValueRange(1, 19)]
    public static byte DigitCount(this long number) =>
        number switch
        {
            < 10 and > -10 => 1,
            < 100 and > -100 => 2,
            < 1000 and > -1000 => 3,
            < 10000 and > -10000 => 4,
            < 100000 and > -100000 => 5,
            < 1000000 and > -1000000 => 6,
            < 10000000 and > -10000000 => 7,
            < 100000000 and > -100000000 => 8,
            < 1000000000 and > -1000000000 => 9,
            < 10000000000 and > -10000000000 => 10,
            < 100000000000 and > -100000000000 => 11,
            < 1000000000000 and > -1000000000000 => 12,
            < 10000000000000 and > -10000000000000 => 13,
            < 100000000000000 and > -100000000000000 => 14,
            < 1000000000000000 and > -1000000000000000 => 15,
            < 10000000000000000 and > -10000000000000000 => 16,
            < 100000000000000000 and > -100000000000000000 => 17,
            < 1000000000000000000 and > -1000000000000000000 => 18,
            _ => 19,
        };
}
