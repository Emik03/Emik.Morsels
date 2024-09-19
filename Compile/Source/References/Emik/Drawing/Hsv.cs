// SPDX-License-Identifier: MPL-2.0

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Contains methods to convert HSV (Hue, Saturation, and Value) into color values.</summary>
static partial class Hsv
{
    /// <summary>The alias for the maximum number that the <see langword="byte"/> can hold.</summary>
    const byte M = byte.MaxValue;

    /// <summary>Converts the HSV values to RGB.</summary>
    /// <remarks><para>
    /// Implementation based on
    /// <a href="https://github.com/SGauvin/HsvConverter/blob/master/HsvConverter.cpp">SGauvin's HsvConverter</a>.
    /// </para></remarks>
    /// <param name="hue">The hue, generally ranging from 0 to 1529.</param>
    /// <param name="saturation">The saturation.</param>
    /// <param name="value">The value.</param>
    /// <returns>The RGB components of the HSV parameters.</returns>
    public static (byte, byte, byte) ToRgb(this int hue, byte saturation, byte value) =>
        ToRgb((ushort)hue.Mod(M * 6), saturation, value);

    /// <summary>Converts the HSV values to RGB.</summary>
    /// <remarks><para>
    /// Implementation based on
    /// <a href="https://github.com/SGauvin/HsvConverter/blob/master/HsvConverter.cpp">SGauvin's HsvConverter</a>.
    /// </para></remarks>
    /// <param name="hue">The hue, generally ranging from 0 to 1529.</param>
    /// <param name="saturation">The saturation.</param>
    /// <param name="value">The value.</param>
    /// <returns>The RGB components of the HSV parameters.</returns>
    public static (byte, byte, byte) ToRgb(this ushort hue, byte saturation, byte value) =>
        (hue %= M * 6) switch
        {
            <= M => (value, (byte)(V(hue % M, saturation) * value / M), (byte)((M - saturation) * value / M)),
            <= M * 2 => ((byte)(V(M - hue % M, saturation) * value / M), value, (byte)((M - saturation) * value / M)),
            <= M * 3 => ((byte)((M - saturation) * value / M), value, (byte)(V(hue % M, saturation) * value / M)),
            <= M * 4 => ((byte)((M - saturation) * value / M), (byte)(V(M - hue % M, saturation) * value / M), value),
            <= M * 5 => ((byte)(V(hue % M, saturation) * value / M), (byte)((M - saturation) * value / M), value),
            _ => (value, (byte)((M - saturation) * value / M), (byte)(V(M - hue % M, saturation) * value / M)),
        };
#if XNA
    /// <summary>Converts the HSV values to RGB.</summary>
    /// <remarks><para>
    /// Implementation based on
    /// <a href="https://github.com/SGauvin/HsvConverter/blob/master/HsvConverter.cpp">SGauvin's HsvConverter</a>.
    /// </para></remarks>
    /// <param name="hue">The hue, generally ranging from 0 to 1529.</param>
    /// <param name="saturation">The saturation.</param>
    /// <param name="value">The value.</param>
    /// <returns>The RGB components of the HSV parameters.</returns>
    public static Color ToColor(this int hue, byte saturation, byte value) =>
        ToColor((ushort)hue.Mod(M * 6), saturation, value);

    /// <summary>Converts the HSV values to <see cref="Color"/>.</summary>
    /// <remarks><para>
    /// Implementation based on
    /// <a href="https://github.com/SGauvin/HsvConverter/blob/master/HsvConverter.cpp">SGauvin's HsvConverter</a>.
    /// </para></remarks>
    /// <param name="hue">The hue, generally ranging from 0 to 360.</param>
    /// <param name="saturation">The saturation.</param>
    /// <param name="value">The value.</param>
    /// <param name="alpha">The alpha channel.</param>
    /// <returns>The RGB components of the HSV parameters.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color ToColor(this ushort hue, byte saturation, byte value, byte alpha = M)
    {
        var (r, g, b) = ToRgb(hue, saturation, value);
        return new(r, g, b, alpha);
    }
#endif
    /// <summary>Computes the value for one of the RGB channels.</summary>
    /// <remarks><para>
    /// Implementation based on
    /// <a href="https://github.com/SGauvin/HsvConverter/blob/master/HsvConverter.cpp">SGauvin's HsvConverter</a>.
    /// </para></remarks>
    /// <param name="color">The color.</param>
    /// <param name="saturation">The saturation.</param>
    /// <returns>The value for the RGB channel.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static byte V(int color, byte saturation) => (byte)(color + (M - saturation) * (M - color) / M);
}
