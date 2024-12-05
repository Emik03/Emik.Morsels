// SPDX-License-Identifier: MPL-2.0
#if XNA // ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>Provides methods to create <see cref="Texture2D"/> at runtime.</summary>
static partial class Texture2DFactory
{
    /// <summary>The delegate responsible for painting the graphic.</summary>
    /// <param name="canvas">The eventual <see cref="Texture2D"/>.</param>
    public delegate void Painter(Span2D<Color> canvas);

    /// <summary>Creates the <see cref="Texture2D"/> at runtime.</summary>
    /// <param name="device">The device to associate the texture with.</param>
    /// <param name="width">The width of the texture.</param>
    /// <param name="height">The height of the texture.</param>
    /// <param name="painter">The callback for coloring the graphic.</param>
    /// <returns>The <see cref="Texture2D"/> containing the image painted by the <paramref name="painter"/>.</returns>
    [MustDisposeResource, MustUseReturnValue]
    public static Texture2D CreateTexture2D(
        this GraphicsDevice device,
        [NonNegativeValue] int width,
        [NonNegativeValue] int height,
        [InstantHandle] Painter painter
    )
    {
        Texture2D texture = new(device, width, height);
        var data = new Color[width * height];
        painter(new(data, height, width));
        texture.SetData(data);
        return texture;
    }
}
#endif
