// SPDX-License-Identifier: MPL-2.0
#if XNA // ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Contains scaling methods for resolutions.</summary>
static partial class ResolutionScaling
{
    /// <summary>Gets the height of the screen.</summary>
    /// <param name="device">The current width.</param>
    /// <returns>The width of the screen.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int Height(this GraphicsDevice device) =>
        OperatingSystem.IsAndroid() ? device.DisplayMode.Height : device.Viewport.Height;

    /// <summary>Gets the width of the screen.</summary>
    /// <param name="device">The current width.</param>
    /// <returns>The width of the screen.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static int Width(this GraphicsDevice device) =>
        OperatingSystem.IsAndroid() ? device.DisplayMode.Width : device.Viewport.Width;

    /// <summary>Gets the resolution for <see cref="SpriteBatch.Draw(Texture2D, Rectangle, Color)"/>.</summary>
    /// <param name="device">The current width and height.</param>
    /// <param name="w">The native width.</param>
    /// <param name="h">The native height.</param>
    /// <returns>The <see cref="Rectangle"/> for <see cref="SpriteBatch.Draw(Texture2D, Rectangle, Color)"/>.</returns>
    public static Rectangle Resolution(this GraphicsDevice device, float w, float h)
    {
        var (currentWidth, currentHeight) = (device.Width(), device.Height());
        var (scaledWidth, scaledHeight) = (currentWidth / w, currentHeight / h);
        var min = scaledWidth.Min(scaledHeight);
        var (width, height) = ((int)(min * w), (int)(min * h));
        var (x, y) = ((currentWidth - width) / 2, (currentHeight - height) / 2);
        return new(x, y, width, height);
    }

    /// <summary>Gets the resolution for <see cref="SpriteBatch.Draw(Texture2D, Rectangle, Color)"/>.</summary>
    /// <param name="device">The current width and height.</param>
    /// <param name="v">The native resolution.</param>
    /// <returns>The <see cref="Rectangle"/> for <see cref="SpriteBatch.Draw(Texture2D, Rectangle, Color)"/>.</returns>
    public static Rectangle Resolution(this GraphicsDevice device, Vector2 v) => device.Resolution(v.X, v.Y);
}
#endif
