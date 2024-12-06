// SPDX-License-Identifier: MPL-2.0
#if XNA // ReSharper disable once CheckNamespace
namespace Emik.Morsels;

/// <summary>The basic wrapper around <see cref="Game"/> that handles letterboxing for a 2D game.</summary>
// ReSharper disable NullableWarningSuppressionIsUsed
[CLSCompliant(false)]
public abstract partial class Letterboxed2DGame : Game
{
    /// <summary>The device manager that contains this instance.</summary>
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
        Width = width;
        Height = height;
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        const float ScaledDown = 5 / 6f;

        var ratio = (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / Width)
           .Min(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / Height);

#pragma warning disable IDISP001
        _manager = new(this)
#pragma warning restore IDISP001
        {
            SynchronizeWithVerticalRetrace = true,
            PreferredBackBufferWidth = (int)(Width * ratio * ScaledDown),
            PreferredBackBufferHeight = (int)(Height * ratio * ScaledDown),
        };

        setup?.Invoke(_manager);
        _manager.ApplyChanges();
        Window.KeyDown += CheckForBorderlessOrFullScreenBind;
        GraphicsDevice.BlendState = BlendState.NonPremultiplied;
    }

    /// <summary>Determines whether the game is being played in a desktop environment.</summary>
    [Pure, SupportedOSPlatformGuard("freebsd"), SupportedOSPlatformGuard("linux"), SupportedOSPlatformGuard("macos"),
     SupportedOSPlatformGuard("windows")]
    public static bool IsDesktop =>
        OperatingSystem.IsWindows() ||
        OperatingSystem.IsMacOS() ||
        OperatingSystem.IsLinux() ||
        OperatingSystem.IsFreeBSD();

    /// <summary>Gets the height of the native (world) resolution.</summary>
    [NonNegativeValue, Pure]
    public int Height { get; }

    /// <summary>Gets the width of the native (world) resolution.</summary>
    [NonNegativeValue, Pure]
    public int Width { get; }

    /// <summary>Gets the background, shown in letterboxing.</summary>
    [Pure]
    public Color Background { get; set; }

    /// <summary>Gets the batch to draw with.</summary>
    [Pure]
    public SpriteBatch Batch { get; private set; } = null!;

    /// <summary>Gets the texture containing a single white pixel.</summary>
    [Pure]
    public Texture2D WhitePixel { get; private set; } = null!;

    /// <inheritdoc />
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
        var resolution = GraphicsDevice.Resolution(Width, Height);
        Batch.Draw(_target, resolution, Color.White);
        Batch.End();
        base.EndDraw();
    }

    /// <inheritdoc />
    [CLSCompliant(false), MemberNotNull(nameof(Batch), nameof(_target), nameof(WhitePixel))]
    protected override void Initialize()
    {
        base.Initialize();
        _target = new(GraphicsDevice, Width, Height);
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
        var world = Width / Height;
        var window = width / height;
        var ratio = window < world ? width / Width : height / Height;

        return window < world
            ? new(x / ratio, (y - (height - ratio * Height) / 2) / ratio)
            : new((x - (width - ratio * Width) / 2) / ratio, y / ratio);
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
    void CheckForBorderlessOrFullScreenBind(object? sender, InputKeyEventArgs e)
    {
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (e.Key)
        {
            case Keys.F9 when IsDesktop:
                Window.IsBorderless = !Window.IsBorderless;
                break;
            case Keys.F11:
                _manager.IsFullScreen = !_manager.IsFullScreen;
                _manager.ApplyChanges();
                break;
        }
    }
}
#endif
