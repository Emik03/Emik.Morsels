// SPDX-License-Identifier: MPL-2.0
#if XNA // ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>The basic wrapper around <see cref="Game"/> that handles letterboxing for a 2D game.</summary>
// ReSharper disable NullableWarningSuppressionIsUsed
abstract partial class Letterboxed2DGame : Game
{
    /// <summary>Gets the native resolutions.</summary>
    [NonNegativeValue]
    readonly float _width, _height;

    readonly GraphicsDeviceManager _manager;

    /// <summary>Gets the target to draw to.</summary>
    RenderTarget2D? _target;

    /// <summary>Initializes a new instances of the <see cref="Letterboxed2DGame"/> class.</summary>
    /// <param name="width">The width of the world.</param>
    /// <param name="height">The height of the world.</param>
    /// <param name="setup">
    /// The callback invoked before <see cref="GraphicsDeviceManager.ApplyChanges"/> is invoked.
    /// </param>
    protected Letterboxed2DGame(int width, int height, Action<GraphicsDeviceManager>? setup = null)
    {
        _width = width;
        _height = height;
        IsMouseVisible = true;
        const float ScaledDown = 5 / 6f;
        Window.AllowUserResizing = true;

        var ratio = (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / _width)
           .Min(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / _height);

#pragma warning disable IDISP001
        _manager = new(this)
#pragma warning restore IDISP001
        {
            PreferredBackBufferWidth = (int)(_width * ratio * ScaledDown),
            PreferredBackBufferHeight = (int)(_height * ratio * ScaledDown),
            SynchronizeWithVerticalRetrace = true,
        };

        setup?.Invoke(_manager);
        _manager.ApplyChanges();
        Window.KeyDown += FullScreenBind;
    }

    /// <summary>Gets the background, shown in letterboxing.</summary>
    [Pure]
    protected Color Background { get; set; }

    /// <summary>Gets the batch to draw with.</summary>
    [Pure]
    protected SpriteBatch Batch { get; private set; } = null!;

    /// <summary>Gets the texture containing a single white pixel.</summary>
    [Pure]
    protected Texture2D WhitePixel { get; private set; } = null!;

    protected override bool BeginDraw()
    {
        Debug.Assert(Batch is not null);
        GraphicsDevice.SetRenderTarget(_target);
        GraphicsDevice.Clear(Background);
        Batch.Begin();
        return base.BeginDraw();
    }

    /// <inheritdoc />
    protected override void EndDraw()
    {
        Debug.Assert(Batch is not null);
        Batch.End();
        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Background);
        Batch.Begin();
        var resolution = GraphicsDevice.Resolution(_width, _height);
        Batch.Draw(_target, resolution, Color.White);
        Batch.End();
        base.EndDraw();
    }

    /// <inheritdoc />
    [CLSCompliant(false), MemberNotNull(nameof(Batch), nameof(_target), nameof(WhitePixel))]
    protected override void Initialize()
    {
        base.Initialize();
        _target = new(GraphicsDevice, (int)_width, (int)_height);
        Services.AddService(Batch = new(GraphicsDevice));
        WhitePixel = new(GraphicsDevice, 1, 1);
        WhitePixel.SetData([Color.White]);
    }

    /// <summary>Converts the window coordinate to the world coordinate.</summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <returns>The world coordinate of the parameters given.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    protected Vector2 World(float x, float y)
    {
        var bounds = Window.ClientBounds;
        float width = bounds.Width, height = bounds.Height;
        var world = _width / _height;
        var window = width / height;
        var ratio = window < world ? width / _width : height / _height;

        return window < world
            ? new(x / ratio, (y - (height - ratio * _height) / 2) / ratio)
            : new((x - (width - ratio * _width) / 2) / ratio, y / ratio);
    }

    /// <inheritdoc cref="World(Vector2)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    protected Vector2 World(in MouseState v) => World(v.Position);

    /// <inheritdoc cref="World(Vector2)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    protected Vector2 World(Point v) => World(v.X, v.Y);

    /// <inheritdoc cref="World(Vector2)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    protected Vector2 World(in TouchLocation v) => World(v.Position);

    /// <summary>Converts the window coordinate to the world coordinate.</summary>
    /// <param name="v">The vector to convert.</param>
    /// <returns>The world coordinate of the vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    protected Vector2 World(Vector2 v) => World(v.X, v.Y);

    /// <summary>Invoked when a keyboard button is pressed.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event arguments containing the key that was pressed.</param>
    void FullScreenBind(object? sender, InputKeyEventArgs e)
    {
        if (e.Key is not Keys.F11)
            return;

        _manager.IsFullScreen = !_manager.IsFullScreen;
        _manager.ApplyChanges();
    }
}
#endif
