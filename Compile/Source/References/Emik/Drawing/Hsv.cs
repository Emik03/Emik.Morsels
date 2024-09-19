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
    /// <param name="hue">The hue, generally ranging from <c>0</c> to <c>1529</c>.</param>
    /// <param name="saturation">The saturation.</param>
    /// <param name="value">The value.</param>
    /// <returns>The RGB components of the HSV parameters.</returns>
    // Intentionally omitting `AggressiveInlining` due to complexity.
    public static (byte, byte, byte) ToRgb(ushort hue, byte saturation = M, byte value = M) =>
        (hue %= M * 6) switch
        {
            < M => (value, (byte)(V(hue % M, saturation) * value / M), (byte)((M - saturation) * value / M)),
            < M * 2 => ((byte)(V(M - hue % M, saturation) * value / M), value, (byte)((M - saturation) * value / M)),
            < M * 3 => ((byte)((M - saturation) * value / M), value, (byte)(V(hue % M, saturation) * value / M)),
            < M * 4 => ((byte)((M - saturation) * value / M), (byte)(V(M - hue % M, saturation) * value / M), value),
            < M * 5 => ((byte)(V(hue % M, saturation) * value / M), (byte)((M - saturation) * value / M), value),
            _ => (value, (byte)((M - saturation) * value / M), (byte)(V(M - hue % M, saturation) * value / M)),
        };
#if NET7_0_OR_GREATER
    /// <summary>Converts the HSV values to RGB.</summary>
    /// <remarks><para>
    /// Implementation based on
    /// <a href="https://github.com/SGauvin/HsvConverter/blob/master/HsvConverter.cpp">SGauvin's HsvConverter</a>.
    /// </para></remarks>
    /// <param name="hue">The hue, generally ranging from <c>0</c> to <c>1529</c>.</param>
    /// <param name="saturation">The saturation.</param>
    /// <param name="value">The value.</param>
    /// <returns>The RGB components of the HSV parameters.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (byte, byte, byte) ToRgb<T>(this T hue, byte saturation = M, byte value = M)
        where T : IComparisonOperators<T, T, bool>, IModulusOperators<T, T, T>, INumberBase<T> =>
        ToRgb(ushort.CreateChecked(hue.Mod(T.CreateChecked(M * 6))), saturation, value);
#endif
#if XNA
    /// <summary>Converts the HSV values to <see cref="Color"/>.</summary>
    /// <remarks><para>
    /// Implementation based on
    /// <a href="https://github.com/SGauvin/HsvConverter/blob/master/HsvConverter.cpp">SGauvin's HsvConverter</a>.
    /// </para></remarks>
    /// <param name="hue">The hue, generally ranging from <c>0</c> to <c>1529</c>.</param>
    /// <param name="saturation">The saturation.</param>
    /// <param name="value">The value.</param>
    /// <param name="alpha">The alpha channel.</param>
    /// <returns>The RGB components of the HSV parameters.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color ToColor(ushort hue, byte saturation = M, byte value = M, byte alpha = M)
    {
        var (r, g, b) = ToRgb(hue, saturation, value);
        return new(r, g, b, alpha);
    }
#if NET7_0_OR_GREATER
    /// <summary>Converts the HSV values to RGB.</summary>
    /// <remarks><para>
    /// Implementation based on
    /// <a href="https://github.com/SGauvin/HsvConverter/blob/master/HsvConverter.cpp">SGauvin's HsvConverter</a>.
    /// </para></remarks>
    /// <param name="hue">The hue, generally ranging from <c>0</c> to <c>1529</c>.</param>
    /// <param name="saturation">The saturation.</param>
    /// <param name="value">The value.</param>
    /// <param name="alpha">The alpha channel.</param>
    /// <returns>The RGB components of the HSV parameters.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color ToColor<T>(this T hue, byte saturation = M, byte value = M, byte alpha = M)
        where T : IComparisonOperators<T, T, bool>, IModulusOperators<T, T, T>, INumberBase<T> =>
        ToColor(ushort.CreateChecked(hue.Mod(T.CreateChecked(M * 6))), saturation, value, alpha);
#endif
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
    static byte V(int color, byte saturation) => (byte)((byte)color + (M - saturation) * (M - (byte)color) / M);
}
