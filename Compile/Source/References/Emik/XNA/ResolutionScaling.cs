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

    /// <inheritdoc cref="Rectangle.Inflate(int, int)"/>
    public static Rectangle Inflated(this Rectangle rectangle, int amount)
    {
        rectangle.Inflate(amount, amount);
        return rectangle;
    }

    /// <inheritdoc cref="Rectangle.Inflate(int, int)"/>
    public static Rectangle Inflated(this Rectangle rectangle, int horizontalAmount, int verticalAmount)
    {
        rectangle.Inflate(horizontalAmount, verticalAmount);
        return rectangle;
    }

    /// <inheritdoc cref="Rectangle.Inflate(float, float)"/>
    public static Rectangle Inflated(this Rectangle rectangle, float amount)
    {
        rectangle.Inflate(amount, amount);
        return rectangle;
    }

    /// <inheritdoc cref="Rectangle.Inflate(float, float)"/>
    public static Rectangle Inflated(this Rectangle rectangle, float horizontalAmount, float verticalAmount)
    {
        rectangle.Inflate(horizontalAmount, verticalAmount);
        return rectangle;
    }

    /// <inheritdoc cref="Rectangle.Offset(int, int)"/>
    public static Rectangle Offsetted(this Rectangle rectangle, int amount)
    {
        rectangle.Offset(amount, amount);
        return rectangle;
    }

    /// <inheritdoc cref="Rectangle.Offset(int, int)"/>
    public static Rectangle Offsetted(this Rectangle rectangle, int horizontalAmount, int verticalAmount)
    {
        rectangle.Offset(horizontalAmount, verticalAmount);
        return rectangle;
    }

    /// <inheritdoc cref="Rectangle.Offset(float, float)"/>
    public static Rectangle Offsetted(this Rectangle rectangle, float amount)
    {
        rectangle.Offset(amount, amount);
        return rectangle;
    }

    /// <inheritdoc cref="Rectangle.Offset(float, float)"/>
    public static Rectangle Offsetted(this Rectangle rectangle, float horizontalAmount, float verticalAmount)
    {
        rectangle.Offset(horizontalAmount, verticalAmount);
        return rectangle;
    }

    /// <summary>Multiples some quantity with the given <see cref="Rectangle"/>.</summary>
    /// <param name="rectangle">The <see cref="Rectangle"/> to multiply.</param>
    /// <param name="amount">The amount to multiply with.</param>
    /// <returns>The scaled up <see cref="Rectangle"/>.</returns>
    public static Rectangle Multiplied(this Rectangle rectangle, int amount) => Multiplied(rectangle, amount, amount);

    /// <summary>Multiples some quantity with the given <see cref="Rectangle"/>.</summary>
    /// <param name="rectangle">The <see cref="Rectangle"/> to multiply.</param>
    /// <param name="x">The amount to multiply with horizontally.</param>
    /// <param name="y">The amount to multiply with vertically.</param>
    /// <returns>The scaled up <see cref="Rectangle"/>.</returns>
    public static Rectangle Multiplied(this Rectangle rectangle, int x, int y) =>
        new(rectangle.X * x, rectangle.Y * y, rectangle.Width * x, rectangle.Height * y);

    /// <summary>Multiples some quantity with the given <see cref="Rectangle"/>.</summary>
    /// <param name="rectangle">The <see cref="Rectangle"/> to multiply.</param>
    /// <param name="amount">The amount to multiply with.</param>
    /// <returns>The scaled up <see cref="Rectangle"/>.</returns>
    public static Rectangle Multiplied(this Rectangle rectangle, float amount) => Multiplied(rectangle, amount, amount);

    /// <summary>Multiples some quantity with the given <see cref="Rectangle"/>.</summary>
    /// <param name="rectangle">The <see cref="Rectangle"/> to multiply.</param>
    /// <param name="x">The amount to multiply with horizontally.</param>
    /// <param name="y">The amount to multiply with vertically.</param>
    /// <returns>The scaled up <see cref="Rectangle"/>.</returns>
    public static Rectangle Multiplied(this Rectangle rectangle, float x, float y) =>
        new((int)(rectangle.X * x), (int)(rectangle.Y * y), (int)(rectangle.Width * x), (int)(rectangle.Height * y));
}
#endif
